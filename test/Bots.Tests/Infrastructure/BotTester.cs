using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bots.Tests.Infrastructure
{
    /// <summary>
    /// Tests bots with sessions in the following format:
    /// 
    /// <code>
    /// user: message
    /// user@id: message
    /// from>to: private message
    /// wait 5
    /// </code>
    /// 
    /// The wait times are in milliseconds.
    /// The bot itself is named <c>bot</c>.
    /// </summary>
    /// <typeparam name="TBot"></typeparam>
    public static class BotTester
    {
        public static async Task TestAsync<TServices, TResources, TInitialState>(
            Func<INetwork, Bot<TServices, TResources, TInitialState>> botCreator, string session )
            where TServices : IBotServices
            where TResources : IBotResources
            where TInitialState : Bot<TServices, TResources, TInitialState>.State, new()
        {
            var actualMessages = new List<BotMessageInfo>();
            var expectedMessages = new List<BotMessageInfo>();

            var network = new FakeNetwork();
            var botUser = network.GetUser( "bot" );

            network.MessageProcessor = m =>
            {
                actualMessages.Add( new BotMessageInfo( botUser, null, m ) );
                return Task.CompletedTask;
            };
            Func<IUser, Func<string, Task>> userMessageProcessor = u => m =>
            {
                actualMessages.Add( new BotMessageInfo( u, botUser, m ) );
                return Task.CompletedTask;
            };

            var bot = botCreator( network );
            var runTask = bot.RunAsync();

            foreach( var line in session.Split( new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ) )
            {
                if( line.StartsWith( "wait" ) )
                {
                    int millis = int.Parse( line.Split()[1] );
                    await Task.Delay( TimeSpan.FromMilliseconds( millis ) );
                    continue;
                }

                int userSepIndex = line.IndexOf( ':' );
                var name = line.Substring( 0, userSepIndex );

                FakeUser from;
                FakeUser to;
                int privateSepIndex = name.IndexOf( '>' );
                if( privateSepIndex == -1 )
                {
                    from = GetUser( network, name );
                    to = null;
                }
                else
                {
                    var fromName = name.Substring( 0, privateSepIndex );
                    var toName = name.Substring( privateSepIndex + 1 );

                    from = GetUser( network, fromName );
                    to = GetUser( network, toName );
                }

                var text = line.Substring( userSepIndex + 1 ).Trim();

                if( from == botUser )
                {
                    expectedMessages.Add( new BotMessageInfo( from, to, text ) );
                }
                else
                {
                    from.SendMessage( text, to == null ? MessageKind.Public : MessageKind.Private );
                    from.MessageProcessor = userMessageProcessor( from );
                }
                if( to != null )
                {
                    to.MessageProcessor = userMessageProcessor( to );
                }
            }

            await bot.StopAsync();

            await runTask;

            Assert.Equal( PrintMessages( expectedMessages ), PrintMessages( actualMessages ) );
        }

        private static FakeUser GetUser( FakeNetwork network, string fullName )
        {
            int idSepIndex = fullName.IndexOf( '@' );
            if( idSepIndex != -1 )
            {
                var id = fullName.Substring( 0, idSepIndex );
                var name = fullName.Substring( idSepIndex + 1 );

                var user = network.GetUser( id );
                user.Name = name;
                return user;
            }

            return network.GetUser( fullName );
        }

        private static string PrintMessages( IEnumerable<BotMessageInfo> messages )
        {
            return string.Join( Environment.NewLine, messages.Select( m => m.ToString() ) );
        }

        private sealed class BotMessageInfo
        {
            public IUser Sender { get; }
            public IUser Receiver { get; }
            public string Text { get; }


            public BotMessageInfo( IUser sender, IUser receiver, string text )
            {
                Sender = sender;
                Receiver = receiver;
                Text = text;
            }


            public override bool Equals( object obj )
            {
                var other = obj as BotMessageInfo;
                return other != null
                    && other.Sender == Sender
                    && other.Receiver == Receiver
                    && other.Text == Text;
            }

            public override int GetHashCode()
            {
                int hash = Sender.GetHashCode();
                hash = 31 * hash + ( Receiver == null ? 0 : Receiver.GetHashCode() );
                return 31 * hash + Text.GetHashCode();
            }

            public override string ToString()
            {
                if( Receiver == null )
                {
                    return $"{Sender.Name}@{Sender.Id}: {Text}";
                }

                return $"{Sender.Name}@{Sender.Id} > {Receiver.Name}@{Receiver.Id}: {Text}";
            }
        }
    }
}