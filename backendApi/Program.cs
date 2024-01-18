using Microsoft.EntityFrameworkCore;
using backendApi.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<backendApiDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("ApiDemoConnectionString")));

builder.Services.AddCors(opts => opts.AddPolicy("origins", policy => // <-- added this
{
    policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    ;
}));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("origins");

app.UseAuthorization();


app.MapControllers();

app.Run();
