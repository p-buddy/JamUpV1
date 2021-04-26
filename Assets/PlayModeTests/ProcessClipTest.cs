using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClipManagement;
using ECS.Systems.Jobs;
using ECS.Systems.Jobs.DTO;
using ECS.Systems.Utility;
using MonoBehaviours.Utility;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;
using Utility;
using Random = System.Random;

namespace PlayModeTests
{
    [Category("Clip Processing")]
    public class ProcessClipTest
    {
        public static List<List<PlayEventComponent>> GetEvents()
        {
            List<List<PlayEventComponent>> testCases = new List<List<PlayEventComponent>>();
            
            int count = 1000; 
            float spacing = 0.3f;
            var events = new List<PlayEventComponent>();
            Random rnd = new Random();
            while (events.Count < count)
            {
                events.Add(new PlayEventComponent(new SampleState(0, 0, 1),
                    new PlayEventDetails(events.Count * spacing, 10.0f)));
            }
            testCases.Add(events);
            
            
            
            return testCases;
        }
        
        [UnityTest]
        public IEnumerator Test([ValueSource(nameof(GetEvents))]List<PlayEventComponent> events)
        {
            IClipRegister register = new ClipRegister();
            register.TryRegisterClip("/Users/parkermalachowsky/Downloads/fef.wav", out ClipAliasComponent alias);
            while (register.LoadingInProgress())
            {
                yield return null;
            }

            Assert.IsTrue(register.TryGetClip(in alias, out AudioClip clip));

            var stereoIntervalsByPitch = new Dictionary<float, NativeList<TimeInterval>>();
            var stereoClipDataByPitch = new Dictionary<float, NativeList<StereoClipData>>();
            var stereoIntervalHandlesByPitch = new Dictionary<float, JobHandle>();
            var stereoClipHandlesByPitch = new Dictionary<float, JobHandle>();
            var stereoProcessCountByPitch = new Dictionary<float, int>();
            
            Debug.Log(Time.time);
            foreach (var eventDetails in events)
            {
                if (!stereoProcessCountByPitch.ContainsKey(eventDetails.Pitch))
                {
                    stereoProcessCountByPitch[eventDetails.Pitch] = 1;
                }
                else
                {
                    stereoProcessCountByPitch[eventDetails.Pitch] = stereoProcessCountByPitch[eventDetails.Pitch] + 1;
                }
                var clipProcessor = new ProcessClipHelper<StereoClipData, FloatToStereoData>(clip, in eventDetails);
                CoroutineProcessor.Instance.EnqueCoroutine(clipProcessor.Process(stereoIntervalsByPitch,
                    stereoClipDataByPitch,
                    stereoIntervalHandlesByPitch,
                    stereoClipHandlesByPitch,
                    () =>
                    {
                        stereoProcessCountByPitch[eventDetails.Pitch] = stereoProcessCountByPitch[eventDetails.Pitch] - 1;
                        if (stereoProcessCountByPitch[eventDetails.Pitch] == 0)
                        {
                            stereoProcessCountByPitch.Remove(eventDetails.Pitch);
                        }
                    }));
            }

            float maxFrameTime = 0;
            float averageFrameTime = 0;
            int frameCount = 0;
            float previous = Time.time;
            
            while (stereoProcessCountByPitch.Count > 0)
            {
                averageFrameTime += (Time.time - previous);
                maxFrameTime = Mathf.Max(maxFrameTime, Time.time - previous);
                previous = Time.time;
                frameCount++;
                yield return null;
            }

            averageFrameTime *= (frameCount > 0) ? 1 / (float) frameCount : 1;
            
            Debug.Log("Avg: " + averageFrameTime);
            Debug.Log("Max: " + maxFrameTime);
            Debug.Log(Time.time);

            stereoIntervalHandlesByPitch.Values.ToList().ForEach((handle) => handle.Complete());
            stereoClipHandlesByPitch.Values.ToList().ForEach((handle) => handle.Complete());

            yield return null;

            Dictionary<float, NativeArray<float>> sampleArrayByPitch = new Dictionary<float, NativeArray<float>>();
            Dictionary<float, JobHandle> sampleHandlesByPitch = new Dictionary<float, JobHandle>();

            foreach (var kvp in stereoClipDataByPitch)
            {
                float pitch = kvp.Key;
                var samples = new NativeArray<float>(kvp.Value.Length * 2, Allocator.TempJob);
                FloatsFromClipData getDataJob = new FloatsFromClipData()
                {
                    ClipData = kvp.Value,
                    toFill = samples,
                };
                sampleArrayByPitch[pitch] = samples;
                sampleHandlesByPitch[pitch] = getDataJob.Schedule(samples.Length, 32);
            }

            yield return null;
            yield return null;
            yield return null;
            sampleHandlesByPitch.Values.ToList().ForEach(handle => handle.Complete());
            
            double timeToPlay = AudioSettings.dspTime + 0.2;
            foreach (var kvp in sampleArrayByPitch)
            {
                float pitch = kvp.Key;
                AudioClip runtimeClip = AudioClip.Create($"{pitch}", kvp.Value.Length / clip.channels, clip.channels, clip.frequency, false);
                runtimeClip.SetData(kvp.Value.ToArray(), 0);
                kvp.Value.Dispose();

                AudioSource source = (new GameObject($"{pitch}")).AddComponent<AudioSource>();
                source.clip = runtimeClip;
                source.pitch = pitch;
                source.PlayScheduled(timeToPlay);
            }

            (new GameObject("Listener")).AddComponent<AudioListener>();

            float time = 100f;
            float elapsed = 0f;
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            stereoIntervalsByPitch.Values.ToList().ForEach((collection) => collection.Dispose());
            stereoClipDataByPitch.Values.ToList().ForEach((collection) => collection.Dispose());
        }
    }
}