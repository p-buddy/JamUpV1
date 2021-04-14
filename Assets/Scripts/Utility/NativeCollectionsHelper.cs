using System;
using System.Collections.Generic;
using ECS.Systems.Jobs.DTO;
using Unity.Collections;
using UnityEngine;

namespace Utility
{
    public static class NativeCollectionsHelper
    {
        public static void Fill<T>(this NativeList<T> toFill, List<T> from) where T : unmanaged
        {
            foreach (var item in from)
            {
                toFill.Add(item);
            }
        }
        
        public static void Fill<T, T2>(this NativeList<T> toFill, List<T2> from, Func<T2, T> convert) where T : unmanaged
        {
            foreach (var item in from)
            {
                toFill.Add(convert.Invoke(item));
            }
        }

        public static void Log<T>(this NativeList<T> toLog) where T : unmanaged
        {
            foreach (var item in toLog)
            {
                Debug.Log(item);
            }
        }
        
        public static void ExpandAtIndex<T>(this NativeList<T> toExpand, int index) where T : unmanaged
        {
            if (index == toExpand.Length)
            {
                toExpand.Add(default);
                return;
            }
            toExpand.InsertRangeWithBeginEnd(index, index + 1);
        }

        
        // move this
        public static double Inverse(this int value)
        {
            return 1 / (double) value;
        }

        public static int ToEndIndex(this TimeInterval interval, int startIndex, int frequency)
        {
            return new IndexRange(interval.Length, startIndex, frequency.Inverse()).EndIndex;
        }
        
        public static int ToIndexCount(this TimeInterval interval, int frequency)
        {
            return new IndexRange(interval.Length, -1, frequency.Inverse()).Count;
        }
        
    }
}