# 📋 更新日誌

所有重要的專案變更都會記錄在此檔案中。

格式基於 [Keep a Changelog](https://keepachangelog.com/zh-TW/1.0.0/)，
版本編號遵循 [語義化版本](https://semver.org/lang/zh-TW/) 規範。

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