Just a small example to show that https://github.com/fsprojects/SQLProvider seems to work with pure .net core 3.1 without mono (macOS).

* Apply the database schema: mysql test < people.sql
* Adjust the connection string
* dotnet restore
* dotnet build
* dotnet run
