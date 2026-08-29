FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["MarketplaceSystem.sln", "MarketplaceSystem/"]

COPY ["MarketplaceSystem.API/MarketplaceSystem.API.csproj", "MarketplaceSystem/MarketplaceSystem.API/"]
COPY ["MarketplaceSystem.Application/MarketplaceSystem.Application.csproj", "MarketplaceSystem/MarketplaceSystem.Application/"]
COPY ["MarketplaceSystem.Infrastructure/MarketplaceSystem.Infrastructure.csproj", "MarketplaceSystem/MarketplaceSystem.Infrastructure/"]
COPY ["MarketplaceSystem.Common/MarketplaceSystem.Common.csproj", "MarketplaceSystem/MarketplaceSystem.Common/"]
COPY ["MarketplaceSystem.Domain/MarketplaceSystem.Domain.csproj", "MarketplaceSystem/MarketplaceSystem.Domain/"]

RUN dotnet restore "MarketplaceSystem/MarketplaceSystem.API/MarketplaceSystem.API.csproj"

COPY ["MarketplaceSystem.API/", "MarketplaceSystem/MarketplaceSystem.API/"]
COPY ["MarketplaceSystem.Application/", "MarketplaceSystem/MarketplaceSystem.Application/"]
COPY ["MarketplaceSystem.Infrastructure/", "MarketplaceSystem/MarketplaceSystem.Infrastructure/"]
COPY ["MarketplaceSystem.Common/", "MarketplaceSystem/MarketplaceSystem.Common/"]
COPY ["MarketplaceSystem.Domain/", "MarketplaceSystem/MarketplaceSystem.Domain/"]

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