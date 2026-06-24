var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("PizzaApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7204");
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pizzas}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();