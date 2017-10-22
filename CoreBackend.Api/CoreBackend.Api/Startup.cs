using CoreBackend.Api.Dto;
using CoreBackend.Api.Entities;
using CoreBackend.Api.Repositories;
using CoreBackend.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.Web;

namespace CoreBackend.Api
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        // ConfigureServices方法是用来把services(各种服务, 例如identity, ef, mvc等等包括第三方的, 或者自己写的)加入(register)到container(asp.net core的容器)中去, 并配置这些services. 
        // 这个container是用来进行dependency injection的(依赖注入). 所有注入的services(此外还包括一些框架已经注册好的services) 在以后写代码的时候, 都可以将它们注入(inject)进去. 
        // 例如上面的Configure方法的参数, app, env, loggerFactory都是注入进去的services.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(option => {
                    // 如果 web api提供了多种内容格式, 那么可以通过Accept Header来选择最好的内容返回格式: 例如:
                    // application / json, application / xml等等
                    // 如果设定的格式在web api里面没有, 那么web api就会使用默认的格式.
                    // asp.net core 默认提供的是json格式, 也可以配置xml等格式.
                    option.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                })
                .AddJsonOptions(option=> {
                    // asp.net core 2.0 默认返回的结果格式是Json,并使用json.net对结果默认做了camel case的转化(大概可理解为首字母小写). 
                    // 与老.net web api 不一样, 原来的 asp.net web api 默认不适用任何NamingStrategy, 需要手动加上camelcase的转化.
                    // 以下代码去除json转化后的默认的命名策略
                    if (option.SerializerSettings.ContractResolver is DefaultContractResolver resolver)
                {
                    resolver.NamingStrategy = null;
                }
            });

            var connectionString = Configuration["connectionStrings:productionInfoDbConnectionString"];
            services.AddDbContext<MyContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<IProductRepository, ProductRepository>();// 每个请求生成一个实例
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Configure方法是asp.net core程序用来具体指定如何处理每个http请求的
        // 几个方法的调用顺序: Main -> ConfigureServices -> Configure
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MyContext myContext)
        {
            //loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }
            // 向数据库中添加种子数据
            myContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            // 添加automapper映射
            AutoMapper.Mapper.Initialize(config =>
            {
                config.CreateMap<Product, ProductWithoutMaterialDto>();
                config.CreateMap<Product, ProductDto>();
                config.CreateMap<Material, MaterialDto>();
                config.CreateMap<ProductCreation, Product>();
                config.CreateMap<ProductModification, Product>();
                config.CreateMap<Product, ProductModification>();
            });

            // 在异常处理中间件后再调用mvc
            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
