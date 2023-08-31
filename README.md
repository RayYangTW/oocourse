# OOcourse

整合視訊家教與實體教學的網站

## 專案進度

**專案目前最新進度在 dev 分支上，尚未推到 Main 上。**

## 啟動專案執行步驟

- Git clone：`https://github.com/RayYangTW/oocourse.git`
- 安裝 Package：`dotnet restore`
- 在本地端建立 SQL server
- 以`env.example`作為參考，建立`.env`並更新內容（包含 connection string）
- 修改`appsettings.json`內的 Host 為`http://localhost:[your port]`
- 進入`/wwwroot/js`，找到`config.js`，修改 host 為`http://localhost:[your port]`
- 更新資料庫：`dotnet ef database update`
- 執行專案：`dotnet run`

## 開發工具

- VScode
- ASP.NET @7.0.302
- AWS
