#if GSG_STEAMWORKS_ENABLED
using UnityEngine;
using Steamworks;
using System.Collections.Generic;

namespace GSGUnityUtilities.Runtime.Steamworks
{
    /// <summary>
    /// Steam 成就管理器 - 負責管理遊戲成就系統
    /// 提供成就解鎖、查詢、重設等功能
    /// </summary>
    public class SteamAchievementManager : MonoBehaviour
    {
        [System.Serializable]
        public class Achievement
        {
            public string id;           // 成就 ID
            public string name;         // 成就名稱
            public string description;  // 成就描述
            public bool unlocked;       // 是否已解鎖
        }
        
        [Header("成就列表")]
        [SerializeField] private List<Achievement> achievements = new List<Achievement>();
        
        [Header("成就設定")]
        [SerializeField] private bool autoLoadAchievements = true;
        [SerializeField] private bool useFallbackTestAchievements = true;
        
        private static SteamAchievementManager _instance;
        
        /// <summary>
        /// 取得 SteamAchievementManager 單例實例
        /// </summary>
        public static SteamAchievementManager Instance => _instance;
        
        /// <summary>
        /// 取得所有成就列表
        /// </summary>
        public List<Achievement> Achievements => achievements;
        
        // 備用測試成就（當無法讀取實際成就時使用）
        private readonly Achievement[] fallbackAchievements = {
            new Achievement { id = "ACH_WIN_ONE_GAME", name = "第一次勝利", description = "贏得你的第一場遊戲" },
            new Achievement { id = "ACH_WIN_100_GAMES", name = "百戰百勝", description = "贏得100場遊戲" },
            new Achievement { id = "ACH_TRAVEL_FAR_ACCUM", name = "長途旅行者", description = "累積旅行很遠的距離" },
            new Achievement { id = "ACH_TRAVEL_FAR_SINGLE", name = "一次長途", description = "在單次遊戲中旅行很遠" }
        };
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        private void Start()
        {
            // 初始化成就狀態
            Invoke(nameof(InitializeAchievements), 1f); // 延遲1秒確保 Steam 初始化完成
        }
        
        /// <summary>
        /// 動態載入 Steam 中實際可用的成就
        /// </summary>
        private void LoadSteamAchievements()
        {
            achievements.Clear();
            
            if (!SteamManager.Instance.IsSteamRunning())
            {
                Debug.LogWarning("[SteamAchievementManager] Steam 未運行，無法載入成就");
                LoadFallbackAchievements();
                return;
            }
            
            try
            {
                // 獲取成就數量
                uint numAchievements = SteamUserStats.GetNumAchievements();
                Debug.Log($"[SteamAchievementManager] 🎯 檢測到 {numAchievements} 個成就 (App ID: {SteamUtils.GetAppID()})");
                
                if (numAchievements == 0)
                {
                    Debug.LogWarning("[SteamAchievementManager] ⚠️ 沒有找到任何成就，載入備用測試成就...");
                    LoadFallbackAchievements();
                    return;
                }
                
                // 讀取每個成就的資訊
                for (uint i = 0; i < numAchievements; i++)
                {
                    string achievementId = SteamUserStats.GetAchievementName(i);
                    
                    if (!string.IsNullOrEmpty(achievementId))
                    {
                        // 獲取成就顯示名稱和描述
                        string displayName = SteamUserStats.GetAchievementDisplayAttribute(achievementId, "name");
                        string description = SteamUserStats.GetAchievementDisplayAttribute(achievementId, "desc");
                        
                        // 如果沒有顯示名稱，使用 ID 作為名稱
                        if (string.IsNullOrEmpty(displayName))
                            displayName = achievementId;
                        
                        if (string.IsNullOrEmpty(description))
                            description = "無描述";
                        
                        achievements.Add(new Achievement 
                        { 
                            id = achievementId, 
                            name = displayName, 
                            description = description,
                            unlocked = false
                        });
                        
                        Debug.Log($"[SteamAchievementManager] ✅ 載入成就: [{achievementId}] - {displayName}");
                    }
                }
                
                Debug.Log($"[SteamAchievementManager] 成功載入 {achievements.Count} 個成就");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SteamAchievementManager] 載入成就時發生錯誤: {e.Message}");
                LoadFallbackAchievements();
            }
        }
        
        /// <summary>
        /// 載入備用測試成就（當無法讀取實際成就時使用）
        /// </summary>
        private void LoadFallbackAchievements()
        {
            if (!useFallbackTestAchievements)
            {
                Debug.Log("[SteamAchievementManager] 備用測試成就已停用");
                return;
            }
            
            achievements.Clear();
            foreach (var achievement in fallbackAchievements)
            {
                achievements.Add(new Achievement 
                { 
                    id = achievement.id, 
                    name = achievement.name, 
                    description = achievement.description,
                    unlocked = false
                });
            }
            Debug.Log($"[SteamAchievementManager] 載入了 {achievements.Count} 個備用測試成就");
        }
        
        /// <summary>
        /// 初始化成就系統
        /// </summary>
        public void InitializeAchievements()
        {
            Debug.Log($"[SteamAchievementManager] 🚀 開始初始化成就系統");
            
            if (!SteamManager.Instance.IsSteamRunning())
            {
                Debug.LogWarning("[SteamAchievementManager] ❌ Steam 未運行，無法初始化成就");
                LoadFallbackAchievements();
                return;
            }
            
            // 請求用戶統計數據
            bool success = SteamUserStats.RequestCurrentStats();
            if (success)
            {
                Debug.Log("[SteamAchievementManager] ✅ 成功請求用戶統計數據");
                
                // 如果啟用自動載入，動態讀取成就
                if (autoLoadAchievements)
                {
                    LoadSteamAchievements();
                }
                else
                {
                    LoadFallbackAchievements();
                }
                
                // 更新成就狀態
                UpdateAchievementStatus();
            }
            else
            {
                Debug.LogWarning("[SteamAchievementManager] ❌ 無法請求用戶統計數據");
                LoadFallbackAchievements();
            }
        }
        
        /// <summary>
        /// 更新所有成就的解鎖狀態
        /// </summary>
        public void UpdateAchievementStatus()
        {
            if (!SteamManager.Instance.IsSteamRunning()) return;
            
            foreach (var achievement in achievements)
            {
                bool unlocked = false;
                bool success = SteamUserStats.GetAchievement(achievement.id, out unlocked);
                if (success)
                {
                    achievement.unlocked = unlocked;
                }
            }
        }
        
        /// <summary>
        /// 解鎖指定的成就
        /// </summary>
        /// <param name="achievementId">成就 ID</param>
        /// <returns>是否成功解鎖</returns>
        public bool UnlockAchievement(string achievementId)
        {
            if (!SteamManager.Instance.IsSteamRunning())
            {
                Debug.LogWarning("[SteamAchievementManager] Steam 未運行，無法解鎖成就");
                return false;
            }
            
            bool success = SteamUserStats.SetAchievement(achievementId);
            if (success)
            {
                // 儲存統計數據
                bool storeSuccess = SteamUserStats.StoreStats();
                if (storeSuccess)
                {
                    Debug.Log($"[SteamAchievementManager] ✅ 成就已解鎖: {achievementId}");
                    
                    // 更新本地狀態
                    var achievement = achievements.Find(a => a.id == achievementId);
                    if (achievement != null)
                    {
                        achievement.unlocked = true;
                    }
                    
                    return true;
                }
                else
                {
                    Debug.LogError($"[SteamAchievementManager] ❌ 無法儲存成就: {achievementId}");
                }
            }
            else
            {
                Debug.LogWarning($"[SteamAchievementManager] ❌ 無法解鎖成就: {achievementId}");
            }
            
            return false;
        }
        
        /// <summary>
        /// 檢查指定成就是否已解鎖
        /// </summary>
        /// <param name="achievementId">成就 ID</param>
        /// <returns>是否已解鎖</returns>
        public bool IsAchievementUnlocked(string achievementId)
        {
            var achievement = achievements.Find(a => a.id == achievementId);
            return achievement?.unlocked ?? false;
        }
        
        /// <summary>
        /// 重置所有成就（僅供開發測試使用）
        /// </summary>
        public void ResetAllAchievements()
        {
            if (!SteamManager.Instance.IsSteamRunning())
            {
                Debug.LogWarning("[SteamAchievementManager] Steam 未運行，無法重置成就");
                return;
            }
            
            bool success = SteamUserStats.ResetAllStats(true);
            if (success)
            {
                Debug.Log("[SteamAchievementManager] ⚠️ 所有成就已重置");
                
                // 更新本地狀態
                foreach (var achievement in achievements)
                {
                    achievement.unlocked = false;
                }
            }
            else
            {
                Debug.LogError("[SteamAchievementManager] ❌ 無法重置成就");
            }
        }
        
        /// <summary>
        /// 列出所有成就資訊
        /// </summary>
        public void ListAllAchievements()
        {
            Debug.Log($"[SteamAchievementManager] === 成就列表 ({achievements.Count} 個) ===");
            foreach (var achievement in achievements)
            {
                string status = achievement.unlocked ? "✅" : "❌";
                Debug.Log($"[SteamAchievementManager] {status} [{achievement.id}] {achievement.name} - {achievement.description}");
            }
        }
        
        /// <summary>
        /// 重新載入成就
        /// </summary>
        public void ReloadAchievements()
        {
            InitializeAchievements();
        }
        
        /// <summary>
        /// 取得成就數量
        /// </summary>
        /// <returns>成就總數</returns>
        public int GetAchievementCount()
        {
            return achievements.Count;
        }
        
        /// <summary>
        /// 根據 ID 取得成就
        /// </summary>
        /// <param name="id">成就 ID</param>
        /// <returns>成就物件，若找不到則回傳 null</returns>
        public Achievement GetAchievementById(string id)
        {
            return achievements.Find(a => a.id == id);
        }
    }
}
#endif 