#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace SimpleDependencyInjector
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class OnValueChangedEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var targetObj = target;
            var objType = targetObj.GetType();
            var fields = objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var prop = serializedObject.FindProperty(field.Name);
                if (prop == null) continue;

                // Captura estado antes da mudança
                string before = GetPropertySnapshot(prop);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(prop, true);
                bool changed = EditorGUI.EndChangeCheck();

                if (changed)
                {
                    serializedObject.ApplyModifiedProperties();

                    // Captura estado após
                    string after = GetPropertySnapshot(prop);

                    // se algo realmente mudou no valor do array/list/field
                    if (before != after)
                        InvokeOnValueChanged(field, objType, targetObj);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InvokeOnValueChanged(FieldInfo field, System.Type objType, object targetObj)
        {
            var attr = field.GetCustomAttribute<OnValueChangedAttribute>();
            if (attr == null) return;

            var method = objType.GetMethod(
                attr.CallbackName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (method == null)
            {
                Debug.LogError($"Método '{attr.CallbackName}' não encontrado em {objType.Name}");
                return;
            }

            method.Invoke(targetObj, null);
        }

        /// <summary>
        /// Gera um snapshot "serializável" da propriedade inteira
        /// Isso permite detectar mudanças em arrays, listas e objetos complexos.
        /// </summary>
        private string GetPropertySnapshot(SerializedProperty prop)
        {
            SerializedProperty copy = prop.Copy();
            string result = "";

            int depth = copy.depth;
            bool enterChildren = true;

            while (copy.NextVisible(enterChildren) && copy.depth > depth)
            {
                result += copy.propertyPath + "|" + GetValue(copy) + "\n";
                enterChildren = true;
            }

            return result;
        }

        private string GetValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer: return prop.intValue.ToString();
                case SerializedPropertyType.Float: return prop.floatValue.ToString();
                case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
                case SerializedPropertyType.String: return prop.stringValue;
                case SerializedPropertyType.Enum: return prop.enumValueIndex.ToString();
                case SerializedPropertyType.ObjectReference: return prop.objectReferenceInstanceIDValue.ToString();
                default: return prop.ToString();
            }
        }
    }
}
#endif