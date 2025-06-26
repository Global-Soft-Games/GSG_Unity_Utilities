using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace GSGUnityUtilities.Editor.Tools
{
    /// <summary>
    /// 截圖工具編輯器視窗
    /// 專為遊戲上架素材準備而設計的截圖工具
    /// </summary>
    public class ScreenshotTool : EditorWindow
    {
        private Vector2 scrollPosition;
        private string screenshotFolder = "Screenshots";
        private string fileNamePrefix = "Screenshot";
        private ImageFormat imageFormat = ImageFormat.PNG;
        private int jpegQuality = 95;
        private int superSize = 1;
        private bool includeUI = true;
        private bool autoOpenFolder = true;
        
        // 解析度預設
        private List<ResolutionPreset> resolutionPresets = new List<ResolutionPreset>();
        private bool showResolutionSettings = true;
        private bool showAdvancedSettings = false;
        
        // 攝影機相關
        private Camera selectedCamera;
        private bool useSpecificCamera = false;
        
        // 快捷鍵相關
        private bool enableHotkey = true;
        private KeyCode screenshotKey = KeyCode.F12;
        private bool requireCtrl = false;
        private bool requireAlt = false;
        
        public enum ImageFormat
        {
            PNG,
            JPG
        }
        
        [System.Serializable]
        public class ResolutionPreset
        {
            public string name;
            public int width;
            public int height;
            public bool enabled = true;
            public string description;
            
            public ResolutionPreset(string name, int width, int height, string description = "")
            {
                this.name = name;
                this.width = width;
                this.height = height;
                this.description = description;
            }
        }
        
        [MenuItem("Tools/GSG Unity Utilities/Screenshot Tool", priority = 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<ScreenshotTool>("GSG Screenshot Tool");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnEnable()
        {
            InitializeDefaultPresets();
            LoadSettings();
        }
        
        private void OnDisable()
        {
            SaveSettings();
        }
        
        private void OnGUI()
        {
            DrawHeader();
            DrawBasicSettings();
            DrawResolutionSettings();
            DrawAdvancedSettings();
            DrawActionButtons();
            DrawHotkeySettings();
            
            HandleHotkeys();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            var titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("GSG Screenshot Tool", titleStyle);
            EditorGUILayout.LabelField("專為遊戲上架素材準備", EditorStyles.centeredGreyMiniLabel);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("此工具可以快速產生各種平台需要的截圖素材，支援批次截圖和自訂解析度。", MessageType.Info);
            EditorGUILayout.Space(10);
        }
        
        private void DrawBasicSettings()
        {
            EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            // 資料夾設定
            EditorGUILayout.BeginHorizontal();
            screenshotFolder = EditorGUILayout.TextField("截圖資料夾", screenshotFolder);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("選擇截圖資料夾", screenshotFolder, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    screenshotFolder = selectedPath;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // 檔案名稱前綴
            fileNamePrefix = EditorGUILayout.TextField("檔名前綴", fileNamePrefix);
            
            // 圖片格式
            imageFormat = (ImageFormat)EditorGUILayout.EnumPopup("圖片格式", imageFormat);
            
            // JPG 品質（僅在 JPG 格式時顯示）
            if (imageFormat == ImageFormat.JPG)
            {
                jpegQuality = EditorGUILayout.IntSlider("JPG 品質", jpegQuality, 1, 100);
            }
            
            // 超採樣倍數
            superSize = EditorGUILayout.IntSlider("超採樣倍數", superSize, 1, 4);
            EditorGUILayout.HelpBox($"當前解析度: {Screen.width * superSize} x {Screen.height * superSize}", MessageType.None);
            
            // 其他選項
            includeUI = EditorGUILayout.Toggle("包含 UI", includeUI);
            autoOpenFolder = EditorGUILayout.Toggle("自動開啟資料夾", autoOpenFolder);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        private void DrawResolutionSettings()
        {
            showResolutionSettings = EditorGUILayout.Foldout(showResolutionSettings, "解析度預設", true);
            if (showResolutionSettings)
            {
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("預設解析度配置", EditorStyles.boldLabel);
                if (GUILayout.Button("+ 新增", GUILayout.Width(60)))
                {
                    AddCustomPreset();
                }
                if (GUILayout.Button("重設", GUILayout.Width(60)))
                {
                    InitializeDefaultPresets();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(5);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                
                for (int i = 0; i < resolutionPresets.Count; i++)
                {
                    var preset = resolutionPresets[i];
                    DrawPresetItem(preset, i);
                }
                
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(5);
        }
        
        private void DrawPresetItem(ResolutionPreset preset, int index)
        {
            EditorGUILayout.BeginHorizontal("box");
            
            preset.enabled = EditorGUILayout.Toggle(preset.enabled, GUILayout.Width(20));
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            preset.name = EditorGUILayout.TextField(preset.name, GUILayout.Width(120));
            preset.width = EditorGUILayout.IntField(preset.width, GUILayout.Width(80));
            EditorGUILayout.LabelField("x", GUILayout.Width(15));
            preset.height = EditorGUILayout.IntField(preset.height, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            
            if (!string.IsNullOrEmpty(preset.description))
            {
                EditorGUILayout.LabelField(preset.description, EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
            
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                resolutionPresets.RemoveAt(index);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawAdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "進階設定", true);
            if (showAdvancedSettings)
            {
                EditorGUILayout.BeginVertical("box");
                
                // 攝影機設定
                useSpecificCamera = EditorGUILayout.Toggle("使用指定攝影機", useSpecificCamera);
                if (useSpecificCamera)
                {
                    selectedCamera = (Camera)EditorGUILayout.ObjectField("目標攝影機", selectedCamera, typeof(Camera), true);
                    if (selectedCamera == null)
                    {
                        EditorGUILayout.HelpBox("請選擇一個攝影機", MessageType.Warning);
                    }
                }
                
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(5);
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("截圖操作", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            // 單張截圖
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("立即截圖", GUILayout.Height(30)))
            {
                TakeSingleScreenshot();
            }
            if (GUILayout.Button("開啟資料夾", GUILayout.Height(30), GUILayout.Width(100)))
            {
                OpenScreenshotFolder();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // 批次截圖
            if (GUILayout.Button("批次截圖 (所有啟用的解析度)", GUILayout.Height(30)))
            {
                TakeBatchScreenshots();
            }
            
            // 統計資訊
            int enabledCount = resolutionPresets.Count(p => p.enabled);
            EditorGUILayout.HelpBox($"將產生 {enabledCount} 張截圖", MessageType.None);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        private void DrawHotkeySettings()
        {
            EditorGUILayout.LabelField("快捷鍵設定", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            enableHotkey = EditorGUILayout.Toggle("啟用快捷鍵", enableHotkey);
            
            if (enableHotkey)
            {
                screenshotKey = (KeyCode)EditorGUILayout.EnumPopup("截圖按鍵", screenshotKey);
                requireCtrl = EditorGUILayout.Toggle("需要 Ctrl", requireCtrl);
                requireAlt = EditorGUILayout.Toggle("需要 Alt", requireAlt);
                
                string hotkeyText = "";
                if (requireCtrl) hotkeyText += "Ctrl + ";
                if (requireAlt) hotkeyText += "Alt + ";
                hotkeyText += screenshotKey.ToString();
                
                EditorGUILayout.HelpBox($"目前快捷鍵: {hotkeyText}", MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void HandleHotkeys()
        {
            if (!enableHotkey) return;
            
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == screenshotKey)
            {
                bool ctrlOk = !requireCtrl || e.control;
                bool altOk = !requireAlt || e.alt;
                
                if (ctrlOk && altOk)
                {
                    TakeSingleScreenshot();
                    e.Use();
                }
            }
        }
        
        private void TakeSingleScreenshot()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("錯誤", "截圖功能需要在播放模式下使用！", "確定");
                return;
            }
            
            CreateScreenshotFolder();
            string fileName = GenerateFileName();
            string fullPath = Path.Combine(GetScreenshotFolder(), fileName);
            
            try
            {
                // 如果使用指定攝影機或需要自訂解析度，使用 RenderTexture 方法
                if (useSpecificCamera && selectedCamera != null)
                {
                    CaptureWithCamera(selectedCamera, fullPath, Screen.width * superSize, Screen.height * superSize);
                }
                else if (superSize > 1)
                {
                    // 超採樣需要使用攝影機方法
                    Camera targetCamera = Camera.main ?? FindObjectOfType<Camera>();
                    if (targetCamera != null)
                    {
                        CaptureWithCamera(targetCamera, fullPath, Screen.width * superSize, Screen.height * superSize);
                    }
                    else
                    {
                        // 沒有攝影機時回退到系統截圖
                        ScreenCapture.CaptureScreenshot(fullPath, superSize);
                    }
                }
                else
                {
                    // 預設使用系統截圖（最快速）
                    ScreenCapture.CaptureScreenshot(fullPath);
                }
                
                Debug.Log($"截圖已儲存: {fullPath}");
                
                if (autoOpenFolder)
                {
                    EditorUtility.RevealInFinder(fullPath);
                }
                
                ShowNotification(new GUIContent("截圖完成！"));
            }
            catch (Exception e)
            {
                Debug.LogError($"截圖失敗: {e.Message}");
                EditorUtility.DisplayDialog("錯誤", $"截圖失敗: {e.Message}", "確定");
            }
        }
        
        private void TakeBatchScreenshots()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("錯誤", "截圖功能需要在播放模式下使用！", "確定");
                return;
            }
            
            var enabledPresets = resolutionPresets.Where(p => p.enabled).ToList();
            if (enabledPresets.Count == 0)
            {
                EditorUtility.DisplayDialog("警告", "沒有啟用的解析度預設！", "確定");
                return;
            }
            
            // 尋找主攝影機，如果沒有指定攝影機的話
            Camera targetCamera = useSpecificCamera ? selectedCamera : Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindObjectOfType<Camera>();
            }
            
            if (targetCamera == null)
            {
                EditorUtility.DisplayDialog("錯誤", "找不到可用的攝影機進行截圖！請在場景中放置一個攝影機或在進階設定中指定攝影機。", "確定");
                return;
            }
            
            CreateScreenshotFolder();
            string baseName = GenerateFileName(false);
            int successCount = 0;
            
            // 顯示進度條
            float progress = 0f;
            
            foreach (var preset in enabledPresets)
            {
                try
                {
                    // 更新進度條
                    progress = (float)successCount / enabledPresets.Count;
                    EditorUtility.DisplayProgressBar("批次截圖進行中", $"正在截圖: {preset.name} ({preset.width}x{preset.height})", progress);
                    
                    string fileName = $"{baseName}_{preset.name}_{preset.width}x{preset.height}{GetFileExtension()}";
                    string fullPath = Path.Combine(GetScreenshotFolder(), fileName);
                    
                    // 使用 RenderTexture 方法來實現真正的自訂解析度截圖
                    CaptureWithCamera(targetCamera, fullPath, preset.width, preset.height);
                    
                    successCount++;
                    Debug.Log($"截圖已儲存: {fileName}");
                    
                    // 等待一幀確保資源清理完成
                    System.Threading.Thread.Sleep(100);
                }
                catch (Exception e)
                {
                    Debug.LogError($"截圖失敗 ({preset.name}): {e.Message}");
                }
            }
            
            // 清除進度條
            EditorUtility.ClearProgressBar();
            
            Debug.Log($"批次截圖完成！成功產生 {successCount}/{enabledPresets.Count} 張截圖");
            ShowNotification(new GUIContent($"完成 {successCount} 張截圖！"));
            
            if (autoOpenFolder && successCount > 0)
            {
                OpenScreenshotFolder();
            }
        }
        
        private void CaptureWithCamera(Camera camera, string path, int width, int height)
        {
            // 建立 RenderTexture
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            RenderTexture originalTexture = camera.targetTexture;
            RenderTexture originalActive = RenderTexture.active;
            
            try
            {
                // 設定攝影機目標 RenderTexture
                camera.targetTexture = renderTexture;
                
                // 渲染攝影機內容到 RenderTexture
                camera.Render();
                
                // 從 RenderTexture 讀取像素到 Texture2D
                RenderTexture.active = renderTexture;
                Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
                screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                screenshot.Apply();
                
                // 編碼為圖片檔案
                byte[] data;
                if (imageFormat == ImageFormat.PNG)
                {
                    data = screenshot.EncodeToPNG();
                }
                else
                {
                    data = screenshot.EncodeToJPG(jpegQuality);
                }
                
                // 寫入檔案
                File.WriteAllBytes(path, data);
                
                // 清理 Texture2D
                DestroyImmediate(screenshot);
            }
            catch (Exception e)
            {
                Debug.LogError($"攝影機截圖失敗: {e.Message}");
                throw;
            }
            finally
            {
                // 恢復攝影機原始設定
                camera.targetTexture = originalTexture;
                RenderTexture.active = originalActive;
                
                // 清理 RenderTexture
                if (renderTexture != null)
                {
                    DestroyImmediate(renderTexture);
                }
            }
        }
        
        private void OpenScreenshotFolder()
        {
            string folder = GetScreenshotFolder();
            if (Directory.Exists(folder))
            {
                EditorUtility.RevealInFinder(folder);
            }
            else
            {
                EditorUtility.DisplayDialog("錯誤", "截圖資料夾不存在！", "確定");
            }
        }
        
        private void CreateScreenshotFolder()
        {
            string folder = GetScreenshotFolder();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Debug.Log($"建立截圖資料夾: {folder}");
            }
        }
        
        private string GetScreenshotFolder()
        {
            if (Path.IsPathRooted(screenshotFolder))
            {
                return screenshotFolder;
            }
            return Path.Combine(Application.dataPath, "..", screenshotFolder);
        }
        
        private string GenerateFileName(bool includeTimestamp = true)
        {
            if (includeTimestamp)
            {
                return $"{fileNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}{GetFileExtension()}";
            }
            else
            {
                return fileNamePrefix;
            }
        }
        
        private string GetFileExtension()
        {
            return imageFormat == ImageFormat.PNG ? ".png" : ".jpg";
        }
        
        private void AddCustomPreset()
        {
            resolutionPresets.Add(new ResolutionPreset("自訂解析度", 1920, 1080, "使用者自訂"));
        }
        
        private void InitializeDefaultPresets()
        {
            resolutionPresets.Clear();
            
            // Steam 商店頁面解析度
            resolutionPresets.Add(new ResolutionPreset("Steam Header", 460, 215, "Steam 商店頁面標題圖"));
            resolutionPresets.Add(new ResolutionPreset("Steam Screenshot", 1920, 1080, "Steam 商店截圖"));
            resolutionPresets.Add(new ResolutionPreset("Steam Capsule Small", 231, 87, "Steam 小型封面"));
            resolutionPresets.Add(new ResolutionPreset("Steam Capsule Medium", 467, 181, "Steam 中型封面"));
            resolutionPresets.Add(new ResolutionPreset("Steam Capsule Large", 616, 353, "Steam 大型封面"));
            
            // 常用遊戲解析度
            resolutionPresets.Add(new ResolutionPreset("4K Ultra HD", 3840, 2160, "4K 超高清"));
            resolutionPresets.Add(new ResolutionPreset("2K QHD", 2560, 1440, "2K 四倍高清"));
            resolutionPresets.Add(new ResolutionPreset("Full HD", 1920, 1080, "全高清"));
            resolutionPresets.Add(new ResolutionPreset("HD Ready", 1280, 720, "高清就緒"));
            
            // 行動裝置解析度
            resolutionPresets.Add(new ResolutionPreset("Mobile Portrait", 1080, 1920, "手機直向"));
            resolutionPresets.Add(new ResolutionPreset("Mobile Landscape", 1920, 1080, "手機橫向"));
            resolutionPresets.Add(new ResolutionPreset("Tablet", 2048, 1536, "平板電腦"));
            
            // 預設只啟用常用的解析度
            foreach (var preset in resolutionPresets)
            {
                preset.enabled = preset.name.Contains("Steam Screenshot") || 
                                preset.name.Contains("Full HD") || 
                                preset.name.Contains("Steam Header");
            }
        }
        
        private void SaveSettings()
        {
            EditorPrefs.SetString("GSG_Screenshot_Folder", screenshotFolder);
            EditorPrefs.SetString("GSG_Screenshot_Prefix", fileNamePrefix);
            EditorPrefs.SetInt("GSG_Screenshot_Format", (int)imageFormat);
            EditorPrefs.SetInt("GSG_Screenshot_Quality", jpegQuality);
            EditorPrefs.SetInt("GSG_Screenshot_SuperSize", superSize);
            EditorPrefs.SetBool("GSG_Screenshot_IncludeUI", includeUI);
            EditorPrefs.SetBool("GSG_Screenshot_AutoOpen", autoOpenFolder);
            EditorPrefs.SetBool("GSG_Screenshot_EnableHotkey", enableHotkey);
            EditorPrefs.SetInt("GSG_Screenshot_HotkeyKey", (int)screenshotKey);
            EditorPrefs.SetBool("GSG_Screenshot_RequireCtrl", requireCtrl);
            EditorPrefs.SetBool("GSG_Screenshot_RequireAlt", requireAlt);
        }
        
        private void LoadSettings()
        {
            screenshotFolder = EditorPrefs.GetString("GSG_Screenshot_Folder", "Screenshots");
            fileNamePrefix = EditorPrefs.GetString("GSG_Screenshot_Prefix", "Screenshot");
            imageFormat = (ImageFormat)EditorPrefs.GetInt("GSG_Screenshot_Format", 0);
            jpegQuality = EditorPrefs.GetInt("GSG_Screenshot_Quality", 95);
            superSize = EditorPrefs.GetInt("GSG_Screenshot_SuperSize", 1);
            includeUI = EditorPrefs.GetBool("GSG_Screenshot_IncludeUI", true);
            autoOpenFolder = EditorPrefs.GetBool("GSG_Screenshot_AutoOpen", true);
            enableHotkey = EditorPrefs.GetBool("GSG_Screenshot_EnableHotkey", true);
            screenshotKey = (KeyCode)EditorPrefs.GetInt("GSG_Screenshot_HotkeyKey", (int)KeyCode.F12);
            requireCtrl = EditorPrefs.GetBool("GSG_Screenshot_RequireCtrl", false);
            requireAlt = EditorPrefs.GetBool("GSG_Screenshot_RequireAlt", false);
        }
    }
} 