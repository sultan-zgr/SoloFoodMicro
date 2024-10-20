# OrderService Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderService.sln", "./"]
COPY ["OrderService.Api/", "OrderService.Api/"]
RUN dotnet restore "OrderService.sln"
RUN dotnet build "OrderService.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]
