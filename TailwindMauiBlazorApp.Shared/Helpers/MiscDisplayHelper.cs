using TailwindMauiBlazorApp.Core.Models.Entities;

namespace TailwindMauiBlazorApp.Shared.Helpers;

public static class MiscDisplayHelper
{
    public static string FormatCount(int count)
    {
        if (count >= 1000)
            return (count / 1000.0).ToString("0.#") + "k";
        return count.ToString();
    }
}
