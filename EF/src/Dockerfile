# Prepare the base for the image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Copy the solution file and project files, then restore dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release
COPY ["PromoCodeFactory.Core/*.csproj", "PromoCodeFactory.Core/"]
COPY ["PromoCodeFactory.DataAccess/*.csproj", "PromoCodeFactory.DataAccess/"]
COPY ["PromoCodeFactory.WebHost/*.csproj", "PromoCodeFactory.WebHost/"]"
RUN dotnet restore "PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj"

# Copy the remaining files and publish
COPY . .
WORKDIR "/src/PromoCodeFactory.WebHost"
RUN dotnet publish "PromoCodeFactory.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish 

# Use the .NET 8 runtime for the final stage
FROM base AS release
WORKDIR /app
COPY --from=build /app/build ./
ENTRYPOINT ["dotnet", "PromoCodeFactory.WebHost.dll"]