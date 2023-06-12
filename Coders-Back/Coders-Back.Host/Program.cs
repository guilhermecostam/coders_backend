using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;
using Coders_Back.Domain.Services;
using Coders_Back.Infrastructure.DataAbstractions;
using Coders_Back.Infrastructure.EntityFramework.Context;
using Coders_Back.Infrastructure.Extensions;
using Coders_Back.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;  

// Add services to the container.
services.AddControllers();

services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication(builder.Configuration);

#region DI

services.AddScoped<IIdentityService, IdentityService>();
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddTransient<IProjectService, ProjectService>();
services.AddTransient<IRequestService, RequestService>();

#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() 
    { 
        Name = "Authorization", 
        Type = SecuritySchemeType.ApiKey, 
        Scheme = "Bearer", 
        BearerFormat = "JWT", 
        In = ParameterLocation.Header, 
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.Example: Bearer 12345abcdef", 
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
            new string[] {} 
        } 
    }); 
});

builder.Host.UseSerilog((_, lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();