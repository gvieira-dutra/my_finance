using Microsoft.AspNetCore.Identity;

namespace MyFinance.API.Models;

public class User : IdentityUser<Guid>
{
    public List<IdentityRole<Guid >>? Roles { get; set; }
}
