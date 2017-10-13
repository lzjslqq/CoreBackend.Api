using CoreBackend.Api.Dto;
using System.Collections.Generic;

namespace CoreBackend.Api.Services
{
    public class ProductService
    {
        public static ProductService Current { get; } = new ProductService();
        public List<Product> Products { get; }

        private ProductService()
        {
            Products = new List<Product> {
                 new Product { Id = 1 , Name = "牛奶" , Price = 2.5f , Description="这是牛奶",
                     Materials =new List<Material>
                     {
                        new Material{ Id=1,MaterialName="水"},
                        new Material{ Id=2,MaterialName="奶粉"}
                     }
                 },
                 new Product { Id = 2 , Name = "面包" , Price = 4.5f, Description="这是面包",
                    Materials =new List<Material>
                     {
                        new Material{ Id=3,MaterialName="面粉"},
                        new Material{ Id=4,MaterialName="糖"}
                     }
                 },
                 new Product { Id = 3 , Name = "啤酒" , Price = 7.5f, Description="这是啤酒",
                    Materials =new List<Material>
                     {
                        new Material{ Id=5,MaterialName="麦芽"},
                        new Material{ Id=6,MaterialName="山泉"}
                     }
                 }
            };
        }
    }
}
