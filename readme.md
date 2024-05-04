# MediaFileMetadataCheckerAPI
- [MediaFileMetadataCheckerAPI](#mediafilemetadatacheckerapi)
  - [Local testing (dotnet native)](#local-testing-dotnet-native)
  - [Local testing (docker)](#local-testing-docker)
  - [Dynamic App Configuration](#dynamic-app-configuration)
- [Terraform](#terraform)
  - [Running Terraform](#running-terraform)

Basic containerized MVC app written in C#. Infrastructure automation for cloud app configuration written in Terraform.

Takes an uploaded user file, runs FFProbe on it, and returns some of the metadata as a JSON payload.

App is for demonstration purposes - authn/authz + input validation would need to be set up for production usage.

App stores file in a local DB - ideally would use S3, Azure Blob, or some other object storage service in a production scenario.

## Local testing (dotnet native)

An Azure App config must exist before running the code, follow the "Terraform" instructions below to create a free app config for testing if one does not exist.

Run the following in zsh:
```zsh
export pass="<insert_string_here>"
```

Run `scripts/cert-setup.zsh` if on MacOS/Linux, Windows users can [follow instructions here](https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md) (make sure to follow the "Windows using Linux containers" steps as well).

Run the following in zsh:
```zsh
export connectionString="<app_config_connection_string>"
```

Run `dotnet build` and `dotnet run` in the API dir. 

Ctrl+click the localhost link and add `/swagger/index.html` to view the Swagger page. You can upload files and get a response interactively there.

Press ctrl+c in the terminal to stop the app.

## Local testing (docker)

Make sure to run cert setup from dotnet native instructions above and both `export` commands shown.

Make sure a docker daemon service is running (I use [Colima](https://github.com/abiosoft/colima) for this).

In the API dir, run the following:
```zsh
docker build -t mediametadata -f Dockerfile .
```

If the build is successful, run `scripts/docker-run.zsh` and use same steps above for testing the swagger page.

The docker ci workflow in this repo will build the container and run it for 30 seconds using `docker run` on each push.

## Dynamic App Configuration
This application uses [app config dynamic configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-aspnet-core) (aka hot reload) which is setup in the DI in Program.cs. 

The app will check the app config on a cadence determined by the `METADATA_API_CONFIG_CACHE_EXPIRATION` environment variable (its value sets the number of hours before the local app config cache is invalidated). It checks to see if the value of the `MetadataApp:Settings:Sentinel` key has changed, and if it has, it reaches out to update the other config values. The Sentinel key gets updated by the Terraform code any time any of the other config values are changed.

# Terraform

The Terraform in this repo creates the following:
 - An Azure App Configuration resource
 - Key-value configuration pairs in the app config

It then updates the "Sentinel" key in the app configuration so that any running applications know to get new copies of the config values (see the "Dynamic App Configuration section below for more details). It uses the "random" Terraform provider to generate an int between 1-50000 anytime any of the other app config key:value pairs are updated.

## Running Terraform

First create an Azure account if you do not have one yet (these instructions will only create services that are currently Always Free as of May 2024). Then run `az login` to authenticate into your azure tenant. Terraform will use the credentials setup by az login for resource creation.

From the `Terraform` dir, run `terraform init` and then `terraform validate` to setup terraform and make sure your config is syntactically valid. Then run `terraform plan` to see the changes applying the terraform will make. If the changes look as expected, you can run `terraform apply` to make them. You can run `terraform delete` from this same dir later if you wish to delete those resources.

The terraform CI workflow in this repo will run `terrafrom init` and `terraform validate` on each push if any files in the  `Terraform` dir are changd.
