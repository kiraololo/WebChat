using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using WebChat.Inftastructure;
using WebChat.Inftastructure.Data;
using WebChat.Inftastructure.Helpers;
using WebChat.Services.Contract;
using WebChat.Services.Implementation;
using WebChatBotsWorkerService;
using WebChatBotsWorkerService.BotsQueue.Contract;
using WebChatBotsWorkerService.BotsQueue.Implementation;
using WebChatBotsWorkerService.Helpers;
using WebChatBotsWorkerService.Services;
using WebChatBotsWorkerService.Workers;
using WebChatDataData.Models.Context;

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

            services.AddScoped<IChatService, ChatService>();

            // configure strongly typed settings objects
            var secretSettingsSection = Configuration.GetSection(SecretSettings.SecretSettingsSection);
            services.Configure<SecretSettings>(secretSettingsSection);

            var botsSettingsSection = Configuration.GetSection(BotsSettings.BotsSettingsSection);
            services.Configure<BotsSettings>(botsSettingsSection);

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
            services.AddHostedService<BotsWorkerService>();
            services.AddHostedService<BotsTasksSchedulerService>();            
            services.AddSingleton<IBotsTasksQueue, BotTasksQueue>();
            services.AddSingleton<IBot, MessageBot>();
            services.AddSingleton<IBot, DelayBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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
            loggerFactory.AddLog4Net();
            SeedData.EnsurePopulated(app);
        }
    }
}
