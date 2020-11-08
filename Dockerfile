FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

COPY . /app
WORKDIR /app

RUN dotnet nuget add source https://nuget.pkg.github.com/bcolemutech/index.json -n github

RUN dotnet test
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "DolApi.dll"]