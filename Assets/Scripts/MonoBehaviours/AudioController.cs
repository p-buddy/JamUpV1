using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Threading;

[RequireComponent(typeof(AudioSource))]
public class AudioController : Singleton<AudioController>
{
    public bool IsPlaying { get; private set; } = false;

    private List<AudioClip> clipRegister = new List<AudioClip>();
    private AudioSource audioSource;

    public void StartPlaying() => IsPlaying = true;

    public void RegisterAudioClip(ClipProxy clipProxy) => StartCoroutine(LoadAudio(clipProxy));

    public void UpdateStart(ClipProxy clipProxy, float time_s)
    {

    }

    public void UpdateEnd(ClipProxy clipProxy, float time_s)
    {

    }

    public void UpdatePitch(ClipProxy clipProxy, float pitchAmount_semitones)
    {
        
    }

    public void PlaySound(int clipIndex) => audioSource.PlayOneShot(clipRegister[clipIndex]);
    //public void PlaySound(int clipIndex) => ThreadPool.QueueUserWorkItem(ThreadProc, clipIndex);


    // This thread procedure performs the task.
    void ThreadProc(System.Object stateInfo)
    {
        int clipIndex = (int)stateInfo;
        audioSource.PlayOneShot(clipRegister[clipIndex]);
    }

    private IEnumerator LoadAudio(ClipProxy clipProxy)
    {
        WWW request = new WWW("file://" + clipProxy.Filename);
        yield return request;

        AudioClip audioClip = request.GetAudioClip();
        clipRegister.Add(audioClip);
        int clipIndex = clipRegister.Count - 1;
        clipProxy.SetIndex(clipIndex);
    }

    #region Monobehaviour
    void Start()
    {
        audioSource = gameObject.AddComponent<UnityEngine.AudioSource>();
    }
    #endregion
}
