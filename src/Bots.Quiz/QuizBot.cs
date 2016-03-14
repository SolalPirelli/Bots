using System.Threading.Tasks;
using System.Linq;
using System;

namespace Bots.Quiz
{
    public sealed class QuizBot : Bot<QuizBotServices, IQuizBotResources, QuizBot.WaitingState>
    {
        public QuizBot( QuizBotServices services, IQuizBotResources resources ) : base( services, resources ) { }


        public abstract class StoppableState : State
        {
            public override async Task ProcessCommandAsync( BotCommand command )
            {
                if( command.Action == "stop" )
                {
                    await ExecuteAsync( "Pausing", () => Services.Network.SendMessageAsync( Resources.Pausing() ) );
                    await SwitchToAsync<WaitingState>();
                }
            }
        }


        public sealed class WaitingState : State
        {
            public override async Task ProcessCommandAsync( BotCommand command )
            {
                if( command.Action == "start" )
                {
                    await ExecuteAsync( "Resuming", () => Services.Network.SendMessageAsync( Resources.Resuming() ) );
                    await SwitchToAsync<PreQuestionState>();
                }
            }
        }

        public sealed class PreQuestionState : StoppableState
        {
            protected override async Task InitializeAsync()
            {
                var information = Resources.NextQuestionAnnouncement( Services.Settings.QuestionDelay );
                await ExecuteAsync( "Displaying question delay", () => Services.Network.SendMessageAsync( information ) );
                await Task.Delay( Services.Settings.QuestionDelay );

                await SwitchToAsync<QuestionState>();
            }
        }

        public sealed class QuestionState : StoppableState
        {
            private IQuestion _question;


            protected override async Task InitializeAsync()
            {
                _question = Services.QuestionFactory.Create();

                Services.Logger.Log( $"New question: {_question.Id}" );

                // Ensure that the first paragraph is always sent when initialization is done.
                await ExecuteAsync( "Writing first paragraph", () => Services.Network.SendMessageAsync( _question.Paragraphs[0] ) );

#pragma warning disable CS4014 // Task-returning methods called without await

                // Write question paragraphs
                Task.Factory.StartNew( async () =>
                {
                    int index = 1;
                    while( index < _question.Paragraphs.Count )
                    {
                        await Delay( Services.Settings.ParagraphDelay );
                        if( CancellationToken.IsCancellationRequested )
                        {
                            return;
                        }

                        await ExecuteAsync( "Writing paragraph", () => Services.Network.SendMessageAsync( _question.Paragraphs[index] ) );
                        index++;
                    }

                    await Delay( Services.Settings.AnswerDelay );
                    if( !CancellationToken.IsCancellationRequested )
                    {
                        var answerMessage = Resources.NoAnswer( _question.AcceptableAnswers[0] );
                        await ExecuteAsync( "Writing answer", () => Services.Network.SendMessageAsync( answerMessage ) );

                        await SwitchToAsync<PreQuestionState>();
                    }
                } );

#pragma warning restore CS4014
            }

            public override async Task ProcessMessageAsync( BotMessage message )
            {
                if( message.Kind == MessageKind.Private )
                {
                    // Questions cannot be answered in private.
                    return;
                }

                var answer = _question.AcceptableAnswers.FirstOrDefault( a => string.Equals( message.Text, a, _question.AnswersComparison ) );
                if( answer == null )
                {
                    // No feedback on wrong answer, to avoid spamming others.
                    return;
                }

                // DESIGN: proportional to #users?
                var increment = 1;
                var newScore = await ExecuteAsync( "Increasing score", () => Services.Scoreboard.IncreaseScoreAsync( message.Sender.Id, increment ) );

                var congratulation = Resources.Congratulation( message.Sender.Name, answer, newScore );
                await ExecuteAsync( "Sending congratulation message", () => Services.Network.SendMessageAsync( congratulation ) );

                await SwitchToAsync<PreQuestionState>();
            }


            private Task Delay( TimeSpan originalDelay )
            {
                var multiplier = _question.Speed == QuestionSpeed.Fast ? 0.5
                               : _question.Speed == QuestionSpeed.Medium ? 1.0
                               : 1.5;

                var time = TimeSpan.FromTicks( (long) ( originalDelay.Ticks * multiplier ) );

                return Task.Delay( time );
            }
        }
    }
}