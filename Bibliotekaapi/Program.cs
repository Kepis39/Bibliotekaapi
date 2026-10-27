using Bibliotekaapi.Abstractions;
using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Data;
using Bibliotekaapi.Models.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddSwaggerGen();

// Добавлен IgnoreCycles, чтобы при связях таблиц библиотеки не возникало ошибки зацикливания JSON
builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions
{
    TrackStatistics = true,
    SizeLimit = 50,
}));
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICommonService<Book, string>, BooksService>();
builder.Services.AddScoped<BooksService>();

builder.Services.AddScoped<ICommonService<Reader, int>, ReadersService>();
builder.Services.AddScoped<ReadersService>();

builder.Services.AddScoped<ICommonService<Issue, int>, IssuesService>();
builder.Services.AddScoped<IssuesService>();

builder.Services.AddScoped<ICommonService<Copy, string>, CopiesService>();
builder.Services.AddScoped<CopiesService>();

builder.Services.AddScoped<ICommonService<ThematicCatalog, int>, ThematicCatalogsService>();
builder.Services.AddScoped<ThematicCatalogsService>();

builder.Services.AddScoped<ICommonService<BookCatalog, int>, BookCatalogsService>();
builder.Services.AddScoped<BookCatalogsService>();
builder.Services.AddScoped<AuthService, AuthService>();

builder.Services.AddCors();
builder.Services.AddResponseCaching();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseResponseCaching();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();