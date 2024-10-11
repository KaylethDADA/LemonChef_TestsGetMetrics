# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/RecipeService/LemonChefApi/LemonChefApi.csproj", "src/Services/RecipeService/LemonChefApi/"]
COPY ["src/Services/RecipeService/Infrastructure/Infrastructure.csproj", "src/Services/RecipeService/Infrastructure/"]
COPY ["src/Services/RecipeService/Application/Application.csproj", "src/Services/RecipeService/Application/"]
COPY ["src/Services/RecipeService/Domain/Domain.csproj", "src/Services/RecipeService/Domain/"]
RUN dotnet restore "./src/Services/RecipeService/LemonChefApi/LemonChefApi.csproj"
COPY . .
WORKDIR "/src/src/Services/RecipeService/LemonChefApi"
RUN dotnet build "./LemonChefApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LemonChefApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LemonChefApi.dll"]