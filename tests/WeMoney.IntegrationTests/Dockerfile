FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/WeMoney.IntegrationTests/WeMoney.IntegrationTests.csproj", "tests/WeMoney.IntegrationTests/"]
COPY ["src/WeMoney.Api/WeMoney.Api.csproj", "src/WeMoney.Api/"]
COPY ["src/WeMoney.Infrastructure/WeMoney.Infrastructure.csproj", "src/WeMoney.Infrastructure/"]
COPY ["src/WeMoney.Application/WeMoney.Application.csproj", "src/WeMoney.Application/"]
COPY ["src/WeMoney.Domain/WeMoney.Domain.csproj", "src/WeMoney.Domain/"]
RUN dotnet restore "tests/WeMoney.IntegrationTests/WeMoney.IntegrationTests.csproj"
COPY . .
WORKDIR "/src/tests/WeMoney.IntegrationTests"
RUN dotnet build "./WeMoney.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WeMoney.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeMoney.IntegrationTests.dll"]
