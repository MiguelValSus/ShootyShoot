using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using core.gameManagers;

public class WavesManager : MonoBehaviour
{
    [Header("Enemies")]
    public EnemySpawner ActiveSpawner;
    public Transform[] SimplePatternsSpawners,
                       LinearPatternsSpawners,
                       NonLinearPatternsSpawners,
                       ComplexPatternsSpawners;

    [Header("Waves setup")]
    public float WaveCooldown = 4f;
    public int   EnemiesInWave = 10;
    public float SpawnEnemyRate = .5f; //In seconds

    [Header("Wave ongoing status")]
    public int ActiveEnemies;

    [Header("Enemy types")]
    public List<Enemy> EnemyTypes;

    [Header("Patterns")]
    public List<SpawnPattern> EnemyFormations;
    public enum SpawnPattern
    {
        STRAIGHT,
        STRAIGHT_CENTER,
        DIAGONAL_LEFT,
        DIAGONAL_RIGHT,
        ZIGZAG,
        CURVED,
        CIRCULAR,
        SPIRAL,
        RANDOM_JITTER,
        // This is performed in the Enemy's Update() loop:
        SINE
    }

    private int _currentWave = 0;
    private bool _waveEnded = true;

    #region Initialization
    public void ResetWaveState()
    {
        ActiveEnemies = 0;
    }

    public void ReduceActiveEnemiesCount(int reductionRate = 1)
    {
        ActiveEnemies -= reductionRate;
    }

    private void MoveOnToNextWave()
    {
        var sequencer = GameManager.Instance.WavesSequencer;
        ++sequencer.CurrentWave;
        StartCoroutine(sequencer.PrepareNextWave(sequencer.CurrentWave));
    }
    #endregion

    #region Completion check
    private void Update()
    {
        if (!_waveEnded && ActiveEnemies <= 0) 
            OnWaveEnd();
    }

    private void OnWaveEnd()
    {
        _waveEnded = true;
        MoveOnToNextWave();
    }
    #endregion

    #region Timed Waves
    public void BeginWaves()
    {
        var wavePattern = PickRandomWavePattern();
        var spawnSource = PickRandomSpawnSource(wavePattern);
        ActiveSpawner = spawnSource;
        ActiveSpawner.m_prefab_enemy = PickRandomEnemyType();
        ActiveSpawner.LoadEnemiesPool(()=> StartCoroutine(SpawnWave(wavePattern)));
    }
    public void BeginWaveManualSetting(SpawnPattern wavePattern, Enemy enemyType, int enemyCount, float enemyRate, bool multiEnemyRows = false)
    {
        var spawnSource = PickRandomSpawnSource(wavePattern);
        EnemiesInWave  = enemyCount;
        SpawnEnemyRate = enemyRate;
        ActiveSpawner  = spawnSource;
        ActiveSpawner.m_prefab_enemy = enemyType;
        ActiveSpawner.ClearEnemiesPool();
        ActiveSpawner.m_spawn_amount = EnemiesInWave + (multiEnemyRows ? enemyCount * 2 : 0);
        ActiveSpawner.LoadEnemiesPool(() => StartCoroutine(SpawnWave(wavePattern, multiEnemyRows)));
    }

    private IEnumerator SpawnWave(SpawnPattern wavePattern, bool multipleEnemies = false)
    {
        var startTime = Time.time;
        while (Time.time < startTime + WaveCooldown)
        {
            yield return null;
        }
        ++_currentWave;
        StartCoroutine(SpawnEnemies(wavePattern, multipleEnemies));
    }

    private IEnumerator SpawnEnemies(SpawnPattern enemyFormationPattern, bool multiLineEnemy  = false)
    {
        var startTime = Time.time;
        for (var e = 0; e < EnemiesInWave; ++e)
        {
            while (Time.time < startTime + SpawnEnemyRate)
            {
                yield return null;
            }
            SpawnEnemy(enemyFormationPattern, multiLineEnemy);
            startTime = Time.time;  // Reset timer before next enemy
        }
        _waveEnded = false;
    }
    #endregion

    #region Spawn control
    private void SpawnEnemy(SpawnPattern enemyFormationPattern, bool multipleEnemies = false)
    {
        var enemy = ActiveSpawner.SpawnSingleEnemy();
        var pathPoints = GetPathPattern(ActiveSpawner.transform, enemyFormationPattern);
        enemy.SetMovePattern(enemyFormationPattern, pathPoints);
        ++ActiveEnemies;

        if (multipleEnemies)
        {
            var enemy2 = multipleEnemies ? ActiveSpawner.SpawnSingleEnemy() : null;
            var enemy3 = multipleEnemies ? ActiveSpawner.SpawnSingleEnemy() : null;
            if (enemy2 != null) {
                enemy2.SetMovePattern(enemyFormationPattern, pathPoints);
                // Reposition components
                enemy2.m_sprite.transform.position += Vector3.left * .75f;
                enemy2.m_explosion_FX.transform.position += Vector3.left * .75f;
                if (enemy2.name.Contains("Cube")) enemy2.m_shadow.transform.position += Vector3.left * .75f;
                ++ActiveEnemies;
            }
            if (enemy3 != null) {
                enemy3.SetMovePattern(enemyFormationPattern, pathPoints);
                // Reposition components
                enemy3.m_sprite.transform.position += Vector3.right * .75f;
                enemy3.m_explosion_FX.transform.position += Vector3.right * .75f;
                if (enemy2.name.Contains("Cube")) enemy3.m_shadow.transform.position += Vector3.right * .75f;
                ++ActiveEnemies;
            }
        }
    }

    private SpawnPattern PickRandomWavePattern()
    {
        if (EnemyFormations.Count > 0)
            return EnemyFormations[Random.Range(0, EnemyFormations.Count)];
        else
            return SpawnPattern.STRAIGHT;
    }

    private Enemy PickRandomEnemyType()
    {
        if (EnemyTypes.Count > 0)
            return EnemyTypes[Random.Range(0, EnemyTypes.Count)];
        else
            throw null;
    }

    private EnemySpawner PickRandomSpawnSource(SpawnPattern enemyFormationPattern)
    {
        Transform[] patternSpawners = enemyFormationPattern switch {
            var p when p == SpawnPattern.ZIGZAG        || p == SpawnPattern.CIRCULAR || p == SpawnPattern.STRAIGHT_CENTER || p == SpawnPattern.CURVED  || p == SpawnPattern.DIAGONAL_LEFT || p == SpawnPattern.DIAGONAL_RIGHT
                => SimplePatternsSpawners,
            //var p when p == SpawnPattern.DIAGONAL_LEFT || p == SpawnPattern.DIAGONAL_RIGHT 
                //=> NonLinearPatternsSpawners,
            var p when p == SpawnPattern.RANDOM_JITTER || p == SpawnPattern.STRAIGHT
                => LinearPatternsSpawners,
        _   // Rest of them
                => ComplexPatternsSpawners
        };
        if (patternSpawners.Length > 0)
            return patternSpawners[Random.Range(0, patternSpawners.Length)].GetComponent<EnemySpawner>();
        else
            return Instantiate(new GameObject(), transform).AddComponent<EnemySpawner>();
    }

    private Vector3[] GetPathPattern(Transform spawnSource, SpawnPattern requestedPattern)
    {
        Vector3[] pathNodes;

        switch(requestedPattern)
        {
            default:
            case SpawnPattern.STRAIGHT_CENTER:  // Straight line down the screen
            case SpawnPattern.STRAIGHT:
            {
                pathNodes = new Vector3[]
                {
                    spawnSource.position,
                    new Vector3(spawnSource.position.x, spawnSource.position.y-10, spawnSource.position.z)
                };
            }
            break;
            case SpawnPattern.ZIGZAG:  // ZigZag pattern
            {
                pathNodes = new Vector3[]
                {
                    spawnSource.position,
                    new Vector3(spawnSource.position.x+2, spawnSource.position.y-3,  spawnSource.position.z),
                    new Vector3(spawnSource.position.x-2, spawnSource.position.y-6,  spawnSource.position.z),
                    new Vector3(spawnSource.position.x+2, spawnSource.position.y-9,  spawnSource.position.z),
                    new Vector3(spawnSource.position.x,   spawnSource.position.y-12, spawnSource.position.z)
                };
            }
            break;
            case SpawnPattern.DIAGONAL_LEFT:  // Diagonal line, Westward
            {
                pathNodes = new Vector3[]
                {
                    spawnSource.position,
                    new Vector3(spawnSource.position.x-5, spawnSource.position.y-10, spawnSource.position.z)
                };
            }
            break;
            case SpawnPattern.DIAGONAL_RIGHT:  // Diagonal line, Eastward
            {
                pathNodes = new Vector3[]
                {
                    spawnSource.position,
                    new Vector3(spawnSource.position.x+5, spawnSource.position.y-10, spawnSource.position.z)
                };
            }
            break;
            case SpawnPattern.CURVED:  // Nice, simple curvature movement (tweak amount of nodes to control its jankyness)
            {
                pathNodes = new Vector3[]
                {
                    spawnSource.position,
                    new Vector3(spawnSource.position.x+1, spawnSource.position.y-2,  spawnSource.position.z),
                    new Vector3(spawnSource.position.x+2, spawnSource.position.y-4,  spawnSource.position.z),
                    new Vector3(spawnSource.position.x+3, spawnSource.position.y-6,  spawnSource.position.z),
                    new Vector3(spawnSource.position.x+2, spawnSource.position.y-8,  spawnSource.position.z),
                    new Vector3(spawnSource.position.x+1, spawnSource.position.y-10, spawnSource.position.z),
                    new Vector3(spawnSource.position.x,   spawnSource.position.y-12, spawnSource.position.z)
                };
            }
            break;
            case SpawnPattern.CIRCULAR:  // Circle back to confuse player
            {
                var circleCenter = spawnSource.position + Vector3.down*5.5f;
                var patternRadius = 8f;

                pathNodes = new Vector3[]
                {
                    circleCenter + new Vector3(patternRadius, 0, 0),
                    circleCenter + new Vector3(0, patternRadius, 0),
                    circleCenter + new Vector3(-patternRadius, 0, 0),
                    circleCenter + new Vector3(0, -patternRadius, 0),
                    circleCenter + new Vector3(patternRadius, 0, 0),
                    new Vector3(spawnSource.position.x, spawnSource.position.y-25, spawnSource.position.z) // Exit screen downwards
                };
            }
            break;
            case SpawnPattern.SPIRAL:  // Inwards spiral
            {
                var centerPoint = spawnSource.position + Vector3.down*5;
                var spiralAngle  = 0f;
                var spiralRadius = 3f;
                var spiralPoints = 20;

                pathNodes = new Vector3[spiralPoints];
                for (var p = 0; p < spiralPoints; ++p) {
                    var x = centerPoint.x + Mathf.Cos(spiralAngle) * spiralRadius;
                    var y = centerPoint.y - p * 0.5f;
                    var z = 0;
                    pathNodes[p] = new Vector3(x, y, z);

                    spiralAngle += Mathf.PI / 4; // Tweak this for spiral intensity
                    spiralRadius *= 0.9f; // Gradually decrease radius for inward spiralling
                }
            }
            break;
            case SpawnPattern.RANDOM_JITTER:  // Slightly random jittered movement
            {
                    var randomNodes = 15;
                    var pathLength = 20f;
                    var jitterAmount = 3f;

                    pathNodes = new Vector3[randomNodes];
                    var step = pathLength / randomNodes; // Distance between each waypoint on the Y-axis

                    for (var n = 0; n < randomNodes; n++) {
                        var yPosition = spawnSource.position.y - (n * step);
                        var xPosition = spawnSource.position.x + Random.Range(-jitterAmount, jitterAmount);

                        // Set the node's jitter
                        pathNodes[n] = new Vector3(xPosition, yPosition, spawnSource.position.z);
                    }
                } 
            break;
        }        

        return pathNodes;
    }
    #endregion
}
