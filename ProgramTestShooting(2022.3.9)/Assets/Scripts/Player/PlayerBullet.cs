using core.gameManagers;
using UnityEngine;

/// <summary>
/// Player Bullet
/// </summary>
public class PlayerBullet : MonoBehaviour
{
	[Header("Parameters")]
	public float m_move_speed = 8;
	public float m_life_time = 3;
	public int   m_bullet_damage = 1;
    public Ammunition m_bulletsPool;
	public Vector3 m_initial_scale = new Vector3(.5f, .25f, .1f);

    [Header("Visuals")]
    public SpriteRenderer m_bullet_sprite;

    [Header("State")]
	public bool fired = false;

    protected Transform _bulletTR;
	protected Vector3 _aimingTowards = new Vector3();
    protected bool _aimOverrideManual;

    #region Initialization
    protected void Awake()
    {
        CacheBulletsPool();
    }

    protected virtual void OnEnable()
    {
        if (!_aimOverrideManual) {
            _aimingTowards = CalculateBulletDirection();
        }
        transform.SetParent(GameManager.Instance.FiredBulletsHolder.transform);
    }
    protected virtual void CacheBulletsPool()
    {
        if (m_bulletsPool != null) return;
        m_bulletsPool = GameManager.Instance.PController.Weapon.BulletsPool;
    }

    public void SetDirectionManual(Vector3 aimDir)
    {
        _aimOverrideManual = true;
        _aimingTowards = aimDir;
    }

    public void ResetObject()
    {
        fired = false;
        m_life_time = 3;
        CacheBulletsPool();
        _bulletTR = transform;
        _bulletTR.localScale = m_initial_scale;
        gameObject.SetActive(false);
        _bulletTR.SetParent(m_bulletsPool.gameObject.transform);
    }
    #endregion

    #region Movement
    private void Update()
    {
        MoveBullet();

		m_life_time -= Time.deltaTime;
		if (m_life_time <= 0) {
			ResetObject();
		}
	}

    protected virtual void MoveBullet()
    {
        if (!_aimOverrideManual) {
            _aimingTowards = CalculateBulletDirection();
        }
        _bulletTR.position += _aimingTowards * m_move_speed * Time.deltaTime;
    }

	protected virtual Vector3 CalculateBulletDirection()
	{
        // Get the Z rotation of the Player Ship
        var shipTransform = GameManager.Instance.PController.transform;
        var zBulletRotation = new Vector3(-shipTransform.right.y, shipTransform.right.x, 0);
        return zBulletRotation;
    }
    #endregion
}
