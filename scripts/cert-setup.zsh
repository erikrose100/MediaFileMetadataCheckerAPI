#!/usr/bin/env zsh
# doc: https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md
cd ../MediaFileMetadataCheckerAPI

dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p $pass
dotnet dev-certs https --trust

dotnet user-secrets init -p MediaFileMetadataCheckerAPI.csproj
dotnet user-secrets -p MediaFileMetadataCheckerAPI.csproj set "Kestrel:Certificates:Development:Password" $pass