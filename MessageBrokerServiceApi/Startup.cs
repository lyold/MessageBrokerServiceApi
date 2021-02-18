using MessageBrokerServiceApi.Common.Helper;
using MessageBrokerServiceApi.Common.Helper.Interfaces;
using MessageBrokerServiceApi.Common.Middlewares;
using MessageBrokerServiceApi.Domain.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace MessageBrokerServiceApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IHelloWordService _service;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Service MessageBroker",
                        Version = "v1",
                        Description = "Projeto de demonstração de comunicação entre serviços utilizando RabbitMQ.",
                        Contact = new OpenApiContact()
                        {
                            Name = "Rodrigo de Almeida Gustavo",
                            Email = "r.almeidagustavo@gmail.com"
                        }
                    });
                })
                .AddControllers();

            services.AddSingleton<IRabbitMqHelper, RabbitMqHelper>();
            services.AddHostedService<BrokerReceiver>();
            services.AddScoped<IHelloWordService, HelloWordService>();

            services.AddMvc();

            var sp = services.BuildServiceProvider();
            _service = sp.GetService<IHelloWordService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Monitoramento de Microserviços");
            });

            app.UseRouting();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            BrokerReceiver.MessageReceived += BrokerReceiver_MessageReceived;
        }

        private void BrokerReceiver_MessageReceived(Message obj)
        {
            _service.ReceiveMessage(obj.Body);
        }
    }
}
