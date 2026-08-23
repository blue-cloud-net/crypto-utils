using Crypto.Utils.Extensions;
using Crypto.Utils.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// 添加加密工具服务（DI 注册）
builder.Services.AddCertificateServices();

// 添加 Swagger 文档配置
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 全局异常处理中间件
    app.UseExceptionHandling();

    // Swagger UI（根路径）
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Crypto Utils API v1");
        options.RoutePrefix = string.Empty; // 设置 Swagger UI 为根路径
        options.DocumentTitle = "Crypto Utils API Documentation";
    });
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    // 生产环境强制 HTTPS；开发环境走本地 HTTP，避免 Vite 代理触发重定向
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

// 欢迎信息
app.MapGet("/health", () =>
        Results.Ok(new
        {
            Status = "Healthy",
            Service = "Crypto Utils API",
            Version = "1.0.0",
            Timestamp = DateTime.UtcNow
        }))
    .WithTags("Health")
    .WithOpenApi();

app.MapControllers();

// 开发环境 SPA 代理由 Microsoft.AspNetCore.SpaProxy 的 HostingStartup 自动注册
// （读取构建生成的 spa.proxy.json，见 Crypto.Utils.Host.csproj 的 SpaProxy* 属性）

app.Run();