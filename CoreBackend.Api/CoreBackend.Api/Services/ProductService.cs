using CoreBackend.Api.Dto;
using System.Collections.Generic;

namespace CoreBackend.Api.Services
{
    public class ProductService
    {
        public static ProductService Current { get; } = new ProductService();
        public List<ProductDto> Products { get; }

        private ProductService()
        {
            Products = new List<ProductDto> {
                 new ProductDto { Id = 1 , Name = "牛奶" , Price = 2.5f , Description="这是牛奶",
                     Materials =new List<MaterialDto>
                     {
                        new MaterialDto{ Id=1,MaterialName="水"},
                        new MaterialDto{ Id=2,MaterialName="奶粉"}
                     }
                 },
                 new ProductDto { Id = 2 , Name = "面包" , Price = 4.5f, Description="这是面包",
                    Materials =new List<MaterialDto>
                     {
                        new MaterialDto{ Id=3,MaterialName="面粉"},
                        new MaterialDto{ Id=4,MaterialName="糖"}
                     }
                 },
                 new ProductDto { Id = 3 , Name = "啤酒" , Price = 7.5f, Description="这是啤酒",
                    Materials =new List<MaterialDto>
                     {
                        new MaterialDto{ Id=5,MaterialName="麦芽"},
                        new MaterialDto{ Id=6,MaterialName="山泉"}
                     }
                 }
            };
        }
    }
}
