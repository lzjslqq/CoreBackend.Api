using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreBackend.Api.Services;

namespace CoreBackend.Api.Controllers
{
    [Route("api/product")]
    public class MaterialController : Controller
    {
        [HttpGet("{productid}/materials")]
        public IActionResult GetMaterials (int productId)
        {
            var product = ProductService.Current.Products.SingleOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product.Materials);
        }

        [HttpGet("{productid}/material/{id}")]
        public IActionResult GetMaterial(int productId,int id)
        {
            var product = ProductService.Current.Products.SingleOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            var material = product.Materials.SingleOrDefault(m => m.Id == id);
            if(material==null)
            {
                return NotFound();
            }
            return Ok(material);
        }
    }
}