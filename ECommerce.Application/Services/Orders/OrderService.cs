using ECommerce.Application.Abstraction.Repository;
using ECommerce.Application.Dtos.Orders;
using ECommerce.Application.Interface.Orders;
using ECommerce.Application.Results;
using ECommerce.Domain.Entity;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ITransactionManager _transactionManager;
        public OrderService(IOrderRepository orderRepository,
            ICartRepository cartRepository, 
            IProductRepository productRepository,
            IReservationRepository reservationRepository,
            ITransactionManager transactionManager)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _reservationRepository = reservationRepository;
            _transactionManager = transactionManager;
        }
        public async Task<Result<OrderResponse>> CheckoutAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return Result<OrderResponse>.Failure("Cart is empty.");
            }
            if (cart.CartItems.Any(ci => ci.Product.StockQuantity < ci.Quantity))
            {
                return Result<OrderResponse>.Failure("One or more products in the cart are out of stock.");
            }
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepository.GetAvailableByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    return Result<OrderResponse>.Failure($"Product with ID {cartItem.ProductId} is not available.");
                }
            }
            await _transactionManager.BeginAsync();
            try
            {
                foreach (var cartItem in cart.CartItems)
                {
                    var reserved = await _productRepository
                        .TryReserveStockAsync(
                            cartItem.ProductId,
                            cartItem.Quantity);

                    if (!reserved)
                    {
                        await _transactionManager.RollbackAsync();

                        return Result<OrderResponse>.Failure(
                            $"Not enough stock for product {cartItem.ProductId}.");
                    }
                }

                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending,

                    OrderItems = cart.CartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.Price
                    }).ToList()
                };
                await _orderRepository.AddAsync(order);
                var reservation = new InventoryReservation
                {
                    Order = order,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationAt = DateTime.UtcNow.AddMinutes(10),
                    Status = ReservationStatus.Active,
                };
                await _reservationRepository.AddAsync(reservation);

                foreach (var orderItem in order.OrderItems)
                {
                    var inventoryItem = new InventoryItem
                    {
                        
                        ProductId = orderItem.ProductId,
                        Quantity = orderItem.Quantity,
                        InventoryReservation = reservation
                    };
                    reservation.InventoryItems.Add(inventoryItem);
                }
                await _transactionManager.SaveChangesAsync();

                await _transactionManager.CommitAsync();
                var response = new OrderResponse
                {
                    Id = order.Id,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,

                    Items = order.OrderItems.Select(oi =>
                    {
                        var cartItem = cart.CartItems
                            .First(ci => ci.ProductId == oi.ProductId);

                        return new OrderItemResponse
                        {
                            ProductId = oi.ProductId,
                            ProductName = cartItem.Product.Name,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Subtotal = oi.Quantity * oi.UnitPrice
                        };
                    }).ToList(),

                    Total = order.OrderItems
                        .Sum(oi => oi.Quantity * oi.UnitPrice)
                };

                return Result<OrderResponse>.Success(response);
            }
            catch { 
                await _transactionManager.RollbackAsync();
                throw ;
            }


        }
    }
}
