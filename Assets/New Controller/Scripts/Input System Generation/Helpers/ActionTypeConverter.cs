using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Bdiebeak.InputSystemGeneration
{
    public static class ActionTypeConverter
    {
        // In Input system package we have inconvenient type determination code.
        // It's even uncomfortable for C# Reflection. So this dictionary is the most convenient solution in my opinion.
        // ToDo: extract this dictionary into some Scriptable object settings file.
        private static Dictionary<string, string> _typeToSuitable = new Dictionary<string, string>()
        {
            {"analog", "float"},
            {"axis", "float"},
            {"button", "bool"},
            {"digital", "int"},
            {"dpad", "Vector2"},
            {"integer", "int"},
            {"quaternion", "Quaternion"},
            {"stick", "Vector2"},
            {"vector2", "Vector2"},
            {"vector3", "Vector3"},
        };

        public static string ConvertToSuitableType(this InputAction inputAction)
        {
            var controlType = inputAction.expectedControlType;
            var loweredControlType = controlType.ToLower();

            return _typeToSuitable.ContainsKey(loweredControlType) ? _typeToSuitable[loweredControlType] 
                                                                   : string.Empty;
        }
    }
}