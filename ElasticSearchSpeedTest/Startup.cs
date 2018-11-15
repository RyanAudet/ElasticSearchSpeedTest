using ElasticSearchSpeedTest.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace ElasticSearchSpeedTest
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IElasticSearchService, ElasticSearchService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseMvc();

            // Using a workaround as per:
            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/678#issuecomment-394999841

            //const string swaggerPrefix = "api/docs";
            const string swaggerPrefix = "swagger";

            app.UseRewriter(new RewriteOptions().AddRedirect($"(.*){swaggerPrefix}$", $"$1{swaggerPrefix}/index.html"));

            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"{swaggerPrefix}/{{documentName}}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = swaggerPrefix;
                c.SwaggerEndpoint("v1/swagger.json", "My API (V1)");
            });
        }
    }
}
