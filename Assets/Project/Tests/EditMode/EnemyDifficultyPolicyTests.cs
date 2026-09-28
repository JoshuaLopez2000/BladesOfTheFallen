using BladesOfTheFallen.Core;
using NUnit.Framework;

public sealed class EnemyDifficultyPolicyTests
{
    [TestCase(0, 1.5f, 1)]
    [TestCase(5, 2.5f, 2)]
    [TestCase(10, 3.5f, 2)]
    public void Evaluate_ReturnsProfileForCurrentTier(int kills, float expectedSpeed, int expectedLives)
    {
        EnemySpawnProfile profile = EnemyDifficultyPolicy.Evaluate(kills, 0f);

        Assert.That(profile.Speed, Is.EqualTo(expectedSpeed));
        Assert.That(profile.BasicEnemyLives, Is.EqualTo(expectedLives));
    }

    [TestCase(10, true)]
    [TestCase(20, true)]
    [TestCase(35, true)]
    [TestCase(34, false)]
    public void ShouldReduceSpawnInterval_OnlyAtMilestones(int kills, bool expected)
    {
        Assert.That(EnemyDifficultyPolicy.ShouldReduceSpawnInterval(kills), Is.EqualTo(expected));
    }
}
