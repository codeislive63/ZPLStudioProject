using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZPLStudio.Data;
using ZPLStudio.Helpers;
using ZPLStudio.Services;

namespace ZPLStudio.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ILabelRepository _labelRepository;
    private readonly IPrinterService _printerService;
    private readonly IReportService _reportService;
    private readonly INotificationService _notificationService;
    private string _tenam = "";
    private string? _selectedPrinter;
    private bool _hasData;

    public MainViewModel(
        ILabelRepository labelRepository,
        IPrinterService printerService,
        IReportService reportService,
        INotificationService notificationService)
    {
        _labelRepository = labelRepository;
        _printerService = printerService;
        _reportService = reportService;
        _notificationService = notificationService;

        Printers = new ObservableCollection<string>(_printerService.GetInstalledPrinters());
        Labels = new ObservableCollection<ListForTekartonV>();

        LoadCommand = new AsyncRelayCommand(LoadAsync, () => !string.IsNullOrWhiteSpace(Tenam));
        PrintCommand = new RelayCommand(Print, () => _hasData && !string.IsNullOrWhiteSpace(SelectedPrinter));
        EditTemplateCommand = new RelayCommand(EditTemplate, () => _hasData);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> Printers { get; }

    public ObservableCollection<ListForTekartonV> Labels { get; }

    public string Tenam
    {
        get => _tenam;
        set
        {
            if (SetField(ref _tenam, value))
            {
                RaiseCommands();
            }
        }
    }

    public string? SelectedPrinter
    {
        get => _selectedPrinter;
        set
        {
            if (SetField(ref _selectedPrinter, value))
            {
                RaiseCommands();
            }
        }
    }

    public AsyncRelayCommand LoadCommand { get; }

    public RelayCommand PrintCommand { get; }

    public RelayCommand EditTemplateCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            Labels.Clear();
            var data = await _labelRepository.GetLabelsAsync(Tenam);
            foreach (var item in data)
            {
                Labels.Add(item);
            }

            _hasData = Labels.Count > 0;
            RaiseCommands();

            if (!_hasData)
            {
                _notificationService.ShowInfo("Данные по этому TENAM не найдены.", "Нет данных");
            }
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Ошибка загрузки: {ex.Message}", "Ошибка");
        }
    }

    private void Print()
    {
        try
        {
            if (SelectedPrinter is null)
            {
                _notificationService.ShowInfo("Выберите принтер для печати.", "Принтер");
                return;
            }

            // Печать через FastReport с выбранным принтером.
            _reportService.Print(Labels, SelectedPrinter);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Ошибка печати: {ex.Message}", "Ошибка");
        }
    }

    private void EditTemplate()
    {
        try
        {
            // Открываем встроенный дизайнер, чтобы править шаблон ярлыка.
            _reportService.OpenDesigner(Labels);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Ошибка открытия редактора: {ex.Message}", "Ошибка");
        }
    }

    private void RaiseCommands()
    {
        LoadCommand.RaiseCanExecuteChanged();
        PrintCommand.RaiseCanExecuteChanged();
        EditTemplateCommand.RaiseCanExecuteChanged();
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
