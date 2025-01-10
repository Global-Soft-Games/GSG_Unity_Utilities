using UnityEditor;
using UnityEngine;
using GSGUnityUtilities.Runtime;
namespace GSGUnityUtilities.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property drawing scope
            EditorGUI.BeginProperty(position, label, property);

            // Get scene asset and path properties
            var sceneAssetProp = property.FindPropertyRelative("sceneAsset");
            var scenePathProp = property.FindPropertyRelative("scenePath");

            // Draw the scene asset field
            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUI.ObjectField(
                position, 
                label, 
                sceneAssetProp.objectReferenceValue, 
                typeof(SceneAsset), 
                false);

            // Update scene path when scene asset changes
            if (EditorGUI.EndChangeCheck())
            {
                sceneAssetProp.objectReferenceValue = newScene;
                scenePathProp.stringValue = (newScene == null) 
                    ? string.Empty 
                    : AssetDatabase.GetAssetPath(newScene);
                
                // Mark property as dirty
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
} 