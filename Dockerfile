# Этап сборки (СДК .NET 8)
FROM ://microsoft.com AS build
WORKDIR /src

# 1. Копируем файлы проектов для восстановления зависимостей
COPY ["CommunityWeb/CommunityWeb.csproj", "CommunityWeb/"]
COPY ["DbClassLibrary/DbClassLibrary.csproj", "DbClassLibrary/"]

# 2. Восстанавливаем пакеты
RUN dotnet restore "CommunityWeb/CommunityWeb.csproj"

# 3. Копируем абсолютно все остальные исходные файлы
COPY . .

# 4. Собираем и публикуем веб-приложение
WORKDIR "/src/CommunityWeb"
RUN dotnet publish "CommunityWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап запуска приложения
FROM ://microsoft.com AS final
WORKDIR /app
COPY --from=build /app/publish .

# Настройка порта под Render
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CommunityWeb.dll"]
