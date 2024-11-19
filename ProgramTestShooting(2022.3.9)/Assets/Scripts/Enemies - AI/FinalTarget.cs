using core.Audio;
using core.gameManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTarget : Enemy
{
    [Header("Rendering")]
    public MeshRenderer Renderer;
    public Color DefaultColor,
                 DamagedColor;

    [Header("Battle Movement")]
    public List<Vector3> CombatPositions;

    [Header("Defeat")]
    public List<Animator> ChainExplosions;
    public Animator LargeFinalExplosion;
    public string SmallExplosionsAnim = "Explode_Lite";
    public string LargeExplosionAnim  = "Explode";
    public AudioClip SmallExplosionsSound;
    public AudioClip LargeExplosionSound;

    private Vector3 _nextTarget = new Vector3();
    private int _specialAttackCountdown = 10;
    private bool _moving;

    #region Initialization
    public override void EnemySetup() {
        base.EnemySetup();
        // Quick specific setup
        m_enemy_Col.enabled = false;
        _enemy_TR.localPosition += Vector3.up*3f;
        BulletsPool.LoadBulletsPool();
        // Show time!
        AudioManager.Instance.CrossfadeToNewMusic(AudioManager.Instance.BossTheme);
        StartCoroutine(GameManager.Instance.Menu.ShowAlertWarning());
        StartCoroutine(BossEnterBattle());
    }

    public override void ResetEnemyObject() {
        active = false;
        _moving = false;
        m_enemy_Col.enabled = false;
        m_enemy_RB.isKinematic = true;
        m_sprite.transform.localRotation = Quaternion.identity;
        m_sprite.transform.localPosition += Vector3.forward;
        // Explode
        StartCoroutine(ExplodeChainReaction());
    }
    #endregion

    #region Enemy movement
    protected override void MoveEnemy() { }

    protected override void MoveEnemyThroughPath() { }

    protected override void Update() {
        base.Update();
        if (!_moving) return;
        GoToPosition();
    }

    private IEnumerator BossEnterBattle() {
        var startTime = Time.time;
        while (Time.time < startTime + 2f)
        {
            _enemy_TR.localPosition -= Vector3.up * (Time.deltaTime*3f);
            yield return null;
        }
        m_enemy_Col.enabled = true;
        _nextTarget = CombatPositions[Random.Range(0, CombatPositions.Count)];
        isAggresive = true;
        StartCoroutine(AwaitAndShootPlayer());
        _moving = true;
    }

    private void GoToPosition() {

        var distance = Vector3.Distance(_enemy_TR.localPosition, _nextTarget);
        var stopThreshold = .01f;

        if (distance > stopThreshold) {
            _enemy_TR.localPosition = Vector3.MoveTowards(_enemy_TR.localPosition, _nextTarget, m_move_speed/1.5f * Time.deltaTime);
        } else {
            _nextTarget = CombatPositions[Random.Range(0, CombatPositions.Count)];
        }
    }
    #endregion

    #region Animations and Effects
    protected override void Animate() {
        //"animation"
        m_sprite.transform.rotation *= Quaternion.AngleAxis(m_rotation_speed * Time.deltaTime, new Vector3(1, 1, 0));
    }

    protected override void TakeDamage() {
        base.TakeDamage();
    }

    protected override IEnumerator FlashShipHit() {
        var startTime = Time.time;
        Renderer.material.color = DamagedColor;

        while (Time.time < startTime + .025f)
        {
            yield return null;
        }
        Renderer.material.color = DefaultColor;
    }

    private IEnumerator ExplodeChainReaction()
    {
        // Chain reaction explosions sound
        Audio.m_sound_effect = SmallExplosionsSound;

        // Explosions chain reaction
        for (var i = 0; i < 3; ++i)
        {
            for (var e = 0; e < ChainExplosions.Count; ++e)
            {
                ChainExplosions[e].gameObject.SetActive(true);
                ChainExplosions[e].Play(SmallExplosionsAnim);
                Audio.PlaySound();
                var startTime = Time.time;
                while (Time.time < startTime + .1f)
                {
                    yield return null;
                }
            }
        }
        // Send it off... with a BANG!
        LargeFinalExplosion.gameObject.SetActive(true);
        LargeFinalExplosion.Play(LargeExplosionAnim);
        m_shadow.SetActive(false);

        Audio.Audio.Stop();
        Audio.m_sound_effect = LargeExplosionSound;
        Audio.PlaySound();

        GameManager.Instance.Menu.OnStageClear();
    }
    #endregion

    #region Attack
    protected override IEnumerator AwaitAndShootPlayer()
    {
        var startTime = Time.time;
        while (Time.time < startTime + .33f)
        {
            yield return null;
        }
        if (!active) yield break;
        if (_specialAttackCountdown < 1)
        {
            // Special shot
            FireCircularBurst();

            var cooldownTime = Time.time;
            while (Time.time < cooldownTime + 1.5f)
            {
                yield return null;
            }
            _specialAttackCountdown = 10;
        }
        if (!_moving) yield break;
        // Recurrent shot
        FireShipGun();
        --_specialAttackCountdown;
        StartCoroutine(AwaitAndShootPlayer());
    }

    private void FireCircularBurst()
    {
        var bulletCount = 15;
        var bulletSpeed = 5f;
        var radius = 1.25f;

        var angleStep = 360f / bulletCount;

        for (int b = 0; b < bulletCount; ++b)
        {
            EnemyBullet bullet = BulletsPool.FetchBullet() as EnemyBullet;
            var angleOffset = 15f; 
            var spawnAngle = b * angleStep + angleOffset;
            var radian = spawnAngle * Mathf.Deg2Rad;
            var bulletPosition = _enemy_TR.position + new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * radius;
            var direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0).normalized;
            var b_RB = bullet.GetComponent<Rigidbody>();

            bullet.ResetObject();
            bullet.m_move_speed = bulletSpeed;
            bullet.m_life_time = 5.5f;
            bullet.transform.position = bulletPosition;
            bullet.transform.rotation = Quaternion.identity;
            bullet.gameObject.SetActive(true);
            bullet.SetDirectionManual(direction);
            bullet.fired = true;
            b_RB.velocity = direction * bulletSpeed;
        }
    }
    #endregion

    #region End
    public override void HitByPlayer(PlayerBullet a_player_bullet) {
        base.HitByPlayer(a_player_bullet);
        if (a_player_bullet.GetType() == typeof(EnemyBullet)) return;
        // Spawn an explosion
        var impact = Instantiate(m_explosion_FX.gameObject, a_player_bullet.transform.position, Quaternion.identity, null);
        impact.SetActive(true);
        // Gradually reduce the Boss' size with each hit, until a certain threshold is reached
        if (m_sprite.transform.localScale.x <= .45f) return;
        m_sprite.transform.localScale *= .9975f;
        m_shadow.transform.localScale *= .9975f;
        m_shadow.transform.localPosition += Vector3.up *.0025f;
        (m_enemy_Col as BoxCollider).size *= .995f;
    }
    #endregion
}
