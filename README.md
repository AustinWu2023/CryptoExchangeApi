## 實作加分題說明
### 1.印出所有API被呼叫及呼叫外部的API的 request and ewsponse body log
* 透過 RequestResponseLoggingMiddleware(內部) 與 LoggingHttpHandler(外部)分別處理
* Serilog + seq來記錄相關的Log
![image](https://github.com/user-attachments/assets/9bc873ae-93b0-4645-8102-361400b0206e)

### 2.Error handing 處理 API response
* 程式碼中透過Serilog + seq來記錄方便後續追蹤。
* 使用UseDefaultExceptionHandler() 攔截所有未處理的錯誤
![image](https://github.com/user-attachments/assets/a814d3b6-c187-4aee-91c6-b920fd6a304e)

### 3.Swagger-UI
![image](https://github.com/user-attachments/assets/414ab200-3ed0-4cc4-80d0-13f15ad34500)

### 4.多語系設計 [未實作]
### 5.design pattern 實作 [未實作]
### 6.能夠運行在Docker [Swagger-UI 有]
* git checkout Doecker branch, please
* Swagger-UI + seq + SQL Server(DB有問題，問題出在憑證的部分。)
  
### 7 加解密技術應用(AES/RSA) [未實作]

## 其他說明
* api.coindesk.com 的API這幾天有問題， 所以先找了coincap 的API來取得所需資料
* 中文名稱的部分，因為API未提供，所以自行另外自建檔案處理ImportFile\translated_currencies.json
* 資料表建立的檔案放在Doecker branch SqlScripts\init.sql
* 完整程式碼，請切換到Develop分支
* Docker相關部分，請切換到Docker分支
* 測試的部分，請切換到Unittest分支
