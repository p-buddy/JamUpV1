using System.Collections.Generic;
using ECS.Systems.Jobs;
using ECS.Systems.Jobs.DTO;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Utility;

namespace Tests
{
    public class ExpandNativeListTest
    {
        public struct TestCase<T>
        {
            public IndexRange Range;
            public List<T> Initial;

            public override string ToString()
            {
                return $"Range: [Range: {Range}; Count: {Range.Count}]; Initial: [Count: {Initial.Count}]";
            }
        }

        private static List<T> GetTestList<T>(T value, int count)
        {
            List<T> toReturn = new List<T>();
            for (int i = 0; i < count; i++)
            {
                toReturn.Add(value);
            }

            return toReturn;
        }

        private static List<TestCase<int>> IntTestCases = new List<TestCase<int>>()
        {
            new TestCase<int>()
            {
                Range = new IndexRange(1, 0, 1),
                Initial = GetTestList(0, 10),
            },
            new TestCase<int>()
            {
                Range = new IndexRange(10, 0, 1),
                Initial = GetTestList(0, 10),
            },
            new TestCase<int>()
            {
                Range = new IndexRange(20, 10, 1),
                Initial = GetTestList(0, 10),
            },
            new TestCase<int>()
            {
                Range = new IndexRange(20, 10, 2),
                Initial = GetTestList(0, 10),
            },
        };
        
        [Test]
        public void Test([ValueSource(nameof(IntTestCases))]TestCase<int> testCase)
        {
            NativeList<IndexRange> ranges = new NativeList<IndexRange>(Allocator.TempJob);
            ranges.Add(testCase.Range);
            
            NativeList<int> toExpand = new NativeList<int>(Allocator.TempJob);
            toExpand.Fill(testCase.Initial);

            ExpandNativeListJob<int> job = new ExpandNativeListJob<int>()
            {
                RangesToAdd = ranges,
                ToExpand = toExpand
            };
            
            JobHandle handle = job.Schedule();
            handle.Complete();
            
            int[] toExpandArray = toExpand.ToArray();
            ranges.Dispose(handle);
            toExpand.Dispose(handle);

            for (int i = testCase.Range.StartIndex; i <= testCase.Range.EndIndex; i++)
            {
                toExpandArray[i] = 0;
            }

            int actual = toExpandArray.Length;
            int expected = (testCase.Initial.Count + testCase.Range.Count);
            Assert.IsTrue(actual == expected, $"Expanded Length {actual} != Initial Array + Range {expected}");
        }
    }
}