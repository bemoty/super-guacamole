FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["super-guacamole-common/super-guacamole-common.csproj", "super-guacamole-common/"]
RUN dotnet restore "super-guacamole-common/super-guacamole-common.csproj"
COPY . .
WORKDIR "/src/super-guacamole-common"
RUN dotnet build "super-guacamole-common.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "super-guacamole-common.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "super-guacamole-common.dll"]
