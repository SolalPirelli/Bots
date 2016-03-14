using System;
using System.Globalization;
using System.Threading.Tasks;
using Bots.Tests.Infrastructure;
using Bots.Quiz.Tests.Infrastructure;
using Xunit;

namespace Bots.Quiz.Tests
{
    public sealed class FunctionalTests
    {
        [Fact]
        public Task Nothing()
        {
            return BotTester.TestAsync( CreateBot(), @"
bot: Start
bot: Stop
" );
        }

        [Fact]
        public Task Start()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            return BotTester.TestAsync( CreateBot( question ), @"
bot: Start
user: !start
bot: Resume
bot: Next in 300
wait 350
bot: Hello?
bot: Stop
" );
        }

        [Fact]
        public Task Pause()
        {
            return BotTester.TestAsync( CreateBot(), @"
bot: Start
user: !start
bot: Resume
bot: Next in 300
user: !stop
bot: Pause
bot: Stop
" );
        }

        [Fact]
        public Task PauseThenContinue()
        {
            return BotTester.TestAsync( CreateBot(), @"
bot: Start
user: !start
bot: Resume
bot: Next in 300
user: !stop
bot: Pause
user: !start
bot: Resume
bot: Next in 300
bot: Stop
" );
        }

        [Fact]
        public Task NobodyAnswers()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            return BotTester.TestAsync( CreateBot( question ), @"
bot: Start
user: !start
bot: Resume
bot: Next in 300
wait 350
bot: Hello?
wait 250
bot: It was World
bot: Next in 300
bot: Stop
" );
        }

        [Fact]
        public Task UserAnswersCorrectly()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            return BotTester.TestAsync( CreateBot( question ), @"
bot: Start
user: !start
bot: Resume
bot: Next in 300
wait 350
bot: Hello?
user: World
bot: Congrats user on World with score 1
bot: Next in 300
bot: Stop
" );
        }

        [Fact]
        public Task UserAnswersBeforeQuestion()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            return BotTester.TestAsync( CreateBot( question ), @"
bot: Start
user: !start
bot: Resume
bot: Next in 300
user: World
wait 350
bot: Hello?
wait 250
bot: It was World
bot: Next in 300
bot: Stop
" );
        }

        [Fact]
        public Task TwoUsersCompeteInSingleQuestion()
        {
            var question = new FakeQuestion( new[] { "Hello?" }, new[] { "World" } );
            return BotTester.TestAsync( CreateBot( question ), @"
bot: Start
alice: !start
bot: Resume
bot: Next in 300
wait 350
bot: Hello?
alice: Hello
charles: World
bot: Congrats charles on World with score 1
bot: Next in 300
bot: Stop
" );
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
            return BotTester.TestAsync( CreateBot( questions ), @"
bot: Start
alice: !start
bot: Resume
bot: Next in 300
wait 350
bot: Q1?
alice: A1
bot: Congrats alice on A1 with score 1
bot: Next in 300
wait 350
bot: Q2?
alice: A3
charles: A2
bot: Congrats charles on A2 with score 1
bot: Next in 300
wait 350
bot: Q3?
alice: A3
bot: Congrats alice on A3 with score 2
bot: Next in 300
bot: Stop
" );
        }


        private static Func<INetwork, QuizBot> CreateBot( params FakeQuestion[] questions )
        {
            var settings = new QuizBotSettings( TimeSpan.FromMilliseconds( 100 ), TimeSpan.FromMilliseconds( 200 ), TimeSpan.FromMilliseconds( 300 ) );
            return net => new QuizBot(
                new QuizBotServices( net, new FakeLogger(), new FakeQuestionFactory( questions ), new FakeQuizScoreboard(), settings ),
                new FakeQuizBotResources()
            );
        }

        private sealed class FakeQuizBotResources : IQuizBotResources
        {
            public CultureInfo Culture => CultureInfo.InvariantCulture;

            public string Started()
                => "Start";

            public string Stopped()
                => "Stop";

            public string Help()
                => "Help";

            public string Pausing()
                => "Pause";

            public string Resuming()
                => "Resume";

            public string NoAnswer( string answer )
                => $"It was {answer}";

            public string Congratulation( string userName, string answer, long newScore )
                => $"Congrats {userName} on {answer} with score {newScore}";

            public string NextQuestionAnnouncement( TimeSpan delay )
                => $"Next in {delay.TotalMilliseconds}";

        }
    }
}