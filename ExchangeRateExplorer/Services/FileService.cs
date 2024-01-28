using ExchangeRateExplorer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ExchangeRateExplorer.Services
{
    internal class FileService
    {
        public static async Task WriteToJsonFileAsync(List<ExchangeRate> exchangeRateDataList, string fileName)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(exchangeRateDataList, Formatting.Indented);

                using (StreamWriter writer = new StreamWriter(GetBasePath(fileName)))
                {
                    await writer.WriteAsync(jsonData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи данных в файл: {ex.Message}");
            }
        }

        public static List<ExchangeRate> LoadDataFromFile(string fileName)
        {
            try
            {
                if (File.Exists(GetBasePath(fileName)))
                {
                    string jsonData = File.ReadAllText(GetBasePath(fileName));
                    return JsonConvert.DeserializeObject<List<ExchangeRate>>(jsonData);
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении данных из файла: {ex.Message}");
                return null;
            }
        }

        private static string GetBasePath(string fileName)
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exePath, fileName);
        }
    }
}
