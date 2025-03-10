var builder = WebApplication.CreateBuilder(args);
var sameHostClient = Boolean.Parse(builder.Configuration["Same_Host_Client"]!);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

if (!sameHostClient)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(config =>
        {
            config
                .WithOrigins("http://localhost:5173", "http://localhost:5174") //base React app url
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (sameHostClient)
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("/index.html");
}
else
{
    app.UseCors();
}

app.Run();
