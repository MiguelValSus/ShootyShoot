using core;
using core.gameManagers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpsManager : SingletonBase<PowerUpsManager>
{
    [Header("Prefabs")]
    public Transform SpawnParent;
    public List<GameObject> PowerUps;
    public Vector2 SpawnDelayRange;
    public int MaxItemsToSpawn;

    private float _spawnDelay; // In seconds
    private Coroutine _spawnRoutine;
    private List<GameObject> _remainingPowerUps;

    #region Initialization
    private void Awake()
    {
        InitManager();
        _remainingPowerUps = PowerUps.ToList();
    }

    private void OnDestroy() { }

    private void Start() { }

    private void InitManager()
    {
        Instance = this;
    }

    public void SpawnPowerUp(float delay, bool randomPowerUp = false)  // For either Sequential or Random Power-up spawning from a fixed list of them
    {
        if (MaxItemsToSpawn <= 0) return;
        if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
        if (randomPowerUp)
            _spawnRoutine = StartCoroutine(SpawnPowerUpRandom(delay, randomPowerUp));
    }

    public void SpawnPowerUpPrecise(GameObject powerUp)  // For specific Random Power-up instancing
    {
        if (powerUp == null) return;
        var spawnedItem = Instantiate(powerUp, SpawnParent);
        var spawnPosX = Random.Range(-6f, 6f);
        var spawnSpot = new Vector3(spawnPosX, 5.5f, 0f);
        spawnedItem.transform.localPosition = spawnSpot;
    }
    #endregion

    #region Power-up lifetime
    private IEnumerator SpawnPowerUpRandom(float delay, bool randomPick = true, int itemIndex = 0)
    {
        if (_remainingPowerUps.Count <= 0) yield break;

        var startTime = Time.time;

        while (Time.time < startTime + delay)
        {
            yield return null;
        }

        var pickIndex = randomPick ? Random.Range(0, _remainingPowerUps.Count) : itemIndex;
        var spawnedItem = Instantiate(_remainingPowerUps[pickIndex], SpawnParent);
        var spawnPosX = Random.Range(-6f, 6f);
        var spawnSpot = new Vector3(spawnPosX, 5.5f, 0f);

        if (randomPick) {
            _remainingPowerUps.RemoveAt(pickIndex);
            _remainingPowerUps.TrimExcess();
        }
        spawnedItem.transform.localPosition = spawnSpot;
        --MaxItemsToSpawn;
        /*
        // Keep spawning
        var powerUpPick = randomPick ? Random.Range(SpawnDelayRange.x, SpawnDelayRange.y) : 0f;
        SpawnPowerUp(powerUpPick, randomPick);*/
    }
    #endregion

    #region Pickup
    public void PickUpItem(PowerUp.Type powerUpType)
    {
        switch(powerUpType)
        {
            case PowerUp.Type.ENHANCE_LEFT:
            {
                var follower = GameManager.Instance.PController.PowerUps.FollowerL;
                GameManager.Instance.PController.PowerUps.EnableFollower(follower);
            }
            break;
            case PowerUp.Type.ENHANCE_RIGHT:
            {
                var follower = GameManager.Instance.PController.PowerUps.FollowerR;
                GameManager.Instance.PController.PowerUps.EnableFollower(follower);
            }
            break;
            case PowerUp.Type.FIREPOWER:
            {
                GameManager.Instance.PController.PowerUps.IncreaseFirepower();
            }
            break;
            case PowerUp.Type.ENERGY:
            {
                GameManager.Instance.PController.PowerUps.Refuel();
            }
            break;
            default:
                Debug.LogError("Power-Up " + powerUpType + " not found");
            break;
        }
    }
    #endregion
}
