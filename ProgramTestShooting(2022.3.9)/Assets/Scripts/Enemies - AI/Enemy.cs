using core.Audio;
using core.gameManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Pattern")]
    public List<Vector3> m_movement_path_nodes = new List<Vector3>();
    public WavesManager.SpawnPattern m_appear_pattern;
    public bool m_destroy_on_pattern_end;

    [Header("Physics components")]
    public Collider  m_enemy_Col;
    public Rigidbody m_enemy_RB;

    [Header("Bullets")]
    public Ammunition BulletsPool;
    public bool isAggresive;

    [Header("Settings")]
    public EnemyType m_type;
    public float m_move_speed = 5;
    public float m_rotation_speed = 200;
    public float m_life = 25;
    public int   m_score = 100;
    public int   m_hit_damage = 5;
    public bool  flyingEnemy = true;
    public bool  active = false;

    [Header("Effects")]
    public SFXPlayer m_audio_player;
    public Animator  m_explosion_FX;
    public GameObject m_sprite;
    public GameObject m_shadow;
    public string m_explode_animation = "Explode";

    [Header("Audio")]
    public SFXPlayer Audio;

    protected int _currentNodeInPath;
    protected Transform _enemy_TR;
    protected Coroutine _damageCoroutine;

    public enum EnemyType
    {
        ALPHA,
        DELTA,
        GAMMA,
        TANKS,
        PLASMA,
        CUBEX,
        BOSSX
    }

    #region Initialization
    private void Awake()
    {
        _enemy_TR = transform;
        active = true;
    }

    private void Start()
    {
        EnemySetup();
    }

    public virtual void EnemySetup()
    {
        m_explosion_FX.StopPlayback();
        m_explosion_FX.gameObject.SetActive(false);
        m_enemy_RB.isKinematic = false;
        m_enemy_Col.enabled = true;
        m_sprite.SetActive(true);
        if (m_shadow != null) m_shadow.SetActive(true);
        if (isAggresive) StartCoroutine(AwaitAndShootPlayer());
    }

    public virtual void ResetEnemyObject()
    {
        active = false;
        m_sprite.SetActive(false);
        m_enemy_Col.enabled = false;
        m_enemy_RB.isKinematic = true;
        _enemy_TR.rotation = Quaternion.identity;
        // This animation will handle the disabling in its final Event
        m_explosion_FX.gameObject.SetActive(true);
        m_explosion_FX.Play(m_explode_animation);
        // Subtract unit from active flying enemies' count
        if (flyingEnemy) GameManager.Instance.WavesManager.ReduceActiveEnemiesCount(1);
    }
    #endregion

    #region Enemy Lifetime
    protected void OnTriggerEnter(Collider collision)
    {
        PlayerBullet player_bullet = collision.transform.GetComponent<PlayerBullet>();
        if (player_bullet)
        {
            HitByPlayer(player_bullet);
        }
    }
    protected virtual void Update()
    {
        if (active)
        {
            MoveEnemyThroughPath();
            Animate();
        }
    }
    #endregion

    #region Enemy movement
    protected virtual void MoveEnemy()
    {
        // Barebones movement
        _enemy_TR.position += new Vector3(0, -1, 0) * m_move_speed * Time.deltaTime;
    }

    protected virtual void MoveEnemyThroughPath()
    {
        if (m_movement_path_nodes.Count > 0)
        {
            if (_currentNodeInPath >= m_movement_path_nodes.Count)
            {
                if (m_destroy_on_pattern_end) ResetEnemyObject();  // Destroy the enemy when it finishes the path (outside of screen, ideally)
                else MoveEnemy();  // Nonchalantly waltz out of the screen
                return;
            }
            var pathNodePos = m_movement_path_nodes[_currentNodeInPath];
            _enemy_TR.position = Vector3.MoveTowards(_enemy_TR.position, pathNodePos, m_move_speed * Time.deltaTime);

            if (Vector3.Distance(_enemy_TR.position, pathNodePos) < 0.05f) {
                _currentNodeInPath++;
            }
        }
        else
        {
            if (m_appear_pattern == WavesManager.SpawnPattern.SINE) {  // TODO: Refactor and complete this pattern
                var waveAmplitude = 3f;  // Width of the sine wave
                var waveFrequency = 2f;  // Speed of the sine wave
                var verticalSpeed = 1f;  // Controls downwards movement, overrides 'm_move_speed'
                var xPos = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
                _enemy_TR.position += new Vector3(0, -verticalSpeed * Time.deltaTime, 0);
                _enemy_TR.position  = new Vector3(xPos, _enemy_TR.position.y, _enemy_TR.position.z);

            } else {
                MoveEnemy();
            }
        }
    }

    public void SetMovePattern(WavesManager.SpawnPattern patternType, Vector3[] patternNodes)
    {
        for (var n = 0; n < patternNodes.Length; ++n) {
            m_movement_path_nodes.Add(patternNodes[n]);
        }
        m_appear_pattern = patternType;
    }
    #endregion

    #region Attack
    protected virtual IEnumerator AwaitAndShootPlayer()
    {
        var startTime = Time.time;
        BulletsPool.LoadBulletsPool();
        while (Time.time < startTime + .5f)
        {
            yield return null;
        }
        // First shot
        FireShipGun();
        startTime = Time.time;

        while (Time.time < startTime + .1f)
        {
            yield return null;
        }
        // Second shot
        FireShipGun();
    }

    protected void FireShipGun(Transform originPoint = null, float bulletOffset = 1.1f, Sprite bulletSprite = null)
    {
        if (BulletsPool == null) return;

        PlayerBullet bullet = BulletsPool.FetchBullet();
        if (bullet == null) return;

        bullet.transform.position = originPoint == null ? _enemy_TR.position : originPoint.position;
        bullet.transform.position += Vector3.down*bulletOffset;
        bullet.ResetObject();

        if (bulletSprite != null) bullet.GetComponent<PlayerBullet>().m_bullet_sprite.sprite = bulletSprite;

        bullet.transform.rotation = originPoint == null ? _enemy_TR.rotation : originPoint.rotation;  // Bullets inherit the ship's rotation
        bullet.gameObject.SetActive(true);
        bullet.fired = true;
    }
    #endregion

    #region Animations and Effects
    protected virtual void Animate() { }

    protected virtual void TakeDamage() 
    {
        if (_damageCoroutine != null) StopCoroutine(_damageCoroutine);
        _damageCoroutine = StartCoroutine(FlashShipHit());
    }

    protected virtual IEnumerator FlashShipHit()
    {
        var startTime = Time.time;
        // Show the damage visual cue

        while (Time.time < startTime + .1f)
        {
            yield return null;
        }
        
        // Return graphics to their default state
    }
    #endregion

    #region End
    public virtual void HitByPlayer(PlayerBullet a_player_bullet)
    {
        if (a_player_bullet.GetType() == typeof(EnemyBullet)) return;

        // Calculate damage
        m_life -= a_player_bullet.m_bullet_damage;

        // Delete bullet
        if (a_player_bullet) {
            a_player_bullet.ResetObject();
        }
        
        if (m_life <= 0) {
            // Add score
            ScoreManager.Instance.AddScore(m_score, m_type);
            // Play SFX
            m_audio_player.PlaySound();
            // Delete self
            ResetEnemyObject();
        } else {
            // Receive impact
            TakeDamage();
        }
    }
    #endregion

    #region Audio
    protected void PlayExplosionSFX()
    {
        if (!active) return;
        Audio.PlaySound();
    }
    #endregion
}
