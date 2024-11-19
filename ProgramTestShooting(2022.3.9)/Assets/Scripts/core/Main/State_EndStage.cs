using Player;
using States;
using UnityEngine;

namespace States.Main {
    public class State_EndStage : IState {

        #region Variables
        private PlayerController _player;
        private Rigidbody _rigidBody;
        private Transform _playerTr;
        private float _targetY = 7.5f;
        private float _moveDuration = 1f;
        private float _initialY;
        private float _elapsedTime = 0f;
        #endregion

        #region State check
        public State_EndStage() { }

        public string State() {
            return nameof(State_EndStage);
        }
        #endregion

        #region Initialization
        public void OnEnter(PlayerController player) {
            CachePlayerComponents(player);
            StopCharacter();
            ShipSetup();
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

        private void ShipSetup() {
            _initialY = _playerTr.position.y;
            _player.PlayerCollider.enabled = false;
        }

        public void OnExit(PlayerController player) { }
        #endregion

        #region Update
        public void GamePlay(PlayerController player) {
            ExitScreen();
        }
        private void ExitScreen()
        {
            if (_elapsedTime >= _moveDuration) return;

            _elapsedTime += Time.deltaTime;
            var progress = Mathf.Clamp01(_elapsedTime / _moveDuration);
            var newY = Mathf.Lerp(_initialY, _targetY, progress);
            _playerTr.localPosition = new Vector3(_playerTr.localPosition.x, newY, _playerTr.localPosition.z);
        }
        #endregion

        #region FixedUpdate
        public void Execute(PlayerController player) { }
        #endregion

        #region Interaction
        public void Interaction(PlayerController player) { }
        #endregion

        #region Animation events
        public void AnimationEvent(string interactionType = "") { }
        #endregion

        #region Animation
        private void PlayStateAnimation() {
            _player.AnimationsController.Play("Ship_Idle");
        }
        #endregion
    }
}