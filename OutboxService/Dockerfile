FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OutboxService/OutboxService.csproj", "OutboxService/"]
RUN dotnet restore "OutboxService/OutboxService.csproj"
COPY . .
WORKDIR "/src/OutboxService"
RUN dotnet build "OutboxService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OutboxService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OutboxService.dll"]