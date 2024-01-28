using ExchangeRateExplorer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace ExchangeRateExplorer.Services
{
    internal class ApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiRateUrl = "https://api.nbrb.by/exrates/rates";
        private readonly string _apiRateDynamicsUrl = "https://api.nbrb.by/ExRates/Rates/Dynamics/";

        public async Task<List<ExchangeRate>> GetExchangeRates(DateTime date, int periodicity)
        {
            try
            {
                string apiUrlWithParams = $"{_apiRateUrl}?ondate={date.ToString("yyyy-MM-dd")}&periodicity={periodicity}";
                using (HttpResponseMessage response = await _httpClient.GetAsync(apiUrlWithParams))
                {
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ExchangeRate>>(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ExchangeRate>> GetExchangeRateDynamics(DateTime startDate, DateTime endDate, int id)
        {
            try
            {
                string apiUrlWithParams = $"{_apiRateDynamicsUrl}{id}?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrlWithParams);
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ExchangeRate>>(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                return null;
            }
        }
    }
}
