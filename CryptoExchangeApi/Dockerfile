# 使用 .NET 8 SDK 建置 API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 設定 ASP.NET Core 運行環境變數
ENV ASPNETCORE_URLS=http://+:8080

# 建置應用程式
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 安裝 EF
#RUN dotnet tool install --global dotnet-ef
#ENV PATH="${PATH}:/root/.dotnet/tools"
# 連線字串
# ARG ConnectionString="Server=mssql,1433;Database=CryptoExchange;User Id=sa;Password=YourStrong!Passw0rd"

# 只複製主專案的 csproj
COPY ["CryptoExchangeApi.csproj", "./"]
RUN dotnet restore "CryptoExchangeApi.csproj"

# 複製所有檔案（但 `.dockerignore` 可排除 `Tests` 目錄）
COPY . .

#RUN echo "Waiting for SQL Server to start..." && \
    #sleep 60 && \
    #dotnet ef dbcontext scaffold "$ConnectionString" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --force

# 只編譯主 API，排除 `.sln` 自動包含的測試專案
RUN dotnet build "CryptoExchangeApi.csproj" -c Release -o /app/build


# 發佈應用程式
FROM build AS publish
RUN dotnet publish "CryptoExchangeApi.csproj" -c Release -o /app/publish

# 最終運行環境
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoExchangeApi.dll"]


