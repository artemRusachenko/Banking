Along completing the task i was developed using Clean Architecture principles to ensure clear separation of concerns and maintainable code.
This separation allow to devide unit tests for different layers. For managing transactions i applied the Unit of Work pattern to streamline data operations and maintain consistency.



First, fill in the missing variables in the appsettings.json file with the necessary configuration for the SQL Server database.

In the root of the project, run the following command to create the initial migration for the database:
dotnet ef migrations add InitialCreate --project .\Banking.Infrastructure\Banking.Infrastructure.csproj --startup-project .\Banking.API\Banking.API.csproj

The migration will apply automatically when the project is started.
