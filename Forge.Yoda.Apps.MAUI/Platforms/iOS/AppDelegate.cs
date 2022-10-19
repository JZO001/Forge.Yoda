using Forge.Yoda.Apps.MAUI;
using Foundation;

namespace Forge.Yoda.Apps.MAUI.Platforms.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}