using FinancialDataApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataApi.Data;

public class ApiDatabaseContext
    : IdentityUserContext<IdentityUser>
{
    public ApiDatabaseContext(
        DbContextOptions<ApiDatabaseContext> options
    ) : base(options)
    {
    }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsTickerData> NewsTickerData { get; set; }
}