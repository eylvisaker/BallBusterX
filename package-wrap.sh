#!/bin/bash

version=$1
sourceDir=$2
destDir=$3

projectName=BallBusterX
projectRoot=`pwd`
tmpRoot=tmp
tmpDir="$tmpRoot/$projectName"

if [[ $version != "" ]]; then
  $version="-$version"
fi

mkdir -p $destDir

mkdir -p $tmpDir/lib

unzip "$sourceDir/$projectName(version).zip" -d "$tmpDir/lib"

cp Linux/* $tmpDir


cd $tmpRoot

tar zcvf $projectName $projectRoot/$destDir/$projectName.tar.gz
