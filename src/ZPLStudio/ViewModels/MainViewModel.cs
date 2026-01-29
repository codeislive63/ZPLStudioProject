using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Input;
using ZPLStudio.Commands;
using ZPLStudio.Models;
using ZPLStudio.Services;

namespace ZPLStudio.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ILabelRepository _labelRepository;
    private readonly ILabelTemplateService _templateService;
    private readonly ILabelPrinter _labelPrinter;
    private readonly LabelTemplateOptions _options;
    private bool _isBusy;
    private string _tenam;
    private LabelRecord? _selectedRecord;
    private PrintQueue? _selectedPrinter;
    private string _templateText;
    private FrameworkElement? _previewContent;

    public MainViewModel(
        ILabelRepository labelRepository,
        ILabelTemplateService templateService,
        ILabelPrinter labelPrinter,
        LabelTemplateOptions options)
    {
        _labelRepository = labelRepository;
        _templateService = templateService;
        _labelPrinter = labelPrinter;
        _options = options;

        _tenam = options.DefaultTenam;
        _templateText = _templateService.LoadTemplateText();

        Printers = new ObservableCollection<PrintQueue>(GetPrinters());
        Records = new ObservableCollection<LabelRecord>();

        LoadCommand = new RelayCommand(async _ => await LoadAsync(), _ => !IsBusy);
        PrintCommand = new RelayCommand(async _ => await PrintAsync(), _ => CanPrint());
        ReloadTemplateCommand = new RelayCommand(_ => ReloadTemplate(), _ => !IsBusy);
        SaveTemplateCommand = new RelayCommand(_ => SaveTemplate(), _ => !IsBusy);
    }

    public ObservableCollection<PrintQueue> Printers { get; }
    public ObservableCollection<LabelRecord> Records { get; }

    public ICommand LoadCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand ReloadTemplateCommand { get; }
    public ICommand SaveTemplateCommand { get; }

    public string Tenam
    {
        get => _tenam;
        set
        {
            _tenam = value;
            OnPropertyChanged();
        }
    }

    public LabelRecord? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            _selectedRecord = value;
            OnPropertyChanged();
            UpdatePreview();
            RaiseCommandStates();
        }
    }

    public PrintQueue? SelectedPrinter
    {
        get => _selectedPrinter;
        set
        {
            _selectedPrinter = value;
            OnPropertyChanged();
            RaiseCommandStates();
        }
    }

    public string TemplateText
    {
        get => _templateText;
        set
        {
            _templateText = value;
            OnPropertyChanged();
        }
    }

    public FrameworkElement? PreviewContent
    {
        get => _previewContent;
        private set
        {
            _previewContent = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            RaiseCommandStates();
        }
    }

    private IEnumerable<PrintQueue> GetPrinters()
    {
        var server = new LocalPrintServer();
        return server.GetPrintQueues();
    }

    private async Task LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(Tenam))
        {
            MessageBox.Show("Введите номер TENAM.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            Records.Clear();
            var records = await _labelRepository.GetByTenamAsync(Tenam, CancellationToken.None);
            foreach (var record in records)
            {
                Records.Add(record);
            }

            SelectedRecord = Records.FirstOrDefault();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PrintAsync()
    {
        if (SelectedRecord is null || SelectedPrinter is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            await _labelPrinter.PrintAsync(SelectedRecord, SelectedPrinter, CancellationToken.None);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка печати: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ReloadTemplate()
    {
        TemplateText = _templateService.LoadTemplateText();
        UpdatePreview();
    }

    private void SaveTemplate()
    {
        _templateService.SaveTemplateText(TemplateText);
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (SelectedRecord is null)
        {
            PreviewContent = null;
            return;
        }

        try
        {
            _templateService.SaveTemplateText(TemplateText);
            PreviewContent = _templateService.BuildLabel(SelectedRecord);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка шаблона: {ex.Message}", "Шаблон", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private bool CanPrint() => SelectedRecord is not null && SelectedPrinter is not null && !IsBusy;

    private void RaiseCommandStates()
    {
        (LoadCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PrintCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ReloadTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
