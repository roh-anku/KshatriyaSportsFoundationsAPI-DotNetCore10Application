using KshatriyaSportsFoundations.API.Database;
using KshatriyaSportsFoundations.API.MappingProfiles;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using KshatriyaSportsFoundations.API.Repositories.Repository;
using KshatriyaSportsFoundations.API.Utilities.BackgroundTasks;
using KshatriyaSportsFoundations.API.Utilities.SendGridEmailSender;
using KshatriyaSportsFoundations.API.Utilities.WhatsappMessageSender;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddAzureWebAppDiagnostics();

builder.Logging.SetMinimumLevel(LogLevel.Information);

string policyName = "AllowReactOnly";

builder.Services.AddCors(options =>
{
    options.AddPolicy(policyName, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5174",
                "https://www.kshatriyataekwondo.in",
                "https://kshatriyataekwondo.in"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
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

//add redis configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ksf-redis";
});
//now, redis is registered as IDistributedCache

//configure email and whatsapp configs
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddScoped<IEmailSender,SendGridEmailService>();

builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddScoped<IWhatsAppService, TwilioWhatsAppService>();

//configure background task queue
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

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
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<KshatriyaSportsFoundationsDbContext>();
        db.Database.Migrate();

        var db1 = scope.ServiceProvider.GetRequiredService<KshatriyaSportsFoundationsAuthDbContext>();
        db1.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database migration failed: " + ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(policyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
