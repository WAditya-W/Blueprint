using Blueprint.Dtos;
using Blueprint.Models;
using Blueprint.Repositories;
using Blueprint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blueprint.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products (Get All)
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var result = await _productService.GetAllAsync(token);
            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.NotFound)
                    return NotFound(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.BadRequest)
                    return BadRequest(new { Success = true, result.StatusCode, result.Message, result.Data });

                return StatusCode((int)ResponseCode.InternalServerError, result);
            }

            return Ok(new { Success = true, result.StatusCode, result.Message, result.Data });
        }

        // GET: api/products/{Name} (Get by Name)
        [HttpGet("{Name}")]
        public async Task<IActionResult> GetProductByName(string Name)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var result = await _productService.GetByNameAsync(token, Name);
            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.NotFound)
                    return NotFound(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.BadRequest)
                    return BadRequest(new { Success = true, result.StatusCode, result.Message, result.Data });

                return StatusCode((int)ResponseCode.InternalServerError, result);
            }

            return Ok(new { Success = true, result.StatusCode, result.Message, result.Data });
        }

        // POST: api/products (Create)
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var result = await _productService.AddProduct(token, productDto);
            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.NotFound)
                    return NotFound(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.BadRequest)
                    return BadRequest(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.Conflict)
                    return Conflict(new { Success = true, result.StatusCode, result.Message, result.Data });

                return StatusCode((int)ResponseCode.InternalServerError, result);
            }
            return Ok(new { Success = true, result.StatusCode, result.Message, result.Data });
        }

        // PUT: api/products/{name} (Update)
        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateProduct(string name, [FromBody] ProductDto productDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var result = await _productService.UpdateProduct(token, name, productDto);
            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.NotFound)
                    return NotFound(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.BadRequest)
                    return BadRequest(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.Conflict)
                    return Conflict(new { Success = true, result.StatusCode, result.Message, result.Data });

                return StatusCode((int)ResponseCode.InternalServerError, result);
            }

            return Ok(new { Success = true, result.StatusCode, result.Message, result.Data });
        }

        // DELETE: api/products/{id} (Delete)
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteProduct(string name)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var result = await _productService.DeleteProduct(token, name);
            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.NotFound)
                    return NotFound(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.BadRequest)
                    return BadRequest(new { Success = true, result.StatusCode, result.Message, result.Data });

                return StatusCode((int)ResponseCode.InternalServerError, result);
            }

            return Ok(new { Success = true, result.StatusCode, result.Message });
        }
    }

}
