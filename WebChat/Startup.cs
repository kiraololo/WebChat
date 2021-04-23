using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using WebChat.Inftastructure;
using WebChat.Inftastructure.Helpers;
//using WebChat.Models.Context;
using WebChat.Repositories.Contract;
using WebChat.Repositories.Implementation;
using WebChat.Services.Contract;
using WebChat.Services.Implementation;
using AutoMapper;
using WebChatBotsWorkerService;
using WebChatDataData.Models.Context;
using WebChat.Inftastructure.Data;
using WebChatBotsWorkerService.Services;
using WebChatBotsWorkerService.Workers;
using WebChatBotsWorkerService.BotsQueue.Implementation;

namespace WebChat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => {
                options.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: true);
                options.UseNpgsql(
                    Configuration.GetSection(Constants.Settings.ConfigSections.DataSection)
                        .GetSection(Constants.Settings.ConfigSections.ChatWebSection)
                            .GetSection(Constants.Settings.ConfigSections.ConStringSection).Value);
            });

            services.AddDbContext<AppIdentityDBContext>(options => {
                options.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: true);
                options.UseNpgsql(
                    Configuration.GetSection(Constants.Settings.ConfigSections.DataSection)
                        .GetSection(Constants.Settings.ConfigSections.ChatIdentitySection)
                            .GetSection(Constants.Settings.ConfigSections.ConStringSection).Value);
            });

            services.AddControllers();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IChatRepository, ChatRepository>();
                        
            // configure strongly typed settings objects
            var secretSettingsSection = Configuration.GetSection(
                Constants.Settings.ConfigSections.SecretSettingsSection);
            services.Configure<SecretSettings>(secretSettingsSection);

            // configure jwt authentication
            var secretSettings = secretSettingsSection.Get<SecretSettings>();
            var key = Encoding.ASCII.GetBytes(secretSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            
            services.AddHostedService<BotsTasksSchedulerService>();
            services.AddHostedService<BotsWorkerService>();            
            services.AddSingleton<AngryBotWorker>();
            services.AddSingleton<CommandBotWorker>();
            services.AddSingleton<UrlBotWorker>();
            services.AddSingleton<AngryBotTasksQueue>();
            services.AddSingleton<CommandBotTasksQueue>();
            services.AddSingleton<UrlBotTasksQueue>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SeedData.EnsurePopulated(app);
        }
    }
}
