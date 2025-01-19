FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MdWebApplication/MdWebApplication.csproj", "MdWebApplication/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Markdown/Markdown/Markdown/Markdown.csproj", "Markdown/Markdown/Markdown/"]
RUN dotnet restore "MdWebApplication/MdWebApplication.csproj"
COPY . .
WORKDIR "/src/MdWebApplication"
RUN dotnet build "MdWebApplication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MdWebApplication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MdWebApplication.dll"]
