using KshatriyaSportsFoundations.API.Database;
using KshatriyaSportsFoundations.API.MappingProfiles;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using KshatriyaSportsFoundations.API.Repositories.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string policyName = "AllowReactOnly";
builder.Services.AddCors(options => {
    options.AddPolicy(policyName, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://white-smoke-0f7217000.6.azurestaticapps.net")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

//inject DB connection strings
builder.Services.AddDbContext<KshatriyaSportsFoundationsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KshatriyaSportsFoundationsDbConnectionString")));

builder.Services.AddDbContext<KshatriyaSportsFoundationsAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KshatriyaSportsFoundationsAuthDbConnectionString")));

//inject services
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

//adding auto mapper config
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

//set up identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("KshatriyaSportsFoundations")
    .AddEntityFrameworkStores<KshatriyaSportsFoundationsAuthDbContext>()
    .AddDefaultTokenProviders();

//configure password 
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

//add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KshatriyaSportsFoundationsDbContext>();
    db.Database.Migrate();

    var db1 = scope.ServiceProvider.GetRequiredService<KshatriyaSportsFoundationsAuthDbContext>();
    db1.Database.Migrate();
}

app.UseCors(policyName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
