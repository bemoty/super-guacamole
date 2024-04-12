FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Super.Guacamole.Web/Super.Guacamole.Web.csproj", "Super.Guacamole.Web/"]
COPY ["Super.Guacamole.Common/Super.Guacamole.Common.csproj", "Super.Guacamole.Common/"]
COPY ["Super.Guacamole.Image/Super.Guacamole.Image.csproj", "Super.Guacamole.Image/"]
RUN dotnet restore "Super.Guacamole.Web/Super.Guacamole.Web.csproj"
COPY . .
WORKDIR "/src/Super.Guacamole.Web"
RUN dotnet build "Super.Guacamole.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Super.Guacamole.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Super.Guacamole.Web.dll"]
