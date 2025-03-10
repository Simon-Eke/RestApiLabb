
using Microsoft.EntityFrameworkCore;
using RestApiLabb.Data;
using RestApiLabb.Endpoints;

namespace RestApiLabb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<RestApiLabbDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            EducationEndpoints.RegisterEndpoints(app);
            ExperienceEndpoints.RegisterEndpoints(app);
            PersonEndpoints.RegisterEndpoints(app);

            GitHubEndpoints.RegisterEndpoints(app);

            app.Run();
        }
    }
}
