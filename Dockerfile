# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./MyWebApp/MyWebApp.csproj" --disable-parallel
RUN dotnet publish "./MyWebApp/MyWebApp.csproj" -c release -o /app --no-restore

# Serve stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app ./

# RUN set ASPNETCORE_ENVIRONMENT=Production
RUN set IMAGESHARING_DB=Host=localhost;Port=44000;Database=TestDB;Username=postgres;Password=12345678

EXPOSE 5000

ENTRYPOINT ["dotnet", "MyWebApp.dll"]