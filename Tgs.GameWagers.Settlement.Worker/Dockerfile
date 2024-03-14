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
COPY ["Tgs.GameWagers.Settlement.Worker/Tgs.GameWagers.Settlement.Worker.csproj", "Tgs.GameWagers.Settlement.Worker/"]
COPY ["Tgs.GameWagers.Settlement.Worker.Common/Tgs.GameWagers.Settlement.Worker.Common.csproj", "Tgs.GameWagers.Settlement.Worker.Common/"]
COPY ["Tgs.GameWagers.Settlement.Worker.Core/Tgs.GameWagers.Settlement.Worker.Core.csproj", "Tgs.GameWagers.Settlement.Worker.Core/"]
COPY ["Tgs.GameWagers.Settlement.Worker.Repositories/Tgs.GameWagers.Settlement.Worker.Repositories.csproj", "Tgs.GameWagers.Settlement.Worker.Repositories/"]
RUN dotnet restore "./Tgs.GameWagers.Settlement.Worker/./Tgs.GameWagers.Settlement.Worker.csproj"
COPY . .
WORKDIR "/src/Tgs.GameWagers.Settlement.Worker"
RUN dotnet build "./Tgs.GameWagers.Settlement.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameWagers.Settlement.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameWagers.Settlement.Worker.dll"]