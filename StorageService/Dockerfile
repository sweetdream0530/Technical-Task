# Stage 1: Build Storage Service
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-storage-service
WORKDIR /app/storage-service
COPY ./StorageService/StorageService.csproj .
RUN dotnet restore
COPY ./StorageService .
RUN dotnet publish -c Release -o out

# Stage 2: Runtime for Storage Service
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime-storage-service
WORKDIR /app/storage-service
COPY --from=build-storage-service /app/storage-service/out .
ENTRYPOINT ["dotnet", "StorageService.dll"]
