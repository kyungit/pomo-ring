using System.Windows;
using System.Windows.Media;
using PomoRing.Models;
using PomoRing.Services;

namespace PomoRing;

public partial class MainWindow : Window
{
    private readonly SettingsStorageService _storage = new();
    private readonly AppData _data;
    private BuddyWindow? _buddy;

    public MainWindow()
    {
        InitializeComponent();
        _data = _storage.Load();
        LoadSettings();
    }

    private void LoadSettings()
    {
        FocusMinutesBox.Text = _data.Settings.FocusMinutes.ToString();
        RestMinutesBox.Text = _data.Settings.RestMinutes.ToString();
        CountupRadio.IsChecked = _data.Settings.CountUpEnabled;
        CountdownRadio.IsChecked = !_data.Settings.CountUpEnabled;
        RepeatCheck.IsChecked = _data.Settings.RepeatEnabled;
        RepeatCountBox.Text = _data.Settings.RepeatCount.ToString();
        ShowTimerCheck.IsChecked = _data.Settings.ShowTimer;
        if (_data.Settings.CharacterType == "Dog")
            _data.Settings.CharacterType = "Cat";
        CatRadio.IsChecked = _data.Settings.CharacterType != "Bunny";
        BunnyRadio.IsChecked = _data.Settings.CharacterType == "Bunny";
        ProgressText.Text = $"TODAY {_data.Progress.TodayFocusMinutes} MIN  /  TOTAL {_data.Progress.TotalFocusMinutes} MIN";
        UpdateTimerModeUi();
        UpdateCharacterUi();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadSettings(out var settings))
            return;

        _data.Settings = settings;
        _storage.Save(_data);
        _buddy ??= new BuddyWindow(this, _data, _storage);
        Hide();
        _buddy.Start(settings);
    }

    private bool TryReadSettings(out PomodoroSettings settings)
    {
        settings = new PomodoroSettings();
        var isCountUp = CountupRadio.IsChecked == true;
        if (!isCountUp && (!int.TryParse(FocusMinutesBox.Text, out var focus) || focus < 1 ||
            !int.TryParse(RestMinutesBox.Text, out var rest) || rest < 0 ||
            !int.TryParse(RepeatCountBox.Text, out var repeats) || repeats < 1))
        {
            ValidationText.Text = "집중 시간은 1분 이상, 휴식 시간은 0분 이상 입력해 주세요.";
            return false;
        }

        settings.CountUpEnabled = isCountUp;
        settings.CharacterType = BunnyRadio.IsChecked == true ? "Bunny" : "Cat";
        settings.FocusMinutes = isCountUp ? _data.Settings.FocusMinutes : int.Parse(FocusMinutesBox.Text);
        settings.RestMinutes = isCountUp ? _data.Settings.RestMinutes : int.Parse(RestMinutesBox.Text);
        settings.RepeatEnabled = !isCountUp && RepeatCheck.IsChecked == true;
        settings.RepeatCount = isCountUp ? _data.Settings.RepeatCount : int.Parse(RepeatCountBox.Text);
        settings.ShowTimer = ShowTimerCheck.IsChecked == true;
        ValidationText.Text = "";
        return true;
    }

    public void OpenSettings()
    {
        LoadSettings();
        Show();
        Activate();
    }

    private void RepeatCheck_Changed(object sender, RoutedEventArgs e)
    {
        if (RepeatCountBox != null)
            RepeatCountBox.IsEnabled = RepeatCheck.IsChecked == true;
    }

    private void TimerMode_Changed(object sender, RoutedEventArgs e) => UpdateTimerModeUi();

    private void Character_Changed(object sender, RoutedEventArgs e) => UpdateCharacterUi();

    private void UpdateCharacterUi()
    {
        if (CatCard == null || BunnyCard == null || CatRadio == null || BunnyRadio == null)
            return;

        CatCard.Background = CatRadio.IsChecked == true ? (Brush)FindResource("Cream") : Brushes.White;
        BunnyCard.Background = BunnyRadio.IsChecked == true ? (Brush)FindResource("Cream") : Brushes.White;
    }

    private void UpdateTimerModeUi()
    {
        if (CountdownSettings == null || CountupRadio == null)
            return;

        var countUp = CountupRadio.IsChecked == true;
        CountdownSettings.IsEnabled = !countUp;
        RepeatPanel.IsEnabled = !countUp;
        CountdownCard.Background = countUp ? Brushes.White : (Brush)FindResource("Cream");
        CountupCard.Background = countUp ? (Brush)FindResource("Cream") : Brushes.White;
        StartButton.Content = countUp ? "▶ START COUNT UP" : "▶ START FOCUS";
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
}
