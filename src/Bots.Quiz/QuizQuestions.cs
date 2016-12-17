using System;
using System.Collections.Generic;
using System.Linq;
using Bots.Quiz.Infrastructure;

namespace Bots.Quiz
{
    public static class QuizQuestions
    {
        public static Func<QuizQuestion> InfiniteShuffle( IReadOnlyList<QuizQuestion> questions )
        {
            var random = new Random();
            return () => questions[random.Next( 0, questions.Count )];
        }

        public static Func<QuizQuestion> InOrder( IReadOnlyList<QuizQuestion> questions )
        {
            int index = -1;
            return () =>
            {
                index++;
                if( index == questions.Count )
                {
                    return null;
                }

                return questions[index];
            };
        }

        public static IReadOnlyList<QuizQuestion> WithHints( int count, double fraction, IReadOnlyList<QuizQuestion> questions )
        {
            return questions.Select( q =>
                new QuizQuestion( q.Id, q.Category,
                                  q.Paragraphs.Concat( QuestionHints.CreateProportional( q.Answers[0], count, fraction ) ).ToList(),
                                  q.Answers, q.AnswersComparer )
            ).ToList();
        }

        public static IReadOnlyList<QuizQuestion> TakeAll( params IReadOnlyList<QuizQuestion>[] questionLists )
        {
            var all = new List<QuizQuestion>();
            foreach( var list in questionLists )
            {
                all.AddRange( list );
            }
            return all;
        }

        /// <summary>
        /// Format:
        /// 
        /// - Questions are separated by at least two blank lines.
        /// - The first lines of a question contains the question paragraphs, where ___ (three underscores) denote holes.
        /// - Next lines contain hole values (for all paragraphs) separated by commas, 
        ///   followed by -> then the possible answers, also separated by commas.
        /// - If there are no holes, the answers must be preceded by -> anyway.
        /// - Answers may contain parenthesized portions, which are optional.
        /// 
        /// Who ran against Jimmy Carter in 1976?
        /// -> (Gerald (R.)) Ford
        /// 
        /// Who was elected President of ___ in ___?
        /// the United States, 2008 -> (Barack) Obama
        /// France, 2012 -> (François) Hollande
        /// 
        /// In which year was ___ elected President of the United States?
        /// Franklin D. Roosevelt -> 1932, 1936, 1940, 1944
        /// Harry S Truman -> 1948
        /// 
        /// This country was ruled by George W. Bush from 2000 to 2008.
        /// It led the First World during the Cold War.
        /// -> (The) United States, US(A), U.S.(A.)
        /// </summary>
        public static IReadOnlyList<QuizQuestion> ParseRich( string category, IEnumerable<string> lines )
        {
            var questions = new List<QuizQuestion>();
            var index = 0;

            foreach( var group in StringOperations.GroupLines( lines ) )
            {
                if( group.Count < 2 )
                {
                    throw new ArgumentException( $"Invalid group containing only 1 line: {group[0]}" );
                }

                if( !TemplatedString.HasHole( group[0] ) && group.Count > 2 )
                {
                    throw new ArgumentException( $"More than 2 lines, but no holes in the question: {group[0]}" );
                }

                var paragraphs = new List<string>();
                var endedParagraphs = false;

                var lineIndex = 0;
                foreach( var line in group )
                {
                    if( !line.Contains( "->" ) )
                    {
                        if( endedParagraphs )
                        {
                            throw new ArgumentException( $"Line contains no arrow: {line}" );
                        }

                        paragraphs.Add( line );
                        continue;
                    }

                    endedParagraphs = true;
                    if( paragraphs.Count == 0 )
                    {
                        throw new ArgumentException( $"First line contains arrow: {line}" );
                    }

                    var split = StringOperations.Split( line, "->" );
                    if( split.Count != 2 )
                    {
                        throw new ArgumentException( $"Zero or more than one arrow in line: {line}" );
                    }

                    if( split[0].Length == 0 )
                    {
                        if( paragraphs.Any( TemplatedString.HasHole ) )
                        {
                            throw new ArgumentException( $"Statement contains holes, but no values given to fill it: {group[0]}" );
                        }

                        var cleanAnswer = line.Substring( "->".Length );
                        var answers = StringOperations.Split( cleanAnswer, "," ).SelectMany( ComplexString.GetPossibilities ).ToArray();
                        questions.Add( new QuizQuestion( $"{category}_{index}", category, paragraphs, answers ) );
                    }
                    else
                    {
                        var holeValues = StringOperations.Split( split[0], "," );
                        var answers = StringOperations.Split( split[1], "," ).SelectMany( ComplexString.GetPossibilities ).ToArray();
                        var fullParagraphs = TemplatedString.FillHoles( paragraphs, holeValues );
                        questions.Add( new QuizQuestion( $"{category }_{ index}_{lineIndex}", category, fullParagraphs, answers ) );
                    }

                    lineIndex++;
                }

                if( !endedParagraphs )
                {
                    throw new ArgumentException( $"Question without answers: {paragraphs[0]}" );
                }
            }

            return questions;
        }

        /// <summary>
        /// Creates questions from lines in the format used by WQuizz,
        /// a well-known French quiz bot for mIRC: http://wiz0u.free.fr/wquizz/
        /// </summary>
        public static IReadOnlyList<QuizQuestion> ParseWQuizz( IEnumerable<string> lines )
        {
            return WQuizzQuestions.Parse( lines );
        }
    }
}