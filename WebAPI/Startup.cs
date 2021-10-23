using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CORE.Users.Configuration;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using CORE.Users.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebAPI
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
            //services.AddCors(o => o.AddPolicy("CORS_POLICY", builder =>
            //{
            //    builder.WithOrigins("http://localhost:5001/")
            //    .SetIsOriginAllowed(origin => true).AllowAnyMethod()
            //           .AllowAnyHeader();
            //}));

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = Environment.GetEnvironmentVariable("ISSUER_TOKEN"),
                     ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE_TOKEN"),
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY")))
                 };
             });
            }
            else
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = Configuration["JWT:ISSUER_TOKEN"],
                      ValidAudience = Configuration["JWT:AUDIENCE_TOKEN"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SECRET_KEY"]))
                  };
              });
            }

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" });
            });

            //Se agregan estas líneas:
            services.AddTransient((ServiceProvider) => BridgeDBConnection<UserModel>.Create(Configuration.GetConnectionString("LocalServer"), CORE.Connection.Models.DbEnum.Sql));
            services.AddTransient((ServiceProvider) => BridgeDBConnection<LoginModel>.Create(Configuration.GetConnectionString("CloudServer"), CORE.Connection.Models.DbEnum.Sql));

            services.AddScoped<IUser, UserService>();
            services.AddScoped<ILogin, LoginService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //app.UseCors("CORS_POLICY");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
