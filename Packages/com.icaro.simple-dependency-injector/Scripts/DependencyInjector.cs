using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleDependencyInjector
{
    public class DependencyInjector : Singleton<DependencyInjector>
    {
        [OnValueChanged("RemoveScriptableObjectDuplicates")]
        [SerializeField]
        private ScriptableObject[] _scriptableObjectServices;

        [OnValueChanged("RemoveMonoBehaviourDuplicates")]
        [SerializeField]
        private MonoBehaviour[] _monoBehaviorServices;

        [SerializeField]
        private bool _AutoInit = false;

        private Dictionary<Type, ScriptableObject> _SOServiceDictionary;
        private Dictionary<Type, MonoBehaviour> _MonoServiceDictionary;

        private static bool _isSetup = false;

        public static bool IsWorking => _isSetup;

        protected override void Init()
        {
            DontDestroyOnLoad(this);

            if (_AutoInit)
                Setup();
        }

        protected void OnDestroy()
        {
            _isSetup = false;
        }

        public void Setup()
        {
            if (_isSetup)
                return;

            _SOServiceDictionary = new Dictionary<Type, ScriptableObject>();
            _MonoServiceDictionary = new Dictionary<Type, MonoBehaviour>();

            RegisterServices(_scriptableObjectServices, _SOServiceDictionary, "SO");
            RegisterServices(_monoBehaviorServices, _MonoServiceDictionary, "MonoBehavior");

            _isSetup = true;
        }

        private void RegisterServices<T>(IEnumerable<T> serviceList,
                                         Dictionary<Type, T> dictionary,
                                         string errorPrefix) where T : UnityEngine.Object
        {
            foreach (var service in serviceList)
            {
                var interfaces = service.GetType().GetInterfaces();
                var serviceType = GetDerivedServiceInterfaceType(interfaces);

                if (serviceType != null && !dictionary.ContainsKey(serviceType))
                {
                    dictionary.Add(serviceType, service);
                }
                else
                {
                    Debug.LogError($"Duplication in {errorPrefix} Dependency Injector list: {serviceType}");
                }
            }
        }

        private Type GetDerivedServiceInterfaceType(Type[] aInterfaces)
        {
            if (aInterfaces.Length == 0)
                return null;

            for (var i = 0; i < aInterfaces.Length; ++i)
            {
                if (typeof(IService) != aInterfaces[i] && typeof(IService).IsAssignableFrom(aInterfaces[i]))
                    return aInterfaces[i];
            }

            return null;
        }

        public static T GetService<T>() where T : class
        {
            if (instance == null)
            {
                Debug.LogError($"Dependency Injector is null");
                return null;
            }

            if (Instance._SOServiceDictionary.TryGetValue(typeof(T), out var service))
                return service as T;

            if (Instance._MonoServiceDictionary.TryGetValue(typeof(T), out var monoservice))
                return monoservice as T;

            Debug.LogError($"No service found for {typeof(T)}");

            return null;
        }

        public static T LazyGet<T>(ref T field) where T : class
        {
            if (field == null)
                field = GetService<T>();

            return field;
        }

#if UNITY_EDITOR

        private void RemoveScriptableObjectDuplicates()
        {
            List<ScriptableObject> list = new List<ScriptableObject>(_scriptableObjectServices);
            HashSet<ScriptableObject> set = new HashSet<ScriptableObject>();
            List<ScriptableObject> duplicates = new List<ScriptableObject>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!set.Add(list[i]))
                {
                    duplicates.Add(list[i]);
                }
            }

            foreach (var duplicate in duplicates)
            {
                list.Remove(duplicate);
            }

            if (duplicates.Count > 0)
            {
                _scriptableObjectServices = list.ToArray();
                Debug.LogWarning("Duplicate elements were removed from the SO array.");
            }
        }

        private void RemoveMonoBehaviourDuplicates()
        {
            List<MonoBehaviour> list = new List<MonoBehaviour>(_monoBehaviorServices);
            HashSet<MonoBehaviour> set = new HashSet<MonoBehaviour>();
            List<MonoBehaviour> duplicates = new List<MonoBehaviour>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!set.Add(list[i]))
                {
                    duplicates.Add(list[i]);
                }
            }

            foreach (var duplicate in duplicates)
            {
                list.Remove(duplicate);
            }

            if (duplicates.Count > 0)
            {
                _monoBehaviorServices = list.ToArray();
                Debug.LogWarning("Duplicate elements were removed from the monobehaviour array.");
            }
        }
#endif
    }
}

