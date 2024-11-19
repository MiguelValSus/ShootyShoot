using System;
using States;
using States.Main;
using System.Collections;
using UnityEngine;
using core.Audio;
using core.gameManagers;

namespace Player { 
	/// <summary>
	/// Player Character
	/// </summary>
	public class PlayerController : MonoBehaviour
	{
        [Header("Weapons")]
        public WeaponsController  Weapon;
        public PowerUpsController PowerUps;

        [Header("Movement")]
        public PlayerAimMouse AimControl;
		public float FlySpeed = 3.25f;
        public float MaxCharacterVelocity = 1.5f;
        public bool  Active;

        [Header("Energy / Health")]
        public EnergyController EnergyTank;
        public ShieldController ShieldPanel;
        public GameObject ParryShield;

        [Header("Physics")]
        public Transform PlayerTransform;
        public Rigidbody PlayerRigidbody;
        public SphereCollider PlayerCollider;

        [Header("Visuals")]
        public SpriteRenderer ShipRenderer;
        public SpriteRenderer ShadowRenderer;
        public Animator ExplosionFX;
        public ShipAnimController AnimationsController;
        public Color DamageColor;

        [Header("Audio")]
        public SFXPlayer Audio;
        public AudioClip EnhancedLaser;
        public AudioClip ShipExplosion;
        private bool _playingSFX;

        //--State vars--
        [Header("State")]
        public string CharacterState;
        private IState _characterState;

        [Header("Debug")]
        public bool Invincible;

        protected Coroutine _damageCoroutine;

        #region State machine execution
        public void SetState(IState newState)
        {
            //--Exit last state--
            _characterState?.OnExit(this);
            //--Enter new state--
            _characterState = newState;
            CharacterState = _characterState.State();
            _characterState.OnEnter(this);
        }

        private void Awake()
        {
            InitPlayer();
        }

        private void Start()
        {
            StartCoroutine(EnterTheScreen());
        }

        private void InitPlayer()
        {
            Weapon.RestockAmmo();
            PowerUps.ResetPowerUps();
            EnergyTank.RestoreEnergy();
        }

        private void Update()
        {
            if (!Active) return;
            _characterState.GamePlay(this);
        }

        private void FixedUpdate()
        {
            _characterState?.Execute(this);
        }

        public void Interaction() { }
        #endregion

        #region Appear routine
        private IEnumerator EnterTheScreen()
        {
            var startTime = Time.time;
            while (Time.time < startTime + 1f)
            {
                PlayerTransform.localPosition += Vector3.up * (Time.deltaTime*1.75f);
                yield return null;
            }
            SetState(new State_Idle());
            Active = true;
        }
        #endregion

        #region Physics handling
        public Vector2 CalculateMoveForce(Vector3 movementSpeed)
        {
            var maxVelocityChange = MaxCharacterVelocity;
            var currentVelocity = PlayerRigidbody.velocity;
            var velocityChange = movementSpeed - currentVelocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);

            return velocityChange;
        }

        public void StopPlayerRigidbody()
        {
            PlayerRigidbody.velocity = Vector2.zero;
            PlayerRigidbody.isKinematic = true;
            PlayerRigidbody.isKinematic = false;  // This is a lame placeholder trick: by briefly setting the RB to 'kinematic', we make sure that we suddenly stop all physics
        }

        #region Collisions
        private void OnTriggerEnter(Collider col)
        {
            switch (col.gameObject.layer)
            {
                case 6: //Enemies
                {
                    if (col.name.Contains("Enemy_Turret")) return;
                    var enemyHit = col.GetComponent<Enemy>();
                    TakeDamage(enemyHit != null ? -enemyHit.m_hit_damage : -10);
                }
                break;
                case 7: //Bullets
                {
                    var bulletHit = col.GetComponent<EnemyBullet>();
                    if (bulletHit == null) return;
                    TakeDamage(-bulletHit.m_bullet_damage);
                }
                break;
                case 9: //Death Boundaries
                {
                    EnergyTank.EmptyEnergy();
                    ExplodeShip();
                }
                break;
            }
        }

        private void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.layer)
            {
                case 6: //Enemies
                {
                    var enemyHit = col.gameObject.GetComponent<Enemy>();
                    if (enemyHit == null) return;
                    TakeDamage(-enemyHit.m_hit_damage);
                }
                break;
            }
        }
        #endregion

        #endregion

        #region Timed events
        public void StartPlayerCoroutine(float awaitTime, Action endCallback)
        {
            StartCoroutine(AwaitAndChangeState(awaitTime, endCallback));
        }

        private IEnumerator AwaitAndChangeState(float awaitTime, Action endAction)
        {
            var startTime = Time.time;
            while (Time.time < startTime + awaitTime)
            {
                yield return null;
            }
            //--Timed event callback--
            endAction();
        }
        #endregion

        #region Gameplay
        public void Shoot()
		{
            Weapon.Fire();
            PlayLaserSFX();
        }

        public void ParryHit()
        {
            // Parry logic. Disabled for now, requires further iterating and testing
            //SetState(new State_ParryBoost());
        }
        
        public void TakeDamage(float damage)
        {
            if (Invincible) return;

            ShieldPanel.HitShields(damage);
            if (ShieldPanel.Charge > 13.5f) return;  // Our shields are active!

            EnergyTank.ModifyEnergyLevels(damage);

            if (EnergyTank.Energy > 0f) {
                if (_damageCoroutine != null) StopCoroutine(_damageCoroutine);
                _damageCoroutine = StartCoroutine(FlashPlayerDamage());
            } else {
                ExplodeShip();
            }
        }

        private IEnumerator FlashPlayerDamage()
        {
            var startTime = Time.time;
            ShipRenderer.color = DamageColor;

            while (Time.time < startTime + .1f)
            {
                yield return null;
            }
            ShipRenderer.color = Color.white;
        }
        #endregion

        #region GameOver
        public void ExplodeShip()
        {
            // Explode ship
            StopPlayerRigidbody();
            PlayerRigidbody.isKinematic = true;
            PlayerCollider.enabled = false;
            ShipRenderer.enabled = false;
            ShadowRenderer.enabled = false;
            ParryShield.SetActive(false);
            PowerUps.ResetPowerUps();
            EnergyTank.Empty = true;
            ExplosionFX.gameObject.SetActive(true);
            // Play explosion SFX
            Audio.Audio.Stop();
            Audio.m_sound_effect = ShipExplosion;
            Audio.PlaySound();
            // End game
            GameManager.Instance.OnGameOver();
        }
        #endregion

        #region Audio
        public void UpgradeLaserSFX()
        {
            Audio.m_sound_effect = EnhancedLaser;
            Audio.Audio.volume = .25f;
        }

        private void PlayLaserSFX()
        {
            StartCoroutine(ControlSFXRate());
        }

        private IEnumerator ControlSFXRate()
        {
            if (_playingSFX || !Active) yield break;

            Audio.PlaySound();
            var startTime = Time.time;
            _playingSFX = true;

            while (Time.time < startTime + .15f)
            {
                yield return null;
            }
            _playingSFX = false;
        }
        #endregion
    }
}