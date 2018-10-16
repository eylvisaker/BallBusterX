#
# Builds the files that will be used for the package-wrap step.
#
param (
	[string] $version,
  [string] $destDir = "Output"
)

if ($version -eq "") {
  echo "Pass -version to supply a version number."
}

if ($version -ne "") { $version = "-$version" }

function CreateZipFile($sourceDir, $packageName)
{
	$destination = "$($destDir)\$($packageName).zip"
	
	if (Test-Path $destination) { Remove-Item $destination }
	
	"Creating $destination"
	[IO.Compression.ZipFile]::CreateFromDirectory($sourceDir, $destination)
}

Add-Type -assembly "System.IO.Compression.FileSystem"

$dummy = New-Item -ItemType Directory -Force -Path $destDir

.\Build.ps1 -config Release

New-Item -ItemType Directory -Force -Path "temp-package"

Copy-Item "BallBusterX.Desktop\bin\DesktopGL\AnyCPU\Release" -Destination "temp-package\Desktop" -Recurse

CreateZipFile "temp-package\Windows" "BallBusterX_Desktop$version"

"Packaging complete."
