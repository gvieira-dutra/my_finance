// Ignore Spelling: App

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFinance.API.Models;
using MyFinance.Core.Models;
using MyFinance.Core.Models.Reports;
using System.Reflection;

namespace MyFinance.API.Data;

//DbContextOptions, DbContext, DbSet and ModelBuilder
//are all provided by package Microsoft.EntityFrameworkCore.SqlServer
// IdentityDbContext is provided by package
// Microsoft.AspNetCore.Identity.EntityFrameworkCore
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User,
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>(options)
{

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Voucher> Vouchers { get; set; } = null!;

    //Mapping views
    public DbSet<IncomeAndExpenses> IncomesAndExpenses { get; set; } = null!;
    public DbSet<IncomeByCategory> IncomesByCategory { get; set; } = null!;
    public DbSet<ExpensesByCategory> ExpensesByCategory { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Assemblly is provided by System.Reflection
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<IncomeAndExpenses>()
            .ToView("vwGetIncomesAndExpenses")
            .HasNoKey();

        modelBuilder.Entity<IncomeByCategory>()
            .ToView("vwGetIncomesByCategory")
            .HasNoKey();

        modelBuilder.Entity<ExpensesByCategory>()
            .ToView("vwGetExpensesByCategory")
            .HasNoKey();
    }
}
