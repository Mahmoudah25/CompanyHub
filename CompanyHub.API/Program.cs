using CompanyHub.API.Middleware;
using CompanyHub.Application.Auth;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Role;
using CompanyHub.Application.User;
using CompanyHub.Infrastructure.Auth;
using CompanyHub.Infrastructure.Authorization;
using CompanyHub.Infrastructure.Service;
using CompanyHub.Persistence;
using CompanyHub.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CompanyHub.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using CompanyHub.Application.Plan;
using CompanyHub.Application.Subscription;
using CompanyHub.Application.UsageRecord;
using CompanyHub.Application.Aduit;
using CompanyHub.Application.Notification;
using Hangfire;
using CompanyHub.Infrastructure.Jobs;
using CompanyHub.Application.Payment;
using CompanyHub.Application.Dashboard;
using CompanyHub.Infrastructure.Email;
using CompanyHub.Infrastructure.Cache;
using CompanyHub.Infrastructure.Paymob;
using CompanyHub.Application.Common.Setting;
using CompanyHub.Application.Invoice;
using CompanyHub.Infrastructure.Hubs;
using CompanyHub.Application.Tenant;
using CompanyHub.Application.TwoFactor;
using CompanyHub.Infrastructure.TwoFactor;
using CompanyHub.Application.ApiKey;
using CompanyHub.Infrastructure.Storage;
using CompanyHub.Application.Report;
using CompanyHub.Infrastructure.UsageRecord;


namespace CompanyHub.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            // =======================
            //  Database
            // =======================
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });
            builder.Services.Configure<JwtSetting>(
                builder.Configuration.GetSection("JwtSetting"));
            builder.Services.Configure<EmailSetting>(
                builder.Configuration.GetSection("EmailSetting"));
            builder.Services.Configure<PayMobSetting>(
                builder.Configuration.GetSection("PaymobSetting"));
            builder.Services.AddStackExchangeRedisCache(options=>
            {
                options.Configuration = builder.Configuration["Redis:ConnectionString"];
            });

            // =======================
            //  HangFire
            // =======================
            builder.Services.AddHangfire(configuration =>
            {
                configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("cs"));
            });

            builder.Services.AddHangfireServer();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSetting:Issuer"],
                        ValidAudience = builder.Configuration["JwtSetting:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSetting:Key"] ?? string.Empty))
                    };
                });

            // =======================
            // Policies
            // =======================
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdmin", policy =>
                {
                    policy.Requirements.Add(new SuperAdminRequirment());
                });
                options.AddPolicy("User.Create", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("User.Create"));  
                });
                options.AddPolicy("User.Read", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("User.Read"));
                });
                options.AddPolicy("User.Update", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("User.Update"));
                });
                options.AddPolicy("User.Delete", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("User.Delete"));
                });

                options.AddPolicy("Role.Create", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Role.Create"));
                });

                options.AddPolicy("Role.Read", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Role.Read"));
                });

                options.AddPolicy("Role.Update", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Role.Update"));
                });

                options.AddPolicy("Role.Delete", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Role.Delete"));
                });

                options.AddPolicy("Role.AssignPermission", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Role.AssignPermission"));
                });
                options.AddPolicy("Payment.Create", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Payment.Create"));
                });
                options.AddPolicy("Payment.Read", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Payment.Read"));
                });
                options.AddPolicy("Payment.Update", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Payment.Update"));
                });

                options.AddPolicy("Aduit.Read" , policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Aduit.Read"));
                });

                options.AddPolicy("Dashboard.Read", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Dashboard.Read"));
                });

                options.AddPolicy("Report.Read", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Report.Read"));
                });


                options.AddPolicy("Substraction.Read", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Substraction.Read"));
                });
                options.AddPolicy("Substraction.Update", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Substraction.Update"));
                });
                options.AddPolicy("Substraction.Create", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("Substraction.Create"));
                });
            });

            // =======================
            //  Services
            // =======================
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.AddScoped<IApplicationDBContext, ApplicationDBContext>();
            builder.Services.AddScoped<ICurrenttenantService, CurrenttenantService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ISubscirptionService, SubscirptionService>();
            builder.Services.AddScoped<IUsageService, UsageService>();
            builder.Services.AddScoped<IAduitService,AduitService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<ISubscriptionJob, SubscriptionJob>();
            builder.Services.AddScoped<IPaymentService,PaymentService>();
            builder.Services.AddScoped<IUsageJob, UsageJobs>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IEmailService,EmailService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<ICacheService,CacheSerivce>();
            //builder.Services.AddScoped<IPaymentGateway,PayMobService>();
            builder.Services.AddHttpClient<IPaymentGateway, PayMobService>(client =>
            {
                client.BaseAddress = new Uri("https://accept.paymob.com/api/");
            });
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<INotificationSender, SignalRNotificationSender>();
            builder.Services.AddScoped<ITeanantService, TenantService>();
            builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
            builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
            builder.Services.AddScoped<IReportService, ReportService>();    
            builder.Services.AddScoped<IPlanLimitService,PlanLimitService>();
            builder.Services.AddScoped<IAuthJob, AuthJob>();
            builder.Services.AddScoped<IAduitJobs,AduitJobs>();
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler,SuperAdminHandler>();
            builder.Services.AddHealthChecks();
            // =======================
            // SignalR
            // =======================
            builder.Services.AddSignalR();
            builder.Services.AddOpenApi();

             // =======================
            // CORS
            // =======================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .SetIsOriginAllowed(_ => true);
                });
            });

            //builder.Services.AddHttpContextAccessor();
            // =======================
            // Swagger
            // =======================
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CompanyHub",
                    Description = "ASP.NET Core Web API"
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] { }
                    }
                });
            });

 
            var app = builder.Build();

          
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                context.Database.Migrate();
                await PermissionSeeder.SeedAsync(context);
                await SuperAdminSeeder.SeddAsync(context);
            }

            app.MapHealthChecks("/health").AllowAnonymous();
            app.UseHangfireDashboard();
            // =======================
            // Make Hangfire jobs
            // =======================
            RecurringJob.AddOrUpdate<ISubscriptionJob>(
                 "ExpiredSubscriptionJob", x => x.ExpireSubscriptions(), Cron.Daily);

            RecurringJob.AddOrUpdate<ISubscriptionJob>(
                "SubscriptionReminderJob", x => x.SubscriptionReminder(), Cron.Daily);

            RecurringJob.AddOrUpdate<IUsageJob>(
                "RefreshUsage", x => x.Execute(), Cron.Hourly);

            RecurringJob.AddOrUpdate<IAuthJob>(
                "TokenCleanJob", x => x.ToeknClaenJob(), Cron.Daily);

            RecurringJob.AddOrUpdate<IAuthJob>(
                "EmailVerficationCleanJob", x => x.EmailVerficationCleanJob(), Cron.Daily);

            RecurringJob.AddOrUpdate<ISubscriptionJob>(
                "PlanLimitAlertJob", x => x.PlanLimitAlertJob(), Cron.Daily);

            RecurringJob.AddOrUpdate<IAduitJobs>(
                "AuditLogCleanJob", x => x.AuditLogCleanJob(), Cron.Daily);
            // =======================
            // Middleware
            // =======================
            app.UseMiddleware<GlobalExpectionHandler>();
            app.UseMiddleware<ApiKeyMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}
