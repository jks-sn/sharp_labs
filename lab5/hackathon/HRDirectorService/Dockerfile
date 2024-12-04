# HRDirectorService/Dockerfile

# Используем SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проектов и выполняем dotnet restore
COPY HRDirectorService/HRDirectorService.csproj ./HRDirectorService/
COPY Entities/Entities.csproj ./Entities/
COPY Message/Message.csproj ./Message/
COPY Repositories/Repositories.csproj ./Repositories/
COPY Shared/Shared.csproj ./Shared/
COPY DTO/DTO.csproj ./DTO/

RUN dotnet restore ./HRDirectorService/HRDirectorService.csproj

# Копируем остальной код
COPY . .

# Сборка и публикация приложения
WORKDIR /app/HRDirectorService
RUN dotnet publish HRDirectorService.csproj -c Release -o /app/publish

RUN dotnet --list-runtimes
# Создаем финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HRDirectorService.dll"]
