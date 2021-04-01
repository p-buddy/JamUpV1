using MonoBehaviours.Utility;

public interface IAudioEventSpawner : IRetrievableFromGameManager
{
    void SpawnAudioEvent(in ClipAliasComponent clip,
        in TrackAliasComponent track,
        in SampleState state,
        in PlayEventDetails details);

    void ClearAudioEvents();
}
