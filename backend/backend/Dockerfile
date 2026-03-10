# ── Etapa 1: compilación ──────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo los .csproj primero para aprovechar el caché de capas de Docker.
# Si el código fuente cambia pero los proyectos no, Docker reutiliza
# la capa de restore y no vuelve a bajar todos los paquetes NuGet.
COPY StudentEnrollment.Domain/StudentEnrollment.Domain.csproj             StudentEnrollment.Domain/
COPY StudentEnrollment.Application/StudentEnrollment.Application.csproj   StudentEnrollment.Application/
COPY StudentEnrollment.Infrastructure/StudentEnrollment.Infrastructure.csproj StudentEnrollment.Infrastructure/
COPY StudentEnrollment.API/StudentEnrollment.API.csproj                   StudentEnrollment.API/

RUN dotnet restore StudentEnrollment.API/StudentEnrollment.API.csproj

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish StudentEnrollment.API/StudentEnrollment.API.csproj \
    -c Release \
    -o /out \
    --no-restore

# ── Etapa 2: imagen final (solo runtime, sin SDK) ─────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "StudentEnrollment.API.dll"]
