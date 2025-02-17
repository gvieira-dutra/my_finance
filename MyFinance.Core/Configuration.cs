namespace MyFinance.Core;

// static is similar to Singleton
// creates one global instance for entire app
// for all users
public static class Configuration
{
    public const int DefaultStatusCode = 200;
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 25;

    public static string ConnectionStr { get; set; } = string.Empty;
    public static string BackEndURL { get; set; } = string.Empty;
    public static string FrontEndURL { get; set; } = string.Empty;
}
