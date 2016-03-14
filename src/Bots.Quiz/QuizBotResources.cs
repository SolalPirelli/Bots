using System;
using System.Globalization;

namespace Bots.Quiz
{
    public static class QuizBotResources
    {
        public sealed class French : IQuizBotResources
        {
            public CultureInfo Culture => new CultureInfo( "fr-FR" );

            public string Started()
                => "Bonjour, je suis le maître du Quiz.\n"
                 + "!start pour commencer.\n"
                 + "!help pour voir la liste des commandes disponibles.";

            public string Stopped()
                => "Au revoir !";

            public string Help()
                => "!start pour commencer.\n"
                 + "!stop pour arrêter.\n"
                 + "!help pour voir cette liste de commandes.";

            public string Pausing()
                => "Le Quiz est en pause.\n"
                 + "!start pour continuer.";

            public string Resuming()
                => "Le Quiz reprend !";

            public string Congratulation( string userName, string answer, long newScore )
                => $"Bravo, {userName} !\n"
                 + $"La réponse était bien '{answer}'.\n"
                 + $"Tu as maintenant {newScore} points.";

            public string NoAnswer( string answer )
                => $"La réponse était {answer}.";

            public string NextQuestionAnnouncement( TimeSpan delay )
                => $"Prochaine question dans {delay.TotalSeconds} secondes...";
        }
    }
}