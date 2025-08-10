using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JotLink.Shared;

namespace JotLink
{
    public class NoteSharingService
    {
        private readonly HttpClient _httpClient;
         private static bool hasShowedalert=false;

        public NoteSharingService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://jotlink.onrender.com/")
            };
            Connectivity.ConnectivityChanged += connectivityChanged;
        }

        public async Task<NoteFE?> ShareNoteAsync(NoteFE noteFE)
        {
            if (!await CheckInternetAsync())
            {
                hasShowedalert = true;
                return null;
            }
            try
            {
                var dto = NoteMapper.ToDTO(noteFE);
                var response = await _httpClient.PostAsJsonAsync("notes", dto);
                if (!response.IsSuccessStatusCode) return null;

                var returnedDto = await response.Content.ReadFromJsonAsync<NoteDTO>();
                return NoteMapper.ToFE(returnedDto!);
            }
            catch (HttpRequestException ex) 
            {
                var currentPage = Application.Current?.MainPage;
                if (currentPage != null && !hasShowedalert)
                {
                    await currentPage.DisplayAlert("No Internet", "Please check your connection and try again.", "OK");
                    hasShowedalert = true;
                }
                return null;

            }
        }

        public async Task<NoteFE?> UpdateNoteAsync(NoteFE noteFE)
        {
            if (!await CheckInternetAsync())
            {
                hasShowedalert = true;
                return null;
            }
            try
            {
                var dto = NoteMapper.ToDTO(noteFE);
                var response = await _httpClient.PutAsJsonAsync($"notes/{noteFE.Id}", dto);
                if (!response.IsSuccessStatusCode) return null;

                var returnedDto = await response.Content.ReadFromJsonAsync<NoteDTO>();
                return NoteMapper.ToFE(returnedDto!);
            }
            catch (HttpRequestException ex) 
            {
                var currentPage = Application.Current?.MainPage;
                if (currentPage != null && !hasShowedalert)
                {
                    await currentPage.DisplayAlert("No Internet", "Please check your connection and try again.", "OK");
                    hasShowedalert = true;
                }
            }
            return null;
        }

        public async Task<NoteFE?> FetchNoteAsync(string publicId)
        {

            if (!await CheckInternetAsync())
            {
                hasShowedalert = true;
                return null;
            }
            try
            {
                var response = await _httpClient.GetAsync($"n/{publicId}");
                if (!response.IsSuccessStatusCode) return null;

                var dto = await response.Content.ReadFromJsonAsync<NoteDTO>();
                return NoteMapper.ToFE(dto!);
            }
            catch (HttpRequestException ex)
            {
                var currentPage = Application.Current?.MainPage;

                if (currentPage != null && !hasShowedalert)
                {
                    await currentPage.DisplayAlert("No Internet", "Please check your connection and try again.", "OK");
                    hasShowedalert = true;
                }
            }
            return null;
        }









        private async Task<bool> CheckInternetAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet && !hasShowedalert )
            {
                var currentPage = Application.Current?.MainPage;
                if (currentPage != null)
                {
                    await currentPage.DisplayAlert("No Internet", "Please check your connection and try again.", "OK");
                }
                return false;
            }
            return true;
        }

        private void connectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                hasShowedalert = false;
            }

        }
    }
}