# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
WORKDIR /app
EXPOSE 80

# Build image with SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy project files and restore
COPY TailwindMauiBlazorApp.Web/*.csproj TailwindMauiBlazorApp.Web/
COPY TailwindMauiBlazorApp.Shared/*.csproj TailwindMauiBlazorApp.Shared/
RUN dotnet restore TailwindMauiBlazorApp.Web/TailwindMauiBlazorApp.Web.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish TailwindMauiBlazorApp.Web/TailwindMauiBlazorApp.Web.csproj -c Release -o /app/publish

# Runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TailwindMauiBlazorApp.Web.dll"]