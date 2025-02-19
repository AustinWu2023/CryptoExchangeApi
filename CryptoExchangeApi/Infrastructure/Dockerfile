FROM mcr.microsoft.com/mssql/server:2022-latest

# 設定環境變數
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD="1qaz@WSX3edc"

# 以 root 權限安裝 mssql-tools 和其他工具
USER root
RUN apt-get update && apt-get install -y \
    mssql-tools unixodbc-dev \
    curl unzip vim net-tools iputils-ping && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> /etc/bash.bashrc && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> /root/.bashrc && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# 切回 mssql 使用者
USER mssql

# 確保 mssql 用戶可以使用 sqlcmd
ENV PATH="${PATH}:/opt/mssql-tools/bin"