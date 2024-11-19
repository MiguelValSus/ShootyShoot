using core.gameManagers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy SpawnPoint
/// </summary>
public class EnemySpawner : MonoBehaviour
{
	[Header("Prefab")]
	public Enemy m_prefab_enemy;

	[Header("Parameter")]
    public int m_spawn_amount = 10;
    public float m_spawn_interval = 2;

    [Header("Pool")]
    public int m_enemySpawn_count = 0;
    public List<UnityEngine.Object> m_enemies_Pool = new List<UnityEngine.Object>();

    #region Initialization
    public void InitSpawnerSettings()
    {
        m_enemySpawn_count = 0;
    }
    #endregion

    #region Pooling
    public void LoadEnemiesPool(Action customLoadEndAction = null)
    {
        StartCoroutine(GameManager.Instance.PoolManager.LoadPool(m_enemies_Pool, m_prefab_enemy, transform, m_spawn_amount, customLoadEndAction == null ? StartRunning : customLoadEndAction));
    }

    public void ClearEnemiesPool()
    {
        for (var i = 0; i < m_enemies_Pool.Count; ++i) {
            if (m_enemies_Pool[i] == null) continue;
            Enemy enemy = (Enemy)m_enemies_Pool[i];
            Destroy(enemy.gameObject);
        }
        m_enemies_Pool.Clear();
    }
    #endregion

    #region Spawning
    public void StartRunning()
	{
		SpawnEnemies();
	}

    public Enemy SpawnSingleEnemy()
    {
        if (m_enemySpawn_count >= m_enemies_Pool.Count) InitSpawnerSettings();
        Enemy enemy = m_enemies_Pool[m_enemySpawn_count] as Enemy;
        enemy.transform.position = transform.position;
        enemy.gameObject.SetActive(true);
        enemy.active = true;

        ++m_enemySpawn_count;
        return enemy;
    }

	private void SpawnEnemies()
	{
		// Spawn enemy from pool
		if (m_enemies_Pool.Count > 0  &&  m_enemySpawn_count < m_enemies_Pool.Count) {
			StartCoroutine(SpawnEnemiesFromPoolTimed(m_spawn_interval, SpawnEnemies));
		}
	}

    private IEnumerator SpawnEnemiesFromPoolTimed(float timeInterval, Action endAction)
    {
        var startTime = Time.time;
        //--Spawn--
        SpawnSingleEnemy();
        //--Wait--
        while (Time.time < startTime + timeInterval)
        {
            yield return null;
        }
        //--Timed event callback--
        endAction();
    }
    #endregion

    #region Visualization
    private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 1.0f);
	}
    #endregion
}
