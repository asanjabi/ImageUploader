using Azure.Storage.Blobs;

using global::Microsoft.AspNetCore.Components;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

using static System.Net.Mime.MediaTypeNames;

namespace ImageUploader.Pages
{
    public partial class Index
    {
        private InputFile? inputFile;
        private ElementReference previewImageElem;

        [CascadingParameter] private Task<AuthenticationState>? authenticationStateTask { get; set; }
        [Inject] IAccessTokenProvider? tokenProvider { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        private async Task LoadFiles(InputFileChangeEventArgs e)
        {
            await JS.InvokeVoidAsync("previewImage", inputFile!.Element, previewImageElem);

            var file = e.GetMultipleFiles().FirstOrDefault();
            if (file != null)
            {
                var image = await file.RequestImageFileAsync(file.ContentType, 100000000, 100000000);
                var authState = await authenticationStateTask;
                var user = authState.User;

                var foo = new AccessTokenProviderTokenCredential(tokenProvider, NavigationManager);

                
                string blobName = $@"https://imaguploader.blob.core.windows.net/files/{user.Identity.Name}/{DateTime.Now.ToShortDateString().Replace('/', '-')}-{DateTime.Now.ToShortTimeString()}-{image.Name}";
                var blob = new BlobClient(new Uri(blobName), foo);
                var bar = await blob.UploadAsync(image.OpenReadStream(51200000));
            }
        }
        private async Task ButtonClick()
        {

        }
    }
}