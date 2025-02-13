using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using GSGUnityUtilities.Runtime;

[InitializeOnLoad]
public static class CreateAssetMenuExProcessor
{
    static CreateAssetMenuExProcessor()
    {
        // 取得所有有 `CreateAssetMenuEx` 屬性的 ScriptableObject
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(ScriptableObject)) &&
                           type.GetCustomAttribute<CreateAssetMenuExAttribute>() != null)
            .ToList();

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<CreateAssetMenuExAttribute>();
            RegisterMenuItem(type, attr.MenuName);
        }
    }

    /// <summary>
    /// 註冊 `MenuItem` 來自動生成 ScriptableObject
    /// </summary>
    private static void RegisterMenuItem(Type type, string menuName)
    {
        string path = $"Assets/{menuName}";
        string methodName = $"Create_{type.Name}";

        // 使用反射動態建立 `MenuItem`
        var method = new Action(() => CreateAsset(type));
        var menuItemType = typeof(MenuItem);
        var menuItemConstructor = menuItemType.GetConstructor(new[] { typeof(string), typeof(bool), typeof(int) });

        if (menuItemConstructor != null)
        {
            var menuItemAttr = menuItemConstructor.Invoke(new object[] { path, false, 1 });
            MethodInfo addMethod = typeof(EditorApplication).GetMethod("add_delayCall");
            addMethod?.Invoke(null, new object[] { method });
        }
    }

    /// <summary>
    /// 創建 ScriptableObject 並存入當前選中的資料夾
    /// </summary>
    private static void CreateAsset(Type type)
    {
        var asset = ScriptableObject.CreateInstance(type);
        string path = GetSelectedPathOrFallback();
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{type.Name}.asset");

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets/";

        if (Selection.activeObject != null)
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (System.IO.Directory.Exists(selectedPath)) return selectedPath + "/";
                else return System.IO.Path.GetDirectoryName(selectedPath) + "/";
            }
        }

        return path;
    }
}
