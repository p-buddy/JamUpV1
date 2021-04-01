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
        CoroutineProcessor.Instance.EnqueCoroutine(Begin());
    }

    private IEnumerator Begin()
    {
        GameManager.Instance.TryFetch(out IClipRegister register);
        while (register.LoadingInProgress())
        {
            yield return null;
        }
        GameManager.Instance.StartSong();
    }
}
