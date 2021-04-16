using System.Collections;
using System.Collections.Generic;
using ClipManagement;
using MonoBehaviours;
using MonoBehaviours.Utility;
using UnityEngine;
using UnityEngine.Audio;

public class PlayAudio : MonoBehaviour
{
    public void StartPlaying()
    {
        GameManager.Instance.TryFetch(out IAudioEventSpawner spawner);
        CoroutineProcessor.Instance.EnqueCoroutine(spawner.SpawnAll(() => Debug.Log("Done")));
        CoroutineProcessor.Instance.EnqueCoroutine(Begin());
    }

    private IEnumerator Begin()
    {
        yield return null;
        GameManager.Instance.TryFetch(out IClipRegister register);
        while (register.LoadingInProgress())
        {
            yield return null;
        }
        GameManager.Instance.StartSong();
    }
}
