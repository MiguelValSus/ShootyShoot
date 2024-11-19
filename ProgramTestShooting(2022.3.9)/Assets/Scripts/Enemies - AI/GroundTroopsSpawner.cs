using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTroopsSpawner : MonoBehaviour
{
    [Header("Ground Troops Formations")]
    public List<GroundTroopOrders> TroopOrders;
    public float TimeBetweenBattalions = 10f;
    public int TroopCount;

    #region Send army forces
    public void SendGroundTroopsToBattle()
    {
        if (TroopCount >= TroopOrders.Count) return;
        StartCoroutine(TroopOrders[TroopCount].SendForthFormations());
        ++TroopCount;
    }

    public void AwaitOrders()
    {
        StartCoroutine(DelayNextWave());
    }

    private IEnumerator DelayNextWave()
    {
        var startTime = Time.time;
        while (Time.time < startTime + TimeBetweenBattalions)
        {
            yield return null;
        }
        SendGroundTroopsToBattle();
    }
    #endregion
}
