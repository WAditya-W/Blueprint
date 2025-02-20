using Blueprint.Dtos;
using Blueprint.Models;
using Blueprint.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blueprint.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IProductRepository _repository;
        private readonly IAuthService _authService;
        private readonly IAuthRepository _authrepository;

        public ProductService(IConfiguration configuration, IProductRepository repository, IAuthService authService, IAuthRepository authrepository)
        {
            _configuration = configuration;
            _repository = repository;
            _authService = authService;
            _authrepository = authrepository;
        }

        public async Task<AuthResult> GetAllAsync(string token)
        {
            var username = _authService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid token." };

            var user = await _authrepository.GetByNameAsync(username);
            if (user == null || !user.isLogin)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "User not found or not logged in." };

            var product = await _repository.GetAllAsync();
            if (product == null || !product.Any())
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "No products found." };

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "products found.",
                Data = product
            };
        }

        public async Task<AuthResult> GetByNameAsync(string token, string Name)
        {
            var username = _authService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid token." };

            var user = await _authrepository.GetByNameAsync(username);
            if (user == null || !user.isLogin)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "User not found or not logged in." };

            var product = await _repository.GetByNameAsync(Name);
            if (product == null)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "No products found." };

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "product found.",
                Data = product
            };
        }

        public async Task<AuthResult> AddProduct(string token, ProductDto productDto)
        {
            var username = _authService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid token." };

            var user = await _authrepository.GetByNameAsync(username);
            if (user == null || !user.isLogin)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "User not found or not logged in." };

            if (productDto == null)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid product data." };

            var existingProduct = await _repository.GetByNameAsync(productDto.Name);
            if (existingProduct != null)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.Conflict, Message = $"Product name '{productDto.Name}' already exists." };

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price
            };
            await _repository.AddAsync(product);

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "Product Added successfully.",
                Data = product
            };
        }

        public async Task<AuthResult> UpdateProduct(string token, string name, ProductDto productDto)
        {
            var username = _authService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid token." };

            var user = await _authrepository.GetByNameAsync(username);
            if (user == null || !user.isLogin)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "User not found or not logged in." };

            if (productDto == null)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid product data." };

            var existingProduct = await _repository.GetByNameAsync(name);
            if (existingProduct == null)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = $"Product '{name}' not found." };

            var productUpdate = await _repository.GetByNameAsync(productDto.Name);
            if (productUpdate != null && productDto.Name != name)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.Conflict, Message = $"Product name '{productDto.Name}' already exists." };

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.CreatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(existingProduct);

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "Product updated successfully.",
                Data = existingProduct
            };
        }

        public async Task<AuthResult> DeleteProduct(string token, string name)
        {
            var username = _authService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid token." };

            var user = await _authrepository.GetByNameAsync(username);
            if (user == null || !user.isLogin)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "User not found or not logged in." };

            var existingProduct = await _repository.GetByNameAsync(name);
            if (existingProduct == null)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "Product not found." };

            await _repository.DeleteAsync(existingProduct);

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "Product deleted successfully.",
                Data = existingProduct
            };
        }
    }
}
