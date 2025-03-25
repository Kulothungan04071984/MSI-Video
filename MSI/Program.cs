using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddDistributedMemoryCache();  // In-memory cache to store session data
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Optional: set session timeout duration
    options.Cookie.HttpOnly = true;  // Ensures cookies can only be accessed by the server
    options.Cookie.IsEssential = true;  // Ensures cookie is sent to the browser
});



builder.Services.AddScoped<MSI.Models.DataManagementcs>();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10L * 1024L * 1024L * 1024L; // 10 GB, adjust as necessary
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L; // 10 GB, adjust as necessary
});
//builder.Services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
//    // Optional: Specify known proxies or networks if needed
//    // options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("x.x.x.x"), prefixLength));
//});
var app = builder.Build();
//await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

app.UseSession();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Login}/{action=Login}/{id?}");
//await EnsureFFmpegIsAvailable();

app.Run();

