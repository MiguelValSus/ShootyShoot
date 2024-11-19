using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class StageSequencer : MonoBehaviour
{
    [Header("Wave control")]
    public WavesManager WavesManager;

    [Header("Stage Sequences")]
    public WavesSequence[] LevelStages;
    public int CurrentStage;
    public int CurrentWave;

    [Serializable]
    public class WaveSettings
    {
        public WavesManager.SpawnPattern spawnPattern;
        public GameObject powerUp;
        public Enemy enemyType;
        public int enemyCount;
        public float spawnRate;
        public bool multiLine;
    }
    [Serializable]
    public class WavesSequence
    {
        public List<WaveSettings> stageWaves;
    }

    #region Sequence planning
    public IEnumerator PrepareNextWave(int nextWave, int nextStage = 0)
    {
        var startTime = Time.time;
        while (Time.time < startTime + WavesManager.WaveCooldown)
        {
            yield return null;
        }
        CurrentStage = nextStage;
        if (nextWave >= LevelStages[CurrentStage].stageWaves.Count) yield break;
        LaunchStageSequence(nextWave);
    }

    public void LaunchStageSequence(int wave, int stage = 0)
    {
        CurrentStage = stage;
        var waveToSpawn = LevelStages[CurrentStage].stageWaves[CurrentWave];
        var spawnPattern = waveToSpawn.spawnPattern;
        var wavePowerUp = waveToSpawn.powerUp;
        var enemyType = waveToSpawn.enemyType;
        var enemyCount = waveToSpawn.enemyCount;
        var spawnRate = waveToSpawn.spawnRate;
        var multiEnemyWave = waveToSpawn.multiLine;
        WavesManager.ResetWaveState();  // Reset count
        WavesManager.BeginWaveManualSetting(spawnPattern, enemyType, enemyCount, spawnRate, multiEnemyWave);
        PowerUpsManager.Instance.SpawnPowerUpPrecise(wavePowerUp);
    }
    #endregion
}
