#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1.2-alpine3.11 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Pallet Builder Service/Pallet Builder Service.csproj", "Pallet Builder Service/"]
RUN dotnet restore "Pallet Builder Service/Pallet Builder Service.csproj"
COPY . .
WORKDIR "/src/Pallet Builder Service"
RUN dotnet build "Pallet Builder Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pallet Builder Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PalletBuilderService.dll"]