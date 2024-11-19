using Player;
using States;
using UnityEngine;

namespace States.Main {
    public class State_ParryBoost : IState {

        #region Variables
        private PlayerController _player;
        private Rigidbody _rigidBody;

        private float _awaitTime;
        #endregion

        #region State check
        public State_ParryBoost() { }

        public string State() {
            return nameof(State_ParryBoost);
        }
        #endregion

        #region Initialization
        public void OnEnter(PlayerController player) {
            CacheStateVariables(player);
            PlayStateAnimation();
            SetStateEndDelay();
            Boost();
        }

        private void CacheStateVariables(PlayerController player) {
            _player = player;
            _awaitTime = .5f;
            _rigidBody = player.PlayerRigidbody;
        }

        private void SetStateEndDelay() {
            //--Wait and go back to Idle state--
            void EndActionEvent() => _player.SetState(new State_StopMove(_rigidBody.velocity.x, _rigidBody.velocity.y));
            _player.StartPlayerCoroutine(_awaitTime, EndActionEvent);
        }

        public void OnExit(PlayerController player) {
            _player.ShieldPanel.EmptyShield();
        }
        #endregion

        #region Update
        public void GamePlay(PlayerController player) { }
        #endregion

        #region FixedUpdate
        public void Execute(PlayerController player) { }
        #endregion

        #region Interaction
        public void Interaction(PlayerController player) { }
        #endregion

        #region Movement
        private void Boost() {
            //--Strong forward force to boost the ship--
            var forwardDirection = new Vector3(_player.AimControl.ForwardDirection.x, _player.AimControl.ForwardDirection.y, 0f).normalized;
            _rigidBody.AddForce(forwardDirection*10f, ForceMode.Impulse);
        }
        #endregion

        #region Animation events
        public void AnimationEvent(string interactionType = "") {
            _player.SetState(new State_StopMove(_rigidBody.velocity.x, _rigidBody.velocity.y));
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