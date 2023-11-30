using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Syncfusion.Maui.Core.Hosting;

namespace Adhika_2._0;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureMopups()
            .ConfigureFonts(fonts =>
			{
                fonts.AddFont("fontello.ttf", "Icons_");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.ConfigureSyncfusionCore();
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
