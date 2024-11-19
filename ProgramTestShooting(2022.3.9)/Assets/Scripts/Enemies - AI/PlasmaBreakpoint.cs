using UnityEngine;

public class PlasmaBreakpoint : MonoBehaviour
{
    [Header("Main Enemy component")]
    public Enemy_PlasmaWall EnemyController;
    public Collider Breakpoint;
    public GameObject Explosion;
    public Side WeakSpotSide;
    public int BreakpointLife = 15;

    public enum Side
    {
        LEFT,
        RIGHT
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerBullet player_bullet = collision.transform.GetComponent<PlayerBullet>();
        if (player_bullet)
        {
            HitByPlayer(player_bullet);
        }
    }

    public void HitByPlayer(PlayerBullet a_player_bullet)
    {
        // Calculate damage
        BreakpointLife -= a_player_bullet.m_bullet_damage;

        // Delete bullet
        if (a_player_bullet)
        {
            a_player_bullet.ResetObject();
        }

        if (BreakpointLife <= 0)
        {
            // Play VFX & SFX
            Explosion.SetActive(true);
            EnemyController.m_audio_player.PlaySound();
            // Hide self
            Breakpoint.enabled = false;
            EnemyController.BreakWeakSpot(WeakSpotSide);
        }
    }
}
