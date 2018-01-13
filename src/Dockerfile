## :: BUILD ::
FROM microsoft/aspnetcore-build:1.1.6-1.1.7-jessie AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

WORKDIR /app/apiserver
RUN dotnet publish -o ./publish

## :: Runtime Image ::
FROM microsoft/aspnetcore:1.1
ARG API_PORT

WORKDIR /app
COPY --from=build-env /app/apiserver/settings/app-settings /app/settings/app-settings 
COPY --from=build-env /app/apiserver/settings/logger-settings /app/settings/logger-settings
COPY --from=build-env /app/apiserver/publish /app/dist

EXPOSE ${API_PORT}

ENTRYPOINT ["dotnet", "./dist/MongoCore.WebApi.dll"]
