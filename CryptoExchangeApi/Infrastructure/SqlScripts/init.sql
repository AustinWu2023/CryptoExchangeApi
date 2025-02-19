-- 檢查資料庫是否存在，若不存在則建立
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CryptoExchange')
BEGIN
    CREATE DATABASE CryptoExchange;
END
GO

-- 使用該資料庫
USE CryptoExchange;
GO

-- 檢查資料表是否存在，若不存在則建立
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CryptoCurrency')
BEGIN
    CREATE TABLE CryptoCurrency (
        Id INT IDENTITY(1,1) PRIMARY KEY,   
        CurrencyCode NVARCHAR(5) NOT NULL,  -- 幣別 (如 BTC, ETH)
        CurrencyName NVARCHAR(20) NOT NULL, -- 幣別的中文名稱 (如 比特幣, 以太幣)
        ExchangeRate DECIMAL(18,8) NOT NULL, -- 以匯率 (通常是 USD)
        UpdatedAtUTC DATETIME2 NOT NULL DEFAULT GETUTCDATE() -- 更新時間 (UTC)
    );
END
GO
