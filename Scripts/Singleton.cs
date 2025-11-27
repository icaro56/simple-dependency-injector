using System.Text;
using UnityEngine;

namespace SimpleDependencyInjector
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = (T)FindFirstObjectByType(typeof(T));

                    if (!instance)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("An instance of ");
                        stringBuilder.Append(typeof(T).ToString());
                        stringBuilder.Append(" is needed in the scene, but there is none.");

                        Debug.LogWarning(stringBuilder.ToString());
                    }
                }

                return instance;
            }
        }

        protected void Awake()
        {
            if (Instance != this)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Destroying Singleton of ");
                stringBuilder.Append(typeof(T).ToString());
                stringBuilder.Append(" because already there is an instance in the scene");

                Debug.Log(stringBuilder.ToString());
                Destroy(this.gameObject);
            }
            else
            {
                Init();
            }
        }

        // Put the initialization of Singleton class here.
        // DO NOT override Awake method.
        protected virtual void Init() { }
    }
}
