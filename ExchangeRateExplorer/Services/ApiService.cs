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
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseRateUrl = "https://api.nbrb.by/exrates/rates";
        private const string BaseRateDynamicsUrl = "https://api.nbrb.by/ExRates/Rates/Dynamics";

        public async Task<List<ExchangeRate>> GetExchangeRatesAsync(DateTime date, int periodicity)
        {
            var url = string.Format("{0}?ondate={1:yyyy-MM-dd}&periodicity={2}", BaseRateUrl, date, periodicity);
            return await FetchDataAsync<List<ExchangeRate>>(url);
        }

        public async Task<List<ExchangeRate>> GetExchangeRateDynamicsAsync(DateTime startDate, DateTime endDate, int id)
        {
            var url = string.Format("{0}/{1}?startDate={2:yyyy-MM-dd}&endDate={3:yyyy-MM-dd}", BaseRateDynamicsUrl, id, startDate, endDate);
            return await FetchDataAsync<List<ExchangeRate>>(url);
        }

        private async Task<T> FetchDataAsync<T>(string url) where T : new()
        {
            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<T>(content);
                    return data != null ? data : new T();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new T();
            }
        }
    }
}
