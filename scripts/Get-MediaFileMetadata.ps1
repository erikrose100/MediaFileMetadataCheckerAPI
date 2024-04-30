[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [string] $FilePath = "../resources/big_buck_bunny.mp4",

    [Parameter(Mandatory=$false)]
    [string] $URI =  "https://localhost:7230/FileUpload/UploadLargeFileForMetadata"
)

$Form = @{
    f = Get-Item -Path $File
}
$Response = (Invoke-WebRequest -Method Post -URI $URI -Form $Form).Content | ConvertFrom-Json
Write-Output $Response