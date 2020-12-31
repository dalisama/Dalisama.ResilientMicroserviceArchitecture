using Dalisama.ResilientMicroserviceArchitecture.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;
using System;

namespace Dalisama.ResilientMicroserviceArchitecture.Producer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

            Environment.SetEnvironmentVariable("application name", "gateway");

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .Enrich.WithProperty("Index", "dalisama.gateway")
                           .Enrich.WithProperty("Application", Environment.GetEnvironmentVariable("application name"))
                            .MinimumLevel.Debug()
                             .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
                            {
                                clientConfiguration.Username = "guest";
                                clientConfiguration.Password = "guest";
                                clientConfiguration.Exchange = "monitoring";
                                clientConfiguration.ExchangeType = "topic";
                                clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
                                clientConfiguration.RouteKey = "monitoring";
                                clientConfiguration.Port = 5672;
                                clientConfiguration.Hostnames.Add("localhost");


                                sinkConfiguration.TextFormatter = new JsonFormatter();
                            }).CreateLogger();

            services.AddLogging(loggingBuilder =>
                                loggingBuilder.AddSerilog(dispose: true)
                                              .AddConsole());

            services.Configure<RabbitMqExchangeConfiguration>(Configuration.GetSection("RabbitMqExchange"));
            services.Configure<RabbitMqQueueConfiguration>(Configuration.GetSection("RabbitMqQueue"));
            services.AddScoped<ProducerRabbitMQBase>();
            services.AddSwaggerGen();

            services.Configure<ElasticClientConfiguration>(Configuration.GetSection("Monitoring"));
            services.AddTransient<ElasticClientProvider>();
            services.AddTransient<TransactionMonitoring>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
