#!/usr/bin/env zsh
# doc: https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md
docker run --rm -it -p 8000:80 -p 8001:443 \ 
    -e ASPNETCORE_URLS="https://+;http://+" \
    -e ASPNETCORE_HTTPS_PORTS=8001 \
    -e ASPNETCORE_Kestrel__Certificates__Default__Password=$pass \
    -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -e METADATA_API_CONFIG_CACHE_EXPIRATION=1 \
    -e METADATA_API_CONFIG_CONNECTION_STRING=$connectionString \
    -v ${HOME}/.aspnet/https:/https/ mediametadata