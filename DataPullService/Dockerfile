FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DataPullService/DataPullService.csproj", "DataPullService/"]
RUN dotnet restore "DataPullService/DataPullService.csproj"
COPY . .
WORKDIR "/src/DataPullService"
RUN dotnet build "DataPullService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataPullService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataPullService.dll"]