#!/bin/bash

source ./configure.sh

./build.sh --target=CommonTarget

for NUGETPACKAGE in $(ls ./artifacts/Common/*.nupkg)
do
    echo "Pushing ${NUGETPACKAGE} to ${NUGET_SOURCE}"
    #dotnet nuget push ${NUGETPACKAGE} -s ${NUGET_SOURCE}
    echo "Nuget push is not yet implemented: See OKTA-316050"
done
