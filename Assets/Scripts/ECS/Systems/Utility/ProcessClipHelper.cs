using System;
using System.Collections;
using System.Collections.Generic;
using ECS.Systems.Jobs;
using ECS.Systems.Jobs.DTO;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Utility;

namespace ECS.Systems.Utility
{
    public readonly struct ProcessClipHelper<T, U> where T : unmanaged, IClipData<T> where U : unmanaged, IClipDataFactory<T>
    {
        private const int ParallelForBatchCount = 32;
        private const float MaxJobTime = 0.25f;
        private TimeInterval ClipInterval { get; }
        private float Volume { get; }
        private int Frequency { get; }
        private float Pitch { get; }

        private readonly float[] sampleData;
        public ProcessClipHelper(AudioClip clip, in PlayEventComponent eventDetails)
        {
            Volume = eventDetails.Volume;
            Pitch = eventDetails.Pitch;
            Frequency = clip.frequency;
            double startTime = eventDetails.TrackTime;
            double endTime = startTime + Math.Min(eventDetails.MaxPlayTime, clip.length);
            ClipInterval = new TimeInterval(startTime, endTime);
            
            sampleData = new float[clip.samples * clip.channels];
            clip.GetData(sampleData, 0);
        }

        public IEnumerator Process(Dictionary<float, NativeList<TimeInterval>> intervalsByPitch,
            Dictionary<float, NativeList<T>> clipDataByPitch,
            Dictionary<float, JobHandle> intervalHandlesByPitch,
            Dictionary<float, JobHandle> clipHandlesByPitch,
            Action onComplete)
        {
            NativeList<IndexRange> rangesToAdd = new NativeList<IndexRange>(Allocator.Persistent);
            
            yield return null;
            
            NativeArray<MinMaxIndex>? minMaxArray = null;
            JobHandle forwardJobHandle = default;
            if (!intervalsByPitch.TryGetValue(Pitch, out NativeList<TimeInterval> intervals))
            {
                intervalsByPitch[Pitch] = new NativeList<TimeInterval>(Allocator.Persistent);
                intervalsByPitch[Pitch].Add(ClipInterval);
                IndexRange range = new IndexRange(ClipInterval.Length, 0, Frequency.Inverse());
                rangesToAdd.Add(range);
            }
            else
            {
                minMaxArray = new NativeArray<MinMaxIndex>(1, Allocator.TempJob);
                yield return null;
                FindRangesToAddForwardJob forwardJob = new FindRangesToAddForwardJob(intervals,
                    rangesToAdd,
                    ClipInterval,
                    Frequency,
                    false,
                    minMaxArray.Value);
                intervalHandlesByPitch.TryGetValue(Pitch, out JobHandle intervalDependency);
                forwardJobHandle = forwardJob.Schedule(intervalDependency);
                intervalHandlesByPitch[Pitch] = forwardJobHandle;
            }
            
            yield return null;
        
            forwardJobHandle.Complete();

            yield return null;
            
            MinMaxIndex minMax = minMaxArray?[0] ?? new MinMaxIndex(rangesToAdd[0].StartIndex, rangesToAdd[0].EndIndex);
            minMaxArray?.Dispose();
            
            yield return null;
            
            if (!clipDataByPitch.TryGetValue(Pitch, out NativeList<T> clipData))
            {
                clipDataByPitch[Pitch] = new NativeList<T>(Allocator.Persistent);
                clipData = clipDataByPitch[Pitch];
            }

            yield return null;
            
            NativeArray<float> samples = new NativeArray<float>(sampleData.Length, Allocator.Persistent);
            
            yield return null;

            NativeArray<float>.Copy(sampleData, 0, samples, 0, sampleData.Length);

            yield return null;

            ExpandNativeListJob<T> expandNativeListJob = new ExpandNativeListJob<T>(clipData, rangesToAdd, false);
            clipHandlesByPitch.TryGetValue(Pitch, out JobHandle clipDependency);
            JobHandle handle = expandNativeListJob.Schedule(clipDependency);

            AddToClipDataJob<T, U> addToClipDataJob = new AddToClipDataJob<T, U>(minMax.Min, clipData, samples, Volume);
            handle = addToClipDataJob.Schedule(minMax.Max - minMax.Min, ParallelForBatchCount, handle);
            clipHandlesByPitch[Pitch] = handle;
            samples.Dispose(handle);

            float startTime = Time.time;
            while (!handle.IsCompleted && Time.time - startTime < MaxJobTime)
            {
                yield return null;
            }
            handle.Complete();
            onComplete.Invoke();

            rangesToAdd.Dispose();
        }
    }
}