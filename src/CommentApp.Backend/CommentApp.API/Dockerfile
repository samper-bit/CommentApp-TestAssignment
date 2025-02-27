# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update; apt-get install -y fontconfig fonts-liberation
RUN fc-cache -f -v

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CommentApp.Backend/CommentApp.API/CommentApp.API.csproj", "src/CommentApp.Backend/CommentApp.API/"]
COPY ["src/CommentApp.Backend/CommentApp.Application/CommentApp.Application.csproj", "src/CommentApp.Backend/CommentApp.Application/"]
COPY ["src/CommentApp.Backend/CommentApp.Domain/CommentApp.Domain.csproj", "src/CommentApp.Backend/CommentApp.Domain/"]
COPY ["src/CommentApp.Backend/CommentApp.Shared/CommentApp.Shared.csproj", "src/CommentApp.Backend/CommentApp.Shared/"]
COPY ["src/CommentApp.Backend/CommentApp.Infrastructure/CommentApp.Infrastructure.csproj", "src/CommentApp.Backend/CommentApp.Infrastructure/"]
RUN dotnet restore "./src/CommentApp.Backend/CommentApp.API/CommentApp.API.csproj"
COPY . .
WORKDIR "/src/src/CommentApp.Backend/CommentApp.API"
RUN dotnet build "./CommentApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommentApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommentApp.API.dll"]