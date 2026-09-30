FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/WeMoney.UnitTests/WeMoney.UnitTests.csproj", "tests/WeMoney.UnitTests/"]
COPY ["src/WeMoney.Application/WeMoney.Application.csproj", "src/WeMoney.Application/"]
COPY ["src/WeMoney.Domain/WeMoney.Domain.csproj", "src/WeMoney.Domain/"]
COPY ["src/WeMoney.Web/WeMoney.Web.csproj", "src/WeMoney.Web/"]
RUN dotnet restore "tests/WeMoney.UnitTests/WeMoney.UnitTests.csproj"
COPY . .
WORKDIR "/src/tests/WeMoney.UnitTests"
RUN dotnet build "./WeMoney.UnitTests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WeMoney.UnitTests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeMoney.UnitTests.dll"]
