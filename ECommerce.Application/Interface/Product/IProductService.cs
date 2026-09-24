using ECommerce.Application.Common;
using ECommerce.Application.Dtos.Pagination;
using ECommerce.Application.Dtos.Product;
using ECommerce.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interface.Product
{
    public interface IProductService
    {
        Task<Result<CreateProductResponse>> CreateAsync(
        CreateProductRequest request);

        Task<Result<UpdateProductResponse>> UpdateAsync(
        int id,
        UpdateProductRequest request);

        Task<Result<ChangeProductStatusResponse>> ChangeStatusAsync(int id);
        Task<Result<PagedResult<GetProductResponse>>> GetAvailableProductsAsync(int? categoryId,PaginationRequest request);
        Task<Result<PagedResult<GetProductResponse>>> GetAllProductsAsync(int? categoryId, PaginationRequest pagination);
        Task<Result<GetProductResponse>> GetProductByIdAsync(int id);
        Task<Result<GetProductResponse>> GetAvailableProductByIdAsync(int id);

    }
}
