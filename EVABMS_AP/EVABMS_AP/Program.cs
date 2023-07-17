using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EVABMS_AP.Extenstion;
using LogHelper;
using Microsoft.Extensions.Options;
using NLog.Web;
using OracleHelper.TransactSql;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WebCommonHelper;

var builder = WebApplication.CreateBuilder(args);

// Get IConfiguration
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(option =>
{
    //未提供版本請請時，使用預設版號
    option.AssumeDefaultVersionWhenUnspecified = true;

    //預設api版本號，支援時間或數字版本號 
    option.DefaultApiVersion = new ApiVersion(1, 0);

    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
    option.ReportApiVersions = true;
}).AddMvc().AddApiExplorer(options =>
{
    // the default is ToString(), but we want "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";

    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptionsMiddleware>();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApHelper(config);

//LogHelper
//builder.AddNLog();
builder.AddSerilog(config);

//Add Oracle Config
builder.Services.AddOraConfiguration(config);


var app = builder.Build();

// 使用 AP層 服務
app.UseApHelper();

// 使用 Web共用 服務
app.UseWebCommonHelper();

app.UseHttpsRedirection();

app.UseAuthorization();

var swaggerJsonRoot = "";
if (app.Environment.IsDevelopment())
{
    swaggerJsonRoot = "/swagger/";
}
else
{
    swaggerJsonRoot = "/EVABMS_AP/swagger/";
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in app.Services.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions.OrderByDescending(x => x.ApiVersion.MajorVersion))
    {
        string docName = description.GroupName.ToUpperInvariant();
        if (description.ApiVersion.MajorVersion == 1) docName = $"BMS ({description.GroupName.ToUpperInvariant()})";
        if (description.ApiVersion.MajorVersion == 2) docName = $"P13 ({description.GroupName.ToUpperInvariant()})";
        options.SwaggerEndpoint($"{swaggerJsonRoot}{description.GroupName}/swagger.json", docName);
    }
});

app.UseSerilogRequestLogging(options =>
{
    // Customize the message template
    options.MessageTemplate = "Handled {RequestPath}";

    // Emit debug-level events instead of the defaults
    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

    // Attach additional properties to the request completion event
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

app.MapControllers();

app.Run();
