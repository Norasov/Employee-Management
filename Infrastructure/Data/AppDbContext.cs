using System.Data;
using Npgsql;

namespace Infrastructure.Data;

public class AppDbContext
{
    private string connectionString = "Host=localhost;Port=5432;Database=EmployeeManagementDB;Username=postgres;Password=p000;";

    public IDbConnection Connection()
        => new NpgsqlConnection(connectionString);
}
