using ECommerce.Application.Dtos.Carts;
using ECommerce.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interface.Carts
{
    public interface ICartService
    {
        Task<Result<CartResponse>> GetCartAsync(string userId);

        Task<Result<CartResponse>> AddItemToCartAsync(
            string userId,
            AddCartItemRequest request);

        Task<Result<CartResponse>> UpdateCartItemAsync(
            string userId,
            UpdateCartItemRequest request);

        Task<Result<CartResponse>> RemoveItemFromCartAsync(
            string userId,
            int productId);

        Task<Result<CartResponse>> ClearCartAsync(
            string userId);
    }
}
