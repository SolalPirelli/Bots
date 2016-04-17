using System.Linq;
using Bots.Quiz;
using Bots.Quiz.Questions;
using Xunit;

namespace QuizzBot.Tests
{
    public sealed class WQuizzQuestionsTests
    {
        [Fact]
        public void Normal()
        {
            var question = GetQuestion( "What is 2? \\ 1+1" );

            Assert.Null( question.Category );
            Assert.Equal( new[] { "What is 2?" }, question.Paragraphs );
            Assert.Equal( new[] { "1+1" }, question.Answers );
            Assert.Equal( QuestionSpeed.Medium, question.Speed );
        }

        [Fact]
        public void WithCategory()
        {
            var question = GetQuestion( "{Math} What is 2? \\ 1+1" );

            Assert.Equal( "Math", question.Category );
            Assert.Equal( new[] { "What is 2?" }, question.Paragraphs );
            Assert.Equal( new[] { "1+1" }, question.Answers );
        }

        [Fact]
        public void MultipleAnswers()
        {
            var question = GetQuestion( "What is 2? \\ 1+1 \\ 0+2 \\ 2+0" );

            Assert.Null( question.Category );
            Assert.Equal( new[] { "What is 2?" }, question.Paragraphs );
            Assert.Equal( new[] { "1+1", "0+2", "2+0" }, question.Answers );
        }

        [Fact]
        public void Anagram()
        {
            var question = GetQuestion( "#S \\ QWERTY" );

            Assert.Equal( "Anagrammes", question.Category );
            Assert.Equal( 1, question.Paragraphs.Count );
            Assert.Equal( question.Paragraphs[0].OrderBy( c => c ), "EQRTWY" );
            Assert.Equal( new[] { "QWERTY" }, question.Answers );
        }

        [Fact]
        public void Answer_CaseInsensitive()
        {
            var question = GetQuestion( "Question? \\ AnSwEr" );

            Assert.Equal( 0, question.AnswersComparer.Compare( "ANSWER", question.Answers[0] ) );
        }

        [Fact]
        public void Answer_AccentInsensitive()
        {
            var question = GetQuestion( "Question? \\ éào" );

            Assert.Equal( 0, question.AnswersComparer.Compare( "eaô", question.Answers[0] ) );
        }

        [Fact]
        public void Answer_PunctuationInsensitive()
        {
            var question = GetQuestion( "Question? \\ A-B C" );

            Assert.Equal( 0, question.AnswersComparer.Compare( "A B-C", question.Answers[0] ) );
        }

        [Fact]
        public void Compat_NoDelimiter()
        {
            var question = GetQuestion( "What is 2? 1+1" );

            Assert.Null( question.Category );
            Assert.Equal( new[] { "What is 2?" }, question.Paragraphs );
            Assert.Equal( new[] { "1+1" }, question.Answers );
            Assert.Equal( QuestionSpeed.Medium, question.Speed );
        }

        [Fact]
        public void Compat_MessedUpCategoryDelimiter()
        {
            var question = GetQuestion( "{Math What is 2? 1+1" );

            Assert.Null( question.Category );
            Assert.Equal( new[] { "{Math What is 2?" }, question.Paragraphs );
            Assert.Equal( new[] { "1+1" }, question.Answers );
            Assert.Equal( QuestionSpeed.Medium, question.Speed );
        }

        private static QuizQuestion GetQuestion( string line )
        {
            return WQuizzQuestions.Parse( new[] { line } ).Single();
        }
    }
}