using ServiceInterfaces;
using SimpleDependencyInjector;
using TMPro;
using UnityEngine;

public class UserLabelBehavior : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _userLabel;

    private int count = 1;

    private Injected<IUserService> _userService;

#if UNITY_INCLUDE_TESTS
    public void SetUserServiceForTesting(Injected<IUserService> injected)
    {
        _userService = injected;
    }

    public void SetLabelForTesting(TextMeshProUGUI label)
    {
        _userLabel = label;
    }

    public void TestInvokeStart() => Start();
    public void TestInvokeUpdateView() => UpdateView();
#endif


    private void Start()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        _userLabel.text = _userService.Value.GetName();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            count++;
            _userService.Value.SetName($"User {count}");
            UpdateView();
        }
    }
}
