using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Chord : IChord
{
    private ISample[] samples;
    public Chord(ISample[] samples, in TrackAliasComponent trackAlias)
    {
        this.samples = samples;
        TrackAlias = trackAlias;
        Array.ForEach(this.samples, sample => sample.TrackAlias = TrackAlias);
    }

    #region IChord Implementation
    public ISample this[int index] =>
        (index >= 0 && index < samples.Length - 1) 
            ? samples[index]
            : throw new IndexOutOfRangeException();
    
    #region ITrackItem Implementation
    public TrackAliasComponent TrackAlias { get; set; }
    #endregion ITrackItem Implementation

    #region INote Implementation
    public void PlayAt(float time, float volume) =>
        Array.ForEach(samples, sample => sample.PlayAt(time, volume));

    public void PlayNow(float volume) => 
        Array.ForEach(samples, sample => sample.PlayNow(volume));

    public void ResetPitch() =>
        Array.ForEach(samples, sample => sample.ResetPitch());

    public void PitchUp(int numberOfSemitones) =>
        Array.ForEach(samples, sample => sample.PitchUp(numberOfSemitones));

    public void PitchDown(int numberOfSemitones) =>
        Array.ForEach(samples, sample => sample.PitchDown(numberOfSemitones));

    public void TrimStart(double timeToTrim) =>
        Array.ForEach(samples, sample => sample.TrimStart(timeToTrim));

    public void TrimEnd(double timeToTrim) =>
        Array.ForEach(samples, sample => sample.TrimEnd(timeToTrim));

    public void Trim(double timeToTrimFromStart, double timeToTrimFromEnd) => 
        Array.ForEach(samples, sample => sample.Trim(timeToTrimFromStart, timeToTrimFromEnd));

    public void ResetLength() => Array.ForEach(samples, sample => sample.ResetLength());
    #endregion
    #endregion IChord Implementation

}
