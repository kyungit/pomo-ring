using System.Windows.Threading;
using PomoRing.Models;

namespace PomoRing.Services;

public enum PomodoroState { Idle, Focus, Rest, Paused, Completed }

public sealed class PomodoroTimerService
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private PomodoroSettings _settings = new();
    private PomodoroState _stateBeforePause;

    public PomodoroState State { get; private set; } = PomodoroState.Idle;
    public TimeSpan Remaining { get; private set; }
    public TimeSpan Elapsed { get; private set; }
    public bool IsCountUp => _settings.CountUpEnabled;
    public int CompletedFocusSessions { get; private set; }

    public event Action? Tick;
    public event Action<PomodoroState>? StateChanged;
    public event Action? FocusSessionCompleted;
    public event Action? FocusMinuteCompleted;

    public PomodoroTimerService() => _timer.Tick += OnTick;

    public void Start(PomodoroSettings settings)
    {
        _settings = settings;
        CompletedFocusSessions = 0;
        Elapsed = TimeSpan.Zero;
        if (settings.CountUpEnabled)
            BeginCountUp();
        else
            Begin(PomodoroState.Focus, GetDuration(settings.FocusMinutes));
        _timer.Start();
    }

    public void TogglePause()
    {
        if (State == PomodoroState.Paused)
        {
            State = _stateBeforePause;
            _timer.Start();
        }
        else if (State is PomodoroState.Focus or PomodoroState.Rest)
        {
            _stateBeforePause = State;
            State = PomodoroState.Paused;
            _timer.Stop();
        }
        StateChanged?.Invoke(State);
    }

    public void Stop()
    {
        _timer.Stop();
        State = PomodoroState.Idle;
        StateChanged?.Invoke(State);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_settings.CountUpEnabled && State == PomodoroState.Focus)
        {
            Elapsed = Elapsed.Add(TimeSpan.FromSeconds(1));
            if (Elapsed.TotalSeconds % 60 == 0)
                FocusMinuteCompleted?.Invoke();
            Tick?.Invoke();
            return;
        }

        Remaining = Remaining.Subtract(TimeSpan.FromSeconds(1));
        if (Remaining <= TimeSpan.Zero)
            Advance();
        Tick?.Invoke();
    }

    private void Advance()
    {
        if (State == PomodoroState.Focus)
        {
            CompletedFocusSessions++;
            FocusSessionCompleted?.Invoke();
            if (!_settings.RestEnabled || !_settings.RepeatEnabled ||
                CompletedFocusSessions >= _settings.RepeatCount || _settings.RestMinutes == 0)
            {
                Complete();
                return;
            }
            Begin(PomodoroState.Rest, GetDuration(_settings.RestMinutes));
        }
        else if (State == PomodoroState.Rest)
        {
            Begin(PomodoroState.Focus, GetDuration(_settings.FocusMinutes));
        }
    }

    private TimeSpan GetDuration(int value) => _settings.UseSecondsForTesting
        ? TimeSpan.FromSeconds(value)
        : TimeSpan.FromMinutes(value);

    private void Begin(PomodoroState state, TimeSpan duration)
    {
        State = state;
        Remaining = duration;
        StateChanged?.Invoke(State);
        Tick?.Invoke();
    }

    private void BeginCountUp()
    {
        State = PomodoroState.Focus;
        Remaining = TimeSpan.Zero;
        StateChanged?.Invoke(State);
        Tick?.Invoke();
    }

    private void Complete()
    {
        _timer.Stop();
        State = PomodoroState.Completed;
        Remaining = TimeSpan.Zero;
        StateChanged?.Invoke(State);
    }
}
