using System;
using System.Threading.Tasks;
using Xunit;

namespace Bots.Tests.Infrastructure
{
    public delegate Task BotAction( Bot bot, FakeNetwork network, FakeScheduler scheduler );

    public static class BotTester
    {
        private static TimeSpan MaxWaitTime = TimeSpan.FromSeconds( 0.1 );

        public static async Task TestAsync( Func<INetwork, IScheduler, Bot> botCreator, params BotAction[] session )
        {
            using( var network = new FakeNetwork() )
            {
                var scheduler = new FakeScheduler();
                var bot = botCreator( network, scheduler );
                var runTask = bot.RunAsync();

                foreach( var action in session )
                {
                    await action( bot, network, scheduler );
                }

                await runTask;

                Assert.Empty( network.SentMessages );
            }
        }

        public static BotAction Wait( string eventId )
        {
            return ( _, __, sched ) =>
            {
                sched.Advance( eventId );
                // Give some time to keep doing the action interrupted by a delay
                return Task.Delay( MaxWaitTime );
            };
        }

        public static BotAction BotSays( string text )
        {
            return async ( _, network, __ ) =>
            {
                if( network.SentMessages.Count == 0 )
                {
                    // To make sure the bot has has time to process the message
                    await Task.Delay( MaxWaitTime );
                }

                Assert.True( network.SentMessages.Count > 0, "No messages, expected: " + text );

                var message = network.SentMessages.Dequeue();
                Assert.Null( message.Target );
                Assert.Equal( text, message.Text );
            };
        }

        public static BotAction BotSaysPrivately( string userName, string text )
        {
            return async ( _, network, __ ) =>
            {
                if( network.SentMessages.Count == 0 )
                {
                    // To make sure the bot has has time to process the message
                    await Task.Delay( MaxWaitTime );
                }

                Assert.True( network.SentMessages.Count > 0, "No messages, expected: " + text );

                var message = network.SentMessages.Dequeue();
                Assert.Equal( userName, message.Target.Name );
                Assert.Equal( text, message.Text );
            };
        }

        public static UserActionBuilder User( string name )
        {
            return new UserActionBuilder( name );
        }

        public static BotAction ForceStop()
        {
            return ( bot, _, __ ) => bot.StopAsync();
        }


        public sealed class UserActionBuilder
        {
            private readonly string _userName;

            internal UserActionBuilder( string userName )
            {
                _userName = userName;
            }

            public BotAction Says( string text )
            {
                return Says( text, true );
            }

            public BotAction SaysPrivately( string text )
            {
                return Says( text, false );
            }

            private BotAction Says( string text, bool isPublic )
            {
                return ( _, network, __ ) =>
                {
                    network.SendMessage( network.GetUser( _userName ), text, isPublic );
                    // Give some time to process the message
                    return Task.Delay( MaxWaitTime );
                };
            }
        }
    }
}