using AutoMapper;
using AutoMapper.QueryableExtensions;
using Micro.Services.ProductAPI.Data;
using Micro.Services.ProductAPI.DTOs;
using Micro.Services.ProductAPI.Interfaces;
using Micro.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Micro.Services.ProductAPI.Respository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            Product product = _mapper.Map<Product>(productDto);
            if (product.ProductId > 0)
            {
                _context.Products.Update(product);
            }
            else
            {
                _context.Products.Add(product);
            }
            await _context.SaveChangesAsync();
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                Product product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
                if (product == null)
                {
                    return false;
                }
                _context.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            return await _context.Products.Where(p => p.ProductId == productId).ProjectTo<ProductDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            return await _context.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
