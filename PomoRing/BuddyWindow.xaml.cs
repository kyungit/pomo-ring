using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PomoRing.Models;
using PomoRing.Services;

namespace PomoRing;

public partial class BuddyWindow : Window
{
    private readonly MainWindow _settingsWindow;
    private readonly AppData _data;
    private readonly SettingsStorageService _storage;
    private readonly PomodoroTimerService _timer = new();
    private PomodoroSettings _settings = new();

    public BuddyWindow(MainWindow settingsWindow, AppData data, SettingsStorageService storage)
    {
        InitializeComponent();
        _settingsWindow = settingsWindow;
        _data = data;
        _storage = storage;
        _timer.Tick += RefreshTimer;
        _timer.StateChanged += RefreshState;
        _timer.FocusSessionCompleted += CompleteFocusSession;
        _timer.FocusMinuteCompleted += CompleteFocusMinute;
        PositionNearCorner();
        StartPixelHop();
    }

    public void Start(PomodoroSettings settings)
    {
        _settings = settings;
        TimerBubble.Visibility = settings.ShowTimer ? Visibility.Visible : Visibility.Collapsed;
        ApplyCharacter(settings.CharacterType);
        ApplyEvolution();
        Show();
        _timer.Start(settings);
    }

    private void RefreshTimer()
    {
        if (_timer.State == PomodoroState.Completed)
        {
            TimerText.Text = "GOOD JOB!";
            return;
        }

        var display = _timer.IsCountUp ? _timer.Elapsed : _timer.Remaining;
        TimerText.Text = $"{Math.Max(0, (int)display.TotalMinutes):00}:{Math.Max(0, display.Seconds):00}";
    }

    private void RefreshState(PomodoroState state)
    {
        var isRest = state == PomodoroState.Rest;
        var isPaused = state == PomodoroState.Paused;
        FocusProps.Visibility = isRest ? Visibility.Collapsed : Visibility.Visible;
        RestProps.Visibility = isRest ? Visibility.Visible : Visibility.Collapsed;
        LeftEye.Visibility = isRest ? Visibility.Collapsed : Visibility.Visible;
        RightEye.Visibility = isRest ? Visibility.Collapsed : Visibility.Visible;
        SleepLeftEye.Visibility = isRest ? Visibility.Visible : Visibility.Collapsed;
        SleepRightEye.Visibility = isRest ? Visibility.Visible : Visibility.Collapsed;
        SetRestMotion(isRest);
        StateDot.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(
            state switch { PomodoroState.Rest => "#79B99A", PomodoroState.Paused => "#F8CE68", _ => "#F38B76" }));
        PauseMenuItem.Header = isPaused ? "계속하기" : "일시정지";

        if (state == PomodoroState.Completed)
        {
            FocusProps.Visibility = Visibility.Collapsed;
            RestProps.Visibility = Visibility.Visible;
            MessageBox.Show("집중 완료! 버디가 아주 뿌듯해해요.", "PomoRing",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        RefreshTimer();
    }

    private void CompleteFocusSession() => AddFocusMinutes(_settings.FocusMinutes);

    private void CompleteFocusMinute() => AddFocusMinutes(1);

    private void AddFocusMinutes(int minutes)
    {
        _data.Progress.TotalFocusMinutes += minutes;
        _data.Progress.TodayFocusMinutes += minutes;
        _data.Progress.LastStudyDate = DateOnly.FromDateTime(DateTime.Today);
        _storage.Save(_data);
        ApplyEvolution();
    }

    private void ApplyEvolution()
    {
        var total = _data.Progress.TotalFocusMinutes;
        Scarf.Visibility = total >= 60 ? Visibility.Visible : Visibility.Collapsed;
        StarPin.Visibility = total >= 180 ? Visibility.Visible : Visibility.Collapsed;
        Crown.Visibility = total >= 600 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ApplyCharacter(string characterType)
    {
        var isBunny = characterType == "Bunny";
        CatEars.Visibility = !isBunny ? Visibility.Visible : Visibility.Collapsed;
        CatTail.Visibility = !isBunny ? Visibility.Visible : Visibility.Collapsed;
        BunnyEars.Visibility = isBunny ? Visibility.Visible : Visibility.Collapsed;
        BunnyTail.Visibility = isBunny ? Visibility.Visible : Visibility.Collapsed;

        var coat = isBunny ? "#F1D8C2" : "#D6A276";
        var lightCoat = isBunny ? "#FFF0E3" : "#F4C89A";
        HeadCoat.Fill = BodyCoat.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(coat));
        MuzzleCoat.Fill = BellyCoat.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lightCoat));
    }

    private void StartPixelHop()
    {
        var animation = new DoubleAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromSeconds(1.2),
            RepeatBehavior = RepeatBehavior.Forever
        };
        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromPercent(0)));
        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(-4, KeyTime.FromPercent(.5)));
        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromPercent(1)));
        ((TranslateTransform)BuddyBody.RenderTransform).BeginAnimation(TranslateTransform.YProperty, animation);
    }

    private void SetRestMotion(bool isRest)
    {
        var transform = (TranslateTransform)BuddyBody.RenderTransform;
        if (isRest)
        {
            transform.BeginAnimation(TranslateTransform.YProperty, null);
            transform.Y = 2;
        }
        else
        {
            StartPixelHop();
        }
    }

    private void PositionNearCorner()
    {
        Left = SystemParameters.WorkArea.Right - Width - 24;
        Top = SystemParameters.WorkArea.Bottom - Height - 24;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private void PauseMenuItem_Click(object sender, RoutedEventArgs e) => _timer.TogglePause();

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        Hide();
        _settingsWindow.OpenSettings();
    }

    private void TodayMenuItem_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show($"오늘 {_data.Progress.TodayFocusMinutes}분 집중했어요.", "오늘의 PomoRing");

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _storage.Save(_data);
        Application.Current.Shutdown();
    }
}
