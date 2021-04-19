using System.Collections;
using System.Collections.Generic;
using ClipManagement;
using ECS.Systems.Jobs.DTO;
using ECS.Systems.Utility;
using MonoBehaviours.Utility;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;
using Utility;

namespace PlayModeTests
{
    [Category("Clip Processing")]
    public class ProcessClipTest
    {
        public static List<List<PlayEventComponent>> GetEvents()
        {
            List<List<PlayEventComponent>> testCases = new List<List<PlayEventComponent>>()
            {
                new List<PlayEventComponent>()
                {
                    new PlayEventComponent(new SampleState(0, 0, 1), new PlayEventDetails(1.0f, 1.0f))
                },
            };
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

            foreach (var eventDetails in events)
            {
                if (!stereoProcessCountByPitch.ContainsKey(eventDetails.Pitch))
                {
                    stereoProcessCountByPitch[eventDetails.Pitch] = 1;
                }
                var clipProcessor = new ProcessClipHelper<StereoClipData, FloatToStereoData>(clip, in eventDetails);
                CoroutineProcessor.Instance.EnqueCoroutine(clipProcessor.Process(stereoIntervalsByPitch,
                    stereoClipDataByPitch,
                    stereoIntervalHandlesByPitch,
                    stereoClipHandlesByPitch,
                    () =>
                    {
                        stereoClipDataByPitch[eventDetails.Pitch].Log();
                        stereoProcessCountByPitch[eventDetails.Pitch] = stereoProcessCountByPitch[eventDetails.Pitch] - 1;
                    }));
            }

            while (stereoProcessCountByPitch[0.ToFrequency()] > 0)
            {
                yield return null;
            }
        }
    }
}