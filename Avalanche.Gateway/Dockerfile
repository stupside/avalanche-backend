FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 5000
EXPOSE 5001
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Avalanche.Gateway/Avalanche.Gateway.csproj", "Avalanche.Gateway/"]
RUN dotnet restore "Avalanche.Gateway/Avalanche.Gateway.csproj"
COPY . .
WORKDIR "/src/Avalanche.Gateway"
RUN dotnet build "Avalanche.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Avalanche.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Avalanche.Gateway.dll"]
