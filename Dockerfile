# Multi-stage Dockerfile for ASP.NET Core 8 Web API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first to cache NuGet restore layer
COPY ["CollegeManagement.sln", "./"]
COPY ["CollegeManagement.API/CollegeManagement.API.csproj", "CollegeManagement.API/"]

RUN dotnet restore "CollegeManagement.API/CollegeManagement.API.csproj"

# Copy remaining source files
COPY . .

WORKDIR "/src/CollegeManagement.API"
RUN dotnet build "CollegeManagement.API.csproj" -c Release -o /app/build
RUN dotnet publish "CollegeManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Production runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Default environment configuration
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose container port (Render supplies PORT dynamically at runtime)
EXPOSE 10000

ENTRYPOINT ["dotnet", "CollegeManagement.API.dll"]
