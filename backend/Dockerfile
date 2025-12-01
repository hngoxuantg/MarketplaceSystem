FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["src/MarketplaceSystem/MarketplaceSystem.sln", "MarketplaceSystem/"]

COPY ["src/MarketplaceSystem/MarketplaceSystem.API/MarketplaceSystem.API.csproj", "MarketplaceSystem/MarketplaceSystem.API/"]
COPY ["src/MarketplaceSystem/MarketplaceSystem.Application/MarketplaceSystem.Application.csproj", "MarketplaceSystem/MarketplaceSystem.Application/"]
COPY ["src/MarketplaceSystem/MarketplaceSystem.Infrastructure/MarketplaceSystem.Infrastructure.csproj", "MarketplaceSystem/MarketplaceSystem.Infrastructure/"]
COPY ["src/MarketplaceSystem/MarketplaceSystem.Common/MarketplaceSystem.Common.csproj", "MarketplaceSystem/MarketplaceSystem.Common/"]
COPY ["src/MarketplaceSystem/MarketplaceSystem.Domain/MarketplaceSystem.Domain.csproj", "MarketplaceSystem/MarketplaceSystem.Domain/"]

RUN dotnet restore "MarketplaceSystem/MarketplaceSystem.API/MarketplaceSystem.API.csproj"

COPY ["src/MarketplaceSystem/", "MarketplaceSystem/"]

WORKDIR "/src/MarketplaceSystem/MarketplaceSystem.API"
RUN dotnet publish "MarketplaceSystem.API.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/uploads

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MarketplaceSystem.API.dll"]