using System;
using ECS.Systems.Jobs.DTO;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Utility;

namespace ECS.Systems.Jobs
{
    [BurstCompile]
    public struct ExpandNativeListJob<T> : IJob where T : struct
    {
        public NativeList<T> ToExpand;
        [ReadOnly]
        public NativeList<IndexRange> RangesToAdd;

        [ReadOnly] 
        public bool IterateBackwards;
        
        public void Execute()
        {
            if (RangesToAdd.Length == 0)
            {
                return;
            }

            if (IterateBackwards)
            {
                for (int index = RangesToAdd.Length - 1; index >= 0; index--)
                {
                    AddRange(RangesToAdd[index]);
                }
            }
            else
            {
                for (int index = 0; index < RangesToAdd.Length; index++)
                {
                    AddRange(RangesToAdd[index]);
                }
            }
        }

        private void AddRange(IndexRange interval)
        {
            int amountToAdd = interval.Count;
            if (interval.StartIndex == ToExpand.Length)
            {
                ToExpand.Add(default);
                amountToAdd--;
            }
            int amountAdded = 0;
            while (amountAdded < amountToAdd)
            {
                int amountRemaining = amountToAdd - amountAdded;
                int lastIndex = Math.Min(ToExpand.Length, interval.StartIndex + amountRemaining);
                ToExpand.InsertRangeWithBeginEnd(interval.StartIndex, lastIndex);
                amountAdded += (lastIndex - interval.StartIndex);
            }
        }
    }
}