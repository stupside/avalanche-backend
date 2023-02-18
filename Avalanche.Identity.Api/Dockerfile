FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 5678

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Avalanche.Identity.Api/Avalanche.Identity.Api.csproj", "Avalanche.Identity.Api/"]
COPY ["Avalanche.Exceptions/Avalanche.Exceptions.csproj", "Avalanche.Exceptions/"]
COPY ["Avalanche.Identity.Application/Avalanche.Identity.Application.csproj", "Avalanche.Identity.Application/"]
COPY ["Avalanche.Application/Avalanche.Application.csproj", "Avalanche.Application/"]
COPY ["Avalanche.Application.Http/Avalanche.Application.Http.csproj", "Avalanche.Application.Http/"]
RUN dotnet restore "Avalanche.Identity.Api/Avalanche.Identity.Api.csproj"
COPY . .
WORKDIR "/src/Avalanche.Identity.Api"
RUN dotnet build "Avalanche.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Avalanche.Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Avalanche.Identity.Api.dll"]
