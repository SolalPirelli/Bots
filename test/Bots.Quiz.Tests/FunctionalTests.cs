using System;
using System.Globalization;
using System.Threading.Tasks;
using Bots.Tests.Infrastructure;
using Bots.Quiz.Tests.Infrastructure;
using Xunit;
using static Bots.Tests.Infrastructure.BotTester;

namespace Bots.Quiz.Tests
{
    public sealed class FunctionalTests
    {
        [Fact]
        public Task Nothing()
        {
            return TestAsync( CreateBot(),
                BotSays( "Start" ),
                ForceStop(),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task Start()
        {
            return TestAsync( CreateBot(),
                BotSays( "Start" ),
                User( "user" ).Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task Pause()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            var user = User( "user" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                user.Says( "!stop" ),
                BotSays( "Pause" ),
                ForceStop(),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task PauseThenContinue()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            var user = User( "user" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                user.Says( "!stop" ),
                BotSays( "Pause" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task NobodyAnswers()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                User( "user" ).Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Hello?" ),
                Wait( "Answer" ),
                BotSays( "It was World" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task UserAnswersCorrectly()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            var user = User( "user" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Hello?" ),
                user.Says( "World" ),
                BotSays( "Congrats user on World with score 1" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task UserAnswersBeforeQuestion()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            var user = User( "user" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                user.Says( "World" ),
                Wait( "Question" ),
                BotSays( "Hello?" ),
                Wait( "Answer" ),
                BotSays( "It was World" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task QuestionWithMultipleParagraphs()
        {
            var question = new FakeQuestion( new[] { "1", "2", "3" }, new[] { "4!" } );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                User( "user" ).Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "1" ),
                Wait( "Paragraph" ),
                BotSays( "2" ),
                Wait( "Paragraph" ),
                BotSays( "3" ),
                Wait( "Answer" ),
                BotSays( "It was 4!" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task TwoUsersCompeteInSingleQuestion()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            var alice = User( "alice" );
            var bob = User( "bob" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                alice.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Hello?" ),
                alice.Says( "Hello" ),
                bob.Says( "World" ),
                BotSays( "Congrats bob on World with score 1" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task TwoUsersCompeteInThreeQuestions()
        {
            var questions = new[]
            {
                new FakeQuestion( new[] { "Q1?" }, new[] { "A1" } ),
                new FakeQuestion( new[] { "Q2?" }, new[] { "A2" } ),
                new FakeQuestion( new[] { "Q3?" }, new[] { "A3" } )
            };
            var alice = User( "alice" );
            var bob = User( "bob" );

            return TestAsync( CreateBot( questions ),
                BotSays( "Start" ),
                alice.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Q1?" ),
                alice.Says( "A1" ),
                BotSays( "Congrats alice on A1 with score 1" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Q2?" ),
                alice.Says( "A3" ),
                bob.Says( "A2" ),
                BotSays( "Congrats bob on A2 with score 1" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Q3?" ),
                alice.Says( "A3" ),
                BotSays( "Congrats alice on A3 with score 2" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }


        private static Func<INetwork, IScheduler, QuizBot> CreateBot( params FakeQuestion[] questions )
        {
            var settings = new QuizBotSettings(
                paragraphDelay: TimeSpan.FromMilliseconds( 100 ),
                answerDelay: TimeSpan.FromMilliseconds( 200 ),
                questionDelay: TimeSpan.FromMilliseconds( 300 )
            );
            return ( net, sched ) => new QuizBot(
                new QuizBotServices( net, new FakeLogger(), sched,
                                     new FakeQuestionFactory( questions ), new FakeQuizScoreboard(), settings ),
                new FakeQuizBotResources()
            );
        }

        private sealed class FakeQuizBotResources : QuizBotResources
        {
            public override CultureInfo Culture => CultureInfo.InvariantCulture;

            public override string Started()
                => "Start";

            public override string Stopped()
                => "Stop";

            public override string Info()
                => "Info";

            public override string Help()
                => "Help";

            public override string Pausing()
                => "Pause";

            public override string Resuming()
                => "Resume";

            public override string NoAnswer( string answer )
                => $"It was {answer}";

            public override string Congratulation( string userName, string answer, long newScore )
                => $"Congrats {userName} on {answer} with score {newScore}";

            public override string NextQuestionAnnouncement( TimeSpan delay )
                => $"Next in {delay.TotalMilliseconds}";

            public override string NoMoreQuestions()
                => "End";
        }
    }
}