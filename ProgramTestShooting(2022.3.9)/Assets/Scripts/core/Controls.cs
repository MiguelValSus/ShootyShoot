using System;

namespace core {
    public static class Controls {
        
        #region Movement input
        public static bool MoveLeft() {
            return InputManager.GetAxis(InputManager.Horizontal) < -InputManager.AxisDeadZone;
        }

        public static bool MoveRight() {
            return InputManager.GetAxis(InputManager.Horizontal) > InputManager.AxisDeadZone;
        }

        public static bool MoveUp() {
            return InputManager.GetAxis(InputManager.Vertical) > InputManager.AxisDeadZone;
        }

        public static bool MoveDown() {
            return InputManager.GetAxis(InputManager.Vertical) < -InputManager.AxisDeadZone;
        }
        
        public static bool StopMoving() {
            return Math.Abs(InputManager.GetAxis(InputManager.Horizontal)) < InputManager.AxisSensitivity
                && Math.Abs(InputManager.GetAxis(InputManager.Vertical))   < InputManager.AxisSensitivity;
        }
        #endregion

        #region Interaction input
        public static bool Interact() {
            return InputManager.GetButtonDown(InputManager.Action);
        }
        #endregion

        #region Shooting input
        public static bool Fire() {
            return InputManager.GetButton(InputManager.Shoot) || InputManager.GetButton(InputManager.ShootMouse);
        }
        #endregion

        #region Parry input
        public static bool ChargeShield()
        {
            return InputManager.GetButton(InputManager.Charge) || InputManager.GetButton(InputManager.ChargeMouse);
        }

        public static bool Parry()
        {
            return InputManager.GetButtonUp(InputManager.Charge) || InputManager.GetButtonUp(InputManager.ChargeMouse);
        }
        #endregion

        #region Pause input
        public static bool Pause() {
            return InputManager.GetButtonDown(InputManager.Pause);
        }
        #endregion

        #region Debug inputs //TODO: remove or at least block them in final build
        public static bool Retry() {
            return InputManager.GetButtonDown(InputManager.Retry);
        }
        #endregion
    }
}