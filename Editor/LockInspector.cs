using UnityEditor;
using UnityEngine;

public class LockInspector : EditorWindow
{
    [MenuItem("Tools/Lock Inspector %l")]
    public static void ToggleLock()
    {
        // Accessing the active inspector window
        EditorWindow inspectorWindow = EditorWindow.focusedWindow;

        // Checking if the focused window is an inspector window
        if (inspectorWindow != null && inspectorWindow.GetType().Name == "InspectorWindow")
        {
            // Using reflection to change the isLocked property of the inspector
            System.Type type = inspectorWindow.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty("isLocked", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool value = (bool)propertyInfo.GetValue(inspectorWindow, null);
            propertyInfo.SetValue(inspectorWindow, !value, null);
            inspectorWindow.Repaint();
        }
    }
}
