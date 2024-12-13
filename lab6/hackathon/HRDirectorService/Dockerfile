# HRDirectorService/Dockerfile

# Этап 1: СборкаК
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проектов для восстановления зависимостей
COPY ["HRDirectorService/HRDirectorService.csproj", "HRDirectorService/"]
COPY ["DTO/DTO.csproj", "DTO/"]
COPY ["Messages/Messages.csproj", "Messages/"]
# Восстанавливаем зависимости
RUN dotnet restore "HRDirectorService/HRDirectorService.csproj"

# Копируем остальные файлы
COPY . .

# Сборка проекта
WORKDIR "/src/HRDirectorService"
RUN dotnet build "HRDirectorService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап 2: Публикация
FROM build AS publish
WORKDIR "/src/HRDirectorService"
RUN dotnet publish "HRDirectorService.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Этап 3: Запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8083
ENTRYPOINT ["dotnet", "HRDirectorService.dll"]
