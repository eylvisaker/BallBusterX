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

CreatePackage BallBusterX.Desktop\bin\DesktopGL\AnyCPU\Release "BallBusterX_Windows$version"

"Packaging complete."
