using WebCommonHelper;

var builder = WebApplication.CreateBuilder(args);

// Get IConfiguration
var config = builder.Configuration;

// 註冊 Cors 服務
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "EVABMS_WEB_POLICY",
        builder =>
        {
            builder
            .WithOrigins(config.GetSection("AllowOrigins").Get<string[]>()).SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// 註冊 Web共用 服務
builder.Services.AddWebCommonHelper();

// 註冊 Client相關 服務
builder.Services.AddClientHelper(config, "EVABMS");

// 註冊 Web層 服務
builder.Services.AddWebHelper(config);

// 註冊 Controller
builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("EVABMS_WEB_POLICY");

// 使用 Web層 服務
app.UseWebHelper();

// 使用 Web共用 服務
app.UseWebCommonHelper();

if (app.Environment.IsDevelopment())
{
    app.UseEndpoints(endpoints =>
    {
        // you app routes...
        endpoints.MapControllers();
    });
}
else
{
    app.UseStaticFiles();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action=Index}/{id?}");

        endpoints.MapFallbackToFile("index.html");
    });
}

app.Run();
