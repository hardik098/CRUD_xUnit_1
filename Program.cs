using Entities;
using Microsoft.EntityFrameworkCore;
using Service_contracts;
using Sevices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<PersonsDbContext>(op=>op.UseSqlServer(builder.Configuration.GetConnectionString("constring")));

// Add DI services to IoC container
builder.Services.AddSingleton<IPersonsService,PersonsService>();
builder.Services.AddSingleton<ICountriesService,CountriesService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.MapControllers();
//app.MapGet("/", () => "Hello World!");

app.Run();
