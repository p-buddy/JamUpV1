using System;
using System.Collections;
using UnityEngine;

public class SampleFactory : Singleton<SampleFactory>
{
    public Sample CreateSample(string locationOfSample)
    {
        ClipProxy clip = new ClipProxy(locationOfSample);
        CoroutineProcessor cp = CoroutineProcessor.Instance;
        Sample sample = new Sample(clip,
            (float time, float volume) => cp.EnqueCoroutine(SetupPlayEventForClip(clip, time, volume)),
            (float volume) => cp.EnqueCoroutine(SetupPlayClipImmediate(clip, volume)),
            (float pitchAmount) => cp.EnqueCoroutine(SetPitchForClip(clip, pitchAmount)),
            (float time) => cp.EnqueCoroutine(SetStartForClip(clip, time)),
            (float time) => cp.EnqueCoroutine(SetEndForClip(clip, time)));
        AudioController.Instance.RegisterAudioClip(clip);
        return sample;
    }

    private IEnumerator WaitForClip(ClipProxy clip)
    {
        while (!clip.IsReady())
        {
            yield return null;
        }
    }

    private IEnumerator SetupPlayEventForClip(ClipProxy clip, float time, float volume)
    {
        yield return WaitForClip(clip);
        AudioEventEntitySpawner.Instance.SpawnAudioEvent(clip, time);
    }

    private IEnumerator SetupPlayClipImmediate(ClipProxy clip, float volume)
    {
        yield return WaitForClip(clip);
        // AudioEventEntitySpawner.Instance.SpawnAudioEvent(clip, time);
    }

    private IEnumerator SetPitchForClip(ClipProxy clip, float pitchAmount)
    {
        yield return WaitForClip(clip);
        AudioController.Instance.UpdatePitch(clip, pitchAmount);
    }

    private IEnumerator SetStartForClip(ClipProxy clip, float startTime)
    {
        yield return WaitForClip(clip);
        AudioController.Instance.UpdateStart(clip, startTime);
    }

    private IEnumerator SetEndForClip(ClipProxy clip, float endTime)
    {
        yield return WaitForClip(clip);
        AudioController.Instance.UpdateEnd(clip, endTime);
    }
}
