using ServiceInterfaces;
using SimpleDependencyInjector;

public class GameManager : Singleton<GameManager>
{
    protected override void Init()
    {
        base.Init();

        DependencyInjector.Instance.Setup();

        // using service directly
        DependencyInjector.GetService<IUserService>().SetName("User 1");
    }
}
