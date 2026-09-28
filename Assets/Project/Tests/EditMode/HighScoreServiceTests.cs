using BladesOfTheFallen.Core;
using NUnit.Framework;

public sealed class HighScoreServiceTests
{
    [Test]
    public void TryRegister_PersistsOnlyANewRecord()
    {
        FakeHighScoreRepository repository = new() { StoredScore = 20 };
        HighScoreService service = new(repository);

        bool lowerScoreAccepted = service.TryRegister(10);
        bool recordAccepted = service.TryRegister(30);

        Assert.That(lowerScoreAccepted, Is.False);
        Assert.That(recordAccepted, Is.True);
        Assert.That(service.HighScore, Is.EqualTo(30));
        Assert.That(repository.SaveCount, Is.EqualTo(1));
        Assert.That(repository.StoredScore, Is.EqualTo(30));
    }

    private sealed class FakeHighScoreRepository : IHighScoreRepository
    {
        public int StoredScore { get; set; }
        public int SaveCount { get; private set; }

        public int Load() => StoredScore;

        public void Save(int score)
        {
            StoredScore = score;
            SaveCount++;
        }
    }
}
