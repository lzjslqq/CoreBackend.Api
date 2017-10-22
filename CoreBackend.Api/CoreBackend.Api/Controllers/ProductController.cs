using AutoMapper;
using CoreBackend.Api.Dto;
using CoreBackend.Api.Entities;
using CoreBackend.Api.Repositories;
using CoreBackend.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreBackend.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        #region 字段及构造函数

        // Logger是asp.net core 的内置service，所以我们就不需要在ConfigureService里面注册.
        // Container可以直接提供一个ILogger<T>的实例
        private readonly ILogger<ProductController> _logger;
        private readonly IMailService _mailService;
        private readonly IProductRepository _productRepository;

        public ProductController(ILogger<ProductController> logger, IMailService mailService, IProductRepository productRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _productRepository = productRepository;
        }  

        #endregion

        [HttpGet("all")]
        [Route("[action]")]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var results = Mapper.Map<IEnumerable<ProductWithoutMaterialDto>>(products);
           
            return Ok(results);
        }

        //[Route("{id:int}")]
        [HttpGet("{id:int}",Name = "GetProduct")]
        public IActionResult GetProduct(int id , bool includeMaterial = false)
        {
            var product = _productRepository.GetProduct(id, includeMaterial);
            if(product == null)
            {
                return NotFound();
            }
            if (includeMaterial)
            {
                var productWithMaterialResult = Mapper.Map<ProductDto>(product);
                return Ok(productWithMaterialResult);
            }

            var onlyProductResult = Mapper.Map<ProductWithoutMaterialDto>(product);
            return Ok(onlyProductResult);
        }

        [HttpPost(Name = "CreateProduct")]
        public IActionResult Post([FromBody] ProductCreation product)
        {
            if (product == null)
                return BadRequest();

            if (product.Name == "你妹")
            {
                // 后期用FluentValidation实现验证逻辑
                ModelState.AddModelError("Name", "你妹咯！");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newProduct = Mapper.Map<Product>(product);
            _productRepository.AddProduct(newProduct);
            if (!_productRepository.Save())
            {
                _logger.LogCritical($"【保存产品{product.Name}的时候出错】");
                return StatusCode(500, "保存产品的时候出错");
            }

            var dto = Mapper.Map<ProductWithoutMaterialDto>(newProduct);
            return CreatedAtRoute("GetProduct", new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]  // 整体更新
        public IActionResult Put(int id , [FromBody] ProductModification productModificationDto)
        {
            if (productModificationDto == null)
                return BadRequest();

            if (productModificationDto.Name == "你妹")
            {
                // 后期用FluentValidation实现验证逻辑
                ModelState.AddModelError("Name", "你妹咯！");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound(666);
            }

            // 把第一个对象相应的值赋给第二个对象,此时product的state变成了modified
            Mapper.Map(productModificationDto, product);
            if (!_productRepository.Save())
            {
                _logger.LogCritical($"【修改产品{product.Name}的时候出错】");
                return StatusCode(500, "保存产品的时候出错");
            }
            return NoContent();
        }

        /*     Http Patch用于部分更新,它的Request Body应包含需要更新的属性名和值,甚至也可以包含针对这个属性要进行的相应操作.
           针对Request Body这种情况, 标准叫做 Json Patch RFC 6092, 它定义了一种json数据的结构 可以表示上面说的那些东西. 
           Json Patch定义的操作包含替换, 复制, 移除等操作.针对这里的product,Request Body数据结构应类似这样：
           [
              {
                "op":"replace",
                "path":"/name",
                "value":"new Name"
              },
              {
                "op":"replace",
                "path":"/description",
                "value":"new desc"
              }
           ]

           其中的 op 表示操作, replace 是指替换; path就是属性名, value就是值.
         */
        [HttpPatch("{id}")]   // 部分更新
        public IActionResult Patch(int id,[FromBody] JsonPatchDocument<ProductModification> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            var toPatch = Mapper.Map<ProductModification>(product);

            // 将请求传入的数据更新到toPatch对象中,并验证是否有错
            patchDocument.ApplyTo(toPatch, ModelState);

            if (toPatch.Name == "aaa")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'aaa'关键字!");
            }

            // 使用TryValidateModel(xxx)对model进行手动验证, 结果也会反应在ModelState里面
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                // 更新时可能出错，比如更新不存在的属性，是客户端引起的错误
                return BadRequest(ModelState);
            }

            Mapper.Map(toPatch, product);
            if (!_productRepository.Save())
            {
                _logger.LogCritical($"【修改产品{product.Name}的时候出错】");
                return StatusCode(500, "更新的时候出错");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            _productRepository.DeleteProduct(product);
            if (!_productRepository.Save())
            {
                _logger.LogCritical($"【删除产品{product.Name}的时候出错】");
                return StatusCode(500, "删除的时候出错");
            }
            _mailService.Send("Product has been deleted", $"id 为{id}的产品被删除了");
            return NoContent();
        }
    }
}