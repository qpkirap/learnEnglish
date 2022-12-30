using UnityEngine;

namespace Util
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static bool IS_EXITING = false;
        protected bool isExiting => IS_EXITING;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeStartSingleton()
        {
            IS_EXITING = false;
        }
        
        public static T Instance
        {
            get
            {
                if (instance == null && !IS_EXITING)
                {
                    instance = (T) FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        DebugLogFormat("Multiple instances of a singleton gameObject '{0}'", instance.gameObject.name);

                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = string.Format("{0}(singleton)", typeof(T).ToString());

                        Singleton<T> component = instance.GetComponent<Singleton<T>>();
                        component.OnCreate();

                        if (component.ShouldNotDestroyOnLoad()) DontDestroyOnLoad(singleton);
                        DebugLogFormat("[Singleton] Creating an instance of {0} with DontDestroyOnLoad", typeof(T));
                    } 
                    else 
                    {
                        DebugLogFormat("[Singleton] Using instance already created '{0}'", instance.gameObject.name);
                    }
                }

                return instance;
            }
        }
        
        protected virtual void OnCreate() 
        { 
            return; 
        }
        
        protected void WakeUp()
        {
            return;
        }
        
        protected virtual bool ShouldNotDestroyOnLoad()
        {
            return true;
        }
        
        private void OnApplicationQuit()
        {
            IS_EXITING = true;
        }
        
        
        private static void DebugLogFormat(string content, params object[] parameters)
        {
            Debug.LogFormat(content, parameters);
        }
    }

}