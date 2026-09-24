using ECommerce.Application.Abstraction.Repository;
using ECommerce.Application.Dtos.Carts;
using ECommerce.Application.Interface.Carts;
using ECommerce.Application.Results;
using ECommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }
        public async Task<Result<CartResponse>> AddItemToCartAsync
            (string userId, AddCartItemRequest request)
        {
            if (request.Quantity <= 0)
            {
                return Result<CartResponse>.Failure("Quantity must be greater than zero.");
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
            }
            var product = await _productRepository.GetAvailableByIdAsync(request.ProductId);
            if (product == null)
            {
                return Result<CartResponse>.Failure("Product not found or unavailable.");
            }

            var existingCartItem = await _cartRepository.GetCartItemAsync(cart.Id, request.ProductId);
            if (existingCartItem == null) { 
                if (request.Quantity > product.StockQuantity)
                {
                    return Result<CartResponse>.Failure("Requested quantity exceeds available stock.");
                }
                await _cartRepository.AddCartItemAsync(new Domain.Entity.CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }
            else
            {
                var newQuantity = existingCartItem.Quantity + request.Quantity;
                if (newQuantity > product.StockQuantity)
                {
                    return Result<CartResponse>.Failure("Requested quantity exceeds available stock.");
                }
                existingCartItem.Quantity = newQuantity;
                await _cartRepository.UpdateCartItemAsync(existingCartItem);
            }
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return Result<CartResponse>.Success(
             MapToCartResponse(cart));

        }

        public async Task<Result<CartResponse>> ClearCartAsync(string userId)
        {
            
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return Result<CartResponse>.Failure(
                    "Cart not found.");
            }

            
            await _cartRepository.ClearCartAsync(cart.Id);
            

            
            cart = await _cartRepository.GetCartByUserIdAsync(userId);

            
            var items = cart!.CartItems.Select(ci => new CartItemResponse
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                ProductName = ci.Product.Name,
                Price = ci.Product.Price,
                Subtotal = ci.Quantity * ci.Product.Price
            }).ToList();

           
            var total = items.Sum(i => i.Subtotal);

            
            return Result<CartResponse>.Success(new CartResponse
            {
                CartId = cart.Id,
                Items = items,
                Total = total
            });
        }
        public async Task<Result<CartResponse>> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart=await _cartRepository.CreateCartAsync(userId);
            }
            
              return Result<CartResponse>.Success(
             MapToCartResponse(cart));


        }

        public async Task<Result<CartResponse>> RemoveItemFromCartAsync(string userId, int productId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return Result<CartResponse>.Failure(
                    "Cart not found.");
            }

            // 2. Check if the product exists in the cart
            var cartItem = await _cartRepository.GetCartItemAsync(
                cart.Id,
                productId);

            if (cartItem == null)
            {
                return Result<CartResponse>.Failure(
                    "Product is not in the cart.");
            }

            
            await _cartRepository.RemoveCartItemAsync(cartItem);

            
            cart = await _cartRepository.GetCartByUserIdAsync(userId);

            return Result<CartResponse>.Success(
            MapToCartResponse(cart));
        }

        public async Task<Result<CartResponse>> UpdateCartItemAsync(string userId, UpdateCartItemRequest request)
        {
            if (request.Quantity <= 0)
            {
                return Result<CartResponse>.Failure(
                    "Quantity must be greater than zero.");
            }

           
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return Result<CartResponse>.Failure(
                    "Cart not found.");
            }

            
            var product = await _productRepository
                .GetAvailableByIdAsync(request.productId);

            if (product == null)
            {
                return Result<CartResponse>.Failure(
                    "Product is not available.");
            }

            
            var cartItem = await _cartRepository.GetCartItemAsync(
                cart.Id,
                request.productId);

            if (cartItem == null)
            {
                return Result<CartResponse>.Failure(
                    "Product is not in the cart.");
            }

            
            if (request.Quantity > product.StockQuantity)
            {
                return Result<CartResponse>.Failure(
                    "Requested quantity is greater than available stock.");
            }

            
            cartItem.Quantity = request.Quantity;

            await _cartRepository.UpdateCartItemAsync(cartItem);

            
            cart = await _cartRepository.GetCartByUserIdAsync(userId);

            return Result<CartResponse>.Success(
            MapToCartResponse(cart));
        }

        private CartResponse MapToCartResponse(Cart cart)
        {
            var items = cart.CartItems.Select(ci => new CartItemResponse
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                ProductName = ci.Product.Name,
                Price = ci.Product.Price,
                Subtotal = ci.Quantity * ci.Product.Price
            }).ToList();

            var total = items.Sum(i => i.Subtotal);

            return new CartResponse
            {
                CartId = cart.Id,
                Items = items,
                Total = total
            };
        }
    }
}
