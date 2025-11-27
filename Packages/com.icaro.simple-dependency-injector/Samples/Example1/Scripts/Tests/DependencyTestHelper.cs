using SimpleDependencyInjector;

public static class DependencyTestHelper
{
    public static Injected<T> CreateInjected<T>(T instance) where T : class
    {
        var injected = new Injected<T>();
#if UNITY_INCLUDE_TESTS
        injected.InjectTestInstance(instance);
#endif
        return injected;
    }
}