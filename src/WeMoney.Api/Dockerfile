FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/WeMoney.Api/WeMoney.Api.csproj", "src/WeMoney.Api/"]
COPY ["src/WeMoney.Infrastructure/WeMoney.Infrastructure.csproj", "src/WeMoney.Infrastructure/"]
COPY ["src/WeMoney.Application/WeMoney.Application.csproj", "src/WeMoney.Application/"]
COPY ["src/WeMoney.Domain/WeMoney.Domain.csproj", "src/WeMoney.Domain/"]
RUN dotnet restore "src/WeMoney.Api/WeMoney.Api.csproj"
COPY . .
WORKDIR "/src/src/WeMoney.Api"
RUN dotnet build "./WeMoney.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WeMoney.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeMoney.Api.dll"]
