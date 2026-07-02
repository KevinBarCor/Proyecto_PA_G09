using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UamHelpDeskPA.Api.Data;

namespace UamHelpDeskPA.Api.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
    "Server=(localdb)\\MSSQLLocalDB;Database=UamHelpDesk;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;");

        return new AppDbContext(optionsBuilder.Options);
    }
}