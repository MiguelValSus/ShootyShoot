using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTroopOrders : MonoBehaviour
{
    [Header("Ground Troops General")]
    public GroundTroopsSpawner TroopsSpawner;
    [Header("Ground Troops Formations")]
    public List<GameObject> Battalions;
    public float TimeUntilNextWave = 10f;
    public int SentBattalions;

    #region Combat
    public IEnumerator SendForthFormations()
    {
        if (SentBattalions >= Battalions.Count) yield break;

        for (var b = 0; b < Battalions.Count; ++b)
        {
            var startTime = Time.time;
            while (Time.time < startTime + TimeUntilNextWave)
            {
                yield return null;
            }
            Battalions[b].SetActive(true);
            ++SentBattalions;
        }
        var endTime = Time.time;
        while (Time.time < endTime + TimeUntilNextWave)
        {
            yield return null;
        }
        DisableUsedBattalions();
        TroopsSpawner.AwaitOrders();
        StopCoroutine(SendForthFormations());
    }
    #endregion

    #region Disabling
    private void DisableUsedBattalions()
    {
        for (var b = 0; b < Battalions.Count; ++b)
        {
            Battalions[b].SetActive(false);
        }
    }
    #endregion
}
