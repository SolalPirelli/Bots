using System.Linq;

namespace Bots.Quiz
{
    public sealed class QuizBot : StateBasedBot
    {
        public QuizBot( QuizBotServices services, QuizBotResources resources ) : base( services, resources )
        {
            // There are two states for the question: 
            // first it's selected, then if available we wait and switch to asking
            // This ensures that users can't answer the question before its first paragraph has been sent.
            State
               waitingState = new State( "Waiting" ),
               preQuestionState = new State( "Pre-Question" ),
               questionState = new State( "Question" );

            SetInitialState( waitingState );

            CommandHandler
                startHandler = async ( command, token ) =>
                {
                    services.Logger.Log( "Resuming" );

                    await services.Network.SendMessageAsync( resources.Resuming() );
                    await SwitchToAsync( preQuestionState );
                }
            ,
                stopHandler = async ( command, token ) =>
                {
                    services.Logger.Log( "Pausing" );

                    await services.Network.SendMessageAsync( resources.Pausing() );
                    await SwitchToAsync( waitingState );
                };

            IQuestion question = null;

            waitingState
               .AddCommandHandler( "start", startHandler );

            preQuestionState
                .AddCommandHandler( "stop", stopHandler )
                .SetInitializer( async token =>
                {
                    question = services.QuestionFactory.Create();

                    if( question == null )
                    {
                        services.Logger.Log( "No more questions available." );

                        await services.Network.SendMessageAsync( resources.NoMoreQuestions() );

                        await StopAsync();
                    }
                } )
                .SetBackgroundAction( async token =>
                {
                    var information = resources.NextQuestionAnnouncement( services.Settings.QuestionDelay );
                    await services.Network.SendMessageAsync( information );

                    await services.Scheduler.Delay( "Question", services.Settings.QuestionDelay );

                    await SwitchToAsync( questionState );
                } );

            questionState
                .AddCommandHandler( "stop", stopHandler )
                .SetInitializer( async token =>
                {
                    services.Logger.Log( $"Writing first paragraph of new question {question.Id}" );

                    await services.Network.SendMessageAsync( question.Paragraphs[0] );
                } )
                .SetBackgroundAction( async token =>
                {
                    int index = 1;
                    while( index < question.Paragraphs.Count )
                    {
                        await services.Scheduler.Delay( "Paragraph", services.Settings.ParagraphDelay, question.Speed );
                        if( token.IsCancellationRequested )
                        {
                            return;
                        }

                        await services.Network.SendMessageAsync( question.Paragraphs[index] );
                        index++;
                    }

                    await services.Scheduler.Delay( "Answer", services.Settings.AnswerDelay, question.Speed );
                    if( !token.IsCancellationRequested )
                    {
                        services.Logger.Log( "No correct answer was given." );

                        var answerMessage = resources.NoAnswer( question.AcceptableAnswers[0] );
                        await services.Network.SendMessageAsync( answerMessage );

                        await SwitchToAsync( preQuestionState );
                    }
                } )
                .SetMessageHandler( async ( message, token ) =>
                {
                    if( message.Kind != BotMessageKind.PublicMessage )
                    {
                        // Questions cannot be answered in private, and we don't care about other kinds of messages.
                        return;
                    }

                    var answer = question.AcceptableAnswers.FirstOrDefault( a => string.Equals( message.Text, a, question.AnswersComparison ) );
                    if( answer == null )
                    {
                        // No feedback on wrong answer, to avoid spamming others.
                        return;
                    }

                    services.Logger.Log( "Correct answer!" );

                    // DESIGN: proportional to #users?
                    var increment = 1;
                    var newScore = await services.Scoreboard.IncreaseScoreAsync( message.Sender.Id, increment );

                    var congratulation = resources.Congratulation( message.Sender.Name, answer, newScore );
                    await services.Network.SendMessageAsync( congratulation );

                    await SwitchToAsync( preQuestionState );
                } );
        }
    }
}