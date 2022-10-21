/*
 * Starting point of the application. The main method in this class contains the code below.
 * New feature. We dont have to write namespace and class name in Program.cs file.
 * The compiler will generate this.
 */

using CityInfo.API.DBContext;
using CityInfo.API.Infrastructure;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;

Log.Logger = new LoggerConfiguration() // initialize custom logger
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("log/cityinfo.txt", rollingInterval: RollingInterval.Day) // every day a new log file
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args); // used to build a webapplication
//builder.Logging.ClearProviders(); // manually set after clearing the logging settings
//builder.Logging.AddConsole();

builder.Host.UseSerilog(); // used the custom logger

// Add services to the container.

/*
 * This is the buildin dependency injector. 
 * By defining the services here. We can inject them where ever we want in our code;
 */
builder.Services.AddControllers(
    options =>
    {
        // options.InputFormatters by manipulating input or output formatters you can decide what the default should be;
        options.ReturnHttpNotAcceptable = true; // if type of Accept header is not support give error message instead of sending default type
    }).AddNewtonsoftJson() // replaces the default input and output formatters to json.net
    .AddXmlDataContractSerializerFormatters(); // Adds xml input and output formatters;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // shows api documentation
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

    setupAction.IncludeXmlComments(xmlCommentsFullPath);
    setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "CityInfoApiBearerAuth" }
        }, new List<string>() }
    });

}); // shows api documentation
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); // Used to define file type

// Transient lifetime is created each time the service is requested. Best for lightweigt stateless service
// Scoped lifetime services are created once per request
// Singleton lifetime are created the first time it is requested, every subsequent request will use the same instance

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddSingleton<CitiesDataStore>();

builder.Services.AddDbContext<CityInfoContext>(
    dbContextOptions => dbContextOptions.
    UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"])); // adds context with a scoped life time
// a safe location for a connection string is environment variable or azure key vault.
// Note environment variables override any other connection string
// Furthermore environment variables are not encrypted and thus not safe enough. 
// If someone get access to the machine, he would be able to get the value
// azure keyvault is better option

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>(); // for a repository scoped lifetime is best. just like a dbcontext it holds reference to
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// These assemblies scan for profiles that are configured for mapping
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
            };
        }
    );

// Create an authorization policy abac: attribute based access control:
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromAntwerp", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Antwerp");
    });
});

builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true; // if no version given assume default
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Api docs is shown only in development and not in production environment
{
    app.UseSwagger(); // shows api documentation
    app.UseSwaggerUI(); // shows api documentation
}

app.UseHttpsRedirection();

app.MapControllers();
/*
 * To Use endpoint routing inject:
 * app.UseRouting();
 * app.UseEndpoints();
 */
app.UseRouting(); // Marks the position in the middleware pipline where routing decison is made -> Where an endpoint is selected;

app.UseAuthentication(); // authenticate before going to endpoint

app.UseAuthorization(); // Between UseRouting and UseEndpoints middlware -> Between selecting and executing the endpoint

app.UseEndpoints(endpoints => // Marks the position in the middleware where the selected endpoint is executed
{
    endpoints.MapControllers(); // Prefered way for api attribute based instead of convention based;
});
/*
 * Attribute based routing makes it possible to use attributes at controller level: [Route], [HttpGet]
 * Combined with URI template, requests are matched to controller actions;
 * 
 * [Route] -> use it at controller level to provide a template that will prefix all templates defined at action level example:
 * [Route("api/[controller]")] or [Route("api/cities")]
 * 
 */
// app.MapControllers(); same as UseRouting and UseEndpoints; comes after UseAuthorization and UseAuthentication 

//app.Run(async (context) => // a request delegate every endpoint will 
//{
//    await context.Response.WriteAsync("Hello World!");
//});

app.Run();
