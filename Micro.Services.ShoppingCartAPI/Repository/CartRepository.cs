using AutoMapper;
using Micro.Services.ShoppingCartAPI.Data;
using Micro.Services.ShoppingCartAPI.DTOs;
using Micro.Services.ShoppingCartAPI.Interfaces;
using Micro.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Micro.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CartRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId)
            };

            cart.CartDetails = _context.CartDetails.Where(c => c.CartHeaderId == cart.CartHeader.CartHeaderId).Include(p => p.Product);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);

            //check if product exist in database, if not create it!
            Product productFromDb = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cart.CartDetails.FirstOrDefault().ProductId);

            if (productFromDb == null)
            {
                await _context.Products.AddAsync(cart.CartDetails.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }

            //check if header is null
            CartHeader cartHeaderFromDb = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                //create header and detail
                await _context.CartHeaders.AddAsync(cart.CartHeader);
                await _context.SaveChangesAsync();

                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                await _context.AddAsync(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                //if header is not null
                //check if detail has same product
                CartDetail cartDetailFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    c => c.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                    c.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailFromDb == null)
                {
                    //create detail
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    await _context.CartDetails.AddAsync(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //if it has then update the count / cart details
                    cartDetailFromDb.Count += cart.CartDetails.FirstOrDefault().Count;
                    _context.CartDetails.Update(cartDetailFromDb);
                    await _context.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {

                CartDetail cartDetail = await _context.CartDetails.FirstOrDefaultAsync(c => c.CartDetailId == cartDetailId);

                int totalCountOfCartItems = await _context.CartDetails.Where(c => c.CartHeaderId == cartDetail.CartHeaderId).CountAsync();

                _context.CartDetails.Remove(cartDetail);

                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(c => c.CartHeaderId == cartDetail.CartHeaderId);
                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ClearCart(string userId)
        {
            CartHeader cartHeaderFromDb = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cartHeaderFromDb != null)
            {
                _context.CartDetails.RemoveRange(_context.CartDetails.Where(c => c.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                _context.CartHeaders.Remove(cartHeaderFromDb);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
            cartHeader.CouponCode = couponCode;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
            cartHeader.CouponCode = "";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
