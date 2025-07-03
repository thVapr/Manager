# :bar_chart: Project Management System (Graduation project)

[![ASP.NET](https://img.shields.io/badge/ASP.NET_Core-512BD4?logo=dotnet&logoColor=fff)](#) [![EntityFramework](https://img.shields.io/badge/EntityFramework_Core-512BD4?logo=dotnet&logoColor=fff)](#) [![Postgres](https://img.shields.io/badge/Postgres-%23316192.svg?logo=postgresql&logoColor=white)](#) [![Angular](https://img.shields.io/badge/Angular-%23DD0031.svg?logo=angular&logoColor=white)](#) [![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?logo=bootstrap&logoColor=fff)](#) [![PrimeNG](https://img.shields.io/badge/PrimeNG-%23DD0031.svg?logo=PrimeNG&logoColor=white)](#) [![Chart.js](https://img.shields.io/badge/Chart.js-FF6384?logo=chartdotjs&logoColor=fff)](#) [![Quill.js](https://img.shields.io/badge/Quill.js-714B67)](#) [![Telegram](https://img.shields.io/badge/Telegram-2CA5E0?logo=telegram&logoColor=white)](#) [![MinIO](https://img.shields.io/badge/MinIO-black?logo=minio&logoColor=fff)](#)

![image](https://github.com/user-attachments/assets/91d2bc52-e318-4dd6-a8e1-6671db32fa49)

## :running: Running notes

docker run --name manager `
    -p 5432:5432 `
    -e POSTGRES_PASSWORD=password `
    -d postgres:latest

docker run --name pgadmin4 `
    -p 5050:80 `
    -e PGADMIN_DEFAULT_EMAIL=admin@example.com `
    -e PGADMIN_DEFAULT_PASSWORD=password `
    -d dpage/pgadmin4:latest

docker run `
    -p 9000:9000 `
    -p 9001:9001 `
    --name minio `
    -v D:\minio\data:/data `
    -e "MINIO_ROOT_USER=user" `
    -e "MINIO_ROOT_PASSWORD=password" `
    quay.io/minio/minio server /data --console-address ":9001"

## :key: Secrets

dotnet user-secrets init

dotnet user-secrets set "ManagerAuth" "Host=localhost;Port=5432;Database=manager_auth;Username=postgres;Password=password"

dotnet user-secrets set "ManagerData" "Host=localhost;Port=5432;Database=manager_data;Username=postgres;Password=password"

dotnet user-secrets set "StorageAccessKey" "user"

dotnet user-secrets set "StorageSecretKey" "password"

dotnet user-secrets set "TelegramBotToken" ""

dotnet user-secrets set "TokenSecretKey" "nullreference0ee22bbnullreference"
