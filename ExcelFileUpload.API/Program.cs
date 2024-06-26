using ExcelFileUpload.API.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace ExcelFileUpload.API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);


            // Serilog 
            var logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("Logs/app_log.txt", rollingInterval: RollingInterval.Day).MinimumLevel.Information().CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            // CORS
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll", policy => {
                    policy.AllowAnyHeader();
                    policy.AllowAnyHeader();
                    policy.AllowAnyOrigin();
                });
            });


            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
