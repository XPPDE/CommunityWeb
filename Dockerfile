FROM ://microsoft.com AS build
WORKDIR /src
COPY ["CommunityWeb/CommunityWeb.csproj", "CommunityWeb/"]
COPY ["DbClassLibrary/DbClassLibrary.csproj", "DbClassLibrary/"]
RUN dotnet restore "CommunityWeb/CommunityWeb.csproj"
COPY . .
WORKDIR "/src/CommunityWeb"
RUN dotnet publish "CommunityWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CommunityWeb.dll"]
