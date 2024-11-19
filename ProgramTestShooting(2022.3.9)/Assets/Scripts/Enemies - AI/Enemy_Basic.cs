using System.Collections;
using UnityEngine;

public class Enemy_Basic : Enemy
{
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

    #region Enemy movement
    protected override void MoveEnemy() {
        base.MoveEnemy();
    }

    protected override void MoveEnemyThroughPath() {
        base.MoveEnemyThroughPath();
    }
    #endregion

    #region Animations and Effects
    protected override void Animate() { }

    protected override void TakeDamage() {
        base.TakeDamage();
    }

    protected override IEnumerator FlashShipHit()
    {
        var startTime = Time.time;
        Renderer.sprite = DamagedSprite;

        while (Time.time < startTime + .1f)
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
