using System.Globalization;
using System.Text;
using Askmethat.Aspnet.JsonLocalizer.Extensions; // Import the package extensions
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Configuration
{
    public static class LocalizationConfigurator
    {
        private static readonly string[] SupportedCultures = ["en-US", "es", "fr"];

        public static IApplicationBuilder UseCustomLocalization(this IApplicationBuilder app)
        {
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("en-US")
                .AddSupportedCultures(SupportedCultures)
                .AddSupportedUICultures(SupportedCultures);

            return app.UseRequestLocalization(localizationOptions);
        }
    }
}