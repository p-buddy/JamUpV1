using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Sample: IInstrument
{
    private Action<float, float> playAction;
    private Action<float> playNowAction;
    private Action<float> setPitchAction;
    private Action<float> setStartAction;
    private Action<float> setEndAction;
    private IInstrument parent;

    public ClipProxy Clip { get; private set; }
    public IInstrument Parent { get => parent; set => parent = value; }

    public Sample(ClipProxy clip,
        Action<float, float> playAction,
        Action<float> playNowAction,
        Action<float> setPitchAction,
        Action<float> setStartAction,
        Action<float> setEndAction)
    {
        this.Clip = clip;
        this.playAction = playAction;
        this.playNowAction = playNowAction;
        this.setPitchAction = setPitchAction;
        this.setStartAction = setStartAction;
        this.setEndAction = setEndAction;
    }

    public void Play(float time, float volume = 1.0f) => playAction?.Invoke(time, volume);
    public void PlayNow(float volume) => playNowAction?.Invoke(volume);
    public void SetPitch(float pitchAmount) => setPitchAction?.Invoke(pitchAmount);
    public void SetStart(float startTime) => setStartAction?.Invoke(startTime);
    public void SetEnd(float endTime) => setEndAction?.Invoke(endTime);
}