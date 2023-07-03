using Milestones.Models;
using Milestones.Services;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Milestones
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
            services.Configure<DatabaseSettings>(Configuration.GetSection("MilestonesDatabase"));
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddCors(options => 
                options.AddPolicy("AllowAny",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
            );

            services.AddSingleton<ProjectService>(); 
            services.AddSingleton<MilestoneService>(); 
            services.AddSingleton<MemberService>();
            services.AddSwaggerGen(s => 
            {
                s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Milestones-API",
                    Description = "API for creating the milestones for a project"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Cors Allow Origins
            app.UseCors("AllowAny");
            //app.UseCors(policy => policy.WithHeaders(HeaderNames.CacheControl));

            // Begin config Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Milestones-API");
                c.RoutePrefix = string.Empty;
            });
            // End config Swagger UI

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}