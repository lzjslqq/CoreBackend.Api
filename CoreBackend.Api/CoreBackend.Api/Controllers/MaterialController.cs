using AutoMapper;
using CoreBackend.Api.Dto;
using CoreBackend.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoreBackend.Api.Controllers
{
    [Route("api/product")]
    public class MaterialController : Controller
    {
        private readonly IProductRepository _productRepository;
        public MaterialController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpGet("{productId}/materials")]
        public IActionResult GetMaterials (int productId)
        {
            bool product = _productRepository.ProductExist(productId);
            if (!product)
            {
                return NotFound();
            }
            var materials = _productRepository.GetMaterialsForProduct(productId);
            var results = Mapper.Map<IEnumerable<MaterialDto>>(materials);

            return Ok(results);
        }

        [HttpGet("{productid}/material/{id}")]
        public IActionResult GetMaterial(int productId,int id)
        {
            var product = _productRepository.ProductExist(productId);
            if (!product)
            {
                return NotFound();
            }
            var material = _productRepository.GetMaterialForProduct(productId, id);
            if (material == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<MaterialDto>(material);
            return Ok(result);
        }
    }
}