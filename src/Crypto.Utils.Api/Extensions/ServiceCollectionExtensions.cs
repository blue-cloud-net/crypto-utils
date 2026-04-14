using Crypto.Utils.Services;
using Crypto.Utils.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Crypto.Utils.Extensions;

/// <summary>
/// 服务集合扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加证书工具服务
    /// </summary>
    public static IServiceCollection AddCertificateServices(this IServiceCollection services)
    {
        // 注册服务
        services.AddScoped<IKeyService, KeyService>();
        services.AddScoped<ICsrService, CsrService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<ICrlService, CrlService>();
        services.AddScoped<ICertificateChainService, CertificateChainService>();
        services.AddScoped<IFormatService, FormatService>();

        return services;
    }

    /// <summary>
    /// 添加 Swagger 文档配置
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "证书工具 API",
                Version = "v1",
                Description = "提供证书生成、签发、解析、验证等功能的 RESTful API",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Cert Utils",
                    Email = "support@example.com"
                }
            });

            // 启用 XML 注释
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            // 添加响应示例
            options.EnableAnnotations();
        });

        return services;
    }
}
