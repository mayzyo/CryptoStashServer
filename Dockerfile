#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
ENV PGSQLCONNSTR_StatsDb=""
ENV ALLOWED_ORIGINS=""

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["CryptoStashServer.csproj", "."]
RUN dotnet restore "./CryptoStashServer.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CryptoStashServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoStashServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoStashServer.dll"]
