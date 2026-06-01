# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["api clase.csproj", "./"]
RUN dotnet restore "api clase.csproj"

# Copiar código fuente (aa2-frontend se excluye via .dockerignore)
COPY . .
RUN dotnet publish "api clase.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

# Puerto del contenedor de la API: 7859 (4 últimas cifras de usuario)
EXPOSE 7859
ENV ASPNETCORE_URLS=http://+:7859
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD curl -f http://localhost:7859/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "api clase.dll"]
