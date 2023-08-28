using Azure.Storage.Blobs;

using global::Microsoft.AspNetCore.Components;

using ImageUploader.Config;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ImageUploader.Pages
{
    public partial class Index
    {
        private InputFile? inputFile;
        private ElementReference previewImageElem;

        private IBrowserFile? _currentFile;
        private bool _uploadDisabled = true;
        private bool _clearDisabled = true;
        private bool _showPreview = false;


        [CascadingParameter] private Task<AuthenticationState>? authenticationStateTask { get; set; }
        [Inject] IAccessTokenProvider? tokenProvider { get; set; }
        [Inject] NavigationManager? NavigationManager { get; set; }
        [Inject] IOptions<ConversionOptions>? ConversionOptions { get; set; }
        [Inject] IOptions<StorageOptions>? StorageOptions { get; set; }

        private async Task LoadImage(InputFileChangeEventArgs e)
        {
            this._currentFile = e.File;
            await JS.InvokeVoidAsync("previewImage", inputFile!.Element, previewImageElem);
            await this.UpdateUI();
        }

        private async Task UpdateUI()
        {
            if (this._currentFile == null)
            {
                this._clearDisabled = true;
                this._uploadDisabled = true;
                this._showPreview = false;
            }
            else
            {
                this._uploadDisabled = false;
                this._clearDisabled = false;
                this._showPreview = true;
            }
        }

        private async Task Upload()
        {
            if (this.inputFile is null)
            {
                return;
            }
            if (authenticationStateTask is null)
            {
                return;
            }
            if (tokenProvider is null)
            {
                return;
            }
            if (NavigationManager is null)
            {
                return;
            }
            if (ConversionOptions is null)
            {
                return;
            }
            if (StorageOptions is null)
            {
                return;
            }

            var file = this._currentFile;
            if (file != null)
            {
                IBrowserFile? image = null;
                if (ConversionOptions.Value.Convert)
                {
                    image = await file.RequestImageFileAsync(ConversionOptions.Value.Format, ConversionOptions.Value.maxWidth, ConversionOptions.Value.maxHeight);
                }
                else
                {
                    image = file;
                }

                var authState = await authenticationStateTask;
                var user = authState.User;
                if (user.Identity is null)
                {
                    return;
                }

                string blobName = $@"https://imaguploader.blob.core.windows.net/files/{user.Identity.Name}/{DateTime.Now.ToShortDateString().Replace('/', '-')}-{DateTime.Now.ToShortTimeString()}-{image.Name}";

                string formatString = StorageOptions.Value.FileNameFormatString;

                var now = DateTime.UtcNow;
                blobName = string.Format(formatString,
                    user.Identity.Name,
                    image.Name,
                    now.Day,
                    now.Month,
                    now.Year,
                    now.DayOfWeek.ToString(),
                    now.DayOfYear,
                    now.Hour,
                    now.TimeOfDay.Hours,
                    now.Minute,
                    now.Second,
                    now.Ticks
                    );

                var blobUrl = new Uri($@"{StorageOptions.Value.StorageBaseUrl}/{blobName}");

                var accessToken = new AccessTokenProviderTokenCredential(tokenProvider, NavigationManager);
                var blob = new BlobClient(blobUrl, accessToken);
                var blobInfo = await blob.UploadAsync(image.OpenReadStream(1048576 * StorageOptions.Value.MaxFileSizeInMB));

                await this.Clear();
            }
        }

        private async Task Clear()
        {
            await JS.InvokeVoidAsync("clearPreviewImage", previewImageElem);
            this._currentFile = null;
            await this.UpdateUI();
        }
    }
}