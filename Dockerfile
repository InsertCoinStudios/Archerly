# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Accept build configuration as a build argument (default Release)
ARG CONFIGURATION=Release

# Copy the solution and all projects
COPY ["archerly.sln", "./"]
COPY ["src/", "src/"]
RUN dotnet restore

# Build and publish the API project
RUN dotnet publish "src/archerly.api/archerly.api.csproj" -c $CONFIGURATION -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Run the API
ENTRYPOINT ["dotnet", "archerly.api.dll"]
