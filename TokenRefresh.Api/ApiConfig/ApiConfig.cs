using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace TokenRefresh.Api.ApiConfig
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
                 )
                .AddJsonOptions(n => n.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddResponseCompression(opts =>
            {
                opts.Providers.Add<GzipCompressionProvider>();
                opts.EnableForHttps = true;

            });

            services.Configure<GzipCompressionProviderOptions>(opts =>
            {
                opts.Level = CompressionLevel.Optimal;
            });


            return services;

        }
    }
}
