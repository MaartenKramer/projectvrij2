using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using static UnityEditor.FilePathAttribute;

[CustomPropertyDrawer(typeof(IFormBehaviour), true)]
public class IFormBehaviourDrawer : PropertyDrawer
{
    private static System.Type[] formTypes;
    private static string[] typeNames;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // get all types that implement IFormBehaviour interface
        if(formTypes == null)
        {
            formTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => typeof(IFormBehaviour).IsAssignableFrom(t) && !t.IsInterface).ToArray();
            typeNames = formTypes.Select(t => t.Name).ToArray();
        }

        if(property.managedReferenceValue == null)
        {
            int selected = EditorGUI.Popup(position, "Select a type", -1, typeNames);

            if(selected >= 0)
            {
                property.managedReferenceValue = Activator.CreateInstance(formTypes[selected]);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

        }
        else
        {
            string typeName = ObjectNames.NicifyVariableName(property.managedReferenceValue.GetType().Name);
            Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label.text, typeName);

            Rect buttonRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2f, position.width, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect, "Clear"))
            {
                Debug.Log("Clear pressed!");
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }

        }

            EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(property.managedReferenceValue != null)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 2f;
        }
        else
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
