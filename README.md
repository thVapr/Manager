# Manager

## Running notes

docker run --name manager -p 5432:5432 -e POSTGRES_PASSWORD=password -d postgres:latest
docker run --name pgadmin4 -p 5050:80 -e PGADMIN_DEFAULT_EMAIL=admin@example.com -e PGADMIN_DEFAULT_PASSWORD=password -d dpage/pgadmin4:latest