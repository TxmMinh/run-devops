var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add http client services at ConfigureServices(IServiceCollection services)
builder.Services.AddHttpClient("ShoppingAPIClient", client =>
{

    // client.BaseAddress = new Uri("http://host.docker.internal:5192/"); // Local environment
    client.BaseAddress = new Uri(configuration["ShoppingAPIUrl"]); // Docker environment

});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
