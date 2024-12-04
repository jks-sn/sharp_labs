# HRManagerService/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY HRDirectorService/*.csproj ./HRDirectorService/
COPY HRManagerService/*.csproj ./HRManagerService/
COPY TeamleadService/*.csproj ./TeamleadService/
COPY JuniorService/*.csproj ./JuniorService/
COPY ParticipantService/*.csproj ./ParticipantService/
COPY Entities/*.csproj ./Entities/
COPY DTO/*.csproj ./DTO/
COPY Repositories/*.csproj ./Repositories/
COPY Shared/*.csproj ./Shared/

RUN dotnet restore ./HRManagerService/HRManagerService.csproj

COPY . .

WORKDIR /app/HRManagerService
RUN dotnet publish HRManagerService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HRManagerService.dll"]
