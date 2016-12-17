using System;
using System.Globalization;
using static Bots.Quiz.Infrastructure.RandomOperations;

namespace Bots.Quiz
{
    public abstract class QuizBotResources : BotResources
    {
        public abstract CultureInfo Culture { get; }

        public abstract string Pausing();
        public abstract string Resuming();
        public abstract string ScoreboardTitle();
        public abstract string ScoreboardEntry( string userName, long score );
        public abstract string Question( string category, string firstParagraph );
        public abstract string QuestionParagraph( string paragraph );
        public abstract string Congratulation( string userName, string answer, long newScore );
        public abstract string NoAnswer( string answer );
        public abstract string NextQuestionAnnouncement( TimeSpan delay );
        public abstract string NoMoreQuestions();


        /// <summary>
        /// French quiz bot.
        /// </summary>
        public sealed class French : QuizBotResources
        {
            public override CultureInfo Culture => new CultureInfo( "fr-FR" );

            public override string Started()
                => "Bonjour, je suis le maître du quizz." + Environment.NewLine
                 + "**!start** pour commencer." + Environment.NewLine
                 + "!help pour voir la liste des commandes disponibles.";

            public override string Stopped()
                => "Au revoir !";

            public override string Info()
                => "Je suis un bot de quizz.";

            public override string Help()
                => "**!start** pour commencer." + Environment.NewLine
                 + "**!stop** pour arrêter." + Environment.NewLine
                 + "**!scores** pour voir les scores." + Environment.NewLine
                 + "**!help** pour voir cette liste de commandes.";

            public override string Pausing()
                => "**Le quizz est en pause**." + Environment.NewLine
                 + "!start pour continuer.";

            public override string Resuming()
                => "Le quizz reprend !";

            public override string ScoreboardTitle()
                => "Meilleurs scores :";

            public override string ScoreboardEntry( string userName, long score )
                => $"**{userName}** : {score}";

            public override string Question( string category, string firstParagraph )
                => category == null
                ? $"**{firstParagraph}**"
                : $"*{category}*: **{firstParagraph}**";

            public override string QuestionParagraph( string paragraph )
                => $"**{paragraph}**";

            public override string Congratulation( string userName, string answer, long newScore )
                => $"Bravo, **{userName}** !" + Environment.NewLine
                 + $"La réponse était bien **{answer}**." + Environment.NewLine
                 + $"Tu as maintenant {newScore} points.";

            public override string NoAnswer( string answer )
                => $"La réponse était **{answer}**.";

            public override string NextQuestionAnnouncement( TimeSpan delay )
                => $"Prochaine question dans *{delay.TotalSeconds}* secondes...";

            public override string NoMoreQuestions()
                => "Je n'ai plus de questions..." + Environment.NewLine
                 + "Au revoir !";
        }

        /// <summary>
        /// Quiz bot imitating the famous French quiz show host, Julien Lepers.
        /// </summary>
        public sealed class JulienLepers : QuizBotResources
        {
            public override CultureInfo Culture => new CultureInfo( "fr-FR" );

            public override string Started()
                => Pick(
                    "On s'installe, on met le son plus fort...",
                    "Bonsoir mes amis !",
                    "Bonsoir à tous !",
                    "Bonsoir mes amis, bonsoir à tous !",
                    "C'est parti avec ces candidats-là !",
                    "Bon courage !",
                    "Allez !"
                    ) + Environment.NewLine
                 + "*(!start pour commencer, !help pour la liste des commandes)*";

            public override string Stopped()
                => "Au revoir !";

            public override string Info()
                => "Je suis un présentateur français, j'anime Questions pour un Champion, je suis, je suis... ?";

            public override string Help()
                => "*!start* pour commencer." + Environment.NewLine
                 + "*!stop* pour arrêter." + Environment.NewLine
                 + "*!scores* pour voir les scores." + Environment.NewLine
                 + "*!help* pour voir cette liste de commandes.";

            public override string Pausing()
                => "On se retrouve après la pub.";

            public override string Resuming()
                => "C'est parti !";

            public override string ScoreboardTitle()
                => "Les meilleurs :";

            public override string ScoreboardEntry( string userName, long score )
                => $"**{userName}** : {score}";

            public override string Question( string category, string firstParagraph )
            {
                if( category == null )
                {
                    return Pick(
                       $"Top... **{firstParagraph}**",
                       $"**{firstParagraph}**"
                   );
                }

                return Pick(
                       $"Dans la catégorie *{category}*, **{firstParagraph}**",
                       $"En *{category}*, **{firstParagraph}**",
                       $"Nous avons *{category}*... Top! **{firstParagraph}**",
                       $"*{category}*... Top! **{firstParagraph}**"
                );
            }
            public override string QuestionParagraph( string paragraph )
                => $"**{paragraph}**";

            public override string Congratulation( string userName, string answer, long newScore )
            {
                var answerAndScore = Environment.NewLine + $"**{answer}**.* ({newScore} points)*";
                var nameAndScore = Environment.NewLine + $"*({userName}, {newScore} points)*";
                var score = Environment.NewLine + $"*({newScore} points)*";

                return Pick(
                    $"Bien sûr, **{userName}**." + answerAndScore,
                    $"Bien sûr, oui oui, **{userName}**." + answerAndScore,
                    $"Ah oui, **{userName}." + answerAndScore,
                    $"C'est une bonne réponse de **{userName}**." + answerAndScore,

                    $"Oui bien sûr, **{answer}**." + nameAndScore,
                    $"Ah oui oui oui oui ! **{answer}** !" + nameAndScore,
                    $"Oui oui oui oui oui, **{answer}**." + nameAndScore,
                    $"C'est oui ! {answer} !" + nameAndScore,
                    $"Oui, **{answer}**, d'accord." + nameAndScore,
                    $"**{answer}**, voilà..." + nameAndScore,
                    $"**{answer}**, voilà, d'accord." + nameAndScore,
                    $"Voilà, **{answer}**." + nameAndScore,
                    $"Mais bien sûr, **{answer}** !" + nameAndScore,
                    $"Mais oui, d'accord, **{answer}**." + nameAndScore,
                    $"**{answer}**, d'accord." + nameAndScore,
                    $"**{answer}**, bien sûr !" + nameAndScore,
                    $"**{answer}** !" + nameAndScore,
                    $"Exactement, **{answer}**, d'accord." + nameAndScore,
                    $"**{answer}**, oui !" + nameAndScore,
                    $"**{answer}**, ça c'est bon." + nameAndScore,
                    $"**{answer}** !" + nameAndScore,
                    $"Bien sûr ! **{answer}**." + nameAndScore,
                    $"Exactement, **{answer}**." + nameAndScore,
                    $"Oh oui ! **{answer}** !" + nameAndScore,
                    $"C'est **{answer}** !" + nameAndScore,
                    $"**{answer}**, très bien." + nameAndScore,

                    $"**{answer}**, ah oui, bien joué **{userName}**." + score,
                    $"**{answer}**, **{userName}** a raison." + score,
                    $"**{answer}**, très bonne réponse de **{userName}** qui s'en sort bien." + score,
                    $"**{answer}**, bien joué **{userName}**." + score,
                    $"Oui oui **{answer}**, **{userName}**." + score
                );
            }

            public override string NoAnswer( string answer )
                => Pick(
                    $"Non non... **{answer}**",
                    $"Non, ça c'est non... **{answer}**.",
                    $"C'est pas ça du tout... c'est **{answer}**",
                    $"... **{answer}**",
                    $"... **{answer}**, {answer}."
                    );

            public override string NextQuestionAnnouncement( TimeSpan delay )
                => $"*(prochaine question dans {delay.TotalSeconds} secondes)*";

            public override string NoMoreQuestions()
                => "C'est fini.";
        }
    }
}