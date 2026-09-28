using BladesOfTheFallen.Core;
using NUnit.Framework;

public sealed class GameSessionTests
{
    [Test]
    public void RemoveLife_WhenLivesReachZero_EndsSessionOnce()
    {
        GameSession session = new(2, 3f, 0.75f, 1f);
        int gameOverCount = 0;
        session.GameOver += () => gameOverCount++;

        session.RemoveLife();
        session.RemoveLife();
        session.RemoveLife();
        session.ChangeState(SessionState.GameOver);

        Assert.That(session.Lives, Is.Zero);
        Assert.That(session.State, Is.EqualTo(SessionState.GameOver));
        Assert.That(gameOverCount, Is.EqualTo(1));
    }

    [Test]
    public void Reset_RestoresConfiguredDefaults()
    {
        GameSession session = new(5, 4f, 1f, 2f);
        session.AddScore(50);
        session.RegisterEnemyKilled();
        session.RemoveLife();
        session.ReduceSpawnInterval(2f);

        session.Reset();

        Assert.That(session.Lives, Is.EqualTo(5));
        Assert.That(session.Score, Is.Zero);
        Assert.That(session.EnemiesKilled, Is.Zero);
        Assert.That(session.SpawnInterval, Is.EqualTo(4f));
        Assert.That(session.EnemySpeed, Is.EqualTo(2f));
        Assert.That(session.State, Is.EqualTo(SessionState.Playing));
    }

    [Test]
    public void ReduceSpawnInterval_NeverPassesConfiguredMinimum()
    {
        GameSession session = new(3, 3f, 1.25f, 1f);

        session.ReduceSpawnInterval(100f);

        Assert.That(session.SpawnInterval, Is.EqualTo(1.25f));
    }
}
