

namespace SimpleDependencyInjector
{
    public struct Injected<T> where T : class
    {
        private T _cached;

        public T Value
        {
            get
            {
                if (_cached == null)
                    _cached = DependencyInjector.GetService<T>();

                return _cached;
            }
        }

#if UNITY_INCLUDE_TESTS
        public void InjectTestInstance(T instance)
        {
            _cached = instance;
        }
#endif

        public static implicit operator T(Injected<T> injected)
        {
            return injected.Value;
        }
    }
}
