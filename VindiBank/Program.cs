
using VindiBank.Business.Controll;
using VindiBank.Business.Interfaces;
using VindiBank.Repository.DataBase;
using VindiBank.Repository.Interfaces;
using VindiBank.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFluxoAPIControll, FluxoAPIControll>();
builder.Services.AddScoped<IRequisicoesRepository, RequisicoesRepository>();
builder.Services.AddScoped<IConnectionDB, ConnectionDB>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DefaultModelExpandDepth(-1);
    c.SwaggerEndpoint("swagger/v1/swagger.json","Vindi Banki API - Rest");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
