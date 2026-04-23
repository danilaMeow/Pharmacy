FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/Pharmacy.Api/Pharmacy.Api.csproj src/Pharmacy.Api/
COPY src/Pharmacy.Client/Pharmacy.Client.csproj src/Pharmacy.Client/

# Восстанавливаем API напрямую
RUN dotnet restore src/Pharmacy.Api/Pharmacy.Api.csproj

COPY . .

WORKDIR /app/src/Pharmacy.Api
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Pharmacy.Api.dll"]