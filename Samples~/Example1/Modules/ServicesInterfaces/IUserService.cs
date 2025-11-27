using SimpleDependencyInjector;

namespace ServiceInterfaces
{
    public interface IUserService : IService
    {
        string GetName();
        void SetName(string name);
    }
}

