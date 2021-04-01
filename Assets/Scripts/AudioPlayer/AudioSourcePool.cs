using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AudioSourcePool : IAudioClipPlayer
{
    private const int RefillThreshold = 3;
    private const string ContainerName = "Audio Source Container";
    private const float StoMs = 1000;
    private const int ReleaseDelay = 100;

    private Transform container;

    private readonly ConcurrentQueue<AudioSource> pool;
    private readonly Dictionary<Guid, List<AudioSource>> sourcesByID;

    public AudioSourcePool()
    {
        container = new GameObject(ContainerName).transform;
        pool = new ConcurrentQueue<AudioSource>();
        sourcesByID = new Dictionary<Guid, List<AudioSource>>();
    }

    #region IAudioClipPlayer implementation
    public void Schedule(AudioClip clip, double when, float pitch, float volume, Guid trackID, bool isMono = true)
    {
        CheckPoolStatus();
        if (!pool.TryDequeue(out AudioSource source))
        {
            return;
        }

        source.clip = clip;
        source.pitch = pitch;
        source.volume = volume;
        source.PlayScheduled(when);

        //HandleMonoTracks(trackID, when, source);

        float? clipEnd = clip.length;
        ThreadPool.QueueUserWorkItem(ReleaseAudioSource, new object[] {clipEnd, source});
    }

    public void Now(AudioClip clip, float pitch, float volume, Guid trackID, bool isMono = true)
    {
        throw new NotImplementedException();
    }
    #endregion

    private void HandleMonoTracks(Guid trackID, double dspTime, AudioSource newSource)
    {
        if (!sourcesByID.TryGetValue(trackID, out List<AudioSource> sources))
        {
            sourcesByID[trackID] = new List<AudioSource>() {newSource};
            return;
        }
        
        foreach (AudioSource sameTrackSources in sourcesByID[trackID])
        {
            if (sameTrackSources.isPlaying)
            {
                sameTrackSources.SetScheduledEndTime(dspTime);
            }
        }
        sourcesByID[trackID].Add(newSource);
    }
    private void CheckPoolStatus()
    {
        if (pool.Count < RefillThreshold)
        {
            AudioSource toAdd = new GameObject($"Audio Source (from {Time.time}").AddComponent<AudioSource>();
            toAdd.transform.parent = container;
            pool.Enqueue(toAdd);
        }
    }

    private void ReleaseAudioSource(object state)
    {
        object[] array = state as object[];
        float time = (float)(array[0] as float?);
        int timeMs = (int) (time * StoMs) + ReleaseDelay;
        Thread.Sleep(timeMs);
        AudioSource source = array[1] as AudioSource;
        source.clip = null;
        pool.Enqueue(source);
    }
}
