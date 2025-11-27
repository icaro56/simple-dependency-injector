using System;

namespace SimpleDependencyInjector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OnValueChangedAttribute : Attribute
    {
        public string CallbackName { get; }

        public OnValueChangedAttribute(string callbackName)
        {
            CallbackName = callbackName;
        }
    }
}
