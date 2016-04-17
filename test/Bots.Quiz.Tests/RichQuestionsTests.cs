using System;
using Bots.Quiz.Questions;
using Xunit;

namespace QuizzBot.Tests
{
    public sealed class RichQuestionsTests
    {
        public sealed class Templated
        {
            [Fact]
            public void Simple()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> 1+1"
                } );

                Assert.Equal( 1, questions.Count );
                Assert.Equal( "Categ", questions[0].Category );
                Assert.Equal( new[] { "What is 2?" }, questions[0].Paragraphs );
                Assert.Equal( new[] { "1+1" }, questions[0].Answers );
            }

            [Fact]
            public void MultipleAnswers()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> 1+1, 0+2, 2+0"
                } );

                Assert.Equal( 1, questions.Count );
                Assert.Equal( "Categ", questions[0].Category );
                Assert.Equal( new[] { "What is 2?" }, questions[0].Paragraphs );
                Assert.Equal( new[] { "1+1", "0+2", "2+0" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedAnswer_OptionalBeginning()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> (0+(0+))2"
                } );

                Assert.Equal( new[] { "0+0+2", "0+2", "2" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedAnswer_OptionalMiddle()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> 1(+0(+0)+0)+1"
                } );

                Assert.Equal( new[] { "1+0+0+0+1", "1+0+0+1", "1+1" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedAnswer_OptionalEnd()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> 2(+0(+0))"
                } );

                Assert.Equal( new[] { "2+0+0", "2+0", "2" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedAnswer_ComplexOptional()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> (0+(0+))1(+0(+0))+1((+0)+0)"
                } );

                Assert.Equal( new[] {
                    "0+0+1+0+0+1+0+0",
                    "0+0+1+0+0+1+0",
                    "0+0+1+0+0+1",
                    "0+0+1+0+1+0+0",
                    "0+0+1+0+1+0",
                    "0+0+1+0+1",
                    "0+0+1+1+0+0",
                    "0+0+1+1+0",
                    "0+0+1+1",
                    "0+1+0+0+1+0+0",
                    "0+1+0+0+1+0",
                    "0+1+0+0+1",
                    "0+1+0+1+0+0",
                    "0+1+0+1+0",
                    "0+1+0+1",
                    "0+1+1+0+0",
                    "0+1+1+0",
                    "0+1+1",
                    "1+0+0+1+0+0",
                    "1+0+0+1+0",
                    "1+0+0+1",
                    "1+0+1+0+0",
                    "1+0+1+0",
                    "1+0+1",
                    "1+1+0+0",
                    "1+1+0",
                    "1+1",
                }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedAnswers()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> 2(+0), (0+)1+1"
                } );

                Assert.Equal( new[] { "2+0", "2", "0+1+1", "1+1" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedQuestion_SingleHole()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is ___?",
                    "2 -> 1+1"
                } );

                Assert.Equal( 1, questions.Count );
                Assert.Equal( "Categ_0_0", questions[0].Id );
                Assert.Equal( "Categ", questions[0].Category );
                Assert.Equal( new[] { "What is 2?" }, questions[0].Paragraphs );
                Assert.Equal( new[] { "1+1" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedQuestion_MultipleValues()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is ___?",
                    "2 -> 1+1",
                    "4 -> 2+2",
                } );

                Assert.Equal( 2, questions.Count );

                Assert.Equal( "Categ_0_0", questions[0].Id );
                Assert.Equal( "Categ", questions[0].Category );
                Assert.Equal( new[] { "What is 2?" }, questions[0].Paragraphs );
                Assert.Equal( new[] { "1+1" }, questions[0].Answers );

                Assert.Equal( "Categ_0_1", questions[1].Id );
                Assert.Equal( "Categ", questions[1].Category );
                Assert.Equal( new[] { "What is 4?" }, questions[1].Paragraphs );
                Assert.Equal( new[] { "2+2" }, questions[1].Answers );
            }

            [Fact]
            public void TemplatedQuestion_MultipleAnswers()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is ___?",
                    "2 -> 1+1, 0+2"
                } );

                Assert.Equal( new[] { "1+1", "0+2" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedQuestion_MultipleHoles()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is ___+___?",
                    "1,1 -> 2"
                } );

                Assert.Equal( new[] { "What is 1+1?" }, questions[0].Paragraphs );
                Assert.Equal( new[] { "2" }, questions[0].Answers );
            }

            [Fact]
            public void TemplatedQuestion_Complex()
            {
                var questions = RichQuestions.Parse( "Categ", new[]
                {
                    "What is ___+___+___?",
                    "1, 2, 3 -> 6, 4+2",
                    "3, 2, 1 -> 3+3",
                    "1, 1, 1 -> (0+)3, 1+2"
                } );

                Assert.Equal( 3, questions.Count );

                Assert.Equal( "Categ_0_0", questions[0].Id );
                Assert.Equal( "Categ", questions[0].Category );
                Assert.Equal( new[] { "What is 1+2+3?" }, questions[0].Paragraphs );
                Assert.Equal( new[] { "6", "4+2" }, questions[0].Answers );

                Assert.Equal( "Categ_0_1", questions[1].Id );
                Assert.Equal( "Categ", questions[1].Category );
                Assert.Equal( new[] { "What is 3+2+1?" }, questions[1].Paragraphs );
                Assert.Equal( new[] { "3+3" }, questions[1].Answers );

                Assert.Equal( "Categ_0_2", questions[2].Id );
                Assert.Equal( "Categ", questions[2].Category );
                Assert.Equal( new[] { "What is 1+1+1?" }, questions[2].Paragraphs );
                Assert.Equal( new[] { "0+3", "3", "1+2" }, questions[2].Answers );
            }

            [Fact]
            public void CannotHaveMultipleAnswersOnDifferentLines()
            {
                Assert.Throws<ArgumentException>( () => RichQuestions.Parse( "Categ", new[]
                {
                    "What is 2?",
                    "-> 1+1",
                    "-> 0+2",
                    "-> 2+0"
                } ) );
            }
        }
    }
}