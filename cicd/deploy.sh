NUGET_KEY=$1

ls -a
cd Release
ls -a
nugetFilename=$(ls -a | grep '.nupkg$')

dotnet nuget push $nugetFilename --api-key $NUGET_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate