using System;

namespace BladesOfTheFallen.Core
{
    public interface IHighScoreRepository
    {
        int Load();
        void Save(int score);
    }

    public sealed class HighScoreService
    {
        private readonly IHighScoreRepository repository;

        public HighScoreService(IHighScoreRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            HighScore = Math.Max(0, repository.Load());
        }

        public int HighScore { get; private set; }

        public bool TryRegister(int score)
        {
            if (score <= HighScore)
            {
                return false;
            }

            HighScore = score;
            repository.Save(HighScore);
            return true;
        }
    }
}
