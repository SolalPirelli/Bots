namespace Bots.Quiz.Tests.Infrastructure
{
    public sealed class FakeQuestionFactory : IQuestionFactory
    {
        private readonly IQuestion[] _questions;
        private int _index;


        public FakeQuestionFactory( IQuestion[] questions )
        {
            _questions = questions;
            _index = -1;
        }


        public IQuestion Create()
        {
            _index++;
            return _questions[_index];
        }
    }
}