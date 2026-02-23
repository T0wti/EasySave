FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app
EXPOSE 11000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY EasySave.EasyLog/ ./EasySave.EasyLog/
COPY EasySave.LogServer/ ./EasySave.LogServer/

RUN dotnet restore "EasySave.LogServer/EasySave.LogServer.csproj"
RUN dotnet publish "EasySave.LogServer/EasySave.LogServer.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
VOLUME ["/app/logs"]
ENTRYPOINT ["dotnet", "EasySave.LogServer.dll"]