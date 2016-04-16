using System;
using System.Collections.Generic;

namespace Bots.Quiz.Tests.Infrastructure
{
    public sealed class FakeQuestion : IQuestion
    {
        public string Id { get; }
        public string Category { get; }
        public IReadOnlyList<string> Paragraphs { get; }
        public StringComparison AnswersComparison { get; }
        public IReadOnlyList<string> AcceptableAnswers { get; }
        public QuestionSpeed Speed { get; }

        public FakeQuestion(
            string[] paragraphs, string[] answers,
            string category = "",
            StringComparison answersComparison = StringComparison.OrdinalIgnoreCase,
            QuestionSpeed speed = QuestionSpeed.Medium )
        {
            Id = $"FakeQuestion ({paragraphs[0]}; {Guid.NewGuid().ToString()})";
            Category = category;
            Paragraphs = paragraphs;
            AnswersComparison = answersComparison;
            AcceptableAnswers = answers;
            Speed = speed;
        }
    }
}