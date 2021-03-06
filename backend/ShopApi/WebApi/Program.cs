using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WebApi.Configuration.Database;
using WebApi.Configuration.IoC;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediator();
builder.Services.AddAuth();
builder.Services.AddDatabase(builder.Environment);

var app = builder.Build();

app.UseCors(options => {
                         options
			    .AllowCredentials()
			    .AllowAnyMethod()
			    .AllowAnyHeader()
			    .SetIsOriginAllowed(origin => true);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MigrateAndSeed();

app.UseMiddleware<ExceptionHandlerMiddlerware>();

app.Run();
