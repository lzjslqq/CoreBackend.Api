using System.Collections.Generic;
using System.Linq;

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
                    Price = 2.5m,
                    Description = "这是牛奶啊",
                    Materials = new List<Material>
                    {
                        new Material
                        {
                            Name = "水"
                        },
                        new Material
                        {
                            Name = "奶粉"
                        }
                    }
                },
                new Product
                {
                    Name = "面包",
                    Price = 4.5m,
                    Description = "这是面包啊",
                    Materials = new List<Material>
                    {
                        new Material
                        {
                            Name = "面粉"
                        },
                        new Material
                        {
                            Name = "糖"
                        }
                    }
                },
                new Product
                {
                    Name = "啤酒",
                    Price = 7.5m,
                    Description = "这是啤酒啊",
                    Materials = new List<Material>
                    {
                        new Material
                        {
                            Name = "麦芽"
                        },
                        new Material
                        {
                            Name = "地下水"
                        }
                    }
                }
            };

            context.Products.AddRange(products);
            // 在使用了SaveChanges后才会把数据保存到数据库
            context.SaveChanges();
        }
    }
}
