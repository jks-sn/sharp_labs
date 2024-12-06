# HRManagerService/Dockerfile
# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /srs

# Копируем файлы проектов для восстановления зависимостей
COPY HRManagerService/*.csproj ./HRManagerService/
COPY Entities/*.csproj ./Entities/
COPY DTO/*.csproj ./DTO/

# Восстанавливаем зависимости
RUN dotnet restore ./HRManagerService/HRManagerService.csproj

# Копируем остальные файлы
COPY . .

# Сборка проекта
WORKDIR "/src/HRManagerService"
RUN dotnet build "HRManagerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап 2: Публикация
FROM build AS publish
RUN dotnet publish "HRManagerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8082
ENTRYPOINT ["dotnet", "HRManagerService.dll"]
