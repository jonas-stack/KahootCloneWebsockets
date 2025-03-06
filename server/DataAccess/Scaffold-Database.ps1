# Scaffold-Database.ps1

# Ensure the EF Core tools are installed
dotnet tool install --global dotnet-ef

# Run the scaffold command with the --force option to overwrite existing files
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=exampledb;Username=example;Password=example" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c KahootDbContext --force