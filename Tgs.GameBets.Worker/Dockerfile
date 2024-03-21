#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameBets.Worker/Tgs.GameBets.Worker.csproj", "Tgs.GameBets.Worker/"]
COPY ["Tgs.GameBets.Worker.Common/Tgs.GameBets.Worker.Common.csproj", "Tgs.GameBets.Worker.Common/"]
COPY ["Tgs.GameBets.Worker.Core/Tgs.GameBets.Worker.Core.csproj", "Tgs.GameBets.Worker.Core/"]
COPY ["Tgs.GameBets.Worker.Repositories/Tgs.GameBets.Worker.Repositories.csproj", "Tgs.GameBets.Worker.Repositories/"]
RUN dotnet restore "./Tgs.GameBets.Worker/./Tgs.GameBets.Worker.csproj"
COPY . .
WORKDIR "/src/Tgs.GameBets.Worker"
RUN dotnet build "./Tgs.GameBets.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameBets.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameBets.Worker.dll"]