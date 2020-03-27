using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASOS.Api.Data;
using ASOS.Api.Security;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ASOS.Api
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
      services.AddCors();
      services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = OAuthValidationDefaults.AuthenticationScheme;
          options.DefaultForbidScheme = OAuthValidationDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = OAuthValidationDefaults.AuthenticationScheme;
        })
        .AddOAuthValidation()
        .AddOpenIdConnectServer(options =>
        {
          options.TokenEndpointPath = "/authenticate";
          options.Provider = new AuthorizationProvider();
        });
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddSingleton<IMongoClient>(x => new MongoClient(Configuration.GetConnectionString("DefaultConnection")));
      services.AddScoped(x =>
        new DataContext(x.GetRequiredService<IMongoClient>(), Configuration.GetSection("MongoDb:Database").Value));
      services.AddAutoMapper(typeof(Startup));
      var mappingConfig = new MapperConfiguration(mc =>
      {
        mc.AddProfile(new MappingProfile());
      });

      var mapper = mappingConfig.CreateMapper();
      services.AddSingleton(mapper);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      UseCorsPolicy(app);
      app.UseHttpsRedirection();
      app.UseAuthentication();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }

    private static void UseCorsPolicy(IApplicationBuilder app)
    {
      app.UseCors(policy =>
      {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
      });
    }

  }
}
