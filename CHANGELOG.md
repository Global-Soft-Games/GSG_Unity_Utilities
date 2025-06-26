# 📋 更新日誌

所有重要的專案變更都會記錄在此檔案中。

格式基於 [Keep a Changelog](https://keepachangelog.com/zh-TW/1.0.0/)，
版本編號遵循 [語義化版本](https://semver.org/lang/zh-TW/) 規範。

---

## [1.0.20] - 2025-01-17 🎯

### ✨ 新增
- **📸 截圖工具 GameView 支援**
  - 新增 GameView 截圖模式，可直接捕獲 Unity Game 視窗內容
  - 無需播放模式即可進行 GameView 截圖
  - 支援 GameView 的自訂解析度批次截圖
  - 新增截圖模式選擇：全螢幕、GameView、攝影機三種模式
  - 根據不同模式自動調整操作介面和說明

### 🎨 改進
- **🛠️ 截圖工具架構優化**
  - 重構截圖核心邏輯，統一處理不同截圖模式
  - 優化使用者介面，根據選擇的模式動態顯示相關設定
  - 改進錯誤處理和使用者提示訊息
  - 新增更詳細的超採樣說明

### 🔧 技術改進
- **🎯 GameView 截圖實現**
  - 新增 `CaptureGameView()` 方法處理單張 GameView 截圖
  - 新增 `CaptureGameViewWithResolution()` 方法處理自訂解析度截圖
  - 使用反射 API 獲取 GameView 視窗資訊
  - 透過 RenderTexture 實現精確的解析度控制

### 📖 使用體驗提升
- **💡 智能模式切換**
  - 不同截圖模式的專屬說明和指引
  - GameView 模式自動跳過播放模式檢查
  - 攝影機設定僅在相關模式下顯示
  - 改進進度條顯示和狀態回饋

---

## [1.0.19] - 2025-06-26 📸

### ✨ 新增
- **📸 截圖工具 (Screenshot Tool)**
  - 全新的編輯器截圖工具，專為遊戲上架素材準備
  - 位置：`Tools > GSG Unity Utilities > Screenshot Tool`
  - 支援單張和批次截圖功能
  - 預設多種 Steam 商店頁面解析度配置
  - 支援自訂解析度預設管理
  - 指定攝影機截圖功能
  - 多種圖片格式支援 (PNG/JPG)
  - 可調整 JPG 品質和超採樣倍數
  - 快捷鍵支援（預設 F12，可自訂）
  - 自動建立和開啟截圖資料夾
  - 設定記憶功能（EditorPrefs）

### 🏗️ 架構優化
- **📁 編輯器資料夾重組**
  - 建立 `Editor/Tools/` 資料夾存放工具類
  - 建立 `Editor/Modules/` 資料夾存放模組管理器
  - 建立 `Editor/Inspectors/` 資料夾存放檢視器繪製器
  - 重新組織現有編輯器檔案到對應分類資料夾

### 🎯 預設配置
- **🎮 Steam 平台解析度**
  - Steam Header: 460x215
  - Steam Screenshot: 1920x1080
  - Steam Capsule Small: 231x87
  - Steam Capsule Medium: 467x181
  - Steam Capsule Large: 616x353
- **📱 常用解析度**
  - 4K Ultra HD: 3840x2160
  - 2K QHD: 2560x1440
  - Full HD: 1920x1080
  - HD Ready: 1280x720
  - 行動裝置直向/橫向解析度

### 📖 文檔更新
- 更新 README.md 新增截圖工具說明
- 新增使用指南和功能介紹

---

## [1.0.18] - 2025-01-15 🚨

### 🚨 重大修復
- **📦 Package Manager 相容性修復**
  - 完全移除 Assembly Define 檔案動態修改功能
  - 修復在 Package Manager 安裝插件中無法修改只讀檔案的錯誤
  - 改為純 Define Symbols 控制架構

### 🔧 架構重大變更
- **⚙️ 模組管理器簡化**
  - 移除所有 Assembly Define 檔案修改邏輯
  - 僅保留 Define Symbols 控制機制
  - 確保在 Packages 資料夾中的只讀插件正常工作

### 🛠️ Assembly Define 檔案更新
- **📋 預設條件編譯約束**
  - UI Extensions: 新增 `GSG_UI_EXTENSIONS_ENABLED` 約束
  - Steamworks: 新增 `GSG_STEAMWORKS_ENABLED` 約束  
  - File Browser: 新增 `GSG_FILEBROWSER_ENABLED` 約束
  - Core: 保持無約束（基礎模組）

### 💡 使用說明更新
- **📖 介面優化**
  - 新增 "Define Symbols Only" 說明
  - 移除 Assembly Define 狀態顯示
  - 簡化模組狀態指示

### 🎯 修復問題
- ✅ 修復路徑錯誤：`Could not find a part of the path "C:\...\GSGUnityUtilities.Runtime.UIExtensions.asmdef"`
- ✅ 確保插件在任何安裝方式下都能正常工作
- ✅ 相容 Package Manager 的只讀限制

---

## [1.0.17] - 2025-01-15 🔧

### ✨ 新增
- **📦 智能套件管理系統**
  - 新增內部套件與外部套件的區分管理
  - 支援 Unity Package Manager 自動安裝內部套件
  - 支援外部套件（如 Steamworks.NET）的引導安裝
  - 新增下載連結和詳細安裝說明功能

### 🎨 改進
- **⚙️ GSGModuleManager 套件檢測升級**
  - 修正 Steamworks.NET 套件檢測邏輯
  - 新增 manifest.json 雙重檢查機制
  - 改進套件狀態顯示：內部套件 vs 外部套件
  - 更精確的套件安裝狀態判斷

### 🔧 技術改進
- **🔍 套件檢測邏輯**
  - 實際套件名稱映射（`Steamworks.NET` → `com.rlabrecque.steamworks.net`）
  - 支援透過 Package Manager API 和檔案系統雙重檢測
  - 新增 `CheckSteamworksInManifest()` 備用檢查機制

### 🚀 使用體驗提升
- **💡 智能安裝流程**
  - 內部套件：一鍵自動安裝 → Unity Package Manager
  - 外部套件：自動開啟下載頁面 + 安裝指南
  - 安裝後自動提示重新整理套件狀態

### 🐛 修復
- **🎯 套件檢測修復**
  - 修正已安裝 Steamworks.NET 卻顯示未安裝的問題
  - 修正外部套件檢測邏輯錯誤
  - 優化套件快取更新機制

---

## [1.0.16] - 2025-01-15 🎯

### ✨ 新增
- **🏗️ 模組化架構重構**
  - 完全重組 Runtime 資料夾結構
  - 按功能模組分離：Core、UIExtensions、FileBrowser、Steamworks
  - 實現真正的模組化插件架構

### 🎨 改進
- **⚙️ GSGModuleManager 重大升級**
  - 新增 Assembly Define 動態管理功能
  - 雙重控制機制：Define Symbols + Assembly Define
  - 智能狀態檢測和視覺化顯示
  - 支援模組的完全啟用/部分啟用/完全停用狀態
  - 新增進階設定面板，顯示詳細的 Assembly 狀態

### 🔧 技術改進
- **📦 Assembly Define 優化**
  - 每個模組獨立的 Assembly Define 檔案
  - 透過 `defineConstraints` 實現動態載入控制
  - 更高效的編譯和記憶體管理
  - 避免循環依賴問題

### 🚀 效能提升
- **⚡ 編譯效能**
  - 停用的模組不參與編譯流程
  - 減少記憶體占用
  - 更快的 Unity 啟動時間

### 📖 文檔更新
- **📋 技術文檔**
  - 更新 PACKAGE_STRUCTURE_PROPOSAL.md
  - 新增 Assembly Define 管理說明
  - 完善模組管理器使用指南

---

## [1.0.15] - 2025-01-15 🚀

### ✨ 新增
- **🎮 Steam 整合模組**
  - 新增 `SteamManager` - Steam API 管理器
  - 新增 `SteamAchievementManager` - 動態成就管理系統
  - 新增 `SteamUIController` - Steam UI 控制介面
  - 支援條件編譯，僅在安裝 Steamworks.NET 時啟用
  - 完整的 Steam 功能文檔和使用指南

### 🎨 改進
- **📖 文檔美化**
  - 全新設計的 README.md 介面
  - 美化的 CHANGELOG.md 格式
  - 新增 emoji 圖示提升可讀性
  - 完整的功能說明和程式碼範例

### 🔧 技術改進
- **⚙️ Assembly Definition 優化**
  - 新增 Steamworks.NET 版本檢測
  - 自動條件編譯支持
  - 更好的命名空間組織

---

## [1.0.14] - 2025-03-05

### ✨ 新增
- 新增 GSG_Unity_Utilities.asmdef 檔案

---

## [1.0.13] - 2025-03-05

### 🗑️ 移除
- 移除 Editor Assembly Definition

---

## [1.0.12] - 2025-03-05

### 🔧 修復
- 修正 Assembly Definition 僅適用於編輯器的問題

---

## [1.0.11] - 2025-03-05

### ✨ 新增
- 為 `SceneReferenceDrawer` 和 `LockInspector` 新增命名空間

---

## [1.0.10] - 2025-02-25

### ✨ 新增
- 為 `Tween` 系統新增命名空間

---

## [1.0.9] - 2025-02-13

### 🗑️ 移除
- 移除 `CreateAssetMenuEx` 功能

---

## [1.0.8] - 2025-02-13

### 🔧 修復
- 修正 `CreateAssetMenuEx` 路徑問題

---

## [1.0.7] - 2025-02-13

### 🔧 修復
- 修正找不到 `CreateAssetMenuEx` 的問題

---

## [1.0.6] - 2025-02-13

### ✨ 新增
- 新增 `CreateAssetMenuEx` 功能

---

## [1.0.5] - 2025-01-10

### 🎨 改進
- **📦 相依性更新**
  - 更新套件相依性版本
- **⚡ 效能優化**
  - `StaticReferenceManager` 改用 Dictionary 取代 List 提升效能
- **🔧 編輯器工具**
  - 新增 `LockInspector` 工具用於鎖定和解鎖 Inspector 視窗

---

## [1.0.4] - 2024-XX-XX

### ✨ 新增
- **📐 場景管理**
  - 新增 Scene Reference 功能，提供更好的 Unity 場景管理

---

## [1.0.3] - 2024-XX-XX

### 🎨 改進
- **⚙️ 配置更新**
  - 修改 package.json 配置
  - 更新套件元數據和設定

---

## [1.0.2.1] - 2024-XX-XX

### 🔧 修復
- **🔧 Reference Manager**
  - Reference Manager 小幅更新和修正

---

## [1.0.2] - 2024-XX-XX

### 🎨 改進
- **🔧 Reference Manager**
  - Reference Manager 重大更新
  - 改進引用處理系統架構

---

## [1.0.1] - 2024-XX-XX

### 🎨 改進
- **⚡ 一般改進**
  - 整體改進和更新
  - 程式碼優化

---

## [1.0.0] - 2024-XX-XX

### 🎉 首次發布
- **🏗️ 專案基礎**
  - 初始版本發布
  - 基礎專案結構和核心功能
  - Reference Manager 系統

---

## 🏷️ 版本說明

### 版本編號格式
- **主版本號 (Major)**: 不相容的 API 變更
- **次版本號 (Minor)**: 向下相容的功能新增
- **修訂版本 (Patch)**: 向下相容的問題修正

### 變更類型說明
- `✨ 新增`: 新功能
- `🎨 改進`: 現有功能改進
- `🔧 修復`: 錯誤修正
- `🗑️ 移除`: 功能移除
- `⚡ 效能`: 效能改善
- `📖 文檔`: 文檔更新
- `🏗️ 重構`: 程式碼重構

---

**📅 持續更新中...**

*感謝您使用 GSG Unity Utilities！如有任何問題或建議，歡迎聯繫我們。*