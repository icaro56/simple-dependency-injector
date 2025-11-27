using ServiceInterfaces;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UserService", menuName = "Services/UserService")]
public class UserService : ScriptableObject, IUserService
{
    [NonSerialized]
    private string _userName;

    public string GetName()
    {
        return _userName;
    }

    public void SetName(string name)
    {
        _userName = name;
    }
}
