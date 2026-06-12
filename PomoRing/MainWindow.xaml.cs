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
        PomodoroCheck.IsChecked = _data.Settings.RestEnabled && _data.Settings.RepeatEnabled;
        RepeatCountBox.Text = _data.Settings.RepeatCount.ToString();
        SecondsTestCheck.IsChecked = _data.Settings.UseSecondsForTesting;
        ShowTimerCheck.IsChecked = _data.Settings.ShowTimer;

        if (_data.Settings.CharacterType == "Dog")
            _data.Settings.CharacterType = "Cat";
        CatRadio.IsChecked = _data.Settings.CharacterType != "Bunny";
        BunnyRadio.IsChecked = _data.Settings.CharacterType == "Bunny";

        ProgressText.Text = $"오늘 {_data.Progress.TodayFocusMinutes}분  /  누적 {_data.Progress.TotalFocusMinutes}분";
        UpdateTimerModeUi();
        UpdateSessionModeUi();
        UpdateCharacterUi();
        UpdateUnitLabels();
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
        var pomodoroEnabled = PomodoroCheck.IsChecked == true;

        if (!isCountUp)
        {
            if (!int.TryParse(FocusMinutesBox.Text, out var focus) || focus < 1)
            {
                ValidationText.Text = "집중 시간은 1 이상 입력해 주세요.";
                return false;
            }
            if (pomodoroEnabled && (!int.TryParse(RestMinutesBox.Text, out var rest) || rest < 1))
            {
                ValidationText.Text = "휴식 시간은 1 이상 입력해 주세요.";
                return false;
            }
            if (pomodoroEnabled && (!int.TryParse(RepeatCountBox.Text, out var repeats) || repeats < 1))
            {
                ValidationText.Text = "반복 횟수는 1회 이상 입력해 주세요.";
                return false;
            }
        }

        settings.CountUpEnabled = isCountUp;
        settings.CharacterType = BunnyRadio.IsChecked == true ? "Bunny" : "Cat";
        settings.RestEnabled = pomodoroEnabled;
        settings.UseSecondsForTesting = !isCountUp && SecondsTestCheck.IsChecked == true;
        settings.FocusMinutes = isCountUp ? _data.Settings.FocusMinutes : int.Parse(FocusMinutesBox.Text);
        settings.RestMinutes = isCountUp || !pomodoroEnabled ? _data.Settings.RestMinutes : int.Parse(RestMinutesBox.Text);
        settings.RepeatEnabled = !isCountUp && pomodoroEnabled;
        settings.RepeatCount = isCountUp || !pomodoroEnabled ? _data.Settings.RepeatCount : int.Parse(RepeatCountBox.Text);
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

    private void TimerMode_Changed(object sender, RoutedEventArgs e) => UpdateTimerModeUi();

    private void PomodoroCheck_Changed(object sender, RoutedEventArgs e) => UpdateSessionModeUi();

    private void SecondsTestCheck_Changed(object sender, RoutedEventArgs e) => UpdateUnitLabels();

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
        CountdownSettings.IsHitTestVisible = !countUp;
        CountdownSettings.Opacity = countUp ? .52 : 1;
        CountdownCard.Background = countUp ? Brushes.White : (Brush)FindResource("Cream");
        CountupCard.Background = countUp ? (Brush)FindResource("Cream") : Brushes.White;
        StartButton.Content = countUp ? "▶ 카운트업 시작" : "▶ 집중 시작";
        UpdateSessionModeUi();
    }

    private void UpdateSessionModeUi()
    {
        if (PomodoroCheck == null || PomodoroPanel == null ||
            RestTimePanel == null || RepeatCountBox == null)
            return;

        var pomodoroEnabled = PomodoroCheck.IsChecked == true;
        RestTimePanel.IsEnabled = pomodoroEnabled;
        RepeatCountBox.IsEnabled = pomodoroEnabled;
        var countUp = CountupRadio?.IsChecked == true;
        var disabledBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E7E4DF"));
        PomodoroPanel.Background = countUp
            ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E7E4DF"))
            : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF7E5"));
    }

    private void UpdateUnitLabels()
    {
        if (FocusUnitText == null || SecondsTestCheck == null)
            return;

        var unit = SecondsTestCheck.IsChecked == true ? "초" : "분";
        FocusUnitText.Text = unit;
        RestUnitText.Text = unit;
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
}
