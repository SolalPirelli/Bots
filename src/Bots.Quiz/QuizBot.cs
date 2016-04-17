using System;
using System.Linq;
using System.Text;

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
                },
                stopHandler = async ( command, token ) =>
                {
                    services.Logger.Log( "Pausing" );

                    await services.Network.SendMessageAsync( resources.Pausing() );
                    await SwitchToAsync( waitingState );
                },
                scoreHandler = async ( command, token ) =>
                {
                    services.Logger.Log( "Displaying scoreboard" );

                    var scores = await services.Scoreboard.GetScoresByNameAsync();
                    var orderedScores = scores.OrderByDescending( p => p.Value ).Take( services.Settings.ScoreboardLength );

                    var text = resources.ScoreboardTitle() + Environment.NewLine
                             + string.Join( Environment.NewLine, orderedScores.Select( p => resources.ScoreboardEntry( p.Key, p.Value ) ) );

                    await command.Sender.SendMessageAsync( text );
                };

            var questionsSource = services.Questions.GetEnumerator();
            QuizQuestion question = null;

            AddGlobalCommandHandler( "scores", scoreHandler );

            waitingState
               .AddCommandHandler( "start", startHandler );

            preQuestionState
                .AddCommandHandler( "stop", stopHandler )
                .SetInitializer( async token =>
                {
                    if( questionsSource.MoveNext() )
                    {
                        question = questionsSource.Current;
                    }
                    else
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

                    if( !token.IsCancellationRequested )
                    {
                        await SwitchToAsync( questionState );
                    }
                } );

            questionState
                .AddCommandHandler( "stop", stopHandler )
                .SetInitializer( async token =>
                {
                    services.Logger.Log( $"Writing first paragraph of new question {question.Id}" );

                    var message = resources.Question( question.Category, question.Paragraphs[0] );
                    await services.Network.SendMessageAsync( message );
                } )
                .SetBackgroundAction( async token =>
                {
                    for( int index = 1; index < question.Paragraphs.Count; index++ )
                    {
                        await services.Scheduler.Delay( "Paragraph", services.Settings.ParagraphDelay, question.Speed );
                        if( token.IsCancellationRequested )
                        {
                            return;
                        }

                        var message = resources.QuestionParagraph( question.Paragraphs[index] );
                        await services.Network.SendMessageAsync( message );
                    }

                    await services.Scheduler.Delay( "Answer", services.Settings.AnswerDelay, question.Speed );

                    if( !token.IsCancellationRequested )
                    {
                        services.Logger.Log( "No correct answer was given." );

                        var answerMessage = resources.NoAnswer( question.Answers[0] );
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

                    var candidate = NormalizeAnswer( message.Text );

                    var answer = question.Answers.FirstOrDefault( a => question.AnswersComparer.Compare( candidate, a ) == 0 );
                    if( answer == null )
                    {
                        // No feedback on wrong answer, to avoid spamming others.
                        return;
                    }

                    services.Logger.Log( "Correct answer!" );

                    // DESIGN: proportional to #users?
                    var increment = 1;
                    var newScore = await services.Scoreboard.IncreaseScoreAsync( message.Sender.Id, message.Sender.Name, increment );

                    var congratulation = resources.Congratulation( message.Sender.Name, answer, newScore );
                    await services.Network.SendMessageAsync( congratulation );

                    await SwitchToAsync( preQuestionState );
                } );
        }

        private static string NormalizeAnswer( string answer )
        {
            var builder = new StringBuilder();
            bool wasSpace = true;

            foreach( var character in answer )
            {
                if( char.IsWhiteSpace( character ) )
                {
                    if( !wasSpace )
                    {
                        builder.Append( ' ' );
                    }

                    wasSpace = true;
                }
                else
                {
                    builder.Append( character );
                    wasSpace = false;
                }
            }

            return builder.ToString().TrimEnd();
        }
    }
}