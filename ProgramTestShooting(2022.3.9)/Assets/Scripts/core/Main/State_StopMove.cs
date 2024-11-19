using Player;
using States;
using UnityEngine;

namespace States.Main {
    public class State_StopMove : IState {
	    
        #region Variables
        private PlayerController _player;
        private Rigidbody _rigidBody;
        
        private float _awaitTime;
        private readonly float _hMoveForce, _vMoveForce; //Inherited input from last movement state
        #endregion
		
        #region State check
        public State_StopMove(float hForce, float vForce) {
            _hMoveForce = hForce;
            _vMoveForce = vForce;
        }
		
        public string State() {
            return nameof(State_StopMove);	
        }
        #endregion
		
        #region Initialization
        public void OnEnter (PlayerController player) {
            CacheStateVariables(player);
            PlayStateAnimation();
            SetStateEndDelay();
        }

        private void CacheStateVariables(PlayerController player) {
            _player = player;
            _awaitTime = .1f;
            _rigidBody = player.PlayerRigidbody;
        }

        private void SetStateEndDelay() {
            //--Wait and go back to Idle state--
            void EndActionEvent() => _player.SetState(new State_Idle());
            _player.StartPlayerCoroutine(_awaitTime, EndActionEvent);
        }
		
        public void OnExit (PlayerController player) { }
        #endregion
		
        #region Update
        public void GamePlay (PlayerController player) { }
        #endregion

        #region FixedUpdate
        public void Execute(PlayerController player) {
            Brake();
        }
        #endregion
		
        #region Interaction
        public void Interaction (PlayerController player) { }
        #endregion
        
        #region Movement
        private void Brake() {
            //--Strong opposite force to the one received (to slow down the rigidBody)--
            var xSpeed = _player.FlySpeed * _hMoveForce;
            var ySpeed = _player.FlySpeed * _vMoveForce;
            var finalSpeed = new Vector2(-xSpeed, -ySpeed) * 2000; 
            var forceDir = _player.CalculateMoveForce(finalSpeed);
            _rigidBody.AddForce(forceDir);
        }
        #endregion
		
        #region Animation events
        public void AnimationEvent(string interactionType = "") {
            _player.SetState(new State_Idle());
        }
        #endregion

        #region Animation
        private void PlayStateAnimation() {
            //--BAREBONES EXAMPLE OF POTENTIAL USE CASE OF ANIMATIONS--
            _player.AnimationsController.Play("Ship_Idle");
        }
        #endregion
    }
}