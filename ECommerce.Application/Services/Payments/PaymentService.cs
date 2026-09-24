using ECommerce.Application.Abstraction.Payment;
using ECommerce.Application.Abstraction.Repository;
using ECommerce.Application.Dtos.Payments;
using ECommerce.Application.Interface.Payments;
using ECommerce.Application.Results;
using ECommerce.Domain.Entity;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services.Payments
{

        public class PaymentService : IPaymentService
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IPaymentAttemptRepository _paymentAttemptRepository;
            private readonly IPaymentProvider _paymentProvider;
            private readonly IReservationRepository _reservationRepository;
            private readonly IProductRepository _productRepository;
            private readonly ITransactionManager _transactionManager;
            private readonly ICartRepository _cartRepository;

            public PaymentService(
                IOrderRepository orderRepository,
                IPaymentAttemptRepository paymentAttemptRepository,
                IPaymentProvider paymentProvider,
                IReservationRepository reservationRepository,
                IProductRepository productRepository,
                ITransactionManager transactionManager,
                ICartRepository cartRepository)
            {
                _orderRepository = orderRepository;
                _paymentAttemptRepository = paymentAttemptRepository;
                _paymentProvider = paymentProvider;
                _reservationRepository = reservationRepository;
                _productRepository = productRepository;
                _transactionManager = transactionManager;
                _cartRepository = cartRepository;
            }

            public async Task<Result<PaymentResponse>> PayAsync(
                string userId,
                int orderId)
            {
                // 1. Get Order
                var order = await _orderRepository
                    .GetByIdWithItemsAsync(orderId);

                if (order == null || order.UserId != userId)
                {
                    return Result<PaymentResponse>.Failure(
                        "Order not found.");
                }

                // 2. Order must be Pending
                if (order.Status != OrderStatus.Pending)
                {
                    return Result<PaymentResponse>.Failure(
                        "Order is not pending.");
                }

                // 3. Get Reservation
                var reservation = await _reservationRepository
                    .GetByOrderIdAsync(order.Id);

                if (reservation == null)
                {
                    return Result<PaymentResponse>.Failure(
                        "Reservation not found.");
                }

                // 4. Reservation must be Active and not expired
                if (reservation.Status != ReservationStatus.Active ||
                    reservation.ExpirationAt <= DateTime.UtcNow)
                {
                    return Result<PaymentResponse>.Failure(
                        "Reservation has expired.");
                }

                // 5. Calculate amount from Order
                var amount = order.OrderItems
                    .Sum(item => item.Quantity * item.UnitPrice);

                // 6. Create Payment Attempt
                var paymentAttempt = new PaymentAttempt
                {
                    OrderId = order.Id,
                    Amount = amount,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentAttemptRepository
                    .AddAsync(paymentAttempt);

                // 7. Get Cart
                // Cart is NOT required for payment to succeed.
                // We only need it if we want to clear it after success.
                var cart = await _cartRepository
                    .GetCartByUserIdAsync(userId);

                // 8. Call external payment provider
                var paymentResult =
                    await _paymentProvider
                        .ProcessPaymentAsync(amount);

                // 9. Payment Failed
                if (!paymentResult.IsSuccess)
                {
                    paymentAttempt.Status =
                        PaymentStatus.Failed;

                    paymentAttempt.FailureReason =
                        paymentResult.FailureReason;

                    await _transactionManager.SaveChangesAsync();

                    return Result<PaymentResponse>.Success(
                        new PaymentResponse
                        {
                            PaymentAttemptId = paymentAttempt.Id,
                            OrderId = order.Id,
                            Amount = amount,
                            Status = PaymentStatus.Failed,
                            ReferenceId = null,
                            FailureReason =
                                paymentResult.FailureReason
                        });
                }

                // 10. Payment Succeeded
                await _transactionManager.BeginAsync();

                try
                {
                    // 11. Update Payment Attempt
                    paymentAttempt.Status =
                        PaymentStatus.Successful;

                    paymentAttempt.ReferenceId =
                        paymentResult.ReferenceId;

                    // 12. Confirm Order
                    order.Status =
                        OrderStatus.Confirmed;

                    // 13. Finalize Reserved Stock
                    foreach (var inventoryItem in
                             reservation.InventoryItems)
                    {
                        var finalized =
                            await _productRepository
                                .FinalizeStockAsync(
                                    inventoryItem.ProductId,
                                    inventoryItem.Quantity);

                        if (!finalized)
                        {
                            throw new InvalidOperationException(
                                $"Failed to finalize stock for product " +
                                $"{inventoryItem.ProductId}.");
                        }
                    }

                    // 14. Confirm Reservation
                    reservation.Status =
                        ReservationStatus.Confirmed;

                    // 15. Clear Cart
                    if (cart != null)
                    {
                        _cartRepository.RemoveCartItems(cart.Id);
                    }

                    // 16. Save all changes
                    await _transactionManager.SaveChangesAsync();

                    // 17. Commit Transaction
                    await _transactionManager.CommitAsync();

                    // 18. Return Success
                    return Result<PaymentResponse>.Success(
                        new PaymentResponse
                        {
                            PaymentAttemptId =
                                paymentAttempt.Id,

                            OrderId =
                                order.Id,

                            Amount =
                                amount,

                            Status =
                                PaymentStatus.Successful,

                            ReferenceId =
                                paymentResult.ReferenceId,

                            FailureReason =
                                null
                        });
                }
                catch
                {
                    await _transactionManager.RollbackAsync();
                    throw;
                }
            }
        }
}


