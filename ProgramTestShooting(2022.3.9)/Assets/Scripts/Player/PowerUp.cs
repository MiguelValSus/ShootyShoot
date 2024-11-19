using UnityEngine;
using Player;
using core.Audio;

public class PowerUp : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer ItemSprite;

    [Header("Audio")]
    public SFXPlayer ItemSFX;

    [Header("Settings")]
    public Type PowerUp_Type;
    public float PowerUp_MovementDelta = .5f;

    private Transform _itemTR;

    public enum Type
    {
        ENHANCE_LEFT,
        ENHANCE_RIGHT,
        FIREPOWER,
        ENERGY
    }

    #region Initialization
    private void Awake()
    {
        // Cache components for a more performant acces to them
        _itemTR = transform;
    }
    #endregion

    #region Movement
    private void Update()
    {
        MovePowerUp();
    }

    private void MovePowerUp()
    {
        var itemCurrentPos = _itemTR.position;
        itemCurrentPos.y -= Time.deltaTime * PowerUp_MovementDelta;
        _itemTR.position = itemCurrentPos;
    }
    #endregion

    #region Powering up
    public void GetPowerUp()
    {
        PowerUpsManager.Instance.PickUpItem(PowerUp_Type);
        ItemSFX.PlaySound();
        Destroy(gameObject);
    }

    private void OnTriggerEnter (Collider col)
    {
        // Will only collide with Player as the Physics collision matrix is configured that way
        var player = col.GetComponent<PlayerController>();
        if (player == null) return;
        var shipState = player.EnergyTank;
        if (!shipState.Empty) GetPowerUp();
    }
    #endregion
}
