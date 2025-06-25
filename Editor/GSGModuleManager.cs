using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace GSGUnityUtilities.Editor
{
    /// <summary>
    /// GSG Unity Utilities 模組管理器
    /// 用於管理和配置各種功能模組的啟用狀態（僅使用 Define Symbols）
    /// </summary>
    public class GSGModuleManager : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false;
        private Dictionary<string, bool> packageCache = new Dictionary<string, bool>();
        private bool isCheckingPackages = false;
        
        // 模組定義
        [System.Serializable]
        public class ModuleInfo
        {
            public string name;
            public string description;
            public string defineSymbol;
            public bool isEnabled;
            public bool isCore;
            public string[] dependencies;
            public string packageDependency;
            public string minVersion;
            public bool isExternalPackage;
            public string downloadUrl;
            public string installInstructions;
        }
        
        private List<ModuleInfo> modules = new List<ModuleInfo>
        {
            new ModuleInfo
            {
                name = "Core Utilities",
                description = "核心實用工具：參考管理器、動畫系統、UI 工具等基本功能",
                defineSymbol = "GSG_CORE_ENABLED",
                isEnabled = true,
                isCore = true,
                dependencies = new string[0],
                isExternalPackage = false
            },
            new ModuleInfo
            {
                name = "UI Extensions",
                description = "進階 UI 組件：動畫控制、狀態處理、互動元件等",
                defineSymbol = "GSG_UI_EXTENSIONS_ENABLED",
                isEnabled = true,
                isCore = false,
                dependencies = new string[] { "GSG_CORE_ENABLED" },
                isExternalPackage = false
            },
            new ModuleInfo
            {
                name = "Steamworks Integration",
                description = "Steam 整合：Steam API 管理、成就系統、用戶介面控制",
                defineSymbol = "GSG_STEAMWORKS_ENABLED",
                isEnabled = false,
                isCore = false,
                dependencies = new string[] { "GSG_CORE_ENABLED" },
                packageDependency = "Steamworks.NET",
                minVersion = "12.0.0",
                isExternalPackage = true,
                downloadUrl = "https://github.com/rlabrecque/Steamworks.NET/releases",
                installInstructions = "1. 前往 GitHub 下載最新版本\n2. 將 UnityPackage 匯入到專案中\n3. 確保 steam_appid.txt 在專案根目錄"
            },
            new ModuleInfo
            {
                name = "Advanced File Browser",
                description = "進階檔案瀏覽器：跨平台檔案選擇和處理工具",
                defineSymbol = "GSG_FILEBROWSER_ENABLED",
                isEnabled = true,
                isCore = false,
                dependencies = new string[] { "GSG_CORE_ENABLED" },
                isExternalPackage = false
            }
        };
        
        [MenuItem("Tools/GSG Unity Utilities/Module Manager", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<GSGModuleManager>("GSG 模組管理器");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }
        
        private void OnEnable()
        {
            LoadCurrentModuleStates();
            RefreshPackageCache();
        }
        
        private void OnGUI()
        {
            DrawHeader();
            DrawModuleList();
            DrawButtons();
            DrawAdvancedSettings();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            // 標題
            var titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("🎮 GSG Unity Utilities", titleStyle);
            EditorGUILayout.LabelField("模組管理器 (Define Symbols Only)", EditorStyles.centeredGreyMiniLabel);
            
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("💡 注意：此版本僅使用 Define Symbols 控制模組，適用於 Package Manager 安裝的插件", MessageType.Info);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("選擇要啟用的功能模組：", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
        }
        
        private void DrawModuleList()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (var module in modules)
            {
                DrawModuleItem(module);
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawModuleItem(ModuleInfo module)
        {
            EditorGUILayout.BeginVertical("box");
            
            // 模組標題和啟用狀態
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = !module.isCore; // 核心模組不能被停用
            bool newEnabled = EditorGUILayout.Toggle(module.isEnabled, GUILayout.Width(20));
            if (newEnabled != module.isEnabled && !module.isCore)
            {
                module.isEnabled = newEnabled;
            }
            GUI.enabled = true;
            
            // 模組名稱
            var nameStyle = new GUIStyle(EditorStyles.boldLabel);
            if (module.isCore)
            {
                nameStyle.normal.textColor = new Color(0.3f, 0.7f, 1f); // 藍色表示核心模組
            }
            else if (module.isEnabled)
            {
                nameStyle.normal.textColor = new Color(0.3f, 0.8f, 0.3f); // 綠色表示啟用
            }
            else
            {
                nameStyle.normal.textColor = Color.gray; // 灰色表示停用
            }
            
            EditorGUILayout.LabelField(module.name, nameStyle);
            
            // 狀態指示
            string statusText = module.isCore ? "核心模組" : (module.isEnabled ? "✓ 啟用" : "✗ 停用");
            
            var statusStyle = new GUIStyle(EditorStyles.miniLabel);
            if (module.isCore)
            {
                statusStyle.normal.textColor = Color.blue;
            }
            else if (module.isEnabled)
            {
                statusStyle.normal.textColor = Color.green;
            }
            else
            {
                statusStyle.normal.textColor = Color.red;
            }
            
            EditorGUILayout.LabelField(statusText, statusStyle, GUILayout.Width(80));
            
            EditorGUILayout.EndHorizontal();
            
            // 模組描述
            EditorGUILayout.LabelField(module.description, EditorStyles.wordWrappedMiniLabel);
            
            // 相依性檢查
            if (module.dependencies.Length > 0 || !string.IsNullOrEmpty(module.packageDependency))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                
                // 模組相依性
                if (module.dependencies.Length > 0)
                {
                    string depText = "需要: " + string.Join(", ", module.dependencies.Select(d => GetModuleName(d)));
                    EditorGUILayout.LabelField(depText, EditorStyles.miniLabel);
                }
                
                // 套件相依性
                if (!string.IsNullOrEmpty(module.packageDependency))
                {
                    bool packageExists = GetCachedPackageStatus(module.packageDependency);
                    string packageType = module.isExternalPackage ? "外部套件" : "內部套件";
                    string packageText = $"{packageType}: {module.packageDependency}";
                    if (!string.IsNullOrEmpty(module.minVersion))
                    {
                        packageText += $" (>= {module.minVersion})";
                    }
                    
                    var packageStyle = new GUIStyle(EditorStyles.miniLabel);
                    if (isCheckingPackages && !module.isExternalPackage)
                    {
                        packageStyle.normal.textColor = Color.yellow;
                        packageText += " (檢查中...)";
                    }
                    else
                    {
                        packageStyle.normal.textColor = packageExists ? Color.green : Color.red;
                        if (module.isExternalPackage)
                        {
                            packageText += packageExists ? " ✓ 已安裝" : " ✗ 未安裝";
                        }
                    }
                    EditorGUILayout.LabelField(packageText, packageStyle);
                    
                    if (!packageExists && module.isEnabled && !isCheckingPackages)
                    {
                        string warningMsg = module.isExternalPackage ? 
                            $"警告：缺少外部套件 {module.packageDependency}，請手動安裝" :
                            $"警告：缺少必要套件 {module.packageDependency}";
                        EditorGUILayout.HelpBox(warningMsg, MessageType.Warning);
                    }
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawButtons()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            
            // 套用設定按鈕
            if (GUILayout.Button("🔧 套用設定", GUILayout.Height(30)))
            {
                ApplyModuleSettings();
            }
            
            // 重設按鈕
            if (GUILayout.Button("🔄 重設", GUILayout.Height(30), GUILayout.Width(80)))
            {
                ResetToDefaults();
            }
            
            // 匯出配置按鈕
            if (GUILayout.Button("📤 匯出配置", GUILayout.Height(30), GUILayout.Width(100)))
            {
                ExportConfiguration();
            }
            
            // 重新整理套件狀態按鈕
            GUI.enabled = !isCheckingPackages;
            if (GUILayout.Button(isCheckingPackages ? "🔄 檢查中..." : "🔄 重新整理", GUILayout.Height(30), GUILayout.Width(100)))
            {
                RefreshPackageCache();
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawAdvancedSettings()
        {
            EditorGUILayout.Space(10);
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "🔧 進階設定");
            
            if (showAdvancedSettings)
            {
                EditorGUILayout.BeginVertical("box");
                
                // Define Symbols 區域
                EditorGUILayout.LabelField("📝 編譯符號管理", EditorStyles.boldLabel);
                
                var currentSymbols = GetCurrentDefineSymbols();
                EditorGUILayout.LabelField("目前的編譯符號：", EditorStyles.miniLabel);
                EditorGUILayout.SelectableLabel(string.Join("; ", currentSymbols), EditorStyles.textArea, GUILayout.Height(60));
                
                EditorGUILayout.Space(5);
                
                if (GUILayout.Button("🧹 清理未使用的 GSG 編譯符號"))
                {
                    CleanupUnusedDefineSymbols();
                }
                
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("💡 提示：此版本僅使用 Define Symbols 控制模組啟用/停用。\n• 插件的 Assembly Define 檔案已預設配置為使用這些符號作為條件編譯約束\n• 這樣可以確保在 Package Manager 安裝的只讀插件中正常工作", MessageType.Info);
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void LoadCurrentModuleStates()
        {
            var symbols = GetCurrentDefineSymbols();
            foreach (var module in modules)
            {
                module.isEnabled = module.isCore || symbols.Contains(module.defineSymbol);
            }
        }
        
        private void ApplyModuleSettings()
        {
            var currentSymbols = GetCurrentDefineSymbols().ToList();
            bool hasChanges = false;
            
            foreach (var module in modules)
            {
                // 檢查相依性
                if (module.isEnabled && module.dependencies.Length > 0)
                {
                    foreach (var dep in module.dependencies)
                    {
                        if (!modules.Any(m => m.defineSymbol == dep && m.isEnabled))
                        {
                            EditorUtility.DisplayDialog("相依性錯誤", 
                                $"模組 '{module.name}' 需要 '{GetModuleName(dep)}' 模組啟用", "確定");
                            return;
                        }
                    }
                }
                
                // 檢查套件相依性
                if (module.isEnabled && !string.IsNullOrEmpty(module.packageDependency))
                {
                    bool packageExists = GetCachedPackageStatus(module.packageDependency);
                    if (!packageExists)
                    {
                        if (module.isExternalPackage)
                        {
                            // 外部套件：提供下載連結和安裝說明
                            string message = $"模組 '{module.name}' 需要外部套件 '{module.packageDependency}'。\n\n";
                            message += "安裝說明：\n" + module.installInstructions;
                            
                            bool openUrl = EditorUtility.DisplayDialog("需要外部套件", 
                                message, 
                                "打開下載頁面", "取消");
                            
                            if (openUrl && !string.IsNullOrEmpty(module.downloadUrl))
                            {
                                Application.OpenURL(module.downloadUrl);
                                EditorUtility.DisplayDialog("安裝提醒", 
                                    "請完成套件安裝後重新套用設定。\n\n" +
                                    "💡 提示：安裝完成後，可以點選「🔄 重新整理」按鈕更新套件狀態。", 
                                    "確定");
                            }
                            return;
                        }
                        else
                        {
                            // 內部套件：自動安裝
                            bool install = EditorUtility.DisplayDialog("缺少套件", 
                                $"模組 '{module.name}' 需要套件 '{module.packageDependency}'。\n是否要自動安裝此套件？", 
                                "安裝", "取消");
                            
                            if (install)
                            {
                                UnityEditor.PackageManager.Client.Add(module.packageDependency);
                                EditorUtility.DisplayDialog("套件安裝", "套件安裝已開始，請等待完成後重新套用設定。", "確定");
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                
                // 處理 Define Symbols
                bool shouldHaveSymbol = module.isEnabled;
                bool currentlyHasSymbol = currentSymbols.Contains(module.defineSymbol);
                
                if (shouldHaveSymbol && !currentlyHasSymbol)
                {
                    currentSymbols.Add(module.defineSymbol);
                    hasChanges = true;
                }
                else if (!shouldHaveSymbol && currentlyHasSymbol)
                {
                    currentSymbols.Remove(module.defineSymbol);
                    hasChanges = true;
                }
            }
            
            if (hasChanges)
            {
                SetDefineSymbols(currentSymbols.ToArray());
                
                string message = "模組設定已成功套用！";
                Debug.Log("[GSG Module Manager] " + message);
                EditorUtility.DisplayDialog("設定完成", message, "確定");
                
                // 強制重新編譯
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("[GSG Module Manager] 沒有需要變更的設定");
            }
        }
        
        private void ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog("重設確認", "確定要重設所有模組設定為預設值嗎？", "確定", "取消"))
            {
                // 重設為預設狀態
                modules[0].isEnabled = true;  // Core
                modules[1].isEnabled = true;  // UI Extensions  
                modules[2].isEnabled = false; // Steamworks
                modules[3].isEnabled = true;  // File Browser
                
                ApplyModuleSettings();
            }
        }
        
        private void ExportConfiguration()
        {
            var config = new System.Text.StringBuilder();
            config.AppendLine("# GSG Unity Utilities 模組配置");
            config.AppendLine($"# 產生時間: {System.DateTime.Now}");
            config.AppendLine();
            
            foreach (var module in modules)
            {
                config.AppendLine($"# {module.name}");
                config.AppendLine($"# {module.description}");
                config.AppendLine($"{module.defineSymbol}={module.isEnabled}");
                config.AppendLine();
            }
            
            string path = EditorUtility.SaveFilePanel("匯出模組配置", "", "GSG_ModuleConfig.txt", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                System.IO.File.WriteAllText(path, config.ToString());
                Debug.Log($"[GSG Module Manager] 配置已匯出到: {path}");
                EditorUtility.DisplayDialog("匯出完成", $"配置已匯出到:\n{path}", "確定");
            }
        }
        
        private string[] GetCurrentDefineSymbols()
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            return symbols.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }
        
        private void SetDefineSymbols(string[] symbols)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var symbolString = string.Join(";", symbols.Where(s => !string.IsNullOrWhiteSpace(s)));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, symbolString);
        }
        
        private string GetModuleName(string defineSymbol)
        {
            var module = modules.FirstOrDefault(m => m.defineSymbol == defineSymbol);
            return module?.name ?? defineSymbol;
        }
        
        private void RefreshPackageCache()
        {
            isCheckingPackages = true;
            packageCache.Clear();
            
            // 異步檢查套件狀態
            var request = UnityEditor.PackageManager.Client.List();
            EditorApplication.update += () => CheckPackageRequestComplete(request);
        }
        
        private void CheckPackageRequestComplete(UnityEditor.PackageManager.Requests.ListRequest request)
        {
            if (!request.IsCompleted) return;
            
            EditorApplication.update -= () => CheckPackageRequestComplete(request);
            
            if (request.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                // 更新緩存
                foreach (var module in modules)
                {
                    if (!string.IsNullOrEmpty(module.packageDependency))
                    {
                        if (module.isExternalPackage && module.packageDependency == "Steamworks.NET")
                        {
                            // Steamworks.NET 對應的實際套件名稱
                            bool exists = request.Result.Any(package => package.name == "com.rlabrecque.steamworks.net");
                            packageCache["com.rlabrecque.steamworks.net"] = exists;
                        }
                        else
                        {
                            // 一般套件直接檢查
                            bool exists = request.Result.Any(package => package.name == module.packageDependency);
                            packageCache[module.packageDependency] = exists;
                        }
                    }
                }
            }
            
            isCheckingPackages = false;
            Repaint(); // 重新繪製視窗
        }
        
        private bool GetCachedPackageStatus(string packageName)
        {
            var module = modules.FirstOrDefault(m => m.packageDependency == packageName);
            if (module != null && module.isExternalPackage)
            {
                // 對於外部套件，檢查Package Manager中是否有對應套件
                if (packageName == "Steamworks.NET")
                {
                    // 檢查是否有安裝 com.rlabrecque.steamworks.net
                    return packageCache.ContainsKey("com.rlabrecque.steamworks.net") ? 
                           packageCache["com.rlabrecque.steamworks.net"] : 
                           CheckSteamworksInManifest();
                }
                // 可以在此添加其他外部套件的檢查邏輯
            }
            
            // 內部套件使用快取檢查
            return packageCache.ContainsKey(packageName) ? packageCache[packageName] : false;
        }
        
        /// <summary>
        /// 檢查 manifest.json 中是否包含 Steamworks.NET
        /// </summary>
        private bool CheckSteamworksInManifest()
        {
            try
            {
                string manifestPath = "Packages/manifest.json";
                if (File.Exists(manifestPath))
                {
                    string content = File.ReadAllText(manifestPath);
                    return content.Contains("com.rlabrecque.steamworks.net");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[GSG Module Manager] 無法讀取 manifest.json: {e.Message}");
            }
            return false;
        }
        
        private void CleanupUnusedDefineSymbols()
        {
            var currentSymbols = GetCurrentDefineSymbols().ToList();
            var gsgSymbols = currentSymbols.Where(s => s.StartsWith("GSG_")).ToList();
            var usedSymbols = modules.Where(m => m.isEnabled).Select(m => m.defineSymbol).ToList();
            
            var unusedSymbols = gsgSymbols.Except(usedSymbols).ToList();
            
            if (unusedSymbols.Any())
            {
                foreach (var unused in unusedSymbols)
                {
                    currentSymbols.Remove(unused);
                }
                
                SetDefineSymbols(currentSymbols.ToArray());
                Debug.Log($"[GSG Module Manager] 已清理未使用的編譯符號: {string.Join(", ", unusedSymbols)}");
                EditorUtility.DisplayDialog("清理完成", $"已清理 {unusedSymbols.Count} 個未使用的編譯符號", "確定");
            }
            else
            {
                EditorUtility.DisplayDialog("清理完成", "沒有發現未使用的 GSG 編譯符號", "確定");
            }
        }
    }
} 