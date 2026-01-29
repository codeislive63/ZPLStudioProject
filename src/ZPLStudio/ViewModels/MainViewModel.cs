using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ZPLStudio.Models;
using ZPLStudio.Services;

namespace ZPLStudio.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ILabelRepository _labelRepository;
    private readonly IPrinterService _printerService;
    private readonly IFastReportService _fastReportService;
    private string _tenam = string.Empty;
    private LabelRecord? _selectedItem;
    private string? _selectedPrinter;
    private string _statusMessage = "Введите TENAM и нажмите «Загрузить».";

    public MainViewModel(
        ILabelRepository labelRepository,
        IPrinterService printerService,
        IFastReportService fastReportService)
    {
        _labelRepository = labelRepository;
        _printerService = printerService;
        _fastReportService = fastReportService;

        Items = new ObservableCollection<LabelRecord>();
        Printers = new ObservableCollection<string>();

        LoadCommand = new RelayCommand(LoadAsync);
        PrintCommand = new RelayCommand(Print, CanPrint);
        EditTemplateCommand = new RelayCommand(EditTemplate);
        RefreshPrintersCommand = new RelayCommand(RefreshPrinters);

        RefreshPrinters();
    }

    public ObservableCollection<LabelRecord> Items { get; }
    public ObservableCollection<string> Printers { get; }

    public string Tenam
    {
        get => _tenam;
        set
        {
            if (SetProperty(ref _tenam, value))
            {
                PrintCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public LabelRecord? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                PrintCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? SelectedPrinter
    {
        get => _selectedPrinter;
        set => SetProperty(ref _selectedPrinter, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public RelayCommand LoadCommand { get; }
    public RelayCommand PrintCommand { get; }
    public RelayCommand EditTemplateCommand { get; }
    public RelayCommand RefreshPrintersCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async void LoadAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Tenam))
            {
                StatusMessage = "TENAM не заполнен.";
                return;
            }

            StatusMessage = "Загрузка данных...";
            Items.Clear();

            var records = await _labelRepository.GetByTenamAsync(Tenam, CancellationToken.None);
            foreach (var record in records)
            {
                Items.Add(record);
            }

            StatusMessage = Items.Count == 0
                ? "Ничего не найдено."
                : $"Загружено: {Items.Count}. Выберите запись для печати.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки: {ex.Message}";
        }
    }

    private void Print()
    {
        if (SelectedItem == null)
        {
            StatusMessage = "Сначала выберите запись.";
            return;
        }

        try
        {
            _fastReportService.Print(SelectedItem, SelectedPrinter);
            StatusMessage = $"Печать отправлена на {SelectedPrinter ?? "принтер по умолчанию"}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка печати: {ex.Message}";
        }
    }

    private bool CanPrint() => SelectedItem != null;

    private void EditTemplate()
    {
        try
        {
            _fastReportService.EditTemplate();
            StatusMessage = "Шаблон открыт в дизайнере.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка шаблона: {ex.Message}";
        }
    }

    private void RefreshPrinters()
    {
        Printers.Clear();
        foreach (var printer in _printerService.GetInstalledPrinters())
        {
            Printers.Add(printer);
        }

        SelectedPrinter ??= Printers.FirstOrDefault();
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
