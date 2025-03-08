using Microsoft.EntityFrameworkCore;
using MyFinance.API.Data;
using MyFinance.Core.Enum;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Requests.Stripe;
using MyFinance.Core.Response;

namespace MyFinance.API.Handlers;

public class OrderHandler(AppDbContext context, IStripeHandler stripeHandler) : IOrderHandler
{
    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context.Orders
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>(null, 404, "Order not found");
        }
        catch (Exception)
        {
            return new Response<Order?>(null, 500, "Unable to find order");
        }

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "Order already canceled");

            case EOrderStatus.WaitingPayment:
                break;

            case EOrderStatus.Paid:
                return new Response<Order?>(order, code: 400, "Order already paid, cannot be canceled");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "Order already refunded, cannot be canceled");

            default:
                return new Response<Order?>(order, 400, "Order cannot be canceled, unknown reason. Contact sales");
        }

        order.Status = EOrderStatus.Canceled;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Unable to cancel order");
        }

        return new Response<Order?>(order, 200, "Order canceled successfully");

    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        Product? product;
        try
        {
            product = await context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProductId
                                          && x.IsActive == true);

            if (product is null)
                return new Response<Order?>(null, 400, "Product not found");

            //Prevents creation of a product that already exists
            //every time we reference product EF know that is the
            //attach product that we are want to access
            context.Attach(product);
        }
        catch
        {
            return new Response<Order?>(null, 500, "Unable to find found");
        }

        Voucher? voucher = null;
        try
        {
            if (request.VoucherId is not null)
            {
                voucher = await context.Vouchers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.VoucherId
                                              && x.IsActive == true);

                if (voucher is null) return new Response<Order?>(null, 400, "Voucher invalid or inexistent");
                if (voucher.IsActive == false) return new Response<Order?>(null, 400, "Voucher invalid or inexistent");

                //voucher.IsActive = false;
                context.Vouchers.Update(voucher);
            }
        }
        catch
        {
            return new Response<Order?>(null, 500, "Unable to retrieve voucher");
        }

        var order = new Order
        {
            UserId = request.UserId,
            Product = product,
            ProductId = request.ProductId,
            Voucher = voucher,
            VoucherId = request.VoucherId
        };

        try
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(null, 500, "Unable to create order");
        }

        return new Response<Order?>(order, 201, $"Order: {order.Number} registered successfully");
    }

    public async Task<PagedResponse<List<Order>?>> GetAllAsync(GetAllOrdersRequest request)
    {
        try
        {
            var query = context.Orders
                .AsNoTracking()
                .Include(x => x.Voucher)
                .Include(x => x.Product)
                .Where(x => x.UserId == request.UserId)
                .OrderByDescending(x => x.CreatedAt);

            var orders = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Order>?>(
                orders,
                count,
                request.PageNumber,
                request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Order>?>(null, 500, "Unable to retrieve orders");
        }
    }

    public async Task<Response<Order?>> GetByNumberAsync(GetOrderByNumberRequest request)
    {
        try
        {
            var order = await context
                .Orders
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x =>
                x.Number == request.Number &&
                x.UserId == request.UserId);

            return order is null
                ? new Response<Order?>(null, 404, "Order not found")
                : new Response<Order?>(order);
        }
        catch (Exception)
        {
            return new Response<Order?>(null, 500, "Unable to find order");
        }
    }

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        Order? order;

        try
        {
            order = await context
                .Orders
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.Number == request.Number
                                          && x.UserId == request.UserId);

            if (order is null) return new Response<Order?>(null, 404, "Order not found");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Unable to find order");
        }

        switch (order.Status)
        {
            case EOrderStatus.Paid:
                return new Response<Order?>(order, 400, "This order is already paid");

            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "This order is canceled and cannot paid");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "This order was refunded and cannot paid");

            case EOrderStatus.WaitingPayment:
                break;

            default:
                return new Response<Order?>(order, 400, "Unable to pay order, contact sales");
        }

        try
        {
            var getTransactionsRequest = new GetTransactionsByOrderNumberRequest
            {
                Number = order.Number
            };

            var result = await stripeHandler.GetTransactionsByOrderNumberAsync(getTransactionsRequest);

            if (result.IsSuccess == false)
                return new Response<Order?>(null, 500, "Unable to retrieve transaction 1");

            if (result.Data is null)
                return new Response<Order?>(null, 500, "Unable to retrieve transaction 2");

            if (result.Data.Any(x => x.Refunded))
                return new Response<Order?>(null, 400, "This order was already paid");

            if (!result.Data.Any(x => x.Paid))
                return new Response<Order?>(null, 400, "Unable to pay order");

            request.ExternalReference = result.Data[0].Id;

        }
        catch (Exception)
        {
            return new Response<Order?>(null, 500, "Although paid, there was an error finalizing your order, contact support");
        }


        order.Status = EOrderStatus.Paid;
        order.UpdatedAt = DateTime.Now;
        order.ExternalReference = request.ExternalReference;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Unable to pay order, contact sales");
        }

        return new Response<Order?>(order, 200, $"Order: {order.Number} paid successfully");

    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        Order? order;

        try
        {
            order = await context
                .Orders
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
            if (order is null) return new Response<Order?>(null, 404, "Order not found");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Unable to find order");
        }

        switch (order.Status)
        {

            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "This order is canceled, cannot be refunded");

            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "This order was refunded, cannot be refunded again");

            case EOrderStatus.WaitingPayment:
                return new Response<Order?>(order, 400, "This order is waiting payment, cannot be refunded");

            case EOrderStatus.Paid:
                break;

            default:
                return new Response<Order?>(order, 400, "Unable to process refund, contact sales");
        }

        order.Status = EOrderStatus.Refunded;
        order.UpdatedAt = DateTime.Now;


        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Unable to refund order, contact sales");
        }

        return new Response<Order?>(order, 200, $"Order: {order.Number} refunded successfully");

    }
}
