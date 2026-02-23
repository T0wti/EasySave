FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 11000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "EasySave.LogServer.csproj"
RUN dotnet publish "EasySave.LogServer.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
VOLUME ["/app/logs"]
ENTRYPOINT ["dotnet", "EasySave.LogServer.dll"]