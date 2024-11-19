using core.gameManagers;
using System.Collections;
using UnityEngine;

public class Enemy_GroundTurret : Enemy
{
    [Header("Turret")]
    public Transform MovingTurret;
    public Transform Cannon;
    [Header("Sprites")]
    public SpriteRenderer Renderer;
    public Sprite RegularSprite,
                  DamagedSprite;

    #region Initialization
    public override void EnemySetup() {
        base.EnemySetup();
    }

    public override void ResetEnemyObject() {
        base.ResetEnemyObject();
        PlayExplosionSFX();
    }
    #endregion

    #region Enemy Lifetime
    protected override void Update() {
        MoveEnemy();
    }
    #endregion

    #region Enemy movement
    protected override void MoveEnemy() {
        base.MoveEnemy();
        // Plus, rotate towards Player's ship, aiming at it
        AimAtPlayer();
    }

    protected override void MoveEnemyThroughPath() { }

    private void AimAtPlayer() {
        var pointToPlayer = (GameManager.Instance.PController.transform.position - MovingTurret.position).normalized;
        var rotAngle = Mathf.Atan2(pointToPlayer.y, pointToPlayer.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.Euler(0, 0, rotAngle);
        MovingTurret.rotation = targetRotation;
    }
    #endregion

    #region Attack
    protected override IEnumerator AwaitAndShootPlayer() {

        if (!active) yield break;

        var startTime = Time.time;
        BulletsPool.LoadBulletsPool();
        while (Time.time < startTime + 1f)
        {
            yield return null;
        }
        // First shot
        if (active) FireShipGun(Cannon, 0f);
        StartCoroutine(AwaitAndShootPlayer());
    }
    #endregion

    #region Animations and Effects
    protected override void Animate() { }

    protected override void TakeDamage() {
        base.TakeDamage();
    }

    protected override IEnumerator FlashShipHit() {
        var startTime = Time.time;
        Renderer.sprite = DamagedSprite;

        while (Time.time < startTime + .25f)
        {
            yield return null;
        }
        Renderer.sprite = RegularSprite;
    }
    #endregion

    #region End
    public override void HitByPlayer(PlayerBullet a_player_bullet) {
        base.HitByPlayer(a_player_bullet);
    }
    #endregion
}
