using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Sequencer : ISequencer
{
    private int currentIndex;
    private readonly ISample[] samples;
    public Sequencer(ISample[] samples, in TrackAliasComponent trackAlias)
    {
        this.samples = samples;
        TrackAlias = trackAlias;
        Array.ForEach(this.samples, sample => sample.TrackAlias = TrackAlias);
    }

    #region ISequencer Implementation

    #region ITrackItem Implementation
    public TrackAliasComponent TrackAlias { get; set; }
    #endregion

    public ISample Current => samples[currentIndex];

    public ISample MoveNext()
    {
        currentIndex = (++currentIndex % samples.Length);
        return Current;
    }

    public ISample BackToStart()
    {
        currentIndex = 0;
        return Current;
    }

    public void PitchAllUp(int numberOfSemitones) =>
        Array.ForEach(samples, sample => sample.PitchUp(numberOfSemitones));

    public void PitchAllDown(int numberOfSemitones) =>
        Array.ForEach(samples, sample => sample.PitchDown(numberOfSemitones));
    #endregion
}