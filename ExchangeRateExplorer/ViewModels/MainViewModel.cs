using ExchangeRateExplorer.Common.Commands;
using ExchangeRateExplorer.Models;
using ExchangeRateExplorer.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ExchangeRateExplorer.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly ApiService _exchangeRatesApiService;
        private const string FileNameRate = "rate.json";
        private const string FileNameRateDynamics = "rateDynamics.json";

        public MainViewModel()
        {
            _exchangeRatesApiService = new ApiService();
            UpdateDataAsync();
            ShowChartCommand = new DelegateCommand(ShowChart, CanShowChart);
        }

        private DateTime _startDatePicker = DateTime.Today.AddDays(-1);

        public DateTime StartDatePicker
        {
            get
            {
                listExchangeRate = FileService.LoadDataFromFile(FileNameRate);
                return _startDatePicker;
            }
            set
            {
                Set(ref _startDatePicker, value);
                UpdateDataAsync();
            }
        }

        private DateTime _endDatePicker = DateTime.Today;

        public DateTime EndDatePicker
        {
            get => _endDatePicker;
            set => Set(ref _endDatePicker, value);
        }

        private List<ExchangeRate> listExchangeRate;

        public List<ExchangeRate> ListExchangeRate
        {
            get => listExchangeRate;
            set => Set(ref listExchangeRate, value);
        }

        public ICommand SaveChangesCommand
        {
            get
            {
                return new DelegateCommand(async (obj) =>
                {
                    var dataList = ListExchangeRate;
                    await FileService.WriteToJsonFileAsync(dataList, FileNameRate);
                    MessageBox.Show("Данные сохранены!");
                });
            }
        }

        public ICommand ShowChartCommand { get; private set; }

        private async void ShowChart(object parameter)
        {
            if (parameter is ExchangeRate exchangeRate)
            {
                List<ExchangeRate> updatedRates = await _exchangeRatesApiService.GetExchangeRateDynamicsAsync(exchangeRate.Date, _endDatePicker, exchangeRate.Cur_ID);
                ExchangeRates = updatedRates;
                await FileService.WriteToJsonFileAsync(updatedRates, FileNameRateDynamics);
            }
        }

        private List<ExchangeRate> _exchangeRates;

        public List<ExchangeRate> ExchangeRates
        {
            get { return _exchangeRates; }
            set => Set(ref _exchangeRates, value);
        }

        private bool CanShowChart(object parameter)
        {
            ExchangeRate rate = (ExchangeRate)parameter;
            return rate != null && rate.Date < _endDatePicker;
        }

        private async void UpdateDataAsync()
        {
            var updatedData = await _exchangeRatesApiService.GetExchangeRatesAsync(StartDatePicker, 0);
            await FileService.WriteToJsonFileAsync(updatedData, FileNameRate);
            ListExchangeRate = updatedData;
        }
    }
}
