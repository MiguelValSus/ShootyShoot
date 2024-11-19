using UnityEngine;

public class BoundariesCleaner : MonoBehaviour
{

    #region Cleanup
    private void OnTriggerEnter(Collider col)
    {
        switch(col.gameObject.layer)
        {
            case 6: //Enemies
            {
                var enemyOutOfBounds = col.GetComponent<Enemy>();
                if (enemyOutOfBounds == null) return;
                enemyOutOfBounds.ResetEnemyObject();
            }
            break;
            case 7: //Bullets
            {
                var bulletOutOfBounds = col.GetComponent<PlayerBullet>();
                if (bulletOutOfBounds == null) return;
                bulletOutOfBounds.ResetObject();
            }
            break;
            case 8: //Power-Ups
            {
                Destroy(col.gameObject);
            }
            break;
        }
    }
    #endregion
}
