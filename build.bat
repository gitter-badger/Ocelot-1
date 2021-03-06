echo -------------------------

echo Running Ocelot.UnitTests

dotnet restore test/Ocelot.UnitTests/
dotnet test test/Ocelot.UnitTests/

echo Running Ocelot.AcceptanceTests

cd test/Ocelot.AcceptanceTests/
dotnet restore 
dotnet test 
cd ../../

echo Building Ocelot

dotnet restore src/Ocelot
dotnet restore src/Ocelot.Library
dotnet build src/Ocelot
dotnet publish src/Ocelot -o site/wwwroot


