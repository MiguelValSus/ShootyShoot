using UnityEngine;

namespace core {
    public class SingletonBase<T> : MonoBehaviour where T:SingletonBase<T> {
        
        #region Instance variables
        private static volatile T _instance;
        private static readonly object _lock = new object();
        #endregion
 
        public static T Instance {
            get
            {
                if (_instance != null) return _instance;
                lock(_lock) {
                    if (_instance != null) return _instance;
                    var go = new GameObject();
                    _instance = go.AddComponent<T>();
                    go.name = typeof(T).ToString();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
            protected set => _instance = value;
        }
    }
}