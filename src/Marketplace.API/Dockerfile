FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Marketplace.Domain/Marketplace.Domain.csproj", "src/Marketplace.Domain/"]
COPY ["src/Marketplace.Application/Marketplace.Application.csproj", "src/Marketplace.Application/"]
COPY ["src/Marketplace.Infrastructure/Marketplace.Infrastructure.csproj", "src/Marketplace.Infrastructure/"]
COPY ["src/Marketplace.API/Marketplace.API.csproj", "src/Marketplace.API/"]
RUN dotnet restore "src/Marketplace.API/Marketplace.API.csproj"

COPY . .
WORKDIR "/src/src/Marketplace.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.API.dll"]
