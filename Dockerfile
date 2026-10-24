FROM docker.io/library/mono:latest AS build_env
FROM docker.io/library/ubuntu:latest AS runtime_env

FROM docker.io/microsoft/dotnet-samples:sdk-8.0 AS build
WORKDIR /src
COPY ["CommunityWeb/CommunityWeb.csproj", "CommunityWeb/"]
COPY ["DbClassLibrary/DbClassLibrary.csproj", "DbClassLibrary/"]
RUN dotnet restore "CommunityWeb/CommunityWeb.csproj"
COPY . .
WORKDIR "/src/CommunityWeb"
RUN dotnet publish "CommunityWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM docker.io/microsoft/dotnet-samples:aspnetapp-8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CommunityWeb.dll"]
