# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только файлы проектов для кэширования restore
COPY ["CommunityWeb/CommunityWeb.csproj", "CommunityWeb/"]
COPY ["DbClassLibrary/DbClassLibrary.csproj", "DbClassLibrary/"]

# Восстанавливаем зависимости
RUN dotnet restore "CommunityWeb/CommunityWeb.csproj"

# Копируем всё остальное и публикуем
COPY . .
WORKDIR "/src/CommunityWeb"
RUN dotnet publish "CommunityWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Настройка порта и переменных окружения
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CommunityWeb.dll"]