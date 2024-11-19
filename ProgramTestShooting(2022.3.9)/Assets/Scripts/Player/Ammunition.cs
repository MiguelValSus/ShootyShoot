using core.gameManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{
    [Header("Bullet types prefabs")]
    public PlayerBullet m_prefab_bullet_T1;

    [Header("Bullets Pool")]
    public int m_pool_size = 100;
    public List<Object> m_bullets_Pool = new List<Object>();

    [Header("Clip size")]
    public int m_fired_bullets = 0;
    public float m_cooldown_interval = .15f;

    private bool _fireCooldown;

    #region Initialization
    public void LoadBulletsPool()
    {
        StartCoroutine(GameManager.Instance.PoolManager.LoadPool(m_bullets_Pool, m_prefab_bullet_T1, transform, m_pool_size));
    }
    #endregion

    #region Fire
    public PlayerBullet FetchBullet()
    {
        if (_fireCooldown) return null;

        var bulletToFire = m_bullets_Pool[m_fired_bullets] as PlayerBullet;

        // Set Bullet to its initial state
        bulletToFire.ResetObject();

        // Bullets extracted from the Pool count
        if (m_fired_bullets < m_bullets_Pool.Count-1) ++m_fired_bullets;
        else m_fired_bullets = 0;
        
        return bulletToFire;
    }
    #endregion

    #region Cooldown
    public void CoolDownGuns()
    {
        if (_fireCooldown) return;
        StartCoroutine(FireCooldown());
    }

    private IEnumerator FireCooldown()
    {
        var startTime = Time.time;
        _fireCooldown = true;

        while (Time.time < startTime + m_cooldown_interval)
        {
            yield return null;
        }

        _fireCooldown = false;
    }
    #endregion
}
