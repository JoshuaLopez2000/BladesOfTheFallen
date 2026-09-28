using NUnit.Framework;
using UnityEngine;

public sealed class GameManagerSOTests
{
    private GameManagerSO gameManager;

    [SetUp]
    public void SetUp()
    {
        gameManager = ScriptableObject.CreateInstance<GameManagerSO>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameManager);
    }

    [Test]
    public void IncreaseScore_UpdatesValueAndRaisesEvent()
    {
        int observedScore = -1;
        gameManager.OnScoreChanged += score => observedScore = score;

        gameManager.IncreaseScore(25);

        Assert.That(gameManager.PlayerScore, Is.EqualTo(25));
        Assert.That(observedScore, Is.EqualTo(25));
    }

    [Test]
    public void DecreaseLife_AtZero_RaisesGameOverOnlyOnce()
    {
        int gameOverCount = 0;
        gameManager.OnGameOver += () => gameOverCount++;

        gameManager.DecreaseLife();
        gameManager.DecreaseLife();
        gameManager.DecreaseLife();
        gameManager.DecreaseLife();

        Assert.That(gameManager.PlayerLives, Is.Zero);
        Assert.That(gameManager.CurrentState, Is.EqualTo(GameManagerSO.GameState.GameOver));
        Assert.That(gameOverCount, Is.EqualTo(1));
    }

    [Test]
    public void DecreaseSpawnInterval_ClampsToMinimum()
    {
        for (int index = 0; index < 10; index++)
        {
            gameManager.DecreaseSpawnInterval();
        }

        Assert.That(gameManager.SpawnInterval, Is.EqualTo(0.75f));
    }

    [Test]
    public void ParryTimeEffect_ClampsRequestValues()
    {
        float observedSlowFactor = -1f;
        float observedDuration = -1f;
        gameManager.ParryTimeEffectRequested += (slowFactor, duration) =>
        {
            observedSlowFactor = slowFactor;
            observedDuration = duration;
        };

        gameManager.ParryTimeEffect(2f, -1f);

        Assert.That(observedSlowFactor, Is.EqualTo(1f));
        Assert.That(observedDuration, Is.Zero);
    }
}
