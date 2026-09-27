using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ShutdownScheduler;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private TimeSpan _remaining;
    private string _mode = "shutdown";
    private Button? _activeModeButton;

    public MainWindow()
    {
        InitializeComponent();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Timer_Tick;
        SelectMode(BtnShutdown, "shutdown");
    }

    // ===== Title bar =====
    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    // ===== Mode selection =====
    private void Mode_Click(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        SelectMode(btn, (string)btn.Tag);
    }

    private void SelectMode(Button btn, string mode)
    {
        _mode = mode;

        // reset previous highlight
        if (_activeModeButton is not null)
        {
            _activeModeButton.Background = (Brush)FindResource("Card");
            _activeModeButton.BorderBrush = (Brush)FindResource("Stroke");
            _activeModeButton.Foreground = (Brush)FindResource("TextPrimary");
        }

        btn.Background = (Brush)FindResource("AccentSoft");
        btn.BorderBrush = (Brush)FindResource("Accent");
        btn.Foreground = (Brush)FindResource("TextPrimary");
        _activeModeButton = btn;

        if (!_timer.IsEnabled)
            StatusText.Text = $"Seçili işlem: {ModeLabel(_mode)}";
    }

    // ===== Presets & input =====
    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        MinutesBox.Text = (string)((Button)sender).Tag;
    }

    private void Minutes_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    }

    // ===== Start / Cancel =====
    private void Start_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(MinutesBox.Text, out int minutes) || minutes <= 0)
        {
            StatusText.Text = "Geçerli bir dakika değeri girin";
            StatusText.Foreground = (Brush)FindResource("Danger");
            return;
        }

        _remaining = TimeSpan.FromMinutes(minutes);
        _timer.Start();
        UpdateCountdown();

        StartBtn.IsEnabled = false;
        CancelBtn.IsEnabled = true;
        MinutesBox.IsEnabled = false;
        StatusText.Foreground = (Brush)FindResource("Success");
        StatusText.Text = $"{ModeLabel(_mode)} zamanlandı";
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        StopTimer();
        CountdownText.Text = "Zamanlama yok";
        StatusText.Foreground = (Brush)FindResource("TextMuted");
        StatusText.Text = "İşlem iptal edildi";
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));

        if (_remaining <= TimeSpan.Zero)
        {
            StopTimer();
            ExecuteAction();
            return;
        }
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        CountdownText.Text = _remaining.ToString(@"hh\:mm\:ss");
    }

    private void StopTimer()
    {
        _timer.Stop();
        StartBtn.IsEnabled = true;
        CancelBtn.IsEnabled = false;
        MinutesBox.IsEnabled = true;
    }

    // ===== Execute the OS action =====
    private void ExecuteAction()
    {
        CountdownText.Text = "Yürütülüyor...";
        StatusText.Text = $"{ModeLabel(_mode)} çalıştırılıyor";

        try
        {
            switch (_mode)
            {
                case "shutdown":
                    Run("shutdown", "/s /t 0");
                    break;
                case "restart":
                    Run("shutdown", "/r /t 0");
                    break;
                case "logoff":
                    Run("shutdown", "/l");
                    break;
                case "sleep":
                    Run("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");
                    break;
            }
        }
        catch (Exception ex)
        {
            StatusText.Foreground = (Brush)FindResource("Danger");
            StatusText.Text = "Hata: " + ex.Message;
        }
    }

    private static void Run(string file, string args)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = file,
            Arguments = args,
            CreateNoWindow = true,
            UseShellExecute = false
        });
    }

    private static string ModeLabel(string mode) => mode switch
    {
        "shutdown" => "Kapatma",
        "restart" => "Yeniden başlatma",
        "sleep" => "Uyku",
        "logoff" => "Oturum kapatma",
        _ => mode
    };
}
