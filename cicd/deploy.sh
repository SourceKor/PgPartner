NUGET_KEY=$1

cd src/PgPartner/bin/Release
nugetFilename=$(ls -a | grep '.nupkg$')

dotnet nuget push $nugetFilename --api-key $NUGET_KEY --source https://api.nuget.org/v3/index.json