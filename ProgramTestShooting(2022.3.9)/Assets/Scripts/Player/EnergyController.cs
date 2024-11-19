using core.gameManagers;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
    [Header("Energy")]
    public float Energy = 100;

    [Header("Status")]
    public bool Empty;

    #region Energy management
    public void RestoreEnergy()
    {
        var newEnergyTankSize = GameManager.Instance.HealthBar.localScale;
        Energy = 100f;
        newEnergyTankSize.y = Energy;
        GameManager.Instance.HealthBar.localScale = newEnergyTankSize;
        Empty = false;
    }
    public void EmptyEnergy()
    {
        var newEnergyTankSize = GameManager.Instance.HealthBar.localScale;
        Energy = 0f;
        newEnergyTankSize.y = Energy;
        GameManager.Instance.HealthBar.localScale = newEnergyTankSize;
        Empty = true;
    }

    public void ModifyEnergyLevels(float energySpike)
    {
        var newEnergyTankSize = GameManager.Instance.HealthBar.localScale;
        /*
        Energy += energySpike;
        */        
        Energy -= 33.34f;  // I ended up making it simpler to comply with genre standards: 3 hits and we're done
        Mathf.Clamp(Energy, 0f, 100f);
        newEnergyTankSize.y = Energy;
        Mathf.Clamp(newEnergyTankSize.y, 0f, 100f);
        GameManager.Instance.HealthBar.localScale = newEnergyTankSize;
    }
    #endregion
}
