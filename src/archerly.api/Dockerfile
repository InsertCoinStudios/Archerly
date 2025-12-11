# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Accept build configuration as a build argument (default Release)
ARG CONFIGURATION=Release

# Copy csproj and restore dependencies
COPY ["src/archerly.api/archerly.api.csproj", "./"]
RUN dotnet restore

# Copy the source code
COPY ["src/archerly.api/", "./"]

# Build and publish
RUN dotnet publish -c $CONFIGURATION -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Run the API
ENTRYPOINT ["dotnet", "archerly.api.dll"]
