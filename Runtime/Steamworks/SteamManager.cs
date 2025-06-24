#if GSG_STEAMWORKS_ENABLED
using UnityEngine;
using Steamworks;

namespace GSGUnityUtilities.Runtime.Steamworks
{
    /// <summary>
    /// Steam API 管理器 - 負責初始化和管理 Steam API
    /// 此類別使用單例模式，確保整個應用程式中只有一個 Steam 管理實例
    /// </summary>
    public class SteamManager : MonoBehaviour
    {
        [Header("Steam 設定")]
        [SerializeField] private bool enableSteam = true;
        
        [Header("狀態顯示")]
        [SerializeField] private bool steamInitialized = false;
        [SerializeField] private string steamUsername = "";
        [SerializeField] private CSteamID userID;
        
        private static SteamManager _instance;
        
        /// <summary>
        /// 取得 SteamManager 單例實例
        /// </summary>
        public static SteamManager Instance => _instance;
        
        /// <summary>
        /// Steam 是否已啟用
        /// </summary>
        public bool EnableSteam => enableSteam;
        
        /// <summary>
        /// Steam 是否已初始化成功
        /// </summary>
        public bool SteamInitialized => steamInitialized;
        
        /// <summary>
        /// 當前 Steam 用戶名稱
        /// </summary>
        public string SteamUsername => steamUsername;
        
        /// <summary>
        /// 當前 Steam 用戶 ID
        /// </summary>
        public CSteamID UserID => userID;
        
        private void Awake()
        {
            // 確保只有一個 SteamManager 實例
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            // 初始化 Steam
            InitializeSteam();
        }
        
        /// <summary>
        /// 初始化 Steam API
        /// </summary>
        private void InitializeSteam()
        {
            if (!enableSteam)
            {
                Debug.Log("[SteamManager] Steam 已被停用");
                return;
            }
            
            try
            {
                if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
                {
                    Debug.Log("[SteamManager] Steam 重新啟動應用程式");
                    Application.Quit();
                    return;
                }
                
                // 初始化 Steam API
                steamInitialized = SteamAPI.Init();
                
                if (steamInitialized)
                {
                    Debug.Log("[SteamManager] Steam API 初始化成功!");
                    Debug.Log($"[SteamManager] 📱 實際載入的 App ID: {SteamUtils.GetAppID()}");
                    
                    // 獲取用戶資訊
                    steamUsername = SteamFriends.GetPersonaName();
                    userID = SteamUser.GetSteamID();
                    
                    Debug.Log($"[SteamManager] 歡迎, {steamUsername}! Steam ID: {userID}");
                    
                    // 設定 Steam 回調
                    SetupSteamCallbacks();
                }
                else
                {
                    Debug.LogError("[SteamManager] Steam API 初始化失敗! 請確保 Steam 客戶端正在運行");
                }
            }
            catch (System.DllNotFoundException e)
            {
                Debug.LogError($"[SteamManager] Steam API DLL 未找到: {e.Message}");
                steamInitialized = false;
            }
        }
        
        /// <summary>
        /// 設定 Steam 回調
        /// </summary>
        private void SetupSteamCallbacks()
        {
            // 這裡可以設定各種 Steam 回調
            Debug.Log("[SteamManager] 設定 Steam 回調...");
        }
        
        private void Update()
        {
            if (steamInitialized)
            {
                // 處理 Steam 回調
                SteamAPI.RunCallbacks();
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (steamInitialized)
            {
                if (pauseStatus)
                {
                    Debug.Log("[SteamManager] 應用程式暫停 - Steam API");
                }
                else
                {
                    Debug.Log("[SteamManager] 應用程式恢復 - Steam API");
                }
            }
        }
        
        private void OnDestroy()
        {
            if (steamInitialized)
            {
                SteamAPI.Shutdown();
                Debug.Log("[SteamManager] Steam API 已關閉");
            }
        }
        
        /// <summary>
        /// 檢查 Steam 是否正在運行
        /// </summary>
        /// <returns>Steam 是否運行中</returns>
        public bool IsSteamRunning()
        {
            return steamInitialized;
        }
        
        /// <summary>
        /// 取得 Steam 用戶名稱
        /// </summary>
        /// <returns>用戶名稱或未連接訊息</returns>
        public string GetSteamUsername()
        {
            return steamInitialized ? steamUsername : "未連接到 Steam";
        }
        
        /// <summary>
        /// 取得 Steam 用戶 ID
        /// </summary>
        /// <returns>Steam 用戶 ID</returns>
        public CSteamID GetSteamID()
        {
            return steamInitialized ? userID : CSteamID.Nil;
        }
    }
}
#endif 