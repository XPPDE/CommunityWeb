# Этап сборки — используем SDK 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["CommunityWeb/CommunityWeb.csproj", "CommunityWeb/"]
COPY ["DbClassLibrary/DbClassLibrary.csproj", "DbClassLibrary/"]

RUN dotnet restore "CommunityWeb/CommunityWeb.csproj"

COPY . .
WORKDIR "/src/CommunityWeb"
RUN dotnet publish "CommunityWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный этап — используем ASP.NET Runtime 10.0
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CommunityWeb.dll"]