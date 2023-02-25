using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Client.Shared
{
    public partial class FileStorageTester : ComponentBase
    {
        private List<FileDto> FileList { get; set; } = new();
        [Inject] public HttpClient Client { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetData();
            await base.OnInitializedAsync();
        }

        private async Task GetData()
        {
            FileList = await Client.GetFromJsonAsync<List<FileDto>>($"/files") ?? new();
            StateHasChanged();
        }

        private async Task UploadFile(InputFileChangeEventArgs file)
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.File.OpenReadStream(long.MaxValue));
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.File.ContentType);

            content.Add(streamContent, "file", file.File.Name);
            using var response = await Client.PostAsync($"/files", content);

            if (response.IsSuccessStatusCode)
            {
                await GetData();
            }
        }


        private async Task Remove(string fileName)
        {
            var response = await Client.DeleteAsync($"/files/{fileName}");
            if (response.IsSuccessStatusCode)
            {
                await GetData();
            }
        }
    }
}
