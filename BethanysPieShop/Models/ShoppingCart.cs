﻿using BethanysPieShop.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Repositories
{
    public class ShoppingCart
    {
        private readonly AppDbContext _appDbContext;

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
        public ShoppingCart(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<AppDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);
            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Pie pie,int amount=1)
        {
            var shoppingCartItem = _appDbContext.ShoppingCartItems
                .FirstOrDefault(c => c.Pie.PieId == pie.PieId && c.ShoppingCartId == ShoppingCartId);
            if (shoppingCartItem==null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    Pie = pie,
                    ShoppingCartId = ShoppingCartId,
                    Amount = amount
                };
                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }
            _appDbContext.SaveChanges();
        }
        
        public int RemoveFromCart(Pie pie)
        {
            var shoppingCartItem = _appDbContext.ShoppingCartItems.SingleOrDefault(
                        s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);
            int localAmount = 0;
            if (shoppingCartItem !=null)
            {
                if (shoppingCartItem.Amount>1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            _appDbContext.SaveChanges();

            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??
                  (ShoppingCartItems =
                      _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                          .Include(s => s.Pie)
                          .ToList());
        }

        public void ClearCart()
        {
            var cartItems = _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId);
            _appDbContext.ShoppingCartItems.RemoveRange(cartItems);
            _appDbContext.SaveChanges();
        }
        public decimal GetTotal()
        {
            var total = _appDbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId)
                            .Select(p => p.Amount * p.Pie.Price).Sum();
            return total;
        }
    }
}
