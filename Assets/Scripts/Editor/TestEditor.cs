using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestInspector))]



public class TestEditor : Editor
{
    TestInspector testInspector;
    private void OnEnable()
    {
        testInspector = (TestInspector)target;

    }

    Component selectedComponent;
    PropertyInfo selectedProperty;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        var menu = new GenericMenu();

        var components = testInspector.obj.GetComponents<Component>();

        Dictionary<KeyValuePair<string, Component>, List<PropertyInfo>> dropDownMenu = new Dictionary<KeyValuePair<string, Component>, List<PropertyInfo>>();


        foreach (var component in components)
        {
            dropDownMenu.Add(new KeyValuePair<string, Component>(component.GetType().Name, component), new List<PropertyInfo>());
            //    menu.AddItem(new GUIContent(component.GetType().Name), false, () => { });
        }

        foreach (var component in components)
        {
            PropertyInfo[] properties = component.GetType().GetProperties();
            var key = new KeyValuePair<string, Component>(component.GetType().Name, component);
            foreach (PropertyInfo property in properties)
            {
                dropDownMenu[key].Add(property);
            }
        }

        foreach (var item in dropDownMenu)
        {
            foreach (var child in item.Value)
            {
                menu.AddItem(new GUIContent($"{item.Key.Key}/{child.Name}"), false, () =>
                {

                    selectedProperty = child;
                    selectedComponent = item.Key.Value;
                });

            }
        }

        if (GUILayout.Button("show menu"))
            menu.ShowAsContext();


        if (selectedProperty != null && selectedComponent != null)
        {
            Debug.Log($"type {selectedProperty.PropertyType}");

            if (selectedProperty.PropertyType == typeof(UnityEngine.Vector3))
            {
                var value = EditorGUILayout.Vector3Field(selectedProperty.Name, (Vector3)selectedProperty.GetValue(selectedComponent));
                selectedProperty.SetValue(selectedComponent, value);

                //EditorGUILayout.PropertyField(serializedObject.FindProperty(selectedProperty.Name));
            }


            SerializedObject serializedObject = new SerializedObject(selectedComponent);

            var sp = serializedObject.GetIterator();

            while (sp.Next(true))
            {
                Debug.Log(sp.name);
            }
            //EditorGUILayout.PropertyField(selectedProperty.PropertyType);
        }
    }

}
