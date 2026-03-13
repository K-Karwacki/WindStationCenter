# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all source files
COPY Src/ Src/

# Restore, build and publish in one step
RUN dotnet publish Src/Api/Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

# Ensure ASP.NET binds to 0.0.0.0:80 by default inside the container
ENV ASPNETCORE_URLS="http://0.0.0.0:8080"

ENTRYPOINT ["dotnet", "Api.dll"]
