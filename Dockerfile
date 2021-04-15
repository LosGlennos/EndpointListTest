FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
ENV BuildingDocker true
COPY ["EndpointListTest/EndpointListTest.csproj", "EndpointListTest/"]
RUN dotnet restore "EndpointListTest/EndpointListTest.csproj"
COPY . .

WORKDIR "/src/EndpointListTest"
RUN dotnet build "EndpointListTest.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "EndpointListTest.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EndpointListTest.dll"]