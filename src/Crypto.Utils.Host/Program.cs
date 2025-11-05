var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// 添加加密工具服务
// builder.Services.AddCryptographyServices();

// 添加 Swagger 文档配置
// builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseExceptionHandling();

    // app.UseSwagger();
    // app.UseSwaggerUI(options =>
    // {
    //     options.SwaggerEndpoint("/swagger/v1/swagger.json", "Crypto Utils API v1");
    //     options.RoutePrefix = string.Empty; // 设置 Swagger UI 为根路径
    //     options.DocumentTitle = "Crypto Utils API Documentation";
    // });
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
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

app.Run();