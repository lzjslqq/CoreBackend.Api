using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBackend.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyContext _context;
        public ProductRepository(MyContext context)
        {
            _context = context;
        }

        public bool ProductExist(int productId)
        {
            return _context.Products.Any(x => x.Id == productId);
        }
        public Material GetMaterialForProduct(int productId, int materialId)
        {
            return _context.Materials.FirstOrDefault(x => x.ProductId == productId && x.Id == materialId);
        }

        public IEnumerable<Material> GetMaterialsForProduct(int productId)
        {
            return _context.Materials.Where(x => x.ProductId == productId).ToList();
        }

        public Product GetProduct(int productId, bool includeMaterials = false)
        {
            if (includeMaterials)
            {
                return _context.Products.Include(x => x.Materials).FirstOrDefault(x => x.Id == productId);
            }
            return _context.Products.Find(productId);
        }

        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.OrderBy(p=>p.Name).ToList();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }
    }
}
