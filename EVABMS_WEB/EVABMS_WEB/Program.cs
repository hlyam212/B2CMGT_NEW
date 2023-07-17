using WebCommonHelper;

var builder = WebApplication.CreateBuilder(args);

// Get IConfiguration
var config = builder.Configuration;

// ���U Cors �A��
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

// ���U Web�@�� �A��
builder.Services.AddWebCommonHelper();

// ���U Client���� �A��
builder.Services.AddClientHelper(config, "EVABMS");

// ���U Web�h �A��
builder.Services.AddWebHelper(config);

// ���U Controller
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

// �ϥ� Web�h �A��
app.UseWebHelper();

// �ϥ� Web�@�� �A��
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
