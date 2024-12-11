using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

