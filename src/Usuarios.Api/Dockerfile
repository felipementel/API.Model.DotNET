# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Usuarios.Api/Usuarios.Api.csproj", "src/Usuarios.Api/"]
COPY ["src/Usuarios.Adapters/Usuarios.Adapters.csproj", "src/Usuarios.Adapters/"]
COPY ["src/Usuarios.Application/Usuarios.Application.csproj", "src/Usuarios.Application/"]
COPY ["src/Usuarios.Domain/Usuarios.Domain.csproj", "src/Usuarios.Domain/"]
RUN dotnet restore "./src/Usuarios.Api/Usuarios.Api.csproj"
COPY . .
WORKDIR "/src/src/Usuarios.Api"
RUN dotnet build "./Usuarios.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Usuarios.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Usuarios.Api.dll"]