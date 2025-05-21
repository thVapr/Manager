# Manager

## Running notes

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

## Secrets

dotnet user-secrets init
dotnet user-secrets set "ManagerAuth" "Host=localhost;Port=5432;Database=manager_auth;Username=postgres;Password=password"
dotnet user-secrets set "ManagerData" "Host=localhost;Port=5432;Database=manager_data;Username=postgres;Password=password"
