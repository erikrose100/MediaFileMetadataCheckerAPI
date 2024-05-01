FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ARG METADATA_API_CONFIG_CONNECTION_STRING
ARG METADATA_API_CONFIG_CACHE_EXPIRATION
WORKDIR /App
COPY --from=build-env /App/out .
# Install ffmpeg/ffprobe dependencies
RUN apt-get update \
    && apt-get install -y ffmpeg libgdiplus \
    && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "MediaFileMetadataCheckerAPI.dll"]