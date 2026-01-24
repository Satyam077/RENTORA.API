//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using MongoDB.Driver;
//using RENTORA.API.Models.MongoDB;
//using RENTORA.API.Repository;
//using RENTORA.API.Repository.IRepository;
//using RENTORA.API.Services;
//using RENTORA.API.Services.IServices;
//using RENTORA.API.WebSettings;
//using System.Text;
//using System.IO;
//using Microsoft.AspNetCore.Authentication;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

//builder.Services.AddSingleton<MongoDbSettings>(sp =>
//{
//    var config = sp.GetRequiredService<IConfiguration>();
//    return new MongoDbSettings(config);
//});
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Sendgrid"));

//builder.Services.Configure<JwtSettings>(
//    builder.Configuration.GetSection("Jwt"));

//var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
//var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = true,
//        ValidIssuer = jwtSettings.Issuer,
//        ValidateAudience = true,
//        ValidAudience = jwtSettings.Audience,
//        ValidateLifetime = true,
//        ClockSkew = TimeSpan.Zero
//    };
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//{
//    options.Cookie.Name = "auth_cookie";
//    options.Cookie.MaxAge = TimeSpan.FromHours(12);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.Cookie.SameSite = SameSiteMode.Strict;

//    options.LoginPath = "/login";
//    options.LogoutPath = "/logout";
//    options.AccessDeniedPath = "/access-denied";

//    options.ExpireTimeSpan = TimeSpan.FromHours(12);
//    options.SlidingExpiration = true;
//});

////var google = builder.Configuration.GetSection("Authentication:Google");
////builder.Services.AddAuthentication()
////    .AddGoogle(options =>
////    {
////        options.ClientId = google["ClientId"]!;
////        options.ClientSecret = google["SecretKey"]!;
////        options.CallbackPath = "/auth/google/callback";
////    });

//builder.Services.AddAuthorization();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
//builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
//builder.Services.AddScoped<IUnitsRepository, UnitsRepository>();
//builder.Services.AddScoped<ITenantsRepository, TenantsRepository>();
//builder.Services.AddScoped<IAgreementRepository, AgreementRepository>();
//builder.Services.AddScoped<IFileUploadService, FileUploadService>();
//builder.Services.AddScoped<IMaintenanceRepositoy, MaintenanceRepositoy>();



//// Add CORS with credentials support for cookie authentication
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", builder =>
//    {
//        builder.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//    });

//    options.AddPolicy("AllowCredentials", builder =>
//    {
//        builder.WithOrigins("http://localhost:4200", "https://localhost:4200", "https://agent-696688dc22deb24f8ed59d3c--rentora-ui.netlify.app") // Add your frontend URLs
//               .AllowAnyMethod()
//               .AllowAnyHeader()
//               .AllowCredentials();
//    });
//});

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "RENTORA API",
//        Version = "v1",
//        Description = "Property Management System API"
//    });

//    // Add JWT Authentication to Swagger
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseHttpsRedirection();

//// Configure static files for uploaded images
//var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
//if (!Directory.Exists(wwwrootPath))
//{
//    Directory.CreateDirectory(wwwrootPath);
//}
//app.UseStaticFiles();

//app.UseCors("AllowAll");

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.Services;
using RENTORA.API.Services.IServices;
using RENTORA.API.WebSettings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- CONFIG --------------------
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<MongoDbSettings>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new MongoDbSettings(config);
});

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Sendgrid"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>();

var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

// -------------------- AUTH --------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "rentora_auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    // 🔑 REQUIRED FOR ANGULAR (cross-domain)
    options.Cookie.SameSite = SameSiteMode.None;

    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.SlidingExpiration = true;
});

// -------------------- CORS (ONLY ONE POLICY) --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://rentora.tryasp.net",
                "https://rentora-ui.netlify.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// -------------------- SERVICES --------------------
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IUnitsRepository, UnitsRepository>();
builder.Services.AddScoped<ITenantsRepository, TenantsRepository>();
builder.Services.AddScoped<IAgreementRepository, AgreementRepository>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IMaintenanceRepositoy, MaintenanceRepositoy>();
builder.Services.AddScoped<IFeaturesRepository, FeaturesRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

// -------------------- SWAGGER (DEV ONLY) --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RENTORA API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// -------------------- PIPELINE --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AngularPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Rentora API running");
app.MapControllers();

app.Run();

