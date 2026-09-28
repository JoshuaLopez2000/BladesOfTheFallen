using System;
using System.IO;
using BladesOfTheFallen.Core;
using UnityEngine;

namespace BladesOfTheFallen.Infrastructure.Persistence
{
    public sealed class JsonHighScoreRepository : IHighScoreRepository
    {
        private readonly string filePath;
        private readonly Action<string> reportWarning;

        public JsonHighScoreRepository(string filePath, Action<string> reportWarning = null)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            this.reportWarning = reportWarning;
        }

        public int Load()
        {
            if (!File.Exists(filePath))
            {
                return 0;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
                return Mathf.Max(0, data?.highScore ?? 0);
            }
            catch (Exception exception)
            {
                reportWarning?.Invoke($"Could not load the high score: {exception.Message}");
                return 0;
            }
        }

        public void Save(int score)
        {
            try
            {
                HighScoreData data = new() { highScore = Mathf.Max(0, score) };
                File.WriteAllText(filePath, JsonUtility.ToJson(data, true));
            }
            catch (Exception exception)
            {
                reportWarning?.Invoke($"Could not save the high score: {exception.Message}");
            }
        }

        [Serializable]
        private sealed class HighScoreData
        {
            public int highScore;
        }
    }
}
