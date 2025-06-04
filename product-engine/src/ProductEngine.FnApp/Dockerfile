FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy solution and project files
COPY ["ProductEngine.sln", "./"]
COPY ["ProductEngine.FnApp/ProductEngine.FnApp.csproj", "ProductEngine.FnApp/"]
COPY ["ProductEngine.Domain/ProductEngine.Domain.csproj", "ProductEngine.Domain/"]
COPY ["ProductEngine.Infrastructure/ProductEngine.Infrastructure.csproj", "ProductEngine.Infrastructure/"]
COPY ["ProductEngine.Application/ProductEngine.Application.csproj", "ProductEngine.Application/"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish
RUN dotnet publish "ProductEngine.FnApp/ProductEngine.FnApp.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
WORKDIR /home/site/wwwroot
COPY --from=build /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true 