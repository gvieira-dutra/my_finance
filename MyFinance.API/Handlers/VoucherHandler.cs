using Microsoft.EntityFrameworkCore;
using MyFinance.API.Data;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;

namespace MyFinance.API.Handlers;

public class VoucherHandler(AppDbContext context) : IVoucherHandler
{
    public async Task<Response<Voucher?>> GetByNumberAsync(GetVoucherByNumberRequest request)
    {
        try
        {
            var voucher = await context.Vouchers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Number == request.Number && x.IsActive == true);

            return voucher is null
                ? new Response<Voucher?>(null, 404, "Voucher not found")
                : new Response<Voucher?>(voucher);
        }
        catch
        {
            return new Response<Voucher?>(null, 500, "Unable to find this voucher");
        }
    }
}
