#
# Builds the files that will be used for the package-wrap step.
#
param (
	[string] $version,
  [string] $sourceDir = "Package",
  [string] $destDir = "Output"
)

if ($version -eq "") {
  echo "Pass -version to supply a version number."
}
if ($version -ne "") { $version = "-$version" }

New-Item -ItemType Directory -Force -Path "$destDir"

Copy-Item "$sourceDir\BallBusterX_Desktop$version" -Destination "$destDir\BallBusterX_Windows$version"

"Package wrapping on Windows completed."
