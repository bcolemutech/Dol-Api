FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build_env

ARG package_token

RUN echo ${package_token}

COPY . /app
WORKDIR /app

RUN dotnet nuget add source https://nuget.pkg.github.com/bcolemutech/index.json -n github -u bcolemutech -p ${package_token} --store-password-in-clear-text

RUN dotnet test
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build_env /app/out .
ENTRYPOINT ["dotnet", "DolApi.dll"]