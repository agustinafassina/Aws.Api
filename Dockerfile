FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY Aws.Api.csproj .

RUN dotnet restore *.sln

COPY . .

WORKDIR /app
RUN dotnet publish -c Release -o /release

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=80
COPY --from=build /release ./
ENTRYPOINT ["dotnet", "Aws.Api.dll"]
