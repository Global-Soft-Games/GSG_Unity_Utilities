using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace GSGUnityUtilities.Editor
{
    /// <summary>
    /// GSG Unity Utilities 快速設定工具
    /// 提供一鍵設定常用模組組合的功能
    /// </summary>
    public class GSGQuickSetup : EditorWindow
    {
        [MenuItem("Tools/GSG Unity Utilities/Quick Setup", priority = 0)]
        public static void ShowQuickSetup()
        {
            var window = GetWindow<GSGQuickSetup>("GSG 快速設定");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // 標題
            var titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("🚀 GSG Unity Utilities 快速設定", titleStyle);
            
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("選擇適合您專案的預設配置：", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            // 基礎專案設定
            DrawSetupButton(
                "📱 基礎專案", 
                "核心功能 + UI 工具\n適合簡單的應用程式或原型開發",
                () => SetupBasicProject()
            );
            
            EditorGUILayout.Space(5);
            
            // 完整遊戲專案設定
            DrawSetupButton(
                "🎮 完整遊戲專案", 
                "所有功能 (除了 Steam)\n適合一般的遊戲開發",
                () => SetupFullGameProject()
            );
            
            EditorGUILayout.Space(5);
            
            // Steam 遊戲專案設定
            DrawSetupButton(
                "🚀 Steam 遊戲專案", 
                "包含所有功能\n適合要發布到 Steam 的遊戲",
                () => SetupSteamGameProject()
            );
            
            EditorGUILayout.Space(5);
            
            // 僅工具專案設定
            DrawSetupButton(
                "🔧 開發工具專案", 
                "核心功能 + 編輯器工具\n適合內部工具或編輯器擴展",
                () => SetupToolsProject()
            );
            
            EditorGUILayout.Space(20);
            
            // 進階設定按鈕
            if (GUILayout.Button("⚙️ 開啟進階模組管理器", GUILayout.Height(25)))
            {
                GSGModuleManager.ShowWindow();
                Close();
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("💡 提示：您隨時可以透過模組管理器調整設定", EditorStyles.centeredGreyMiniLabel);
        }
        
        private void DrawSetupButton(string title, string description, System.Action onSetup)
        {
            EditorGUILayout.BeginVertical("box");
            
            // 標題和按鈕
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
            
            if (GUILayout.Button("設定", GUILayout.Width(60), GUILayout.Height(35)))
            {
                onSetup?.Invoke();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        private void SetupBasicProject()
        {
            if (EditorUtility.DisplayDialog("確認設定", 
                "將設定為基礎專案配置：\n\n✅ Core Utilities\n✅ UI Extensions\n❌ File Browser\n❌ Editor Tools\n❌ Steamworks Integration", 
                "確定", "取消"))
            {
                SetDefineSymbols(new[] { "GSG_CORE_ENABLED" });
                ShowSetupComplete("基礎專案");
            }
        }
        
        private void SetupFullGameProject()
        {
            if (EditorUtility.DisplayDialog("確認設定", 
                "將設定為完整遊戲專案配置：\n\n✅ Core Utilities\n✅ UI Extensions\n✅ File Browser\n✅ Editor Tools\n❌ Steamworks Integration", 
                "確定", "取消"))
            {
                SetDefineSymbols(new[] { 
                    "GSG_CORE_ENABLED", 
                    "GSG_FILEBROWSER_ENABLED", 
                    "GSG_EDITOR_TOOLS_ENABLED" 
                });
                ShowSetupComplete("完整遊戲專案");
            }
        }
        
        private void SetupSteamGameProject()
        {
            if (EditorUtility.DisplayDialog("確認設定", 
                "將設定為 Steam 遊戲專案配置：\n\n✅ Core Utilities\n✅ UI Extensions\n✅ File Browser\n✅ Editor Tools\n✅ Steamworks Integration\n\n注意：需要安裝 Steamworks.NET 套件", 
                "確定", "取消"))
            {
                SetDefineSymbols(new[] { 
                    "GSG_CORE_ENABLED", 
                    "GSG_FILEBROWSER_ENABLED", 
                    "GSG_EDITOR_TOOLS_ENABLED",
                    "GSG_STEAMWORKS_ENABLED"
                });
                ShowSetupComplete("Steam 遊戲專案");
                
                // 檢查是否有 Steamworks.NET
                if (!CheckSteamworksInstalled())
                {
                    EditorUtility.DisplayDialog("注意", 
                        "未檢測到 Steamworks.NET 套件。\n\n請透過 Package Manager 安裝：\ncom.rlabrecque.steamworks.net", 
                        "知道了");
                }
            }
        }
        
        private void SetupToolsProject()
        {
            if (EditorUtility.DisplayDialog("確認設定", 
                "將設定為開發工具專案配置：\n\n✅ Core Utilities\n❌ UI Extensions\n❌ File Browser\n✅ Editor Tools\n❌ Steamworks Integration", 
                "確定", "取消"))
            {
                SetDefineSymbols(new[] { "GSG_CORE_ENABLED", "GSG_EDITOR_TOOLS_ENABLED" });
                ShowSetupComplete("開發工具專案");
            }
        }
        
        private void SetDefineSymbols(string[] newSymbols)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            var symbols = currentSymbols.Split(';').ToList();
            
            // 移除所有 GSG 相關符號
            symbols.RemoveAll(s => s.StartsWith("GSG_"));
            
            // 添加新符號
            symbols.AddRange(newSymbols);
            
            var symbolString = string.Join(";", symbols.Where(s => !string.IsNullOrWhiteSpace(s)));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, symbolString);
        }
        
        private void ShowSetupComplete(string projectType)
        {
            EditorUtility.DisplayDialog("設定完成", 
                $"{projectType} 配置已套用！\n\nUnity 將重新編譯腳本以套用變更。", 
                "確定");
            AssetDatabase.Refresh();
            Close();
        }
        
        private bool CheckSteamworksInstalled()
        {
            var request = UnityEditor.PackageManager.Client.List();
            while (!request.IsCompleted)
            {
                System.Threading.Thread.Sleep(10);
            }
            
            if (request.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                return request.Result.Any(package => package.name == "com.rlabrecque.steamworks.net");
            }
            
            return false;
        }
    }
} 