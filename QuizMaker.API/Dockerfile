FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
# Optional: If your app requires non-root user, you can add USER $APP_UID here

# --------------------------------------------
# Build plugin
# --------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-plugins
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["QuizMaker.Plugins.CsvExport/QuizMaker.Plugins.CsvExport.csproj", "QuizMaker.Plugins.CsvExport/"]
RUN dotnet restore "QuizMaker.Plugins.CsvExport/QuizMaker.Plugins.CsvExport.csproj"

COPY . .
WORKDIR "/src/QuizMaker.Plugins.CsvExport"
RUN dotnet publish "QuizMaker.Plugins.CsvExport.csproj" -c $BUILD_CONFIGURATION -o /app/plugins /p:UseAppHost=false

# --------------------------------------------
# Build main app
# --------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["QuizMaker.API/QuizMaker.API.csproj", "QuizMaker.API/"]
COPY ["QuizMaker.Shared/QuizMaker.Shared.csproj", "QuizMaker.Shared/"]
COPY ["QuizMaker.Domain/QuizMaker.Domain.csproj", "QuizMaker.Domain/"]
COPY ["QuizMaker.Database/QuizMaker.Database.csproj", "QuizMaker.Database/"]
RUN dotnet restore "QuizMaker.API/QuizMaker.API.csproj"

COPY . .
WORKDIR "/src/QuizMaker.API"
RUN dotnet publish "QuizMaker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --------------------------------------------
# Final image
# --------------------------------------------
FROM base AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .
COPY --from=build-plugins /app/plugins .

ENTRYPOINT ["dotnet", "QuizMaker.API.dll"]