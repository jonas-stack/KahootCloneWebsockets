# Scaffold-Database.ps1

# Ensure the EF Core tools are installed
dotnet tool install --global dotnet-ef

# Run the scaffold command
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=exampledb;Username=example;Password=example" Npgsql.EntityFrameworkCore.PostgreSQL -o Models