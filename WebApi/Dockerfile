FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
LABEL description="Minimal API"
LABEL organisation="GBS St. Gallen"
LABEL author="Sullivan Renner"
WORKDIR /app
COPY --from=build-env /build/out .
ENV ASPNETCORE_URLS=http://*:5001
EXPOSE 5001
ENTRYPOINT [ "dotnet", "WebApi.dll" ]