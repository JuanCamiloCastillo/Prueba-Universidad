FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/StudentEnrollment.Domain/StudentEnrollment.Domain.csproj             backend/StudentEnrollment.Domain/
COPY backend/StudentEnrollment.Application/StudentEnrollment.Application.csproj   backend/StudentEnrollment.Application/
COPY backend/StudentEnrollment.Infrastructure/StudentEnrollment.Infrastructure.csproj backend/StudentEnrollment.Infrastructure/
COPY backend/StudentEnrollment.API/StudentEnrollment.API.csproj                   backend/StudentEnrollment.API/

RUN dotnet restore backend/StudentEnrollment.API/StudentEnrollment.API.csproj

COPY backend/ backend/

RUN dotnet publish backend/StudentEnrollment.API/StudentEnrollment.API.csproj \
    -c Release \
    -o /out \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "StudentEnrollment.API.dll"]
