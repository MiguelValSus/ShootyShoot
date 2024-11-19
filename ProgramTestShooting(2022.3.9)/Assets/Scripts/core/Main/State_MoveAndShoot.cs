using core;
using Player;
using States;
using UnityEngine;

namespace States.Main {
    public class State_MoveAndShoot : IState {
	    
        #region Variables
        private PlayerController _player;
        private Rigidbody _rigidBody;
        private Transform _playerTR;
        private float _xMoveForce, _vMoveForce; //When input in any of those axes is registered, otherwise value is 0

        private enum Anims
        {
            STRAFE_LEFT, 
            STRAFE_RIGHT,
            SHIP_IDLE
        }
        #endregion
		
        #region State check
        public State_MoveAndShoot() { }
		
        public string State() {
            return nameof(State_MoveAndShoot);	
        }
        #endregion
		
        #region Initialization
        public void OnEnter (PlayerController player) {
	        CachePlayerComponents(player);
            //PlayMovementSFX();
        }

        private void CachePlayerComponents(PlayerController player) {
	        _player = player;
	        _playerTR = player.PlayerTransform;
	        _rigidBody = player.PlayerRigidbody;
	        _rigidBody.isKinematic = false;
        }

        public void OnExit(PlayerController player) {
	        //StopMovementSFX();
	        _playerTR.localRotation = Quaternion.identity;
        }
        #endregion
		
        #region Update
        public void GamePlay (PlayerController player) {
            if (_player.EnergyTank.Empty) return;
            //--MOVEMENT--
            GetInputDirection();
	        CheckMovementStateChangers();
            //--PEW PEW--
            CheckShootingInput();
        }

        private void GetInputDirection() {
	        CheckHorizontal();
	        CheckVertical();
        }

        private void CheckHorizontal() {
            if (Controls.MoveRight()) {
                _xMoveForce = 1;
                PlayStateAnimation(Anims.STRAFE_RIGHT);
            }
            else if (Controls.MoveLeft()) {
                _xMoveForce = -1;
                PlayStateAnimation(Anims.STRAFE_LEFT);
            }
            else {
                _xMoveForce = 0;
                PlayStateAnimation(Anims.SHIP_IDLE);
            }
        }

        private void CheckVertical() {
	        if (Controls.MoveUp()) {
		        _vMoveForce = 1;
	        } else if (Controls.MoveDown()) {
		        _vMoveForce = -1;
	        } else _vMoveForce = 0;
        }

        private void CheckMovementStateChangers() {
	        if (Controls.StopMoving()) {
                _player.SetState(new State_Idle()); //_player.SetState(new State_StopMove(_xMoveForce, _vMoveForce));
		        return;
	        }
        }
        #endregion

        #region FixedUpdate
        public void Execute (PlayerController player) {
	        Fly();
        }
        #endregion
        
        #region Movement
        private void Fly() {
	        var xSpeed = _player.FlySpeed * _xMoveForce;
	        var ySpeed = _player.FlySpeed * _vMoveForce;
	        var finalSpeed = new Vector2(xSpeed, ySpeed); 
	        var forceDir = _player.CalculateMoveForce(finalSpeed);
	        _rigidBody.AddForce(forceDir, ForceMode.VelocityChange);
        }
        #endregion

        #region Shooting
        private void CheckShootingInput() {
            if (Controls.Fire()) {
                _player.Shoot();
            }
        }
        #endregion

        #region Interaction
        public void Interaction(PlayerController player)
        {
            //Switch to whatever Interaction state we have implemented
        }
        #endregion

        #region Animation events
        public void AnimationEvent(string interactionType = "") { }
        #endregion

        #region Animation
        private void PlayStateAnimation(Anims animToPlay) {
            //--BAREBONES EXAMPLE OF POTENTIAL USE CASE OF ANIMATIONS--
            switch (animToPlay)
            {
                case Anims.STRAFE_LEFT:
                {
                    _player.AnimationsController.Play("Strafe_Left");
                }
                break;
                case Anims.STRAFE_RIGHT:
                {
                    _player.AnimationsController.Play("Strafe_Right");
                }
                break;
                case Anims.SHIP_IDLE:
                {
                    _player.AnimationsController.Play("Ship_Idle");
                }
                break;
                default:
                    Debug.LogError("Animation " + animToPlay + " not found");
                break;
            }
        }
        #endregion

        #region Audio
        /*private void PlayMovementSFX() {
	        _player.CharacterSounds.SetSFXSoundID(0); // Shooting audio ID
	        _player.CharacterSounds.PlayShipSFX();
        }

        private void StopMovementSFX() {
	        _player.CharacterSounds.StopShipSFX();
        }*/
        #endregion
    }
}