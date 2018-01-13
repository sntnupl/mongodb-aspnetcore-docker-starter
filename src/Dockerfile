FROM microsoft/aspnetcore-build:1.1.6-1.1.7-jessie AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

WORKDIR /app/apiserver
RUN dotnet publish -o ./publish

EXPOSE 5000

ENTRYPOINT ["dotnet", "./publish/MongoCore.WebApi.dll"]
