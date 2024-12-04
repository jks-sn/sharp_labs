# JuniorService/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проектов и выполняем dotnet restore
COPY JuniorService/JuniorService.csproj ./JuniorService/
COPY Entities/Entities.csproj ./Entities/
COPY Shared/Shared.csproj ./Shared/
COPY DTO/DTO.csproj ./DTO/
COPY Message/Message.csproj ./Message/

RUN dotnet restore ./JuniorService/JuniorService.csproj

# Копируем остальной код
COPY . .

# Сборка и публикация приложения
WORKDIR /app/JuniorService
RUN dotnet publish JuniorService.csproj -c Release -o /app/publish

# Создаем финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JuniorService.dll"]
