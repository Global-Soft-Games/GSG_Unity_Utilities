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
    /// 用於管理和配置各種功能模組的啟用狀態
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
            public string assemblyDefPath;  // 新增：Assembly Define 檔案路徑
            public string assemblyName;     // 新增：Assembly 名稱
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
                assemblyDefPath = "Assets/GSG_Unity_Utilities/Runtime/Core/GSGUnityUtilities.Runtime.Core.asmdef",
                assemblyName = "GSGUnityUtilities.Runtime.Core"
            },
            new ModuleInfo
            {
                name = "UI Extensions",
                description = "進階 UI 組件：動畫控制、狀態處理、互動元件等",
                defineSymbol = "GSG_UI_EXTENSIONS_ENABLED",
                isEnabled = true,
                isCore = false,
                dependencies = new string[] { "GSG_CORE_ENABLED" },
                assemblyDefPath = "Assets/GSG_Unity_Utilities/Runtime/UIExtensions/GSGUnityUtilities.Runtime.UIExtensions.asmdef",
                assemblyName = "GSGUnityUtilities.Runtime.UIExtensions"
            },
            new ModuleInfo
            {
                name = "Steamworks Integration",
                description = "Steam 整合：Steam API 管理、成就系統、用戶介面控制",
                defineSymbol = "GSG_STEAMWORKS_ENABLED",
                isEnabled = false,
                isCore = false,
                dependencies = new string[] { "GSG_CORE_ENABLED" },
                packageDependency = "com.rlabrecque.steamworks.net",
                minVersion = "12.0.0",
                assemblyDefPath = "Assets/GSG_Unity_Utilities/Runtime/Steamworks/GSGUnityUtilities.Runtime.Steamworks.asmdef",
                assemblyName = "GSGUnityUtilities.Runtime.Steamworks"
            },
            new ModuleInfo
            {
                name = "Advanced File Browser",
                description = "進階檔案瀏覽器：跨平台檔案選擇和處理工具",
                defineSymbol = "GSG_FILEBROWSER_ENABLED",
                isEnabled = true,
                isCore = false,
                dependencies = new string[] { "GSG_CORE_ENABLED" },
                assemblyDefPath = "Assets/GSG_Unity_Utilities/Runtime/FileBrowser/GSGUnityUtilities.Runtime.FireBrowser.asmdef",
                assemblyName = "GSGUnityUtilities.Runtime.FireBrowser"
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
            EditorGUILayout.LabelField("模組管理器", EditorStyles.centeredGreyMiniLabel);
            
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
            
            // 檢查 Assembly Define 狀態
            bool asmdefEnabled = string.IsNullOrEmpty(module.assemblyDefPath) || 
                               IsAssemblyDefEnabled(module.assemblyDefPath);
            
            if (!module.isCore && !string.IsNullOrEmpty(module.assemblyDefPath))
            {
                if (module.isEnabled && asmdefEnabled)
                {
                    statusText = "✓ 完全啟用";
                }
                else if (module.isEnabled && !asmdefEnabled)
                {
                    statusText = "⚠ 部分啟用";
                }
                else if (!module.isEnabled && !asmdefEnabled)
                {
                    statusText = "✗ 完全停用";
                }
                else
                {
                    statusText = "⚠ 不一致";
                }
            }
            
            var statusStyle = new GUIStyle(EditorStyles.miniLabel);
            if (module.isCore)
            {
                statusStyle.normal.textColor = Color.blue;
            }
            else if (module.isEnabled && asmdefEnabled)
            {
                statusStyle.normal.textColor = Color.green;
            }
            else if (!module.isEnabled && !asmdefEnabled)
            {
                statusStyle.normal.textColor = Color.red;
            }
            else
            {
                statusStyle.normal.textColor = Color.yellow; // 不一致狀態
            }
            
            EditorGUILayout.LabelField(statusText, statusStyle, GUILayout.Width(80));
            
            EditorGUILayout.EndHorizontal();
            
            // 模組描述
            EditorGUILayout.LabelField(module.description, EditorStyles.wordWrappedMiniLabel);
            
            // Assembly Define 信息
            if (!string.IsNullOrEmpty(module.assemblyDefPath))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(20);
                
                bool asmdefFileExists = File.Exists(module.assemblyDefPath);
                bool asmdefIsEnabled = IsAssemblyDefEnabled(module.assemblyDefPath);
                
                string asmdefStatus = asmdefFileExists ? 
                    (asmdefIsEnabled ? "Assembly: ✓ 啟用" : "Assembly: ✗ 受限") : 
                    "Assembly: ⚠ 檔案不存在";
                    
                var asmdefStyle = new GUIStyle(EditorStyles.miniLabel);
                asmdefStyle.normal.textColor = asmdefFileExists ? 
                    (asmdefIsEnabled ? new Color(0.3f, 0.8f, 0.3f) : Color.red) : 
                    Color.yellow;
                    
                EditorGUILayout.LabelField(asmdefStatus, asmdefStyle);
                EditorGUILayout.EndHorizontal();
            }
            
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
                    string packageText = $"套件相依性: {module.packageDependency}";
                    if (!string.IsNullOrEmpty(module.minVersion))
                    {
                        packageText += $" (>= {module.minVersion})";
                    }
                    
                    var packageStyle = new GUIStyle(EditorStyles.miniLabel);
                    if (isCheckingPackages)
                    {
                        packageStyle.normal.textColor = Color.yellow;
                        packageText += " (檢查中...)";
                    }
                    else
                    {
                        packageStyle.normal.textColor = packageExists ? Color.green : Color.red;
                    }
                    EditorGUILayout.LabelField(packageText, packageStyle);
                    
                    if (!packageExists && module.isEnabled && !isCheckingPackages)
                    {
                        EditorGUILayout.HelpBox($"警告：缺少必要套件 {module.packageDependency}", MessageType.Warning);
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
                
                EditorGUILayout.Space(10);
                
                // Assembly Define 區域
                EditorGUILayout.LabelField("⚙️ Assembly Define 狀態", EditorStyles.boldLabel);
                
                foreach (var module in modules)
                {
                    if (!string.IsNullOrEmpty(module.assemblyDefPath))
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        bool exists = File.Exists(module.assemblyDefPath);
                        bool enabled = IsAssemblyDefEnabled(module.assemblyDefPath);
                        
                        string statusIcon = exists ? (enabled ? "✓" : "✗") : "⚠";
                        Color statusColor = exists ? (enabled ? Color.green : Color.red) : Color.yellow;
                        
                        var statusStyle = new GUIStyle(EditorStyles.miniLabel);
                        statusStyle.normal.textColor = statusColor;
                        
                        EditorGUILayout.LabelField(statusIcon, statusStyle, GUILayout.Width(15));
                        EditorGUILayout.LabelField(module.assemblyName, EditorStyles.miniLabel);
                        
                        string statusText = exists ? (enabled ? "啟用" : "受限") : "檔案不存在";
                        EditorGUILayout.LabelField(statusText, statusStyle);
                        
                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("💡 提示：此模組管理器現在同時控制 Define Symbols 和 Assembly Define 檔案。\n• Define Symbols 控制條件編譯 (#if)\n• Assembly Define 控制 Assembly 的載入和編譯", MessageType.Info);
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void LoadCurrentModuleStates()
        {
            var symbols = GetCurrentDefineSymbols();
            foreach (var module in modules)
            {
                module.isEnabled = module.isCore || symbols.Contains(module.defineSymbol);
                
                // 同時檢查 Assembly Define 檔案是否存在且啟用
                if (!string.IsNullOrEmpty(module.assemblyDefPath))
                {
                    bool asmdefExists = File.Exists(module.assemblyDefPath);
                    bool asmdefEnabled = IsAssemblyDefEnabled(module.assemblyDefPath);
                    
                    // 如果 Assembly Define 被停用，模組也應該被視為停用
                    if (!asmdefEnabled && !module.isCore)
                    {
                        module.isEnabled = false;
                    }
                }
            }
        }
        
        /// <summary>
        /// 檢查 Assembly Define 檔案是否啟用
        /// </summary>
        private bool IsAssemblyDefEnabled(string asmdefPath)
        {
            if (!File.Exists(asmdefPath))
                return false;
                
            try
            {
                string content = File.ReadAllText(asmdefPath);
                
                // 簡單的字串檢查是否包含 defineConstraints
                if (content.Contains("\"defineConstraints\""))
                {
                    // 如果包含 defineConstraints，檢查是否為空陣列
                    if (content.Contains("\"defineConstraints\": []") || 
                        content.Contains("\"defineConstraints\":[]"))
                    {
                        return true;
                    }
                    else
                    {
                        // 包含約束，需要檢查是否滿足
                        return false; // 暫時簡化為停用
                    }
                }
                
                return true; // 沒有約束就是啟用
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 啟用或停用 Assembly Define 檔案
        /// </summary>
        private void SetAssemblyDefEnabled(ModuleInfo module, bool enabled)
        {
            if (string.IsNullOrEmpty(module.assemblyDefPath) || module.isCore)
                return;
                
            try
            {
                string content = File.ReadAllText(module.assemblyDefPath);
                
                if (enabled)
                {
                    // 啟用：移除 defineConstraints 或設定為空陣列
                    if (content.Contains("\"defineConstraints\""))
                    {
                        // 簡單的字串替換，將約束設為空陣列
                        content = System.Text.RegularExpressions.Regex.Replace(
                            content, 
                            "\"defineConstraints\"\\s*:\\s*\\[[^\\]]*\\]", 
                            "\"defineConstraints\": []"
                        );
                    }
                }
                else
                {
                    // 停用：添加 defineConstraints 約束
                    string disableConstraint = "GSG_MODULE_DISABLED_" + module.assemblyName.ToUpper().Replace(".", "_");
                    
                    if (content.Contains("\"defineConstraints\""))
                    {
                        // 替換現有的 defineConstraints
                        content = System.Text.RegularExpressions.Regex.Replace(
                            content,
                            "\"defineConstraints\"\\s*:\\s*\\[[^\\]]*\\]",
                            $"\"defineConstraints\": [\"{disableConstraint}\"]"
                        );
                    }
                    else
                    {
                        // 添加新的 defineConstraints
                        content = content.TrimEnd();
                        if (content.EndsWith("}"))
                        {
                            content = content.Substring(0, content.Length - 1);
                            content += $",\n    \"defineConstraints\": [\"{disableConstraint}\"]\n}}";
                        }
                    }
                }
                
                // 寫回檔案
                File.WriteAllText(module.assemblyDefPath, content);
                
                // 刷新 Asset Database
                AssetDatabase.Refresh();
                
                Debug.Log($"GSG Module Manager: {(enabled ? "啟用" : "停用")} {module.name} Assembly Define");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"GSG Module Manager: 無法修改 {module.name} 的 Assembly Define: {e.Message}");
            }
        }
        
        private void ApplyModuleSettings()
        {
            var currentSymbols = GetCurrentDefineSymbols().ToList();
            bool hasChanges = false;
            bool hasAssemblyChanges = false;
            
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
                        bool install = EditorUtility.DisplayDialog("缺少套件", 
                            $"模組 '{module.name}' 需要套件 '{module.packageDependency}'。\n是否要安裝此套件？", 
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
                
                // 處理 Assembly Define 檔案
                if (!string.IsNullOrEmpty(module.assemblyDefPath))
                {
                    bool currentlyEnabled = IsAssemblyDefEnabled(module.assemblyDefPath);
                    if (module.isEnabled != currentlyEnabled)
                    {
                        SetAssemblyDefEnabled(module, module.isEnabled);
                        hasAssemblyChanges = true;
                    }
                }
            }
            
            if (hasChanges || hasAssemblyChanges)
            {
                if (hasChanges)
                {
                    SetDefineSymbols(currentSymbols.ToArray());
                }
                
                string message = "模組設定已成功套用！";
                if (hasAssemblyChanges)
                {
                    message += "\nAssembly Define 檔案也已更新。";
                }
                
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
                modules[1].isEnabled = false; // Steamworks
                modules[2].isEnabled = true;  // File Browser
                modules[3].isEnabled = true;  // Editor Tools
                
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
                        bool exists = request.Result.Any(package => package.name == module.packageDependency);
                        packageCache[module.packageDependency] = exists;
                    }
                }
            }
            
            isCheckingPackages = false;
            Repaint(); // 重新繪製視窗
        }
        
        private bool GetCachedPackageStatus(string packageName)
        {
            return packageCache.ContainsKey(packageName) ? packageCache[packageName] : false;
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