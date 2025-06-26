# 📦 GSG Unity Utilities 套件結構建議

## 🎯 目標

為了讓不同專案能夠根據需求選擇性導入功能模組，我們建議將 GSG Unity Utilities 拆分成多個獨立套件。

---

## 🏗️ 建議的套件結構

### 1. **Core Package** (必需基礎套件)
```
com.globalsoft.unityutilities.core
```

**包含功能：**
- 🔧 參考管理系統 (ReferenceManager, StaticReferenceManager)
- 🎨 動畫系統 (Tween, TweenManager, TweenSequence)
- 📱 基礎 UI 工具 (ClickEventHandler, SelectEventHandler, UnityEventHandler)
- 📐 場景管理 (SceneReference)
- ⚙️ 核心實用工具 (Singleton)

**檔案結構：**
```
Runtime/
├── Core/
│   ├── Animation/
│   │   ├── Tween.cs
│   │   ├── TweenManager.cs
│   │   ├── TweenSequence.cs
│   │   └── TweenValue.cs
│   ├── References/
│   │   ├── ReferenceManager.cs
│   │   ├── StaticReferenceManager.cs
│   │   └── SceneReference.cs
│   ├── UI/
│   │   ├── ClickEventHandler.cs
│   │   ├── SelectEventHandler.cs
│   │   └── UnityEventHandler.cs
│   └── Utilities/
│       └── Singleton.cs
└── GSGCore.Runtime.asmdef

Editor/
├── Core/
│   └── SceneReferenceDrawer.cs
└── GSGCore.Editor.asmdef
```

### 2. **UI Extensions Package** (UI 擴展套件)
```
com.globalsoft.unityutilities.ui
```

**相依性：** Core Package

**包含功能：**
- 🎨 進階 UI 組件 (LongPressButton, PopupController, MainToggleController)
- 🎭 視覺效果 (CanvasFadeIn, CanvasFadeOut, ImageScaler)
- 🔄 狀態管理 (BooleanStateHandler, ToggleGroupStateHandler, InverseToggleHandler)
- 📊 進度追蹤 (ProgressTracker)
- 🎬 動畫腳本 (FlexibleAnimationScript)

### 3. **File Browser Package** (檔案瀏覽器套件)
```
com.globalsoft.unityutilities.filebrowser
```

**相依性：** Core Package

**包含功能：**
- 📁 跨平台檔案瀏覽器 (StandaloneFileBrowser)
- 📄 檔案導入工具 (FileImporter)
- 📝 條碼處理 (BarcodeDecoder, BarcodeInputHandler)

### 4. **Editor Tools Package** (編輯器工具套件)
```
com.globalsoft.unityutilities.editortools
```

**相依性：** Core Package

**包含功能：**
- 🔒 Inspector 鎖定工具 (LockInspector)
- 🎮 模組管理器 (GSGModuleManager)
- 🛠️ 其他開發輔助工具

### 5. **Steamworks Integration Package** (Steam 整合套件)
```
com.globalsoft.unityutilities.steamworks
```

**相依性：** Core Package + Steamworks.NET

**包含功能：**
- 🎮 Steam API 管理 (SteamManager)
- 🏆 成就系統 (SteamAchievementManager)
- 👤 UI 控制 (SteamUIController)
- 🔧 測試工具

---

## 📋 實施方案

### 方案一：完全拆分 (推薦)

#### 優點：
- ✅ 專案可以只導入需要的功能
- ✅ 套件大小最小化
- ✅ 更清晰的相依性管理
- ✅ 獨立版本控制
- ✅ 更容易維護和測試

#### 缺點：
- ❌ 需要重新組織檔案結構
- ❌ 需要更新現有專案的導入設定
- ❌ 套件管理複雜度增加

#### 實施步驟：
1. 建立新的 Git Repository 分支
2. 依照新結構重新組織檔案
3. 建立各套件的 package.json
4. 設定相依性關係
5. 測試各套件獨立運作
6. 更新文檔和範例

### 方案二：條件編譯模組化 (目前實施)

#### 優點：
- ✅ 保持單一套件結構
- ✅ 透過編輯器工具管理功能
- ✅ 無需重新組織專案
- ✅ 向後相容性好

#### 缺點：
- ❌ 套件檔案大小不會減少
- ❌ 依然需要處理不需要的相依性
- ❌ 編譯符號管理複雜

#### 實施步驟：
1. ✅ 為各模組添加條件編譯符號
2. ✅ 建立模組管理器編輯器
3. ✅ 更新文檔說明各模組功能
4. 🔄 測試各種模組組合
5. 📋 提供最佳實踐指南

---

## 🎯 建議實施順序

### 階段一：條件編譯完善 (立即實施)
1. 完成所有核心組件的條件編譯符號
2. 測試模組管理器功能
3. 完善文檔和使用指南

### 階段二：評估使用反饋 (1-2個月後)
1. 收集開發者使用反饋
2. 分析最常用的模組組合
3. 評估是否需要完全拆分

### 階段三：決定最終方案 (3個月後)
1. 根據反饋決定是否實施完全拆分
2. 如果拆分，制定遷移計劃
3. 提供向下相容的遷移工具

---

## 🔧 模組管理器使用

### 開啟管理器
```
Unity Editor > Tools > GSG Unity Utilities > Module Manager
```

### 功能特色 (v1.0.17 更新)
- ✅ 視覺化模組選擇
- ✅ 自動相依性檢查
- ✅ 智能套件管理系統 (NEW!)
  - 內部套件：Unity Package Manager 自動安裝
  - 外部套件：自動開啟下載頁面 + 安裝指南
- ✅ 雙重套件檢測機制
  - Package Manager API 檢測
  - manifest.json 備用檢查
- ✅ Assembly Define 動態管理
- ✅ 編譯符號管理
- ✅ 配置匯出/導入

### 常用模組組合

#### 基礎專案
```
✅ Core Utilities
✅ UI Extensions
❌ File Browser
❌ Editor Tools
❌ Steamworks Integration
```

#### 完整遊戲專案
```
✅ Core Utilities
✅ UI Extensions
✅ File Browser
✅ Editor Tools
❌ Steamworks Integration
```

#### Steam 遊戲專案
```
✅ Core Utilities
✅ UI Extensions
✅ File Browser
✅ Editor Tools
✅ Steamworks Integration
```

#### 僅工具專案
```
✅ Core Utilities
❌ UI Extensions
❌ File Browser
✅ Editor Tools
❌ Steamworks Integration
```

---

## 📈 版本管理策略

### 當前方案 (條件編譯)
- 主版本號：重大 API 變更
- 次版本號：新功能模組
- 修訂版本：錯誤修正和小改進

### 未來方案 (完全拆分)
- 各套件獨立版本控制
- Core Package 作為基準版本
- 擴展套件與 Core 版本相容性表

---

## 🤝 推薦決策

**目前建議：** 先實施**方案二（條件編譯模組化）**

**理由：**
1. 風險較低，不會破壞現有專案
2. 可以立即提供模組化功能
3. 為未來完全拆分做準備
4. 收集實際使用數據以做最佳決策

**後續評估：** 3個月後根據使用反饋決定是否實施完全拆分

---

**📅 更新時間：** 2025年1月

**👥 負責團隊：** Global Soft Games 開發團隊 