# ParticipantService/Dockerfile

# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проектов для восстановления зависимостей
COPY ["ParticipantService/ParticipantService.csproj", "ParticipantService/"]
COPY ["DTO/DTO.csproj", "DTO/"]
COPY ["Messages/Messages.csproj", "Messages/"]

# Восстанавливаем зависимости
RUN dotnet restore "ParticipantService/ParticipantService.csproj"

# Копируем остальные файлы
COPY . .

# Сборка проекта
WORKDIR "/src/ParticipantService"
RUN dotnet build "ParticipantService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап 2: Публикация
FROM build AS publish
RUN dotnet publish "ParticipantService.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Этап 3: Выполнение
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8081
ENTRYPOINT ["dotnet", "ParticipantService.dll"]
