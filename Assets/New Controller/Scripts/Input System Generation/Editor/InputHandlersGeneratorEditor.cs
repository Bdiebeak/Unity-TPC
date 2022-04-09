using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bdiebeak.InputSystemGeneration
{
    public class InputHandlersGeneratorEditor
    {
        private const string MenuItemPath = "Assets/Bdiebeak/Input System/Generate Input Handlers";

        [MenuItem(MenuItemPath, true)]
        private static bool GenerateMenuValidation() => Selection.activeObject.GetType() == typeof(InputActionAsset);
        
        [MenuItem(MenuItemPath)]
        private static void Generate()
        {
            var inputActionAsset = Selection.activeObject as InputActionAsset;
            if (inputActionAsset == null)
            {
                Debug.LogError("Can't find InputActionAsset.");
                return;
            }

            var selectedFolderPath = EditorUtility.OpenFolderPanel("Save location", "InputSystemInterface", "");
            foreach (var map in inputActionAsset.actionMaps)
            {
                var generator = new InputHandlersGenerator(map);
                var interfaceCode = generator.GenerateInterfaceCode();
                var classCode = generator.GenerateHandlerClassCode();
                
                FileWriter.CreateAndWrite($"{selectedFolderPath}/{map.name}", $"{generator.InterfaceName}.cs", interfaceCode);       
                FileWriter.CreateAndWrite($"{selectedFolderPath}/{map.name}", $"{generator.ClassName}.cs", classCode);       
            }

            AssetDatabase.Refresh();
        }
    }
}