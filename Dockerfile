# ---------- Etapa 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Syspharma.API/Syspharma.API.csproj Syspharma.API/
COPY Syspharma.Business/Syspharma.Business.csproj Syspharma.Business/
COPY Syspharma.Data/Syspharma.Data.csproj Syspharma.Data/
COPY Syspharma.Domain/Syspharma.Domain.csproj Syspharma.Domain/
RUN dotnet restore Syspharma.API/Syspharma.API.csproj
COPY . .
RUN dotnet publish Syspharma.API/Syspharma.API.csproj -c Release -o /app/publish --no-restore

# ---------- Etapa 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Syspharma.API.dll"]