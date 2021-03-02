using animalFinder.Service;
using animalFinder.Service.Interface;
using DAL;
using DAL.Provider;
using DAL.Provider.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using animalFinder.SettingsObject;
using DAL.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace animalFinder
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();

            #region support for razor rendering

            services.AddRazorPages();
            services.AddMvc();

            #endregion

            #region get settings

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));

            #endregion

            #region custom services

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IAnimalDataProvider, AnimalDataProvider>();
            services.AddTransient<IFileDataProvider, FileDataProvider>();
            services.AddTransient<IAnimalService, AnimalService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IConfirmationCodeService, ConfirmationCodeService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IJWTService, JWTService>();
            services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPrivateProfileService, PrivateProfileService>();
            services.AddTransient<IRegisteredAnimalProvider, RegisteredAnimalProvider>();
            services.AddTransient<IGeo, Geo>();

            #endregion

            #region define data prodivers

            services.AddTransient<ITokenDataProvider, TokenDataProvider>();
            services.AddTransient<IUserDataProvider, UserDataProvider>();

            #endregion

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();


            #region Setup a JWT

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["JWTSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWTSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            Configuration["JWTSettings:Bytes"]
                            )
                        ),
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs")))
                        {
                            context.Token = accessToken;
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });

            #endregion

            #region CORS

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                        builder.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); ;
            });

            #endregion

            #region Database

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<Context>(options => options.UseSqlServer(connection));

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else 
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
