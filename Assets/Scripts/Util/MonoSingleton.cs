using UnityEngine;

namespace Util
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        public static T Get
        {
            get
            {
                if (_instance == null)
                    Debug.LogError($"Singleton instance of {typeof(T).FullName} doesn't exist");
                return _instance;
            }
        }

        private static T _instance;

        [SerializeField] private bool isDontDestroyOnLoad;

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                if (isDontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
                _instance = GetComponent<T>();
            }


        }

        public static bool IsNull => _instance == null;
    }

}