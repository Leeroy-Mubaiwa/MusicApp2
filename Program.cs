using MusicApp2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DataService>();
builder.Services.AddScoped<MusicService>();
builder.Services.AddScoped<CollaborationService>();
builder.Services.AddScoped<AcoustIdService>();
builder.Services.AddScoped<ChromaprintService>();
builder.Services.AddHttpClient<AcoustIdService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AcoustId:BaseUrl"] ?? "https://api.acoustid.org/v2/");
});

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
    pattern: "{controller=Music}/{action=Upload}/{id?}");

app.Run();
