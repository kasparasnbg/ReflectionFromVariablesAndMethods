using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestInspector))]
public class TestEditor : Editor
{
    TestInspector testInspector;
    Component selectedComponent;
    string selectedPropertyName;

    private void OnEnable()
    {
        testInspector = (TestInspector)target;

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        var menu = new GenericMenu();

        var components = testInspector.obj.GetComponents<Component>();

        Dictionary<KeyValuePair<string, Component>, List<string>> dropDownMenu = new Dictionary<KeyValuePair<string, Component>, List<string>>();


        foreach (var component in components)
        {
            dropDownMenu.Add(new KeyValuePair<string, Component>(component.GetType().Name, component), new List<string>());

            var key = new KeyValuePair<string, Component>(component.GetType().Name, component);

            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty sp = serializedObject.GetIterator();
            var exists = sp.NextVisible(true);

            while (exists)
            {
                dropDownMenu[key].Add(sp.name);
                exists = sp.NextVisible(false);
            }
            


        }
  

        foreach (var item in dropDownMenu)
        {
            foreach (var child in item.Value)
            {
                menu.AddItem(new GUIContent($"{item.Key.Key}/{child}"), false, () =>
                {
                    selectedPropertyName = child;
                    selectedComponent = item.Key.Value;
                });

            }
        }

        if (GUILayout.Button("show menu"))
            menu.ShowAsContext();


        if (selectedComponent != null && !string.IsNullOrWhiteSpace(selectedPropertyName))
        {
            SerializedObject serializedObject = new SerializedObject(selectedComponent);

            EditorGUILayout.PropertyField(serializedObject.FindProperty(selectedPropertyName),true);
            serializedObject.ApplyModifiedProperties();
        }


    }
}
