FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 8001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Avalanche.Merchant.Server/Avalanche.Merchant.Server.csproj", "Avalanche.Merchant.Server/"]
COPY ["Avalanche.Application.Grpc/Avalanche.Application.Grpc.csproj", "Avalanche.Application.Grpc/"]
COPY ["Avalanche.Application/Avalanche.Application.csproj", "Avalanche.Application/"]
COPY ["Avalanche.Exceptions/Avalanche.Exceptions.csproj", "Avalanche.Exceptions/"]
COPY ["Avalanche.Identity.Client/Avalanche.Identity.Client.csproj", "Avalanche.Identity.Client/"]
COPY ["Avalanche.Merchant.Application/Avalanche.Merchant.Application.csproj", "Avalanche.Merchant.Application/"]
COPY ["Avalanche.Merchant.Events/Avalanche.Merchant.Events.csproj", "Avalanche.Merchant.Events/"]
COPY ["Avalanche.Merchant.Server.Protos/Avalanche.Merchant.Server.Protos.csproj", "Avalanche.Merchant.Server.Protos/"]
RUN dotnet restore "Avalanche.Merchant.Server/Avalanche.Merchant.Server.csproj"
COPY . .
WORKDIR "/src/Avalanche.Merchant.Server"
RUN dotnet build "Avalanche.Merchant.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Avalanche.Merchant.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Avalanche.Merchant.Server.dll"]
