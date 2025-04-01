using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFinance.API.Data;
using MyFinance.API.Handlers;
using MyFinance.API.Models;
using MyFinance.Core;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using Stripe;

namespace MyFinance.API.Common.Api;

public static class BuilderExtension
{
    public static void AddConfigurations(this WebApplicationBuilder builder)
    {
        //Connection string is coming from user-secrets
        Configuration.ConnectionStr =
            builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        Configuration.BackEndURL =
            builder.Configuration.GetValue<string>("BackEndURL") ?? string.Empty;

        Configuration.FrontEndURL =
            builder.Configuration.GetValue<string>("FrontEndURL") ?? string.Empty;

        ApiConfiguration.StripeApiKey =
            builder.Configuration.GetValue<string>("StripeApiKey") ?? string.Empty;

        StripeConfiguration.ApiKey = ApiConfiguration.StripeApiKey;
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        //AddSwagerGen provided by package SwashBuckle.AspNetCore
        builder.Services.AddSwaggerGen(x =>
        {
            x.CustomSchemaIds(o => o.FullName);
        });
    }

    public static void AddSecurity(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services.AddAuthorization();
    }

    public static void AddDataContexts(this WebApplicationBuilder builder)
    {
        // AddDbContext
        //is provided by package Microsoft.EntityFrameworkCore.SqlServer
        builder.Services.AddDbContext<AppDbContext>(x =>
        {
            //x.UseSqlServer(Configuration.ConnectionStr);
            x.UseInMemoryDatabase("MyFinanceDb");
        });

        builder
            .Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddTransient<ICategoryHandler, CategoryHandler>()
            .AddTransient<ITransactionHandler, TransactionHandler>()
            .AddTransient<IReportHandler, ReportsHandler>()
            .AddTransient<IProductHandler, ProductHandler>()
            .AddTransient<IVoucherHandler, VoucherHandler>()
            .AddTransient<IOrderHandler, OrderHandler>()
            .AddTransient<IStripeHandler, StripeHandler>();
    }

    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
            options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy =>
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
            )
        );
    }

    public static async Task SeedMockDataAsync(this WebApplicationBuilder builder)
    {
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await userManager.FindByEmailAsync("test@mail.com") == null)
        {
            var user = new User
            {
                Id = new Guid("36a0bb78-1f40-4814-4d06-08dd4f876cee"),
                Email = "test@mail.com",
                UserName = "test@mail.com",
                NormalizedUserName = "TEST@MAIL.COM",
                NormalizedEmail = "TEST@MAIL.COM",
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, "Welcome123!");

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );
            }
        }

        await context.Categories.AddAsync(
            new Category
            {
                Id = new Guid("830f6297-053f-4052-853d-538eab916bf4"),
                Title = "Housing",
                Description = "Expenses related to monthly rent or mortgage payments",
                UserId = "test@mail.com",
            }
        );

        await context.Categories.AddAsync(
            new Category
            {
                Id = new Guid("a58e2e63-4d34-4055-9b8e-f8301085a5d3"),
                Title = "Utilities",
                Description = "Utility bills such as electricity, water, and gas",
                UserId = "test@mail.com",
            }
        );

        await context.Categories.AddAsync(
            new Category
            {
                Id = new Guid("0d42b976-8d46-4537-bd87-8cd01eb30b3c"),
                Title = "Groceries",
                Description = "Groceries and essential food items",
                UserId = "test@mail.com",
            }
        );

        await context.Categories.AddAsync(
            new Category
            {
                Id = new Guid("2d7fbdf9-d320-4d0e-aeca-cef04aede886"),
                Title = "Restaurants",
                Description = "Dining out, takeout, and restaurant expenses",
                UserId = "test@mail.com",
            }
        );

        await context.Categories.AddAsync(
            new Category
            {
                Id = new Guid("48a4f2e8-35b2-4ffe-89f4-c8dd6e2586a5"),
                Title = "Transportation",
                Description = "Gas, car maintenance, and public transportation costs",
                UserId = "test@mail.com",
            }
        );

        await context.AddRangeAsync(
            // Housing
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Monthly Rent Payment",
                Amount = 1200.00M,
                CategoryId = new Guid("830f6297-053f-4052-853d-538eab916bf4"),
                CreatedAt = DateTime.Now.AddDays(-10),
                PaidOrReceivedAt = DateTime.Now.AddDays(-10),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Home Insurance Payment",
                Amount = 150.00M,
                CategoryId = new Guid("830f6297-053f-4052-853d-538eab916bf4"),
                CreatedAt = DateTime.Now.AddDays(-25),
                PaidOrReceivedAt = DateTime.Now.AddDays(-25),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Utilities
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Electricity Bill",
                Amount = 85.45M,
                CategoryId = new Guid("a58e2e63-4d34-4055-9b8e-f8301085a5d3"),
                CreatedAt = DateTime.Now.AddDays(-8),
                PaidOrReceivedAt = DateTime.Now.AddDays(-8),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Water Bill",
                Amount = 40.00M,
                CategoryId = new Guid("a58e2e63-4d34-4055-9b8e-f8301085a5d3"),
                CreatedAt = DateTime.Now.AddDays(-3),
                PaidOrReceivedAt = DateTime.Now.AddDays(-3),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Groceries
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Weekly Grocery Shopping",
                Amount = 220.75M,
                CategoryId = new Guid("0d42b976-8d46-4537-bd87-8cd01eb30b3c"),
                CreatedAt = DateTime.Now.AddDays(-7),
                PaidOrReceivedAt = DateTime.Now.AddDays(-7),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Organic Produce Purchase",
                Amount = 50.00M,
                CategoryId = new Guid("0d42b976-8d46-4537-bd87-8cd01eb30b3c"),
                CreatedAt = DateTime.Now.AddDays(-14),
                PaidOrReceivedAt = DateTime.Now.AddDays(-14),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Restaurants
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Dinner at Italian Restaurant",
                Amount = 75.99M,
                CategoryId = new Guid("2d7fbdf9-d320-4d0e-aeca-cef04aede886"),
                CreatedAt = DateTime.Now.AddDays(-2),
                PaidOrReceivedAt = DateTime.Now.AddDays(-2),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Coffee Shop Visit",
                Amount = 10.50M,
                CategoryId = new Guid("2d7fbdf9-d320-4d0e-aeca-cef04aede886"),
                CreatedAt = DateTime.Now.AddDays(-6),
                PaidOrReceivedAt = DateTime.Now.AddDays(-6),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Transportation
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Car Fuel",
                Amount = 60.00M,
                CategoryId = new Guid("48a4f2e8-35b2-4ffe-89f4-c8dd6e2586a5"),
                CreatedAt = DateTime.Now.AddDays(-4),
                PaidOrReceivedAt = DateTime.Now.AddDays(-4),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Uber Ride",
                Amount = 25.00M,
                CategoryId = new Guid("48a4f2e8-35b2-4ffe-89f4-c8dd6e2586a5"),
                CreatedAt = DateTime.Now.AddDays(-9),
                PaidOrReceivedAt = DateTime.Now.AddDays(-9),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = new Guid("a6db004c-0bbd-4cc9-9a98-05f6d9475c2d"),
                Title = "Monthly bus pass",
                Amount = 100.99M,
                CategoryId = new Guid("48a4f2e8-35b2-4ffe-89f4-c8dd6e2586a5"),
                CreatedAt = DateTime.Now.AddDays(-5),
                PaidOrReceivedAt = DateTime.Now.AddDays(-5),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Property Tax Payment",
                Amount = 300.00M,
                CategoryId = new Guid("830f6297-053f-4052-853d-538eab916bf4"),
                CreatedAt = DateTime.Now.AddMonths(-2),
                PaidOrReceivedAt = DateTime.Now.AddMonths(-2),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Apartment Maintenance Fee",
                Amount = 75.00M,
                CategoryId = new Guid("830f6297-053f-4052-853d-538eab916bf4"),
                CreatedAt = DateTime.Now.AddDays(-20),
                PaidOrReceivedAt = DateTime.Now.AddDays(-20),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Utilities
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Gas Bill",
                Amount = 55.30M,
                CategoryId = new Guid("a58e2e63-4d34-4055-9b8e-f8301085a5d3"),
                CreatedAt = DateTime.Now.AddMonths(-1),
                PaidOrReceivedAt = DateTime.Now.AddMonths(-1),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Internet Subscription",
                Amount = 70.00M,
                CategoryId = new Guid("a58e2e63-4d34-4055-9b8e-f8301085a5d3"),
                CreatedAt = DateTime.Now.AddDays(-12),
                PaidOrReceivedAt = DateTime.Now.AddDays(-12),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Groceries
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Bulk Grocery Shopping",
                Amount = 350.00M,
                CategoryId = new Guid("0d42b976-8d46-4537-bd87-8cd01eb30b3c"),
                CreatedAt = DateTime.Now.AddDays(-15),
                PaidOrReceivedAt = DateTime.Now.AddDays(-15),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Local Farmer's Market",
                Amount = 45.90M,
                CategoryId = new Guid("0d42b976-8d46-4537-bd87-8cd01eb30b3c"),
                CreatedAt = DateTime.Now.AddDays(-11),
                PaidOrReceivedAt = DateTime.Now.AddDays(-11),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Restaurants
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Fast Food Takeout",
                Amount = 18.75M,
                CategoryId = new Guid("2d7fbdf9-d320-4d0e-aeca-cef04aede886"),
                CreatedAt = DateTime.Now.AddDays(-5),
                PaidOrReceivedAt = DateTime.Now.AddDays(-5),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Weekend Brunch",
                Amount = 40.00M,
                CategoryId = new Guid("2d7fbdf9-d320-4d0e-aeca-cef04aede886"),
                CreatedAt = DateTime.Now.AddDays(-9),
                PaidOrReceivedAt = DateTime.Now.AddDays(-9),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Transportation
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Car Oil Change",
                Amount = 90.00M,
                CategoryId = new Guid("48a4f2e8-35b2-4ffe-89f4-c8dd6e2586a5"),
                CreatedAt = DateTime.Now.AddMonths(-3),
                PaidOrReceivedAt = DateTime.Now.AddMonths(-3),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Airport Taxi Ride",
                Amount = 55.00M,
                CategoryId = new Guid("48a4f2e8-35b2-4ffe-89f4-c8dd6e2586a5"),
                CreatedAt = DateTime.Now.AddDays(-17),
                PaidOrReceivedAt = DateTime.Now.AddDays(-17),
                Type = Core.Enum.ETransactionType.Withdraw,
                UserId = "test@mail.com",
            },
            // Miscellaneous Incomes
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Freelance Web Development",
                Amount = 1500.00M,
                CategoryId = new Guid("a58e2e63-4d34-4055-9b8e-f8301085a5d3"), // Could be a separate "Income" category
                CreatedAt = DateTime.Now.AddDays(-6),
                PaidOrReceivedAt = DateTime.Now.AddDays(-6),
                Type = Core.Enum.ETransactionType.Deposit,
                UserId = "test@mail.com",
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Title = "Stock Dividend",
                Amount = 200.00M,
                CategoryId = new Guid("a58e2e63-4d34-4055-9b8e-f8301085a5d3"), // Could be a separate "Investment" category
                CreatedAt = DateTime.Now.AddDays(-25),
                PaidOrReceivedAt = DateTime.Now.AddDays(-25),
                Type = Core.Enum.ETransactionType.Deposit,
                UserId = "test@mail.com",
            }
        );

        await context.AddRangeAsync(
            // Monthly Subscription
            new Core.Models.Product
            {
                Id = Guid.NewGuid(),
                Title = "Monthly Membership",
                Slug = "monthly-subscription",
                Summary = "Unlimited access to the platform for one month!",
                Description =
                    "Enjoy all features, including premium content and support, with a recurring monthly subscription.",
                Price = 14.99M,
                IsActive = true,
            },
            // Semestral Subscription (6 Months)
            new Core.Models.Product
            {
                Id = Guid.NewGuid(),
                Title = "6-Month Membership",
                Slug = "semestral-subscription",
                Summary = "Six months of premium access at a discounted rate!",
                Description =
                    "A great way to save while enjoying full platform benefits for six months.",
                Price = 74.99M, // Slightly cheaper than 6x monthly price
                IsActive = true,
            },
            // Yearly Subscription (Already Exists)
            new Core.Models.Product
            {
                Id = Guid.NewGuid(),
                Title = "Year Membership",
                Slug = "year-subscription",
                Summary = "This gives you unlimited access to the platform for one year!",
                Description =
                    "Best value for long-term users. Get access to all premium features and content at a discounted annual rate.",
                Price = 129.99M,
                IsActive = true,
            }
        );

        await context.Vouchers.AddAsync(
            new Voucher
            {
                Amount = 10,
                IsActive = true,
                Number = "ABCD1234",
                Id = 123456789,
            }
        );

        context.SaveChanges();
    }
}
