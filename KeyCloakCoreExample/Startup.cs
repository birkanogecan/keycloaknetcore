using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;


namespace test
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    // Store the session to cookies
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    // OpenId authentication
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect(options =>
                {
                    // URL of the Keycloak server
                    options.Authority = "http://localhost:32770/auth/realms/master";
                    // Client configured in the Keycloak
                    options.ClientId = "test";
                
                    
                    // For testing we disable https (should be true for production)
                    options.RequireHttpsMetadata = false;
                    options.SaveTokens = true;

                    // Client secret shared with Keycloak
                    options.ClientSecret = "eab41c16-a86b-4347-b973-e64e8eb1aa5f";
                    options.GetClaimsFromUserInfoEndpoint = true;

                    // OpenID flow to use
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Add this line to ensure authentication is enabled
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}