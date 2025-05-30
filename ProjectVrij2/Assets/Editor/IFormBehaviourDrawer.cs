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

            // draw property fields
            // Foldout for nicer inspector
            Rect foldoutRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2 + 2f, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();
                bool enterChildren = true;

                float y = foldoutRect.y + EditorGUIUtility.singleLineHeight;

                while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    enterChildren = false;
                    Rect fieldRect = new Rect(foldoutRect.x, y, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, iterator, true);
                    y += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }

        }

            EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(property.managedReferenceValue != null)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight * 3 + 2f;

            float height = EditorGUIUtility.singleLineHeight;
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                enterChildren = false;
                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return EditorGUIUtility.singleLineHeight * 2 + 2f + height;
        }
        else
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
