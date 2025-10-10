using MBA.EducaOn.Api.Configurations;
using MBA.EducaOn.Ioc.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDataContextPool();
builder.AddApiConfig()
       .AddCorsConfig()
       .AddSwaggerConfig()
       .AddIdentityConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Production");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
