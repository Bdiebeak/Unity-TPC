using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionGenerator
{
    private const string MenuItemPath = "Assets/Bdiebeak/Input System/Generate Interfaces"; 
    
    [MenuItem(MenuItemPath)]
    private static void DoSomething() 
    {
        var inputActionAsset = Selection.activeObject as InputActionAsset;
        if (inputActionAsset == null)
        {
            Debug.LogError("Can't find InputActionAsset.");
            return;
        }
        
        
        // var path = EditorUtility.SaveFilePanel("Save InputActionAsset's Json.", "", inputActionAsset.name + ".json", "json");
        // if (path.Length != 0)
        // {
        //     var jsonData = inputActionAsset.ToJson();
        //     if (jsonData != null) File.WriteAllText(path, jsonData);
        // }
    }
    
    [MenuItem(MenuItemPath, true)]
    private static bool DoSomethingValidation() => Selection.activeObject.GetType() == typeof(InputActionAsset);
}
