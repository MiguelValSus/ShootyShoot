using core.gameManagers;
using Player;
using UnityEngine;

public class PowerUpsController : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponsController Weapons;

    [Header("Followers")]
    public GameObject FollowerL;
    public GameObject FollowerR;

    #region Initialization
    public void ResetPowerUps()
    {
        FollowerL.SetActive(false);
        FollowerR.SetActive(false);
        Weapons.enabled = false;
    }
    #endregion

    #region Power-ups Handling
    public void EnableFollower(GameObject follower)
    {
        follower.SetActive(true);
    }

    public void IncreaseFirepower()
    {
        GameManager.Instance.PController.Weapon.Enhanced = true;
        GameManager.Instance.PController.UpgradeLaserSFX();
    }

    public void Refuel()
    {
        GameManager.Instance.PController.EnergyTank.RestoreEnergy();
        GameManager.Instance.PController.ShieldPanel.ChargeShield(50);
    }
    #endregion
}