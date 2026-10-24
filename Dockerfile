FROM docker.io/library/ubuntu:latest AS build
RUN apt-get update && apt-get install -y wget apt-transport-https chown && \
    wget https://microsoft.com -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && rm packages-microsoft-prod.deb && \
    apt-get update && apt-get install -y dotnet-sdk-8.0
WORKDIR /src
COPY ["CommunityWeb/CommunityWeb.csproj", "CommunityWeb/"]
COPY ["DbClassLibrary/DbClassLibrary.csproj", "DbClassLibrary/"]
RUN dotnet restore "CommunityWeb/CommunityWeb.csproj"
COPY . .
WORKDIR "/src/CommunityWeb"
RUN dotnet publish "CommunityWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM docker.io/library/ubuntu:latest AS final
RUN apt-get update && apt-get install -y wget apt-transport-https && \
    wget https://microsoft.com -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && rm packages-microsoft-prod.deb && \
    apt-get update && apt-get install -y dotnet-runtime-8.0 aspnetcore-runtime-8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CommunityWeb.dll"]
