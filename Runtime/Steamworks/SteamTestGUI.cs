#if GSG_STEAMWORKS_ENABLED
using UnityEngine;
using Steamworks;
using GSGUnityUtilities.Runtime.Steamworks;

public class SteamTestGUI : MonoBehaviour
{
    [Header("GUI 設定")]
    public bool showGUI = true;
    public int windowWidth = 400;
    public int windowHeight = 600;
    public int buttonHeight = 30;
    public int spacing = 5;
    
    private Vector2 scrollPosition;
    private bool showAchievementDetails = false;
    private string lastActionResult = "";
    
    private void Update()
    {
        // 按 F1 切換 GUI 顯示
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showGUI = !showGUI;
        }
    }
    
    private void OnGUI()
    {
        if (!showGUI) return;
        
        // 設定 GUI 樣式
        GUI.skin.button.fontSize = 12;
        GUI.skin.label.fontSize = 12;
        GUI.skin.box.fontSize = 12;
        
        // 主視窗
        GUILayout.BeginArea(new Rect(10, 10, windowWidth, windowHeight));
        
        // 標題
        GUILayout.BeginVertical("box");
        GUILayout.Label("🎮 Steam 測試面板", GUI.skin.box);
        GUILayout.Label("按 F1 切換顯示 | 按 ESC 關閉", GUI.skin.label);
        GUILayout.EndVertical();
        
        GUILayout.Space(spacing);
        
        // 開始滾動區域
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        
        // Steam 狀態區域
        DrawSteamStatus();
        
        GUILayout.Space(spacing * 2);
        
        // 成就測試區域
        DrawAchievementSection();
        
        GUILayout.Space(spacing * 2);
        
        // Steam API 測試區域
        DrawSteamAPISection();
        
        GUILayout.Space(spacing * 2);
        
        // 結果顯示區域
        DrawResultSection();
        
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        
        // ESC 關閉
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showGUI = false;
        }
    }
    
    private void DrawSteamStatus()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("📊 Steam 狀態", GUI.skin.box);
        
        bool steamRunning = SteamManager.Instance != null && SteamManager.Instance.IsSteamRunning();
        bool achievementManagerExists = SteamAchievementManager.Instance != null;
        
        // 狀態指示器
        GUI.color = steamRunning ? Color.green : Color.red;
        GUILayout.Label($"Steam 連接: {(steamRunning ? "✅ 已連接" : "❌ 未連接")}");
        
        GUI.color = achievementManagerExists ? Color.green : Color.red;
        GUILayout.Label($"成就管理器: {(achievementManagerExists ? "✅ 已載入" : "❌ 未載入")}");
        
        GUI.color = Color.white;
        
        if (steamRunning && SteamManager.Instance != null)
        {
            GUILayout.Label($"用戶名: {SteamManager.Instance.GetSteamUsername()}");
            GUILayout.Label($"Steam ID: {SteamManager.Instance.GetSteamID()}");
        }
        
        GUILayout.EndVertical();
    }
    
    private void DrawAchievementSection()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("🏆 成就測試", GUI.skin.box);
        
        bool canUseAchievements = SteamManager.Instance != null && 
                                 SteamManager.Instance.IsSteamRunning() && 
                                 SteamAchievementManager.Instance != null;
        
        GUI.enabled = canUseAchievements;
        
        // 動態成就解鎖按鈕
        if (canUseAchievements && SteamAchievementManager.Instance.Achievements.Count > 0)
        {
            foreach (var achievement in SteamAchievementManager.Instance.Achievements)
            {
                // 根據成就狀態設定顏色
                GUI.color = achievement.unlocked ? Color.green : Color.white;
                
                string buttonText = achievement.unlocked ? 
                    $"✅ {achievement.name}" : 
                    $"🏆 解鎖「{achievement.name}」";
                
                if (GUILayout.Button(buttonText, GUILayout.Height(buttonHeight)))
                {
                    if (!achievement.unlocked)
                    {
                        UnlockAchievement(achievement.id, achievement.name);
                    }
                    else
                    {
                        lastActionResult = $"成就「{achievement.name}」已經解鎖了";
                    }
                }
            }
            GUI.color = Color.white;
        }
        else if (canUseAchievements)
        {
            GUILayout.Label("沒有可用的成就", GUI.skin.label);
            if (GUILayout.Button("🔄 重新載入成就", GUILayout.Height(buttonHeight)))
            {
                ReloadAchievements();
            }
        }
        
        GUILayout.Space(spacing);
        
        // 成就管理按鈕
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("📋 列出所有成就", GUILayout.Height(buttonHeight)))
        {
            ListAchievements();
        }
        
        if (GUILayout.Button("🔄 更新狀態", GUILayout.Height(buttonHeight)))
        {
            RefreshAchievements();
        }
        GUILayout.EndHorizontal();
        
        // 成就詳細資訊切換
        showAchievementDetails = GUILayout.Toggle(showAchievementDetails, "顯示成就詳細資訊");
        
        if (showAchievementDetails && canUseAchievements)
        {
            DrawAchievementDetails();
        }
        
        GUILayout.Space(spacing);
        
        // 危險操作
        GUI.color = Color.red;
        if (GUILayout.Button("⚠️ 重置所有成就", GUILayout.Height(buttonHeight)))
        {
            ResetAllAchievements();
        }
        GUI.color = Color.white;
        
        GUI.enabled = true;
        GUILayout.EndVertical();
    }
    
    private void DrawAchievementDetails()
    {
        if (SteamAchievementManager.Instance == null) return;
        
        GUILayout.BeginVertical("box");
        GUILayout.Label("成就詳細狀態:", GUI.skin.label);
        
        foreach (var achievement in SteamAchievementManager.Instance.Achievements)
        {
            GUI.color = achievement.unlocked ? Color.green : Color.gray;
            string status = achievement.unlocked ? "✅" : "❌";
            GUILayout.Label($"{status} {achievement.name}: {achievement.description}");
        }
        GUI.color = Color.white;
        
        GUILayout.EndVertical();
    }
    
    private void DrawSteamAPISection()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("🔧 Steam API 測試", GUI.skin.box);
        
        bool steamRunning = SteamManager.Instance != null && SteamManager.Instance.IsSteamRunning();
        GUI.enabled = steamRunning;
        
        if (GUILayout.Button("📊 測試 Steam API", GUILayout.Height(buttonHeight)))
        {
            TestSteamAPI();
        }
        
        if (GUILayout.Button("👥 打開 Steam 覆蓋層", GUILayout.Height(buttonHeight)))
        {
            OpenSteamOverlay();
        }
        
        if (GUILayout.Button("🛒 打開 Steam 商店", GUILayout.Height(buttonHeight)))
        {
            OpenSteamStore();
        }
        
        GUI.enabled = true;
        GUILayout.EndVertical();
    }
    
    private void DrawResultSection()
    {
        if (!string.IsNullOrEmpty(lastActionResult))
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("📝 最後操作結果", GUI.skin.box);
            GUILayout.Label(lastActionResult, GUI.skin.textArea);
            
            if (GUILayout.Button("清除", GUILayout.Height(20)))
            {
                lastActionResult = "";
            }
            GUILayout.EndVertical();
        }
    }
    
    // 成就相關方法
    private void UnlockAchievement(string achievementId, string achievementName)
    {
        if (SteamAchievementManager.Instance == null)
        {
            lastActionResult = "❌ 成就管理器未找到";
            return;
        }
        
        bool success = SteamAchievementManager.Instance.UnlockAchievement(achievementId);
        lastActionResult = success ? 
            $"✅ 成就「{achievementName}」解鎖成功！" : 
            $"❌ 成就「{achievementName}」解鎖失敗";
        
        Debug.Log($"GUI: {lastActionResult}");
    }
    
    private void ListAchievements()
    {
        if (SteamAchievementManager.Instance == null)
        {
            lastActionResult = "❌ 成就管理器未找到";
            return;
        }
        
        SteamAchievementManager.Instance.ListAllAchievements();
        lastActionResult = "📋 成就列表已輸出到 Console";
        Debug.Log("GUI: 成就列表已輸出");
    }
    
    private void RefreshAchievements()
    {
        if (SteamAchievementManager.Instance == null)
        {
            lastActionResult = "❌ 成就管理器未找到";
            return;
        }
        
        SteamAchievementManager.Instance.UpdateAchievementStatus();
        lastActionResult = "🔄 成就狀態已更新";
        Debug.Log("GUI: 成就狀態已更新");
    }
    
    private void ReloadAchievements()
    {
        if (SteamAchievementManager.Instance == null)
        {
            lastActionResult = "❌ 成就管理器未找到";
            return;
        }
        
        SteamAchievementManager.Instance.ReloadAchievements();
        lastActionResult = "🔄 成就已重新載入";
        Debug.Log("GUI: 成就已重新載入");
    }
    
    private void ResetAllAchievements()
    {
        if (SteamAchievementManager.Instance == null)
        {
            lastActionResult = "❌ 成就管理器未找到";
            return;
        }
        
        SteamAchievementManager.Instance.ResetAllAchievements();
        lastActionResult = "⚠️ 所有成就已重置";
        Debug.Log("GUI: 所有成就已重置");
    }
    
    // Steam API 測試方法
    private void TestSteamAPI()
    {
        if (!SteamManager.Instance.IsSteamRunning())
        {
            lastActionResult = "❌ Steam 未運行";
            return;
        }
        
        Debug.Log("=== GUI Steam API 測試開始 ===");
        
        string testResult = "📊 Steam API 測試結果:\n";
        testResult += $"• 用戶名: {SteamFriends.GetPersonaName()}\n";
        testResult += $"• Steam ID: {SteamUser.GetSteamID()}\n";
        testResult += $"• Steam Level: {SteamUser.GetPlayerSteamLevel()}\n";
        testResult += $"• 線上好友: {SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate)}\n";
        testResult += $"• App ID: {SteamUtils.GetAppID().m_AppId}";
        
        lastActionResult = testResult;
        Debug.Log("GUI: Steam API 測試完成");
    }
    
    private void OpenSteamOverlay()
    {
        if (SteamManager.Instance.IsSteamRunning())
        {
            SteamFriends.ActivateGameOverlay("friends");
            lastActionResult = "👥 Steam 覆蓋層已打開";
            Debug.Log("GUI: 打開 Steam 覆蓋層");
        }
        else
        {
            lastActionResult = "❌ Steam 未運行，無法打開覆蓋層";
        }
    }
    
    private void OpenSteamStore()
    {
        if (SteamManager.Instance.IsSteamRunning())
        {
            uint appId = SteamUtils.GetAppID().m_AppId;
            SteamFriends.ActivateGameOverlayToStore(new AppId_t(appId), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
            lastActionResult = "🛒 Steam 商店已打開";
            Debug.Log("GUI: 打開 Steam 商店");
        }
        else
        {
            lastActionResult = "❌ Steam 未運行，無法打開商店";
        }
    }
}
#endif 