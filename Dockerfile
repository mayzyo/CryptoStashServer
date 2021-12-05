#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
ENV PGSQLCONNSTR_StatsDb=""
ENV ALLOWED_ORIGINS=""

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["CryptoStashStats.csproj", "."]
RUN dotnet restore "./CryptoStashStats.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CryptoStashStats.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoStashStats.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoStashStats.dll"]
