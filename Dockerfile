# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["api clase.csproj", "./"]
RUN dotnet restore "api clase.csproj"

# Copiar todo excepto la carpeta del frontend (no es parte de la API)
COPY --exclude=aa2-frontend . .
RUN dotnet publish "api clase.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

# Puerto del contenedor de la API: 7959 (4 últimas cifras de usuario)
EXPOSE 7959
ENV ASPNETCORE_URLS=http://+:7959
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD curl -f http://localhost:7959/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "api clase.dll"]
