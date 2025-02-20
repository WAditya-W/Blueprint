using Blueprint.Dtos;
using Blueprint.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blueprint.Services
{
    public interface IProductService
    {
        Task<AuthResult> GetAllAsync(string token);
        Task<AuthResult> GetByNameAsync(string token, string Name);
        Task<AuthResult> AddProduct(string token, ProductDto productDto);
        Task<AuthResult> UpdateProduct(string token, string name, ProductDto productDto);
        Task<AuthResult> DeleteProduct(string token, string name);
    }
}
