using System;
using System.Globalization;
using System.Threading.Tasks;
using Bots.Tests.Infrastructure;
using Bots.Quiz.Tests.Infrastructure;
using Xunit;
using static Bots.Tests.Infrastructure.BotTester;

namespace Bots.Quiz.Tests
{
    public sealed class QuizBotTests
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
            var question = Question( "Hello?", "World" );
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
        public Task Pause_DoesNotKeepGoing()
        {
            var question = Question( "Hello?", "World" );
            var user = User( "user" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                user.Says( "!stop" ),
                BotSays( "Pause" ),
                Wait( "Question" ),
                ForceStop(),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task PauseThenContinue()
        {
            var question = Question( "Hello?", "World" );
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
            var question = Question( "Hello?", "World" );

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
            var question = Question( "Hello?", "World" );
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
        public Task UserAnswersCorrectlyWithTooManySpaces()
        {
            var question = Question( "Hello?", "The World" );
            var user = User( "user" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Hello?" ),
                user.Says( "  The   World " ),
                BotSays( "Congrats user on The World with score 1" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task UserForgetsSpace()
        {
            var question = Question( "Hello?", "The World" );
            var user = User( "user" );

            return TestAsync( CreateBot( question ),
                BotSays( "Start" ),
                user.Says( "!start" ),
                BotSays( "Resume" ),
                BotSays( "Next in 300" ),
                Wait( "Question" ),
                BotSays( "Hello?" ),
                user.Says( "TheWorld" ),
                Wait( "Answer" ),
                BotSays( "It was The World" ),
                BotSays( "End" ),
                BotSays( "Stop" )
            );
        }

        [Fact]
        public Task UserAnswersBeforeQuestion()
        {
            var question = Question( "Hello?", "World" );
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
            var question = new QuizQuestion( "Q", null, new[] { "1", "2", "3" }, new[] { "4!" } );

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
            var question = Question( "Hello?", "World" );
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
                Question( "Q1?",  "A1" ),
                Question( "Q2?", "A2" ),
                Question( "Q3?", "A3" )
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

        [Fact]
        public async Task Scoreboard()
        {
            var scoreboard = new FakeQuizScoreboard();
            await scoreboard.IncreaseScoreAsync( "A", "A", 100 );
            await scoreboard.IncreaseScoreAsync( "B", "B", 10 );
            await scoreboard.IncreaseScoreAsync( "C", "C", 20 );
            await scoreboard.IncreaseScoreAsync( "D", "D", 50 );

            var settings = new QuizBotSettings(
                scoreboardLength: 3
            );

            Func<INetwork, IScheduler, QuizBot> creator = ( net, sched ) => new QuizBot(
                new QuizBotServices( net, () => null, scoreboard, settings, new FakeLogger(), sched ),
                new FakeQuizBotResources()
            );

            await TestAsync( creator,
                BotSays( "Start" ),
                User( "A" ).Says( "!scores" ),
                BotSaysPrivately( "A",
                    "Scoreboard" + Environment.NewLine
                  + "A: 100" + Environment.NewLine
                  + "D: 50" + Environment.NewLine
                  + "C: 20" ),
                ForceStop(),
                BotSays( "Stop" )
            );
        }

        private static QuizQuestion Question( string question, string answer )
        {
            return new QuizQuestion( question, null, new[] { question }, new[] { answer } );
        }

        private static Func<INetwork, IScheduler, QuizBot> CreateBot( params QuizQuestion[] questions )
        {
            var settings = new QuizBotSettings(
                paragraphDelay: TimeSpan.FromMilliseconds( 100 ),
                answerDelay: TimeSpan.FromMilliseconds( 200 ),
                questionDelay: TimeSpan.FromMilliseconds( 300 )
            );
            return ( net, sched ) => new QuizBot(
                new QuizBotServices( net, QuizQuestions.InOrder( questions ), new FakeQuizScoreboard(), settings, new FakeLogger(), sched ),
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

            public override string ScoreboardTitle()
                => "Scoreboard";

            public override string ScoreboardEntry( string userName, long score )
                => $"{userName}: {score}";

            public override string Question( string category, string firstParagraph )
                => category == null ? firstParagraph : $"[{category}] {firstParagraph}";

            public override string QuestionParagraph( string paragraph )
                => paragraph;

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