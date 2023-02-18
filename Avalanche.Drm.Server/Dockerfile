FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 9000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Avalanche.Drm.Server/Avalanche.Drm.Server.csproj", "Avalanche.Drm.Server/"]
COPY ["Avalanche.Application.Grpc/Avalanche.Application.Grpc.csproj", "Avalanche.Application.Grpc/"]
COPY ["Avalanche.Application/Avalanche.Application.csproj", "Avalanche.Application/"]
COPY ["Avalanche.Exceptions/Avalanche.Exceptions.csproj", "Avalanche.Exceptions/"]
COPY ["Avalanche.Drm.Application/Avalanche.Drm.Application.csproj", "Avalanche.Drm.Application/"]
COPY ["Avalanche.Auth.Events/Avalanche.Auth.Events.csproj", "Avalanche.Auth.Events/"]
COPY ["Avalanche.Drm.Proto/Avalanche.Drm.Proto.csproj", "Avalanche.Drm.Proto/"]
COPY ["Avalanche.Identity.Client/Avalanche.Identity.Client.csproj", "Avalanche.Identity.Client/"]
RUN dotnet restore "Avalanche.Drm.Server/Avalanche.Drm.Server.csproj"
COPY . .
WORKDIR "/src/Avalanche.Drm.Server"
RUN dotnet build "Avalanche.Drm.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Avalanche.Drm.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Avalanche.Drm.Server.dll"]
