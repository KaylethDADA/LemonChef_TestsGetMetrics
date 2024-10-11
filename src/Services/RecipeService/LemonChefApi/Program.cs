using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Infrastructure.Dal.Repositories;
using LemonChefApi.Identity;
using LemonChefApi.Settings;
using LemonChefApi.TestsMetrics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace LemonChefApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
            options.Password.RequireLowercase = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
        });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "LemonChef",
                Version = "v1",
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Please paste your auth token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        // регистрации метрик
        builder.Services.AddMetricServer(options =>
        {
            options.Port = 9090;
        });
        builder.Services.AddScoped<MetricsBase>();


        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));
        
        builder.Services.AddTransient<IIngredientService,IngredientService>();
        builder.Services.AddTransient<IIngredientRepository, IngredientRepository>();
        
        builder.Services.AddTransient<IRecipeService,RecipeService>();
        builder.Services.AddTransient<IRecipeRepository, RecipeRepository>();
        

        builder.Services.AddScoped<ITimestampService, TimestampService>();
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        builder.Services.AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<RecipesDbContext>()
                .AddDefaultTokenProviders();
            
        
        builder.Services.AddDbContext<RecipesDbContext>
            (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseCamelCaseNamingConvention());
        
        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //Добавляем middleware Prometheus для сбора метрик
        app.UseMetricServer();
        //add middleware
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapIdentityApi<User>();
        
        app.MapControllers();

        app.Run();
    }
}