param (
	[string] $version,
  [string] $destDir = "Output"
)

if ($version -ne "") { $version = "-$version" }

function CreatePackage($sourceDir, $packageName)
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

Copy-Item "BallBusterX.Desktop\bin\DesktopGL\AnyCPU\Release" -Destination "temp-package\Windows" -Recurse
Copy-Item "BallBusterX.Desktop\bin\DesktopGL\AnyCPU\Release" -Destination "temp-package\Linux\bin" -Recurse
Copy-Item "Linux\*" -Destination "temp-package\Linux"

CreatePackage "temp-package\Windows" "BallBusterX_Windows$version"
CreatePackage "temp-package\Linux" "BallBusterX_Linux$version"

"Packaging complete."
