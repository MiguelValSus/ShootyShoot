using System.Collections;
using UnityEngine;

public class Enemy_Cube : Enemy
{
    [Header("Rendering")]
    public MeshRenderer Renderer;
    public Color DefaultColor, 
                 DamagedColor;

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

        while (Time.time < startTime + .1f)
        {
            yield return null;
        }
        Renderer.material.color = DefaultColor;
    }
    #endregion

    #region End
    public override void HitByPlayer(PlayerBullet a_player_bullet) {
        base.HitByPlayer(a_player_bullet);
    }
    #endregion
}
