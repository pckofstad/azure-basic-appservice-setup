using System.Net.Http.Json;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Shared
{
    public partial class DatabaseTester : ComponentBase
    {
        private List<ExampleTextDataDto> DataList { get; set; } = new();
        [Inject] public HttpClient Client { get; set; }
        private string FormText { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await GetData();
            await base.OnInitializedAsync();
        }

        private async Task GetData()
        {
            DataList = await Client.GetFromJsonAsync<List<ExampleTextDataDto>>($"/data") ?? new();
            StateHasChanged();
        }

        private async Task Create()
        {
            var model = new ExampleTextDataDto
            {
                Text = FormText
            };

            var response = await Client.PostAsJsonAsync($"/data", model) ?? new();
            if (response.IsSuccessStatusCode)
            {
                await GetData();
                FormText = string.Empty;
            }
        }


        private async Task Remove(int id)
        {
            var response = await Client.DeleteAsync($"/data/{id}");
            if (response.IsSuccessStatusCode)
            {
                await GetData();
            }
        }
    }
}
