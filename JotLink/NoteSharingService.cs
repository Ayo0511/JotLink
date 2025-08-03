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

        public NoteSharingService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7287/")
            };
        }

        public async Task<NoteFE?> ShareNoteAsync(NoteFE noteFE)
        {
            var dto = NoteMapper.ToDTO(noteFE);
            var response = await _httpClient.PostAsJsonAsync("notes", dto);
            if (!response.IsSuccessStatusCode) return null;

            var returnedDto = await response.Content.ReadFromJsonAsync<NoteDTO>();
            return NoteMapper.ToFE(returnedDto!);
        }

        public async Task<NoteFE?> UpdateNoteAsync(NoteFE noteFE)
        {
            var dto = NoteMapper.ToDTO(noteFE);
            var response = await _httpClient.PutAsJsonAsync($"notes/{noteFE.Id}", dto);
            if (!response.IsSuccessStatusCode) return null;

            var returnedDto = await response.Content.ReadFromJsonAsync<NoteDTO>();
            return NoteMapper.ToFE(returnedDto!);
        }

        public async Task<NoteFE?> FetchNoteAsync(string publicId)
        {
            var response = await _httpClient.GetAsync($"n/{publicId}");
            if (!response.IsSuccessStatusCode) return null;

            var dto = await response.Content.ReadFromJsonAsync<NoteDTO>();
            return NoteMapper.ToFE(dto!);
        }
    }
}