using ImageUploader;
using ImageUploader.Config;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ImageUploader;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.Configure<ConversionOptions>(options => builder.Configuration.Bind(ConversionOptions.ConfigSectionName, options));
        builder.Services.Configure<StorageOptions>(options => builder.Configuration.Bind(StorageOptions.ConfigSectionName, options));

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddMsalAuthentication(options =>
        {
            builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            options.ProviderOptions.DefaultAccessTokenScopes.Add("https://storage.azure.com/.default");
        });

        await builder.Build().RunAsync();
    }
}
