using System;
using System.Collections;
using AudioEventSystem.DTO;
using MonoBehaviours.Utility;
using Unity.Jobs;

public interface IAudioEventSpawner : IRetrievableFromGameManager
{
    void EnqueueAudioEvent(in AudioEvent audioEvent);
    IEnumerator SpawnAll(Action onComplete);

    void ClearAudioEvents();
}
