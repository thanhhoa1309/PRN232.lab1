using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prn232.Lab1.Repositories;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Service;
using Prn232.Lab1.Service.Utils;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace FUNewsManagementSystem.Architecture
{
    public static class IocContainer
    {
        public static IServiceCollection SetupIocContainer(this IServiceCollection services)
        {
            var configuration = GetConfiguration();

            services.SetupApiVersioning();
            services.SetupDbContext(configuration);
            services.SetupSwagger();

            // Add generic repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add HttpContextAccessor
            services.AddHttpContextAccessor();

            // Add business services
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ISeedDataService, SeedDataService>();
            services.AddSingleton<PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            // Add JWT Authentication
            services.SetupJwt(configuration);

            return services;
        }

        private static IServiceCollection SetupApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static IServiceCollection SetupDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
            }

            services.AddDbContext<Prn232Lab1DbContext>(options =>
                options.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(typeof(Prn232Lab1DbContext).Assembly.FullName)
                )
            );

            return services;
        }

        private static IServiceCollection SetupSwagger(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(c =>
            {
                c.UseInlineDefinitionsForEnums();

                // JWT Authentication configuration for Swagger
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter 'Bearer' [space] and then your valid JWT token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
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
                };

                c.AddSecurityRequirement(securityRequirement);

                c.UseAllOfForInheritance();
                c.EnableAnnotations();
            });

            return services;
        }

        private static IServiceCollection SetupJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration["JWT:SecretKey"];
            var issuer = configuration["JWT:Issuer"];
            var audience = configuration["JWT:Audience"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT:SecretKey not found in appsettings.json");
            }

            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("JWT:Issuer not found in appsettings.json");
            }

            if (string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT:Audience not found in appsettings.json");
            }

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false; // Set to true in production with HTTPS
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero // Remove delay of token when expire
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("StaffPolicy", policy =>
                    policy.RequireRole("Staff", "Admin"));

                options.AddPolicy("LecturerPolicy", policy =>
                    policy.RequireRole("Lecturer", "Admin"));
            });

            return services;
        }
    }
}
