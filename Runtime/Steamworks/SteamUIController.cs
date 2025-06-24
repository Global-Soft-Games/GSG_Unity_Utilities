#if GSG_STEAMWORKS_ENABLED
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

namespace GSGUnityUtilities.Runtime.Steamworks
{
    /// <summary>
    /// Steam UI 控制器 - 提供 Steam 資訊顯示和控制介面
    /// 用於顯示 Steam 狀態、用戶資訊和基本功能操作
    /// </summary>
    public class SteamUIController : MonoBehaviour
    {
        [Header("UI 元件")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TextMeshProUGUI steamIdText;
        [SerializeField] private Button testButton;
        [SerializeField] private Button achievementButton;
        
        [Header("測試設定")]
        [SerializeField] private string testAchievementId = "ACH_WIN_ONE_GAME"; // 範例成就 ID
        
        /// <summary>
        /// 狀態顯示文字組件
        /// </summary>
        public TextMeshProUGUI StatusText
        {
            get => statusText;
            set => statusText = value;
        }
        
        /// <summary>
        /// 用戶名顯示文字組件
        /// </summary>
        public TextMeshProUGUI UsernameText
        {
            get => usernameText;
            set => usernameText = value;
        }
        
        /// <summary>
        /// Steam ID 顯示文字組件
        /// </summary>
        public TextMeshProUGUI SteamIdText
        {
            get => steamIdText;
            set => steamIdText = value;
        }
        
        /// <summary>
        /// 測試按鈕組件
        /// </summary>
        public Button TestButton
        {
            get => testButton;
            set => testButton = value;
        }
        
        /// <summary>
        /// 成就按鈕組件
        /// </summary>
        public Button AchievementButton
        {
            get => achievementButton;
            set => achievementButton = value;
        }
        
        /// <summary>
        /// 測試成就 ID
        /// </summary>
        public string TestAchievementId
        {
            get => testAchievementId;
            set => testAchievementId = value;
        }
        
        private void Start()
        {
            // 設定按鈕事件
            if (testButton != null)
                testButton.onClick.AddListener(TestSteamAPI);
                
            if (achievementButton != null)
                achievementButton.onClick.AddListener(UnlockTestAchievement);
        }
        
        private void Update()
        {
            UpdateUI();
        }
        
        /// <summary>
        /// 更新 UI 顯示
        /// </summary>
        private void UpdateUI()
        {
            if (SteamManager.Instance == null)
            {
                if (statusText != null)
                    statusText.text = "狀態: SteamManager 未找到";
                return;
            }
            
            // 更新狀態文字
            if (statusText != null)
            {
                statusText.text = SteamManager.Instance.IsSteamRunning() ? 
                    "狀態: Steam 已連接 ✓" : "狀態: Steam 未連接 ✗";
                statusText.color = SteamManager.Instance.IsSteamRunning() ? 
                    Color.green : Color.red;
            }
            
            // 更新用戶名稱
            if (usernameText != null)
            {
                usernameText.text = $"用戶: {SteamManager.Instance.GetSteamUsername()}";
            }
            
            // 更新 Steam ID
            if (steamIdText != null)
            {
                CSteamID steamId = SteamManager.Instance.GetSteamID();
                steamIdText.text = $"Steam ID: {(steamId.IsValid() ? steamId.ToString() : "無效")}";
            }
            
            // 更新按鈕狀態
            bool steamRunning = SteamManager.Instance.IsSteamRunning();
            if (testButton != null)
                testButton.interactable = steamRunning;
            if (achievementButton != null)
                achievementButton.interactable = steamRunning;
        }
        
        /// <summary>
        /// 測試 Steam API 功能
        /// </summary>
        public void TestSteamAPI()
        {
            if (!SteamManager.Instance.IsSteamRunning())
            {
                Debug.LogWarning("[SteamUIController] Steam 未運行，無法執行測試");
                return;
            }
            
            Debug.Log("[SteamUIController] === Steam API 測試開始 ===");
            
            // 測試用戶資訊
            Debug.Log($"[SteamUIController] 用戶名稱: {SteamFriends.GetPersonaName()}");
            Debug.Log($"[SteamUIController] Steam ID: {SteamUser.GetSteamID()}");
            Debug.Log($"[SteamUIController] Steam Level: {SteamUser.GetPlayerSteamLevel()}");
            
            // 測試好友資訊
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            Debug.Log($"[SteamUIController] 線上好友數量: {friendCount}");
            
            // 測試應用程式資訊
            uint appId = SteamUtils.GetAppID().m_AppId;
            Debug.Log($"[SteamUIController] 當前應用程式 ID: {appId}");
            
            Debug.Log("[SteamUIController] === Steam API 測試完成 ===");
        }
        
        /// <summary>
        /// 解鎖測試成就
        /// </summary>
        public void UnlockTestAchievement()
        {
            if (!SteamManager.Instance.IsSteamRunning())
            {
                Debug.LogWarning("[SteamUIController] Steam 未運行，無法解鎖成就");
                return;
            }
            
            if (SteamAchievementManager.Instance != null)
            {
                // 使用成就管理器來解鎖成就
                bool success = SteamAchievementManager.Instance.UnlockAchievement(testAchievementId);
                if (!success)
                {
                    Debug.LogWarning($"[SteamUIController] 無法透過成就管理器解鎖成就: {testAchievementId}");
                }
            }
            else
            {
                // 直接使用 Steam API 解鎖成就
                if (!string.IsNullOrEmpty(testAchievementId))
                {
                    bool success = SteamUserStats.SetAchievement(testAchievementId);
                    if (success)
                    {
                        SteamUserStats.StoreStats();
                        Debug.Log($"[SteamUIController] 成就已解鎖: {testAchievementId}");
                    }
                    else
                    {
                        Debug.LogWarning($"[SteamUIController] 無法解鎖成就: {testAchievementId} (可能成就 ID 不存在)");
                    }
                }
                else
                {
                    Debug.LogWarning("[SteamUIController] 測試成就 ID 未設定");
                }
            }
        }
        
        /// <summary>
        /// 打開 Steam 覆蓋層
        /// </summary>
        public void OpenSteamOverlay()
        {
            if (SteamManager.Instance.IsSteamRunning())
            {
                SteamFriends.ActivateGameOverlay("friends");
                Debug.Log("[SteamUIController] 打開 Steam 覆蓋層");
            }
        }
        
        /// <summary>
        /// 打開 Steam 商店頁面
        /// </summary>
        public void OpenSteamStore()
        {
            if (SteamManager.Instance.IsSteamRunning())
            {
                uint appId = SteamUtils.GetAppID().m_AppId;
                SteamFriends.ActivateGameOverlayToStore(new AppId_t(appId), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
                Debug.Log("[SteamUIController] 打開 Steam 商店頁面");
            }
        }
        
        /// <summary>
        /// 手動設定 UI 組件（用於程式碼動態配置）
        /// </summary>
        /// <param name="statusText">狀態文字組件</param>
        /// <param name="usernameText">用戶名文字組件</param>
        /// <param name="steamIdText">Steam ID 文字組件</param>
        /// <param name="testButton">測試按鈕組件</param>
        /// <param name="achievementButton">成就按鈕組件</param>
        public void SetupUIComponents(TextMeshProUGUI statusText = null, 
                                    TextMeshProUGUI usernameText = null, 
                                    TextMeshProUGUI steamIdText = null, 
                                    Button testButton = null, 
                                    Button achievementButton = null)
        {
            if (statusText != null) this.statusText = statusText;
            if (usernameText != null) this.usernameText = usernameText;
            if (steamIdText != null) this.steamIdText = steamIdText;
            if (testButton != null) 
            {
                this.testButton = testButton;
                this.testButton.onClick.AddListener(TestSteamAPI);
            }
            if (achievementButton != null) 
            {
                this.achievementButton = achievementButton;
                this.achievementButton.onClick.AddListener(UnlockTestAchievement);
            }
        }
    }
}
#endif 