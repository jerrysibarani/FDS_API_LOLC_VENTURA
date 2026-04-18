using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using API.Helpers;
using API.Data.Entities;
using API.IServices;
using API.Services;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.WebRootPath == null)
{
    builder.Environment.WebRootPath = builder.Environment.ContentRootPath;
    //builder.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Ensure configuration is registered
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Register the decryption service.
builder.Services.AddScoped<IConnectionDB, ConnectionDB>();

// Check if connection string encryption is enabled.
var settings = builder.Configuration.GetSection("MySettings").Get<MySettings>();
string rawConnectionString = builder.Configuration.GetConnectionString("DBConfig")!;
bool connectionStringEncryption = false;
if (settings != null && settings.IsEncryption)
{
    connectionStringEncryption = true;
}

string decryptedConnectionString;
if (connectionStringEncryption == false)
{
    decryptedConnectionString = rawConnectionString;
}
else
{
    using (IServiceScope scope = builder.Services.BuildServiceProvider()!.CreateScope())
    {
        var decryptor = scope.ServiceProvider.GetRequiredService<IConnectionDB>();
        var encryptedConnectionString = rawConnectionString;
        decryptedConnectionString = decryptor.DecryptionDB(encryptedConnectionString!);
    }
}

builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Update the existing line to fix the error.
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(decryptedConnectionString), ServiceLifetime.Scoped);

// Configure Identity and Authentication
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Secret").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});


// Injection Repository
InjectionServices.ConfigureRepositories(builder.Services);


//MaxUpload
const int maxRequestLimit = 209715200;
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = maxRequestLimit;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = maxRequestLimit; // if don't set default value is: 30 MB
});

//Versioning 
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    //opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
    //                                                new HeaderApiVersionReader("x-api-version"),
    //                                                new MediaTypeApiVersionReader("x-api-version"));
});
// Add ApiExplorer to discover versions
//builder.Services.AddVersionedApiExplorer(setup =>
//{
//    setup.GroupNameFormat = "'v'VVV";
//    setup.SubstituteApiVersionInUrl = true;
//});

// Configure Swagger to include bearer token support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fidusia API", Version = "v1" });

    // Define the BearerAuth security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add a reference to the security scheme to the global operation filter
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);


// ✅ 3️⃣ Add Authorization Policy (DI SINI TEMPATNYA)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotExternal", policy =>
        policy.RequireAssertion(context =>
        {
            var userType = context.User.FindFirst("UserType")?.Value;
            // ❌ hanya External yang dilarang
            return !string.IsNullOrEmpty(userType) && userType != ConstantaData.EXTERNAL;
        }));
});


var corsAllow = builder.Configuration.GetSection("AppSettings:CorsAllowAll").Value ?? "false";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
