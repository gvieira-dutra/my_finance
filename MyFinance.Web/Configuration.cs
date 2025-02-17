using MudBlazor;
using MudBlazor.Utilities;

namespace MyFinance.Web;

public static class Configuration
{
    public const string HttpClientName = "myFinance";
    public static string BackEndUrl { get; set; } = "";

    public static MudTheme Theme = new()
    {
        Typography = new Typography
        {
            Default =
            {
                FontFamily = ["Raleway", "sans-serif"]
            },
        },
        PaletteLight = new PaletteLight
        {
            Primary = "#1EFA2D",
            PrimaryContrastText = new MudColor("#000000"),
            Secondary = Colors.LightGreen.Darken3,
            Background = Colors.Gray.Lighten4,
            AppbarBackground = new MudColor("#1EFA2D"),
            AppbarText = Colors.Shades.Black,
            TextPrimary = Colors.Shades.Black,
            DrawerText = Colors.Shades.White,
            DrawerBackground = Colors.LightGreen.Lighten4
        },
        PaletteDark = new PaletteDark
        {
            Primary = Colors.LightGreen.Accent3,
            Secondary = Colors.LightGreen.Darken3,
            AppbarBackground = Colors.LightGreen.Accent3,
            AppbarText = Colors.Shades.Black,
            PrimaryContrastText = new MudColor("#000000")
        }
    };
}
