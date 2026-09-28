using System;

namespace BladesOfTheFallen.Core
{
    /// <summary>
    /// Owns the mutable rules of a single run. It deliberately has no Unity
    /// dependency so the rules can be tested and reused by other front ends.
    /// </summary>
    public sealed class GameSession
    {
        private readonly int initialLives;
        private readonly float initialSpawnInterval;
        private readonly float minimumSpawnInterval;
        private readonly float initialEnemySpeed;

        public GameSession(
            int initialLives,
            float initialSpawnInterval,
            float minimumSpawnInterval,
            float initialEnemySpeed)
        {
            this.initialLives = Math.Max(1, initialLives);
            this.minimumSpawnInterval = Math.Max(0f, minimumSpawnInterval);
            this.initialSpawnInterval = Math.Max(this.minimumSpawnInterval, initialSpawnInterval);
            this.initialEnemySpeed = Math.Max(0f, initialEnemySpeed);

            Reset();
        }

        public SessionState State { get; private set; }
        public int Lives { get; private set; }
        public int Score { get; private set; }
        public int EnemiesKilled { get; private set; }
        public float SpawnInterval { get; private set; }
        public float EnemySpeed { get; private set; }

        public event Action<int> ScoreChanged;
        public event Action<int> EnemiesKilledChanged;
        public event Action<int> LivesChanged;
        public event Action<SessionState> StateChanged;
        public event Action GameOver;

        public void Reset()
        {
            State = SessionState.Playing;
            Lives = initialLives;
            Score = 0;
            EnemiesKilled = 0;
            SpawnInterval = initialSpawnInterval;
            EnemySpeed = initialEnemySpeed;

            ScoreChanged?.Invoke(Score);
            EnemiesKilledChanged?.Invoke(EnemiesKilled);
            LivesChanged?.Invoke(Lives);
            StateChanged?.Invoke(State);
        }

        public void AddScore(int amount)
        {
            long updatedScore = (long)Score + amount;
            Score = (int)Math.Max(0L, Math.Min(int.MaxValue, updatedScore));
            ScoreChanged?.Invoke(Score);
        }

        public void RemoveLife()
        {
            if (Lives == 0 || State == SessionState.GameOver)
            {
                return;
            }

            Lives--;
            LivesChanged?.Invoke(Lives);

            if (Lives == 0)
            {
                ChangeState(SessionState.GameOver);
            }
        }

        public void RegisterEnemyKilled()
        {
            if (EnemiesKilled < int.MaxValue)
            {
                EnemiesKilled++;
            }

            EnemiesKilledChanged?.Invoke(EnemiesKilled);
        }

        public void ReduceSpawnInterval(float amount)
        {
            SpawnInterval = Math.Max(minimumSpawnInterval, SpawnInterval - Math.Max(0f, amount));
        }

        public void ChangeState(SessionState state)
        {
            if (State == state)
            {
                return;
            }

            State = state;
            StateChanged?.Invoke(State);

            if (State == SessionState.GameOver)
            {
                GameOver?.Invoke();
            }
        }
    }

    public enum SessionState
    {
        Init,
        Playing,
        Paused,
        GameOver
    }
}
