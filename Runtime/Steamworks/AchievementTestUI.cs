#if GSG_STEAMWORKS_ENABLED
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GSGUnityUtilities.Runtime.Steamworks;

public class AchievementTestUI : MonoBehaviour
{
    [Header("成就測試按鈕")]
    public Button unlockFirstWinButton;
    public Button unlockHundredWinsButton;
    public Button unlockTravelAccumButton;
    public Button unlockTravelSingleButton;
    
    [Header("管理按鈕")]
    public Button listAchievementsButton;
    public Button resetAllButton;
    public Button refreshStatusButton;
    
    [Header("狀態顯示")]
    public TextMeshProUGUI statusText;
    public Transform achievementListParent;
    public GameObject achievementItemPrefab;
    
    private void Start()
    {
        SetupButtons();
        UpdateUI();
    }
    
    private void SetupButtons()
    {
        // 成就解鎖按鈕
        if (unlockFirstWinButton != null)
            unlockFirstWinButton.onClick.AddListener(() => UnlockAchievement("ACH_WIN_ONE_GAME"));
            
        if (unlockHundredWinsButton != null)
            unlockHundredWinsButton.onClick.AddListener(() => UnlockAchievement("ACH_WIN_100_GAMES"));
            
        if (unlockTravelAccumButton != null)
            unlockTravelAccumButton.onClick.AddListener(() => UnlockAchievement("ACH_TRAVEL_FAR_ACCUM"));
            
        if (unlockTravelSingleButton != null)
            unlockTravelSingleButton.onClick.AddListener(() => UnlockAchievement("ACH_TRAVEL_FAR_SINGLE"));
        
        // 管理按鈕
        if (listAchievementsButton != null)
            listAchievementsButton.onClick.AddListener(ListAchievements);
            
        if (resetAllButton != null)
            resetAllButton.onClick.AddListener(ResetAllAchievements);
            
        if (refreshStatusButton != null)
            refreshStatusButton.onClick.AddListener(RefreshAchievementStatus);
    }
    
    private void Update()
    {
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        bool steamRunning = SteamManager.Instance != null && SteamManager.Instance.IsSteamRunning();
        bool achievementManagerExists = SteamAchievementManager.Instance != null;
        
        // 更新狀態文字
        if (statusText != null)
        {
            if (!steamRunning)
                statusText.text = "狀態: Steam 未連接 ❌";
            else if (!achievementManagerExists)
                statusText.text = "狀態: 成就管理器未找到 ⚠️";
            else
                statusText.text = "狀態: 準備就緒 ✅";
        }
        
        // 更新按鈕狀態
        bool buttonsEnabled = steamRunning && achievementManagerExists;
        
        if (unlockFirstWinButton != null)
            unlockFirstWinButton.interactable = buttonsEnabled;
        if (unlockHundredWinsButton != null)
            unlockHundredWinsButton.interactable = buttonsEnabled;
        if (unlockTravelAccumButton != null)
            unlockTravelAccumButton.interactable = buttonsEnabled;
        if (unlockTravelSingleButton != null)
            unlockTravelSingleButton.interactable = buttonsEnabled;
        if (listAchievementsButton != null)
            listAchievementsButton.interactable = buttonsEnabled;
        if (resetAllButton != null)
            resetAllButton.interactable = buttonsEnabled;
        if (refreshStatusButton != null)
            refreshStatusButton.interactable = buttonsEnabled;
    }
    
    private void UnlockAchievement(string achievementId)
    {
        if (SteamAchievementManager.Instance == null)
        {
            Debug.LogError("SteamAchievementManager 實例不存在！");
            return;
        }
        
        Debug.Log($"嘗試解鎖成就: {achievementId}");
        bool success = SteamAchievementManager.Instance.UnlockAchievement(achievementId);
        
        if (success)
        {
            Debug.Log($"🎉 成就解鎖成功: {achievementId}");
            // Steam 會自動顯示成就通知
        }
        else
        {
            Debug.LogWarning($"❌ 成就解鎖失敗: {achievementId}");
        }
    }
    
    private void ListAchievements()
    {
        if (SteamAchievementManager.Instance != null)
        {
            SteamAchievementManager.Instance.ListAllAchievements();
        }
    }
    
    private void ResetAllAchievements()
    {
        if (SteamAchievementManager.Instance != null)
        {
            // 顯示確認對話框（簡單版本）
            Debug.LogWarning("⚠️ 即將重置所有成就！這個操作無法復原！");
            SteamAchievementManager.Instance.ResetAllAchievements();
        }
    }
    
    private void RefreshAchievementStatus()
    {
        if (SteamAchievementManager.Instance != null)
        {
            SteamAchievementManager.Instance.UpdateAchievementStatus();
            Debug.Log("🔄 成就狀態已更新");
        }
    }
    
    // 在遊戲中觸發成就的範例方法
    // 你可以在遊戲的不同事件中調用這些方法
    
    /// <summary>
    /// 模擬遊戲勝利事件 - 可在 Inspector 中手動調用或在程式碼中使用
    /// </summary>
    public void SimulateGameWin()
    {
        Debug.Log("🎮 模擬遊戲勝利事件");
        UnlockAchievement("ACH_WIN_ONE_GAME");
    }
    
    /// <summary>
    /// 模擬長途旅行事件 - 可在 Inspector 中手動調用或在程式碼中使用
    /// </summary>
    public void SimulateLongTravel()
    {
        Debug.Log("🚀 模擬長途旅行事件");
        UnlockAchievement("ACH_TRAVEL_FAR_SINGLE");
    }
    
    // 進階用法：根據遊戲數據觸發成就
    public void CheckAndUnlockProgressAchievements(int gamesWon, float totalDistance)
    {
        if (SteamAchievementManager.Instance == null) return;
        
        // 檢查並解鎖基於進度的成就
        if (gamesWon >= 1)
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_WIN_ONE_GAME");
        }
        
        if (gamesWon >= 100)
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_WIN_100_GAMES");
        }
        
        if (totalDistance >= 1000f) // 假設的距離閾值
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_TRAVEL_FAR_ACCUM");
        }
    }
} 
#endif