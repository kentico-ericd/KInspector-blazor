using Autofac;
using Autofac.Extensions.DependencyInjection;

using KInspector.Core;
using KInspector.Infrastructure;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((containerBuilder) =>
    {
        containerBuilder.RegisterModule(new CoreModule());
        containerBuilder.RegisterModule(new InfrastructureModule());
    });

builder.Services.AddRazorPages().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
