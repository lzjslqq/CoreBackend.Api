using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBackend.Api.Entities
{
    public static class MyContextExtensions
    {
        /// <summary>
        /// 初始化种子数据
        /// </summary>
        /// <param name="context"></param>
        public static void EnsureSeedDataForContext(this MyContext context)
        {
            if (context.Products.Any())
            {
                return;
            }
            var products = new List<Product>
            {
                new Product
                {
                    Name = "牛奶",
                    Price = 2.5f,
                    Description = "这是牛奶啊"
                },
                new Product
                {
                    Name = "面包",
                    Price = 4.5f,
                    Description = "这是面包啊"
                },
                new Product
                {
                    Name = "啤酒",
                    Price = 7.5f,
                    Description = "这是啤酒啊"
                }
            };

            context.Products.AddRange(products);
            // 在使用了SaveChanges后才会把数据保存到数据库
            context.SaveChanges();
        }
    }
}
