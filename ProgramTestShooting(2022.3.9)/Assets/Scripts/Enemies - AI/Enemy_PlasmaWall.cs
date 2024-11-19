using core.gameManagers;
using System.Collections;
using UnityEngine;

public class Enemy_PlasmaWall : Enemy
{
    [Header("Particles")]
    public MonoBehaviour PlasmaLightning;
    public ParticleSystem WeakSpotVFX_L,
                          WeakSpotVFX_R;

    private Renderer WeakSpotRender_L,
                     WeakSpotRender_R;

    #region Initialization
    public override void EnemySetup() {
        WeakSpotRender_L = WeakSpotVFX_L.GetComponent<Renderer>();
        WeakSpotRender_R = WeakSpotVFX_R.GetComponent<Renderer>();
        transform.position += Vector3.left * 5.6f; // Manually reposition it. TODO: Placeholder call
    }

    public override void ResetEnemyObject() {
        m_enemy_Col.enabled = false;
        PlasmaLightning.enabled = false;
        // Subtract unit from active enemies' count
        GameManager.Instance.WavesManager.ReduceActiveEnemiesCount(1);
        // Add score
        ScoreManager.Instance.AddScore(m_score, m_type);
    }
    #endregion

    #region Enemy movement
    protected override void MoveEnemy() {
        base.MoveEnemy();
    }

    protected override void MoveEnemyThroughPath() {
        base.MoveEnemy();
    }
    #endregion

    #region Animations and Effects
    protected override void Animate() { }

    protected override void TakeDamage() {
        base.TakeDamage();
    }

    protected override IEnumerator FlashShipHit() {
        yield return null;
    }
    #endregion

    #region End
    public override void HitByPlayer(PlayerBullet a_player_bullet) {
        base.HitByPlayer(a_player_bullet);
    }

    public void BreakWeakSpot(PlasmaBreakpoint.Side weakSpot)
    {
        switch(weakSpot)
        {
            default:
            case PlasmaBreakpoint.Side.LEFT:
            {
                PlayExplosionSFX();
                WeakSpotRender_L.enabled = false;
                if (!WeakSpotRender_R.enabled) ResetEnemyObject();
            }
            break;
            case PlasmaBreakpoint.Side.RIGHT:
            {
                PlayExplosionSFX();
                WeakSpotRender_R.enabled = false;
                if (!WeakSpotRender_L.enabled) ResetEnemyObject();
            }
            break;
        }
    }
    #endregion
}
