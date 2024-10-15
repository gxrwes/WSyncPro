// FilePickerService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSyncPro.Util.Files;
using Xamarin.Essentials;

#if WINDOWS
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using WinRT.Interop;
#endif

namespace WSyncPro.Util.Files
{
    public class FilePickerService : IFilePickerService
    {
        public async Task<string> PickFileToOpenAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    return result.FullPath;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
            return null;
        }

        public async Task<string> PickFileToSaveAsync(string suggestedFileName)
        {
#if WINDOWS
            return await PickFileToSaveWindowsAsync(suggestedFileName);
#else
            throw new NotImplementedException("Save file picker is not implemented on this platform.");
#endif
        }

#if WINDOWS
        private async Task<string> PickFileToSaveWindowsAsync(string suggestedFileName)
        {
            var savePicker = new FileSavePicker();

            // Get the current window's HWND
            var window = (Application.Current?.Windows[0]?.Handler?.PlatformView) as Microsoft.UI.Xaml.Window;
            var hwnd = WindowNative.GetWindowHandle(window);

            // Initialize the save picker with the window's HWND
            InitializeWithWindow.Initialize(savePicker, hwnd);

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("JSON Files", new List<string>() { ".json" });
            savePicker.SuggestedFileName = suggestedFileName;

            var result = await savePicker.PickSaveFileAsync();
            if (result != null)
            {
                return result.Path;
            }
            return null;
        }
#endif
    }
}
