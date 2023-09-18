FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 8002

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Avalanche.Vault.Server/Avalanche.Vault.Server.csproj", "Avalanche.Vault.Server/"]
COPY ["Avalanche.Application.Grpc/Avalanche.Application.Grpc.csproj", "Avalanche.Application.Grpc/"]
COPY ["Avalanche.Application/Avalanche.Application.csproj", "Avalanche.Application/"]
COPY ["Avalanche.Exceptions/Avalanche.Exceptions.csproj", "Avalanche.Exceptions/"]
COPY ["Avalanche.Identity.Client/Avalanche.Identity.Client.csproj", "Avalanche.Identity.Client/"]
COPY ["Avalanche.Vault.Application/Avalanche.Vault.Application.csproj", "Avalanche.Vault.Application/"]
COPY ["Avalanche.Merchant.Events/Avalanche.Merchant.Events.csproj", "Avalanche.Merchant.Events/"]
COPY ["Avalanche.Auth.Events/Avalanche.Auth.Events.csproj", "Avalanche.Auth.Events/"]
COPY ["Avalanche.Vault.Server.Protos/Avalanche.Vault.Server.Protos.csproj", "Avalanche.Vault.Server.Protos/"]
RUN dotnet restore "Avalanche.Vault.Server/Avalanche.Vault.Server.csproj"
COPY . .
WORKDIR "/src/Avalanche.Vault.Server"
RUN dotnet build "Avalanche.Vault.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Avalanche.Vault.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Avalanche.Vault.Server.dll"]
