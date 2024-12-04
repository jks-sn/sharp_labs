# TeamleadService/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY TeamleadService/TeamleadService.csproj ./TeamleadService/
COPY Entities/Entities.csproj ./Entities/
COPY Shared/Shared.csproj ./Shared/
COPY DTO/DTO.csproj ./DTO/


# Восстанавливаем зависимости
RUN dotnet restore ./TeamleadService/TeamleadService.csproj

# Копируем остальные файлы проекта
COPY . .

# Сборка и публикация приложения
WORKDIR /app/TeamleadService
RUN dotnet publish TeamleadService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TeamleadService.dll"]
