using System;
using System.Globalization;

namespace Bots.Quiz
{
    public static class LocalizedQuizBotResources
    {
        public sealed class French : QuizBotResources
        {
            public override CultureInfo Culture => new CultureInfo( "fr-FR" );

            public override string Started()
                => "Bonjour, je suis le maître du quizz.\n"
                 + "!start pour commencer.\n"
                 + "!help pour voir la liste des commandes disponibles.";

            public override string Stopped()
                => "Au revoir !";

            public override string Info()
                => "Je suis un bot de quizz.";

            public override string Help()
                => "!start pour commencer.\n"
                 + "!stop pour arrêter.\n"
                 + "!help pour voir cette liste de commandes.";

            public override string Pausing()
                => "Le quizz est en pause.\n"
                 + "!start pour continuer.";

            public override string Resuming()
                => "Le quizz reprend !";

            public override string Congratulation( string userName, string answer, long newScore )
                => $"Bravo, {userName} !\n"
                 + $"La réponse était bien '{answer}'.\n"
                 + $"Tu as maintenant {newScore} points.";

            public override string NoAnswer( string answer )
                => $"La réponse était {answer}.";

            public override string NextQuestionAnnouncement( TimeSpan delay )
                => $"Prochaine question dans {delay.TotalSeconds} secondes...";

            public override string NoMoreQuestions()
                => "Je n'ai plus de questions !\n"
                 + "Au revoir !";
        }
    }
}