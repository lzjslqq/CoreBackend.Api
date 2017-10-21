using CoreBackend.Api.Dto;
using CoreBackend.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CoreBackend.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        // Logger是asp.net core 的内置service，所以我们就不需要在ConfigureService里面注册.
        // Container可以直接提供一个ILogger<T>的实例
        private readonly ILogger<ProductController> _logger;

        private readonly IMailService _mailService;

        public ProductController(ILogger<ProductController> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        } 

        [HttpGet("all")]
        [Route("[action]")]
        public IActionResult GetProducts()
        {
            var result = Ok(ProductService.Current.Products);
           
            return result;
        }

        //[Route("{id:int}")]
        [HttpGet("{id:int}",Name = "GetProduct")]
        public IActionResult GetProduct(int id)
        {
            try
            {
                var result = ProductService.Current.Products.SingleOrDefault(p => p.Id == id);
                if (result == null)
                {
                    _logger.LogInformation($"没有找到Id为{id}的产品！");
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"查找Id为{id}的产品时出现了错误!!", ex);
                return StatusCode(500, "处理请求的时候发生了错误！");
            }
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

            var maxId = ProductService.Current.Products.Max(p => p.Id);
            // 手动映射，后期改为AutoMapper
            var newProduct = new ProductDto
            {
                Id = ++maxId ,
                Name = product.Name,
                Price = product.Price
            };

            ProductService.Current.Products.Add(newProduct);
            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]  // 整体更新
        public IActionResult Put(int id , [FromBody] ProductModification product)
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

            var model = ProductService.Current.Products.SingleOrDefault(p => p.Id == id);
            if (model == null)
            {
                return NotFound(666);
            }

            model.Name = product.Name;
            model.Price = product.Price;

            //return Ok(model);
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

            var model = ProductService.Current.Products.SingleOrDefault(p => p.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            var toPatch = new ProductModification
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price
            };

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

            model.Name = toPatch.Name;
            model.Description = toPatch.Description;
            model.Price = toPatch.Price;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            ProductService.Current.Products.Remove(model);
            _mailService.Send("Product has been deleted", $"id 为{id}的产品被删除了");
            return NoContent();
        }
    }
}