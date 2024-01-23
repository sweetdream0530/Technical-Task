# Stage 1: Build Pixel Service
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-pixel-service
WORKDIR /app/pixel-service
COPY ./PixelService/PixelService.csproj .
RUN dotnet restore
COPY ./PixelService .
RUN dotnet publish -c Release -o out

# Stage 2: Runtime for Pixel Service
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime-pixel-service
WORKDIR /app/pixel-service
COPY --from=build-pixel-service /app/pixel-service/out .
ENTRYPOINT ["dotnet", "PixelService.dll"]
