using core.gameManagers;
using UnityEngine;

/// <summary>
/// Enemy Bullet, mimicks Player bullet
/// </summary>
public class EnemyBullet : PlayerBullet
{
    public bool ShotFromGroundLevel;

    #region Movement
    protected override void MoveBullet()
    {
        if (!ShotFromGroundLevel) {
            base.MoveBullet();
            return;
        }
        var pointToPlayer = (GameManager.Instance.PController.transform.position - _bulletTR.position).normalized;
        var rotAngle = Mathf.Atan2(pointToPlayer.y, pointToPlayer.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.Euler(0, 0, rotAngle);
        _bulletTR.rotation = targetRotation;
        _bulletTR.position -= _aimingTowards * m_move_speed * Time.deltaTime;
    }

    protected override Vector3 CalculateBulletDirection()
    {
        if (_aimOverrideManual) return _aimingTowards;
        // Get the Z rotation of the Enemy Ship
        var enemyTransform = _bulletTR.parent;
        var zBulletRotation = new Vector3(-enemyTransform.right.y, enemyTransform.right.x, 0);
        return ShotFromGroundLevel ? (GameManager.Instance.PController.transform.position - _bulletTR.position).normalized : zBulletRotation;
    }
    #endregion

    #region Impact
    private void OnTriggerEnter(Collider col)
    {
        switch (col.gameObject.layer)
        {
            case 3: //Player
            {
                if (col.tag != "Player") return;
                ResetObject();
            }
            break;
        }
    }
    #endregion
}
