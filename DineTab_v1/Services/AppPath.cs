using System.IO;
using Microsoft.Maui.Storage;

public static class AppPaths
{
    public static string GetImagesFolder()
    {
#if ANDROID
        // App-specific external storage: visible in a file manager
        var ctx = Android.App.Application.Context;
        var baseDir = ctx.GetExternalFilesDir(null)?.AbsolutePath 
                      ?? FileSystem.AppDataDirectory;
        return Path.Combine(baseDir, "images");
#else
        return Path.Combine(FileSystem.AppDataDirectory, "images");
#endif
    }
}
