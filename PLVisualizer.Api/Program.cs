using ApiUtils.ContainerConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureLogicServices();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var CorsConfigurationName = "AllowOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsConfigurationName);
app.UseHttpsRedirection();
app.MapControllers();


app.Run();
