using UnityEngine;

namespace core.config {
	public static class Settings {
		
		#region Tech
		public static void SetFramerate() {
			#if UNITY_ANDROID || UNITY_IOS
			Application.targetFrameRate = 60;
			#else
			Application.targetFrameRate = 120;
			#endif
		}

        public static void SetAspect() {
			#if UNITY_STANDALONE
            Camera.main.aspect = 16f / 9f;
			#endif
        }

        public static void SetMouseVisibility(Texture2D cursorTexture = null, bool visible = true) {
			#if UNITY_STANDALONE// && !UNITY_EDITOR
			Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
			Cursor.visible = visible;
			#endif
        }

		public static void SetGameSpeed(float timeScaleValue = 1f) {
			Time.timeScale = timeScaleValue;
		}
        #endregion
    }
}