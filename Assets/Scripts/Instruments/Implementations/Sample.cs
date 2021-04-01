using ClipManagement;
using MonoBehaviours;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Sample: ISample
{
    private string location;
    private ClipAliasComponent baseClip;
    private SampleState state;

    public Sample(string sampleLocation, in TrackAliasComponent trackAlias)
    {
        location = sampleLocation;
        GameManager.Instance.TryFetch(out IClipRegister register);
        register.TryRegisterClip(location, out baseClip);
        TrackAlias = trackAlias;
        state = new SampleState(0, 0f, 0f);
    }

    private ClipAliasComponent GetClipAlias()
    {
        GameManager.Instance.TryFetch(out IClipRegister register);
        register.TryGetClipAliasForState(baseClip, state, out ClipAliasComponent alias);
        return alias;
    }
    private void CreateEvent(in ClipAliasComponent clip, in PlayEventDetails details)
    {
        GameManager.Instance.TryFetch(out IAudioEventSpawner spawner);
        spawner.SpawnAudioEvent(clip, TrackAlias, state, details);
    }

    #region ISample Implementation
    public TrackAliasComponent TrackAlias { get; set; }

    #region INote Implementation

    public void PlayAt(float time, float volume = 1.0f) => 
        CreateEvent(GetClipAlias(), new PlayEventDetails(time, volume));
    
    
    public void PlayNow(float volume) => 
        CreateEvent(GetClipAlias(), new PlayEventDetails(0f, volume));

    public void ResetPitch() => state = new SampleState(0, state.StartOffset, state.EndOffset);

    public void PitchUp(int numberOfSemitones) =>         
        state = new SampleState(numberOfSemitones, state.StartOffset, state.EndOffset);

    public void PitchDown(int numberOfSemitones) =>
        state = new SampleState(-numberOfSemitones, state.StartOffset, state.EndOffset);

    public void TrimStart(double timeToTrim) =>       
        state = new SampleState(state.SemitoneOffset, timeToTrim, state.EndOffset);

    public void TrimEnd(double timeToTrim) =>
        state = new SampleState(state.SemitoneOffset, state.StartOffset, timeToTrim);

    public void Trim(double timeToTrimFromStart, double timeToTrimFromEnd) =>
        state = new SampleState(state.SemitoneOffset, timeToTrimFromStart, timeToTrimFromEnd);

    public void ResetLength() => state = new SampleState(state.SemitoneOffset, 0f, 0f);
    #endregion INote Implementation
    #endregion ISample Implementation
}