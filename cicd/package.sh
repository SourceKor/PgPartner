SOURCE_BRANCH_NAME=$1

if [[ ! "$1" =~ ^v[0-9]+\.[0-9]+\.[0-9]+(-[A-Za-z]+([0-9]+)?)?$ ]]; then
  echo "Invalid version $1"
  exit 1;
fi

version=`echo $1 | sed -e "s/^v//g"`

cd src/PgPartner
dotnet pack /p:PackageVersion=$version -o bin/Release