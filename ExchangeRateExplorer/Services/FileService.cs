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
            if (exchangeRateDataList == null || string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Некорректные данные или имя файла для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var jsonData = JsonConvert.SerializeObject(exchangeRateDataList, Formatting.Indented);
                var filePath = GetFullPath(fileName);

                using (var writer = new StreamWriter(filePath, false))
                {
                    await writer.WriteAsync(jsonData);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка при записи данных в файл", ex);
            }
        }

        public static List<ExchangeRate> LoadDataFromFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Имя файла не задано.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return new List<ExchangeRate>();
            }

            var filePath = GetFullPath(fileName);

            try
            {
                if (!File.Exists(filePath))
                    return new List<ExchangeRate>();

                var jsonData = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<List<ExchangeRate>>(jsonData);
                return data ?? new List<ExchangeRate>();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка при чтении данных из файла", ex);
                return new List<ExchangeRate>();
            }
        }

        private static string GetFullPath(string fileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, fileName);
        }

        private static void ShowErrorMessage(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
