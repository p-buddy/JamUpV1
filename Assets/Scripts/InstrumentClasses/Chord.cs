using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Chord : IInstrument
{
    public Sample[] Samples { get; private set; }

    public Chord(Sample[] samples)
    {
        this.Samples = samples;
        SetParentForSamples();
    }

    public void Play(float time) =>
        Array.ForEach(Samples, sample => sample.Play(time));

    public void SetAllPitches(float pitchAmount) =>
        Array.ForEach(Samples, sample => sample.SetPitch(pitchAmount));

    private void SetParentForSamples() =>
        Array.ForEach(Samples, sample => sample.Parent = this);
}
