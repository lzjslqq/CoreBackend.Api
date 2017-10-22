using CoreBackend.Api.Entities;
using System.Collections.Generic;

namespace CoreBackend.Api.Repositories
{
    public interface IProductRepository
    {
        bool ProductExist(int productId);
        IEnumerable<Product> GetProducts();
        Product GetProduct(int productId, bool includeMaterials = false);
        IEnumerable<Material> GetMaterialsForProduct(int productId);
        Material GetMaterialForProduct(int productId, int materialId);
        void AddProduct(Product product);
        void DeleteProduct(Product product);
        bool Save();
    }
}
