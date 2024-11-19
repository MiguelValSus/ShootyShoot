using core.gameManagers;
using UnityEngine;

namespace Player
{
    public class WeaponsController : MonoBehaviour
    {
        [Header("Power-Ups")]
        public PowerUpsController PowerUps;

        [Header("Bullets")]
        public Ammunition BulletsPool;

        [Header("Visuals")]
        public Sprite MainGun_Basic;
        public Sprite MainGun_Enhanced;
        public Sprite DroneBullets_Basic;
        public Sprite DroneBullets_Enhanced;

        [Header("State")]
        public bool Enhanced;

        #region Init & Load
        public void RestockAmmo()
        {
            BulletsPool.LoadBulletsPool();
        }
        #endregion

        #region Pew Pew
        public void Fire()
        {
            FireShipGun();
            FirePowerUps();
            Cooldown();
        }

        private void FirePowerUps()
        {
            if (PowerUps.FollowerL.activeSelf)
            {
                FireShipGun(PowerUps.FollowerL.transform, Enhanced ? DroneBullets_Enhanced : DroneBullets_Basic);
            }
            if (PowerUps.FollowerR.activeSelf)
            {
                FireShipGun(PowerUps.FollowerR.transform, Enhanced ? DroneBullets_Enhanced : DroneBullets_Basic);
            }
        }

        private void FireShipGun(Transform originPoint = null, Sprite bulletSprite = null)
        {
            PlayerBullet bullet = BulletsPool.FetchBullet();
            if (bullet == null) return;
            
            bullet.transform.position = originPoint == null ? transform.position : originPoint.position;
            bullet.ResetObject();

            if (bulletSprite != null) bullet.GetComponent<PlayerBullet>().m_bullet_sprite.sprite = bulletSprite;
            else bullet.GetComponent<PlayerBullet>().m_bullet_sprite.sprite = Enhanced ? MainGun_Enhanced : MainGun_Basic;

            if (Enhanced)
            {
                bullet.transform.localScale *= 2;
                bullet.m_bullet_damage *= 2;
            }

            bullet.transform.rotation = GameManager.Instance.PController.transform.rotation;  // Bullets inherit the ship's rotation
            bullet.gameObject.SetActive(true);
            bullet.fired = true;
        }

        private void Cooldown()
        {
            BulletsPool.CoolDownGuns();
        }
        #endregion
    }
}