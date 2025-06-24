# 🎮 GSG Unity Utilities

[![Unity Version](https://img.shields.io/badge/Unity-2022.3+-blue.svg)](https://unity3d.com/get-unity/download)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Package](https://img.shields.io/badge/Package-v1.0.16-orange.svg)](CHANGELOG.md)

> 由 **Global Soft Games** 開發的**模組化** Unity 實用工具套件
> 
> 採用先進的 Assembly Define 動態管理技術，提供靈活的模組化架構，讓您只載入需要的功能

---

## ✨ 功能特色

### 🏗️ 模組化架構 (NEW!)
- **⚙️ 智能模組管理器** - 可視化的模組啟用/停用控制
- **📦 動態 Assembly 載入** - 只編譯您需要的功能模組
- **🎯 雙重控制機制** - Define Symbols + Assembly Define 整合管理
- **📊 即時狀態監控** - 清楚顯示每個模組的載入狀態

### 🎯 核心功能
- **🔧 參考管理器** - 輕鬆管理場景物件引用
- **📱 動畫系統** - 靈活的補間動畫工具
- **🎨 UI 工具** - 豐富的 UI 互動組件
- **📁 檔案處理** - 跨平台檔案瀏覽器
- **⚙️ 編輯器工具** - 提升開發效率的編輯器擴展

### 🚀 Steam 整合 (可選)
- **🎮 Steam API 管理** - 完整的 Steam 功能支援
- **🏆 成就系統** - 動態成就管理與解鎖
- **👤 用戶介面** - Steam 狀態顯示與控制
- **🔧 開發工具** - Steam 功能測試與調試

---

## 📦 安裝方式

### 方法一：Unity Package Manager（推薦）
1. 打開 Unity Package Manager (`Window` > `Package Manager`)
2. 點擊 `+` 按鈕選擇 `Add package from git URL`
3. 輸入：`https://github.com/your-repo/gsg-unity-utilities.git`

### 方法二：手動安裝
1. 下載最新的 `.unitypackage` 檔案
2. 在 Unity 中選擇 `Assets` > `Import Package` > `Custom Package`
3. 選擇下載的檔案並導入

---

## 🎮 Steam 功能設定

### 前置需求
- **Steamworks.NET** 套件（版本 12.0.0 或更高）
- 有效的 Steam App ID
- Steam 客戶端正在運行

### 快速開始

#### 1. 安裝 Steamworks.NET
```bash
# 透過 Package Manager 安裝
com.rlabrecque.steamworks.net
```

#### 2. 基本設定
```csharp
// 在場景中創建 SteamManager
using GSGUnityUtilities.Runtime.Steamworks;

// 添加到 GameObject
var steamManager = gameObject.AddComponent<SteamManager>();

// 成就管理（可選）
var achievementManager = gameObject.AddComponent<SteamAchievementManager>();
```

#### 3. UI 整合
```csharp
// 設定 Steam UI 控制器
var uiController = gameObject.AddComponent<SteamUIController>();
uiController.SetupUIComponents(statusText, usernameText, steamIdText, testButton, achievementButton);
```

---

## 📚 核心組件

### 🎯 參考管理系統
```csharp
// 靜態參考管理器
StaticReferenceManager.RegisterReference("PlayerHealth", healthComponent);
var health = StaticReferenceManager.GetReference<HealthComponent>("PlayerHealth");

// 場景參考
[SerializeField] private SceneReference targetScene;
SceneManager.LoadScene(targetScene.SceneName);
```

### 🎨 動畫系統
```csharp
// 使用 Tween 系統
TweenManager.To(transform, 1.0f)
    .MoveTo(new Vector3(10, 0, 0))
    .ScaleTo(Vector3.one * 2)
    .SetEase(EaseType.EaseOutBounce)
    .SetOnComplete(() => Debug.Log("動畫完成"));
```

### 📱 UI 工具
```csharp
// 長按按鈕
var longPressButton = GetComponent<LongPressButton>();
longPressButton.OnLongPress.AddListener(() => Debug.Log("長按觸發"));

// 彈窗控制器
var popup = GetComponent<PopupController>();
popup.ShowPopup();
```

### 🏆 Steam 成就系統
```csharp
// 解鎖成就
SteamAchievementManager.Instance.UnlockAchievement("ACH_WIN_ONE_GAME");

// 檢查成就狀態
bool isUnlocked = SteamAchievementManager.Instance.IsAchievementUnlocked("ACH_WIN_ONE_GAME");

// 列出所有成就
SteamAchievementManager.Instance.ListAllAchievements();
```

---

## 🛠️ 編輯器工具

### ⚙️ GSG 模組管理器 (NEW!)
- **位置**: `Tools > GSG Unity Utilities > Module Manager`
- **功能**: 
  - 可視化模組啟用/停用控制
  - 自動相依性檢查和套件管理
  - Assembly Define 動態載入控制
  - 即時狀態監控和診斷
- **狀態顯示**:
  - ✅ **完全啟用** - Define Symbol + Assembly 都啟用
  - ⚠️ **部分啟用** - 僅 Define Symbol 啟用
  - ❌ **完全停用** - 兩者都停用

### 🔒 Inspector 鎖定工具
- **快捷鍵**: `Ctrl+L` (Windows) / `Cmd+L` (Mac)
- **功能**: 鎖定/解鎖 Inspector 視窗

### 📐 場景參考繪製器
- 自動為 `SceneReference` 欄位提供場景選擇下拉選單
- 支援場景路徑驗證

---

## 📋 系統需求

| 需求 | 版本 |
|------|------|
| Unity | 2022.3 或更高 |
| .NET | .NET Standard 2.1 |
| TextMeshPro | 3.0.6 或更高 |
| UGUI | 1.0.0 或更高 |

### Steam 功能額外需求
| 需求 | 版本 |
|------|------|
| Steamworks.NET | 12.0.0 或更高 |
| Steam Client | 最新版本 |

---

## 🎨 範例場景

套件包含完整的範例場景展示各種功能：

- **🎮 Steam 整合範例** - 展示 Steam API 使用
- **🏆 成就系統範例** - 演示成就解鎖流程
- **🎨 UI 工具範例** - 各種 UI 組件示範
- **📱 動畫範例** - Tween 動畫效果展示

---

## 🤝 支援與回饋

### 📞 聯絡方式
- **Email**: service@globalsoft.games
- **網站**: https://globalsoft.games
- **技術支援**: https://globalsoft.games/unity-utilities/support

### 🐛 問題回報
發現問題？請透過以下方式回報：
1. 建立 Issue 並詳細描述問題
2. 提供重現步驟和 Unity 版本
3. 附上相關的錯誤日誌

### 💡 功能建議
歡迎提出新功能的建議：
1. 在 Issues 中建立功能請求
2. 詳細說明功能需求和使用場景
3. 我們會評估並考慮加入後續版本

---

## 📄 授權條款

本套件採用 **MIT License** 授權，詳細條款請參見 [LICENSE](LICENSE) 檔案。

---

## 🎯 更新日誌

查看 [CHANGELOG.md](CHANGELOG.md) 獲取詳細的版本更新資訊。

---

**Made with ❤️ by Global Soft Games**

*讓遊戲開發變得更簡單、更有趣！* 