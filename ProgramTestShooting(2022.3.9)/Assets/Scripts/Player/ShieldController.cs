using core;
using core.gameManagers;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [Header("Shields")]
    public float Charge = 1;
    public float LoadSpeed = 15f;

    [Header("Status")]
    public bool Parrying;

    #region Energy management
    public void EmptyShield()
    {
        Charge = 1;
        Parrying = false;
        var newShieldsCharge = GameManager.Instance.ShieldBar.localScale;
        newShieldsCharge.y = Charge;
        GameManager.Instance.ShieldBar.localScale = newShieldsCharge;
        GenerateShieldBubble();
    }

    public void ChargeShield(float charge)
    {
        Charge += charge;
        Mathf.Clamp(Charge, 1f, 10f);
        if (Charge < 1) Charge = 1;
        else if (Charge > 50) Charge = 50;
        var newShieldsCharge = GameManager.Instance.ShieldBar.localScale;
        newShieldsCharge.y = Charge;
        Mathf.Clamp(newShieldsCharge.y, 1f, 50f);
        GameManager.Instance.ShieldBar.localScale = newShieldsCharge;
        GenerateShieldBubble();
    }

    private void ManualShieldControls()
    {
        if (Charge >= 1 && Charge < 50)
        {
            if (Controls.ChargeShield())
            {
                ChargeShield(Time.deltaTime * LoadSpeed);
            }
            else
            {
                if (Charge > 1) ChargeShield(-Time.deltaTime * (LoadSpeed * 2));
            }
        } else if (Charge >= 50)
        {
            if (Controls.Parry())
            {
                Parrying = true;
                GameManager.Instance.PController.ParryHit();
            }
        }
    }

    private void Update()
    {
        StabilizeShip();
        if (Parrying) return;
        //ManualShieldControls();  //TODO: Commented out 'cause Shields are now loaded via Power-up
        
    }
    #endregion

    #region Shield FX
    public void HitShields(float damage)
    {
        if (-damage > Charge || Charge+damage <= 0) return;
        ChargeShield(damage);
    }

    private void GenerateShieldBubble()
    {
        var bubbleScaleDelta = Charge / 50;
        var ship = GameManager.Instance.PController;
        ship.ParryShield.transform.localScale = Vector3.one * bubbleScaleDelta;
        ship.PlayerCollider.radius = Charge <= 25 ? .5f : bubbleScaleDelta;
    }
    #endregion

    #region Ship controls
    private void StabilizeShip()
    {
        var ship = GameManager.Instance.PController;
        ship.PlayerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        return; // This next part is only there for 360º Mode

        if (Charge > 25) {
            ship = GameManager.Instance.PController;
            ship.PlayerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        } else {
            ship = GameManager.Instance.PController;
            ship.PlayerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }
    #endregion
}
