using System.Collections.Generic;
using System.Linq;
using ECS.Systems.Jobs;
using ECS.Systems.Jobs.DTO;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Utility;

namespace Tests
{
    public class FindRangesToAddTest
    {
        [Test]
        public void Test()
        {
            int frequency = 10;
            Debug.Log($"Frequency: {frequency}");
            Debug.Log($"Resolution: { 1 / (double)frequency}");
            List<TimeInterval> intervalsList = new List<TimeInterval>()
            {
                new TimeInterval(0.1, 3.0),
                new TimeInterval(3.1, 4.0),
                new TimeInterval(6.8, 6.9),
            };
            NativeList<TimeInterval> intervals = new NativeList<TimeInterval>(Allocator.TempJob);
            intervals.Fill(intervalsList);
            
            Debug.Log("Intervals:");
            intervals.Log();
            Debug.Log("\n");

            NativeList<IndexRange> ranges = new NativeList<IndexRange>(Allocator.TempJob);
            NativeArray<MinMaxIndex> minMaxIndices = new NativeArray<MinMaxIndex>(1, Allocator.TempJob);

            int iterations = 3;
            List<TimeInterval> queries = new List<TimeInterval>
            {
                new TimeInterval(2.5, 4.5),
                new TimeInterval(0, 5.5),
                new TimeInterval(4, 9)
            };
            for (int i =0; i < iterations; i++)
            {
                Debug.Log($"Iteration {i}: Query {queries[i]}");
                TimeInterval query = queries[i];
                
                FindRangesToAddForwardJob forwardJob = new FindRangesToAddForwardJob
                {
                    Intervals = intervals,
                    RangesToAdd = ranges,
                    IntervalQuery = query,
                    Frequency = frequency,
                    RangesWillBeIteratedBackwards = false,
                    MinMax = minMaxIndices,
                };
                JobHandle handle = forwardJob.Schedule();
                handle.Complete();
            
                Debug.Log("Intervals:");
                intervals.Log();
            
                Debug.Log("\nRanges:");
                ranges.Log();
                
                Debug.Log("\nMin Max:");
                Debug.Log(minMaxIndices[0]);
                Debug.Log("\n");
            }
        }
    }
}