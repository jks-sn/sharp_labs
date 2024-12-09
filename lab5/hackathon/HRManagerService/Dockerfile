# HRManagerService/Dockerfile

# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проектов для восстановления зависимостей
COPY ["HRManagerService/HRManagerService.csproj", "HRManagerService/"]
COPY ["DTO/DTO.csproj", "DTO/"]
COPY ["Entities/Entities.csproj", "Entities/"]

# Восстанавливаем зависимости
RUN dotnet restore "HRManagerService/HRManagerService.csproj"

# Копируем остальные файлы
COPY . .

# Сборка проекта
WORKDIR "/src/HRManagerService"
RUN dotnet build "HRManagerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап 2: Публикация
FROM build AS publish
WORKDIR "/src/HRManagerService"
RUN dotnet publish "HRManagerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Этап 3: Запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8082
ENTRYPOINT ["dotnet", "HRManagerService.dll"]
