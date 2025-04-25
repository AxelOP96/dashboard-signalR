using Microsoft.AspNetCore.SignalR;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapHub<DashboardHub>("/dashboardHub");
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
var hubContext = app.Services.GetRequiredService<IHubContext<DashboardHub>>();

_ = Task.Run(async () =>
{
    var rnd = new Random();
    while (await timer.WaitForNextTickAsync())
    {
        var value = rnd.Next(50, 100);
        await hubContext.Clients.All.SendAsync("ReceiveMetrics", value);
    }
});
app.Run();
