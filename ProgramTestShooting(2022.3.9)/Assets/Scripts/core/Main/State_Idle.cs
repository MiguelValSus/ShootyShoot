using core;
using Player;
using States;
using UnityEngine;

namespace States.Main {
    public class State_Idle : IState {
	    
	    #region Variables
        private PlayerController _player;
		private Rigidbody _rigidBody;
		private Transform _playerTr;
		#endregion
		
		#region State check
		public State_Idle() { }
		
		public string State() {
			return nameof(State_Idle);	
		}
		#endregion
		
		#region Initialization
		public void OnEnter (PlayerController player) {
			CachePlayerComponents(player);
			StopCharacter();
			PlayStateAnimation();
		}

		private void CachePlayerComponents(PlayerController player) {
			_player = player;
			_playerTr = player.PlayerTransform;
			_rigidBody = player.PlayerRigidbody;
		}

		private void StopCharacter() {
			_player.StopPlayerRigidbody();
		}

		public void OnExit(PlayerController player) { }
		#endregion
		
		#region Update
		public void GamePlay(PlayerController player) {
			if (_player.EnergyTank.Empty) return;
			CheckMovementInput();
            CheckShootingInput();
        }

		private void CheckMovementInput() {
			//Movement inputs
            if (Controls.MoveLeft()	 ||
                Controls.MoveRight() ||
                Controls.MoveUp()	 ||
                Controls.MoveDown())
            {
                _player.SetState(new State_MoveAndShoot());
            }
        }
        #endregion

        #region Shooting
        private void CheckShootingInput() {
            if (Controls.Fire()) {
                _player.Shoot();
            }
        }
        #endregion

        #region FixedUpdate
        public void Execute (PlayerController player) { }
		#endregion
		
		#region Interaction
		public void Interaction (PlayerController player) {
            //Switch to whatever Interaction state we have implemented
        }
        #endregion

        #region Animation events
        public void AnimationEvent(string interactionType = "") { }
		#endregion

		#region Animation
		private void PlayStateAnimation() {
			//--BAREBONES EXAMPLE OF POTENTIAL USE CASE OF ANIMATIONS--
			_player.AnimationsController.Play("Ship_Idle");
		}
		#endregion
    }
}