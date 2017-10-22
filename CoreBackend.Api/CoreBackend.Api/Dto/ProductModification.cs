using System.ComponentModel.DataAnnotations;

namespace CoreBackend.Api.Dto
{
    public class ProductModification
    {
        [Display(Name = "产品名称")]
        [Required(ErrorMessage = "{0}是必填项")]
        // [MinLength(2, ErrorMessage = "{0}的最小长度是{1}")]
        // [MaxLength(10, ErrorMessage = "{0}的长度不可以超过{1}")]
        // {0}表示Display的Name属性, {1}表示当前注解的第一个变量, {2}表示当前注解的第二个变量
        [StringLength(10, MinimumLength = 2, ErrorMessage = "{0}的长度应该不小于{2}, 不大于{1}")]
        public string Name { get; set; }

        [Display(Name = "价格")]
        [Required(ErrorMessage = "{0}是必填项")]
        [Range(0, double.MaxValue, ErrorMessage = "{0}的值必须大于{1}")]
        public decimal Price { get; set; }

        [Display(Name = "描述")]
        [MaxLength(100, ErrorMessage = "{0}的长度不可以超过{1}")]
        public string Description { get; set; }
    }
}
