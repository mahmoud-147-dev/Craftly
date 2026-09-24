using ECommerce.Application.Abstraction.Repository;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos.Pagination;
using ECommerce.Application.Dtos.Product;
using ECommerce.Application.Interface.Product;
using ECommerce.Application.Results;
using ECommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository; 
        private readonly ICategoryRepository _categoryRepository;
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<Result<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            var existingcategory = await _categoryRepository.GetById(request.CategoryId);
            if (existingcategory == null)
            {
                return Result<CreateProductResponse>.Failure(
                     "Category not found."
                );
            }
            
            if (existingcategory.IsArchived)
            {
                return Result<CreateProductResponse>.Failure(
                     "Cannot add product to an archived category."
                );
            }
            var existingProduct = await _productRepository
                .ExistsByNameAsync(request.Name,request.CategoryId);
            if (existingProduct)
            {
                return Result<CreateProductResponse>.Failure(
                     "Product already exists in this category."
                );
            }

            

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CreateAT=DateTime.UtcNow,
                CategoryId = request.CategoryId
            };
            await _productRepository.AddAsync(product);
            return Result<CreateProductResponse>.Success(
                new CreateProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId

                }
            );
        }

        public async Task<Result<UpdateProductResponse>> UpdateAsync(
            int id,
            UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return Result<UpdateProductResponse>.Failure(
                    "Product not found.");
            }

            var category = await _categoryRepository.GetById(request.CategoryId);

            if (category == null)
            {
                return Result<UpdateProductResponse>.Failure(
                    "Category not found.");
            }

            if (category.IsArchived)
            {
                return Result<UpdateProductResponse>.Failure(
                    "Cannot move product to an archived category.");
            }

            // لو الـ Name أو Category اتغيروا، نتحقق من التكرار
            if (product.Name != request.Name ||
                product.CategoryId != request.CategoryId)
            {
                var existingProduct = await _productRepository
                    .ExistsByNameAsync(request.Name, request.CategoryId);

                if (existingProduct)
                {
                    return Result<UpdateProductResponse>.Failure(
                        "Product already exists in this category.");
                }
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            return Result<UpdateProductResponse>.Success(
                new UpdateProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId
                }
            );
        }

        public async Task<Result<ChangeProductStatusResponse>> ChangeStatusAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return Result<ChangeProductStatusResponse>.Failure(
                    "Product not found.");
            }

            product.IsArchived = !product.IsArchived;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            return Result<ChangeProductStatusResponse>.Success(
                new ChangeProductStatusResponse
                {
                    Id = product.Id,
                    IsArchived = product.IsArchived
                });
        }

        // This method retrieves available products, optionally filtered by category ID, and returns a paginated result. It uses the product repository to fetch the products and maps them to a response DTO. The result includes the list of available products along with pagination details such as total count, page number, and page size.
        public async Task<Result<PagedResult<GetProductResponse>>> GetAvailableProductsAsync(int? categoryId
            ,PaginationRequest request)
        {
            var products = await _productRepository
                .GetAvailableProductsAsync(categoryId
                ,request.PageNumber
                ,request.PageSize);

          
            var response = products.Items.Select(p => new GetProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId
            }).ToList();
            return Result<PagedResult<GetProductResponse>>.Success(
                new PagedResult<GetProductResponse>
                {
                    Items = response,
                    TotalCount = products.TotalCount,
                    PageNumber = products.PageNumber,
                    PageSize = products.PageSize
                }
            );
        }

        // This method retrieves all products, optionally filtered by category ID, and returns a paginated result. It uses the product repository to fetch the products and maps them to a response DTO. The result includes the list of products along with pagination details such as total count, page number, and page size.
        public async Task<Result<PagedResult<GetProductResponse>>> GetAllProductsAsync(
            int? categoryId,PaginationRequest request)
        {
            var products = await _productRepository
                .GetAllProductsAsync(categoryId,request.PageNumber,request.PageSize);

            var response = products.Items.Select(p => new GetProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId
            }).ToList();
            return Result<PagedResult<GetProductResponse>>.Success(
                new PagedResult<GetProductResponse>
                {
                    Items = response,
                    TotalCount = products.TotalCount,
                    PageNumber = products.PageNumber,
                    PageSize = products.PageSize
                }
            );


        }

        // Get product by id
        public async Task<Result<GetProductResponse>> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return Result<GetProductResponse>.Failure(
                    "Product not found.");
            }

            var response = new GetProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId
            };

            return Result<GetProductResponse>.Success(response);
        }
        // This method retrieves an available product by its ID. It checks if the product exists and is available (not archived and has stock). If the product is found, it returns a success result with the product details; otherwise, it returns a failure result indicating that the product was not found or is not available.
        public async Task<Result<GetProductResponse>> GetAvailableProductByIdAsync(int id)
        {
            var product = await _productRepository.GetAvailableByIdAsync(id);

            if (product is null)
            {
                return Result<GetProductResponse>.Failure(
                    "Product not found or is not available.");
            }

            var response = new GetProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId
            };

            return Result<GetProductResponse>.Success(response);
        }
    }
}
