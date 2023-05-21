# Use the .NET 7 SDK as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FinancialDataApi/FinancialDataApi.csproj", "FinancialDataApi/"]
RUN dotnet restore "FinancialDataApi/FinancialDataApi.csproj"
COPY . .
WORKDIR "/src/FinancialDataApi"
RUN dotnet build "FinancialDataApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialDataApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "FinancialDataApi.dll"]