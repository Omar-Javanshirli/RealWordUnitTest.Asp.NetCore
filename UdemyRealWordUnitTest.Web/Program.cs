using Microsoft.EntityFrameworkCore;
using UdemyRealWordUnitTest.Web.Model;
using UdemyRealWordUnitTest.Web.Repository;

//Database
// Scaffold-DbContext "Data Source=DESKTOP-PM5ECFA\SQLEXPRESS;Integrated Security=True;Connect Timeout=30; Initial Catalog=UdemyUnitTest;
// Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<UdemyUnitTestContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlConStr"));
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
