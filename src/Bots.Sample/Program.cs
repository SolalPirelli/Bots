using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bots.Networks.Discord;
using Bots.Quiz;
using Bots.Quiz.Questions;
using Bots.Quiz.Scoreboards;

namespace Bots.Sample
{
    public class Program
    {
        public static void Main( string[] args )
        {
            DoWork().Wait();
        }

        private static async Task DoWork()
        {
            Console.Write( "Auth token:" );
            var authToken = Console.ReadLine();

            Console.Write( "Channel ID:" );
            var channelId = Console.ReadLine();

            var services = new QuizBotServices(
                network: new DiscordNetwork( new DiscordNetworkConfig(
                    authenticationToken: authToken,
                    botDescription: "Quizz",
                    channelId: channelId
                ) ),

                questions: QuizQuestions.WithHints( 1, 0.25,
                    QuizQuestions.InfiniteShuffle(
                        WQuizzQuestions.Parse( File.ReadLines( @"X:\wquizz.txt", Encoding.GetEncoding( 1252 ) ) )
                    )
                ),

                scoreboard: new FileScoreboard( @"X:\quiz_scores.txt" ),

                settings: new QuizBotSettings(
                    paragraphDelay: TimeSpan.FromSeconds( 20 ),
                    answerDelay: TimeSpan.FromSeconds( 40 ),
                    questionDelay: TimeSpan.FromSeconds( 15 )
                )
            );

            var resources = new QuizBotResources.French();

            var bot = new QuizBot( services, resources );

            try
            {
                Console.WriteLine( "Running" );
                await bot.RunAsync();
                Console.WriteLine( "Finished" );
            }
            finally
            {
                await bot.StopAsync();
            }
        }
    }
}