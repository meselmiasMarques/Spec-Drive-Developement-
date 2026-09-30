FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/ src/
RUN dotnet publish src/WeMoney.Web/WeMoney.Web.csproj -c Release -o /app

FROM nginx:alpine
COPY docker/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/wwwroot /usr/share/nginx/html
EXPOSE 80
