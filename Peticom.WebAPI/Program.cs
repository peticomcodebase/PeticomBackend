using System.Reflection;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Peticom.Service.Mapping;
using Microsoft.EntityFrameworkCore;
using Peticom.Repository;
using Peticom.WebAPI.Extensions;
using Peticom.WebAPI.Middlewares;
using WebAPI.Modules;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DB connection
builder.Services.AddDbContext<PeticomDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("PeticomConnectionString"), option =>
    {
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(PeticomDbContext)).GetName().Name);
    });
});

//Automapper implemantasyonu
builder.Services.AddAutoMapper(typeof(MapProfile));

//Autofac implemantasyonu
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    containerBuilder.RegisterModule(new RepositoryServiceModule()));

builder.Services.UseSwaggerAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomException();

app.UseSwagger();

app.UseMiddleware<ApiKeyAuthorizationMiddleware>();

app.MapControllers();

app.Run();