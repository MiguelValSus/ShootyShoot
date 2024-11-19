using UnityEngine;

namespace core {
    public static class InputManager {
        
        #region Variables
        //Key codes
        public const KeyCode Action      = KeyCode.Space;
        public const KeyCode Pause       = KeyCode.Escape;
        public const KeyCode MoveLeft    = KeyCode.LeftArrow;
        public const KeyCode MoveRight   = KeyCode.RightArrow;
        public const KeyCode MoveUp      = KeyCode.UpArrow;
        public const KeyCode MoveDown    = KeyCode.DownArrow;
        public const KeyCode ChargeMouse = KeyCode.Mouse1;
        public const KeyCode ShootMouse  = KeyCode.Mouse0;
        public const KeyCode Charge      = KeyCode.Q;
        public const KeyCode Shoot       = KeyCode.F;
        public const KeyCode Retry       = KeyCode.R;
        //Axis
        public const string Horizontal = "Horizontal";
        public const string Vertical   = "Vertical";

        public const float AxisSensitivity = .07f;
        public const float AxisAcceptedMin = .65f;
        public const float AxisDeadZone    = .55f;
        #endregion
        
        #region Input detection
        public static float GetAxis(string axisName) {
            return Input.GetAxisRaw(axisName);
        }
        
        public static bool GetButtonDown(KeyCode buttonName) {
            return Input.GetKeyDown(buttonName);
        }
        
        public static bool GetButtonUp(KeyCode buttonName) {
            return Input.GetKeyUp(buttonName);
        }
        
        public static bool GetButton(KeyCode buttonName) {
            return Input.GetKey(buttonName);
        }
        #endregion
    }
}