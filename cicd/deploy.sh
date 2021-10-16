NUGET_KEY=$1

cd Release
nugetFilename=$(ls -a | grep -E '[0-9]+\.[0-9]+\.[0-9]+(-[A-Za-z]+([0-9]+)?)?.nupkg$')

dotnet nuget push $nugetFilename --api-key $NUGET_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate