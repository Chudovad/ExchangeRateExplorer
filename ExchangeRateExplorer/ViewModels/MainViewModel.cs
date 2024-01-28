using ExchangeRateExplorer.Models;
using ExchangeRateExplorer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Charting;

namespace ExchangeRateExplorer.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private Nullable<DateTime> startDatePicker = null;
        private Nullable<DateTime> endDatePicker = null;
        private List<ExchangeRate> listExchangeRate = null;
        private ApiService ExchangeRatesApiService = new ApiService();
        private string fileNameRate = "rate.json";
        private string fileNameRateDynamics = "rateDynamics.json";

        public Nullable<DateTime> StartDatePicker
        {
            get
            {
                if (startDatePicker == null)
                    startDatePicker = DateTime.Today;
                listExchangeRate = FileService.LoadDataFromFile(fileNameRate);
                return startDatePicker;
            }
            set
            {
                startDatePicker = value;
                UpdateDataAsync();
            }
        }

        private async void UpdateDataAsync()
        {
            try
            {
                var updatedData = await ExchangeRatesApiService.GetExchangeRates(StartDatePicker.Value, 0);
                await FileService.WriteToJsonFileAsync(updatedData, fileNameRate);
                ListExchangeRate = updatedData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }

        public Nullable<DateTime> EndDatePicker
        {
            get
            {
                if (endDatePicker == null)
                    endDatePicker = DateTime.Today;
                return endDatePicker;
            }
            set
            {
                endDatePicker = value;
                OnPropertyChanged(nameof(EndDatePicker));
            }
        }

        public List<ExchangeRate> ListExchangeRate
        {
            get
            {
                return listExchangeRate;
            }
            set
            {
                listExchangeRate = value;
                OnPropertyChanged(nameof(ListExchangeRate));
            }
        }

        public ICommand SaveChangesCommand
        {
            get
            {
                return new DelegateCommand(async (obj) =>
                {
                    var dataList = ListExchangeRate;
                    await FileService.WriteToJsonFileAsync(dataList, fileNameRate);
                });
            }
        }

        public ICommand ShowChartCommand { get; private set; }

        public MainViewModel()
        {
            ShowChartCommand = new DelegateCommand(ShowChart, CanShowChart);
        }

        private async void ShowChart(object parameter)
        {
            if (parameter is ExchangeRate)
            {
                ExchangeRate exchangeRate = (ExchangeRate)parameter;
                try
                {
                    List<ExchangeRate> updatedRates = await ExchangeRatesApiService.GetExchangeRateDynamics(exchangeRate.Date, endDatePicker.Value, exchangeRate.Cur_ID);
                    ExchangeRates = updatedRates;
                    await FileService.WriteToJsonFileAsync(updatedRates, fileNameRateDynamics);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
                }
            }
        }

        private bool CanShowChart(object parameter)
        {
            ExchangeRate rate = (ExchangeRate)parameter;
            return rate != null && rate.Date < endDatePicker.Value;
        }

        private List<ExchangeRate> _exchangeRates;

        public List<ExchangeRate> ExchangeRates
        {
            get { return _exchangeRates; }
            set
            {
                if (_exchangeRates != value)
                {
                    _exchangeRates = value;
                    OnPropertyChanged(nameof(ExchangeRates));
                }
            }
        }
    }
}
