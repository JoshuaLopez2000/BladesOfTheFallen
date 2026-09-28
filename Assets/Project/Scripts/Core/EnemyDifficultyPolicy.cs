namespace BladesOfTheFallen.Core
{
    /// <summary>Pure difficulty rules used by the Unity spawning adapter.</summary>
    public static class EnemyDifficultyPolicy
    {
        public static EnemySpawnProfile Evaluate(int enemiesKilled, float extraLifeRoll)
        {
            if (enemiesKilled < 5)
            {
                return new EnemySpawnProfile(1.5f, 1);
            }

            if (enemiesKilled < 10)
            {
                return new EnemySpawnProfile(2.5f, extraLifeRoll < 0.3f ? 2 : 1);
            }

            return new EnemySpawnProfile(3.5f, extraLifeRoll < 0.65f ? 2 : 1);
        }

        public static bool ShouldReduceSpawnInterval(int enemiesKilled)
        {
            return enemiesKilled == 10 || enemiesKilled == 20 || enemiesKilled == 35;
        }
    }

    public readonly struct EnemySpawnProfile
    {
        public EnemySpawnProfile(float speed, int basicEnemyLives)
        {
            Speed = speed;
            BasicEnemyLives = basicEnemyLives;
        }

        public float Speed { get; }
        public int BasicEnemyLives { get; }
    }
}
