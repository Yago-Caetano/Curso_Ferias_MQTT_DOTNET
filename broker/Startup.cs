using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MQTTnet.Server;
using MQTTnet.AspNetCore.Extensions;

namespace tutorial
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "tutorial", Version = "v1" });
            });

            var optionsBuilder = new MqttServerOptionsBuilder()
                                    .WithApplicationMessageInterceptor(context =>
                                    {
                                        if (context.ApplicationMessage.Topic == "teste/topic1")
                                        {
                                            String inPayload = Encoding.UTF8.GetString(
                                                context.ApplicationMessage.Payload,0,context.ApplicationMessage.Payload.Length);
                                            Console.WriteLine("Mensagem Recebida!! {0}",inPayload);

                                        }
                                    });

            
            
            services.AddHostedMqttServer(optionsBuilder.Build())
            .AddMqttConnectionHandler()
            .AddConnections();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "tutorial v1"));
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseMqttServer(async server =>{});
        }
    }
}
