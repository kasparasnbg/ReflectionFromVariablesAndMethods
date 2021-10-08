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


        var propertiesMenu = new GenericMenu();
        var methodsMenu = new GenericMenu();

        var components = testInspector.obj.GetComponents<Component>();

        Dictionary<KeyValuePair<string, Component>, List<PropertyInfo>> propertiesDropDownMenu = new Dictionary<KeyValuePair<string, Component>, List<PropertyInfo>>();
        Dictionary<KeyValuePair<string, Component>, List<MethodInfo>> methodsDropDownMenu = new Dictionary<KeyValuePair<string, Component>, List<MethodInfo>>();

        foreach (var component in components)
        {
            propertiesDropDownMenu.Add(new KeyValuePair<string, Component>(component.GetType().Name, component), new List<PropertyInfo>());
            methodsDropDownMenu.Add(new KeyValuePair<string, Component>(component.GetType().Name, component), new List<MethodInfo>());
        }

        foreach (var component in components)
        {
            PropertyInfo[] properties = component.GetType().GetProperties();
            var key = new KeyValuePair<string, Component>(component.GetType().Name, component);
            foreach (PropertyInfo property in properties)
            {
                propertiesDropDownMenu[key].Add(property);
            }

            MethodInfo[] methods = component.GetType().GetMethods();
            foreach (MethodInfo methdod in methods)
            {
                methodsDropDownMenu[key].Add(methdod);
            }
        }

        foreach (var item in propertiesDropDownMenu)
        {
            foreach (var child in item.Value)
            {
                propertiesMenu.AddItem(new GUIContent($"{item.Key.Key}/{child.Name}"), false, () =>
                {

                    selectedProperty = child;
                    selectedComponent = item.Key.Value;
                });

            }
        }

        foreach (var item in methodsDropDownMenu)
        {
            foreach (var child in item.Value)
            {

                ParameterInfo[] pars = child.GetParameters();

                //TEST to check for methods with 0 parameters
                if (pars.Length > 0)
                    continue;

                string paramsList = "";
                foreach (ParameterInfo p in pars)
                {
                    paramsList += p.ParameterType + " ";

                }

                methodsMenu.AddItem(new GUIContent($"{item.Key.Key}/{child.Name} | {paramsList}"), false, () =>
                {
                    var type = child.ReturnType;

                    var result = child.Invoke(item.Key.Value, new object[] { });
                    Debug.Log(result);//no clue how to unbox
                });

            }
        }

        if (GUILayout.Button("Properties"))
            propertiesMenu.ShowAsContext();

        if (GUILayout.Button("Methods"))
            methodsMenu.ShowAsContext();

        if (selectedProperty != null && selectedComponent != null)
        {
            if (selectedProperty.PropertyType == typeof(UnityEngine.Vector3)) // is it possible to convert it to property field? somehow i doubt it
            {
                var value = EditorGUILayout.Vector3Field(selectedProperty.Name, (Vector3)selectedProperty.GetValue(selectedComponent));
                selectedProperty.SetValue(selectedComponent, value);
            }

        }
    }

}
