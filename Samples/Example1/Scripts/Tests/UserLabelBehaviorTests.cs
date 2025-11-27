using NUnit.Framework;
using ServiceInterfaces;
using TMPro;
using UnityEngine;

public class FakeUserService : IUserService
{
    private string _name = "Default";

    public string GetName() => _name;
    public void SetName(string name) => _name = name;
}

public class UserLabelBehaviorTests
{
    private UserLabelBehavior behavior;
    private FakeUserService fakeService;
    private TextMeshProUGUI label;

    [SetUp]
    public void Setup()
    {
        var go = new GameObject("UserLabel");
        behavior = go.AddComponent<UserLabelBehavior>();

        label = new GameObject("Label").AddComponent<TextMeshProUGUI>();
        behavior.SetLabelForTesting(label);

        fakeService = new FakeUserService();
        fakeService.SetName("TestUser");

        behavior.SetUserServiceForTesting(
            DependencyTestHelper.CreateInjected<IUserService>(fakeService)
        );
    }

    [Test]
    public void UpdateView_ShowsServiceName()
    {
        behavior.TestInvokeStart();

        Assert.AreEqual("TestUser", label.text);
    }

    [Test]
    public void UpdateView_UpdatesWhenNameChanges()
    {
        behavior.TestInvokeStart();

        fakeService.SetName("User 2");
        behavior.TestInvokeUpdateView();

        Assert.AreEqual("User 2", label.text);
    }
}