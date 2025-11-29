 
# =========================
# STAGE 1: Build application
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution & restore dependencies
COPY ["TravelBuddyAPI.sln", "./"]
COPY ["TravelBuddyAPI/TravelBuddyAPI.csproj", "TravelBuddyAPI/"]
COPY ["BusinessObject/BusinessObject.csproj", "BusinessObject/"]
COPY ["DataAccessObject/DataAccessObject.csproj", "DataAccessObject/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Services/Services.csproj", "Services/"]

RUN dotnet restore "TravelBuddyAPI.sln"

# Copy all source files and build
COPY . .
RUN dotnet publish "TravelBuddyAPI/TravelBuddyAPI.csproj" -c Release -o /app/publish

# =========================
# STAGE 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TravelBuddyAPI.dll"]