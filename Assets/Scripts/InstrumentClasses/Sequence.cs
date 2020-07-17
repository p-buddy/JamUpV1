using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Sequence : IInstrument
{
    private int currentIndex;
    public Sample[] Samples { get; private set; }

    public Sequence(Sample[] samples)
    {
        this.Samples = samples;
        SetParentForSamples();
    }

    public void PlayNext(float time) => Samples[NextIndex()].Play(time);

    public void SetAllPitches(float pitchAmount) =>
        Array.ForEach(Samples, sample => sample.SetPitch(pitchAmount));

    private void SetParentForSamples() =>
        Array.ForEach(Samples, sample => sample.Parent = this);

    private int NextIndex()
    {
        currentIndex = (++currentIndex % Samples.Length);
        return currentIndex;
    }
}