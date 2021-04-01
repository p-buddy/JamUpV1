

#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.WSA;

namespace ECS.Systems.Jobs
{
    public struct CrawlThroughClipsJob : IJob
    {
        private const int SplitClipThreshold = 1;
        private readonly struct DTO
        {
            public int SamplePosition { get; }
            public float? Left { get; }
            public float? Right { get; }
            public float? Mono { get; }

            public DTO(in ClipMicroSample clipSample)
            {
                SamplePosition = clipSample.SamplePosition;
                Left = clipSample.Left;
                Right = clipSample.Right;
                Mono = clipSample.Mono;
            }
        }

        private struct PitchBuffer
        {
            public float pitch;
            public readonly Dictionary<ChannelType, List<List<DTO>>> buffer;

            public PitchBuffer(in ClipMicroSample sample)
            {
                pitch = sample.Pitch;
                buffer = new Dictionary<ChannelType, List<List<DTO>>>();
                foreach(ChannelType value in Enum.GetValues(typeof(ChannelType)))
                {
                    buffer[value] = new List<List<DTO>>();
                }
                buffer[sample.ChannelType].Add(new List<DTO>{new DTO(sample)});
            }

            public void Add(in ClipMicroSample sample)
            {
                buffer[sample.ChannelType].Add(new List<DTO>{new DTO(sample)}); 
            }

            public List<DTO> GetLatestRelevantCollection(in ClipMicroSample sample)
            {
                int count = buffer[sample.ChannelType].Count;
                return buffer[sample.ChannelType][count - 1];
            }
            
            public DTO GetLatestRelevantEntry(in ClipMicroSample sample)
            {
                List<DTO> collection = GetLatestRelevantCollection(sample);
                int count = collection.Count;
                return collection[count - 1];
            }
        }
            
        
        [Unity.Collections.ReadOnly]
        public NativeArray<ClipMicroSample> sortedSamples;
        [Unity.Collections.ReadOnly]
        public EntityArchetype archetype;
        public EntityCommandBuffer ecb;
        
        public void Execute()
        {
            float SamplePositionToTrackTime(int samplePosition) => (float)samplePosition / 44100;
            
            Dictionary<float, PitchBuffer> buffersForPitch = new Dictionary<float, PitchBuffer>();
            foreach (ClipMicroSample sample in sortedSamples)
            {
                if (!buffersForPitch.TryGetValue(sample.Pitch, out PitchBuffer buffer))
                {
                    buffersForPitch[sample.Pitch] = new PitchBuffer(sample);
                    continue;
                }

                if (buffer.GetLatestRelevantEntry(sample).SamplePosition + SplitClipThreshold >= sample.SamplePosition)
                {
                    buffer.GetLatestRelevantCollection(sample).Add(new DTO(sample));
                }
                else
                {
                    buffer.Add(sample);
                }
            }

        }
    }
}