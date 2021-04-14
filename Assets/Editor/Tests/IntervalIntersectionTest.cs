using System;
using System.Collections.Generic;
using ECS.Systems.Jobs.DTO;
using NUnit.Framework;

namespace Tests
{
    public class IntervalIntersectionTest
    {
        public struct TestCase
        {
            public TimeInterval Query;
            public TimeInterval Base;
            public List<Func<IntervalIntersection, bool>> Criteria;

            public override string ToString()
            {
                return $"Base: [{Base.StartTime}, {Base.EndTime}]; Query: [{Query.StartTime}, {Query.EndTime}]";
            }
        }
        
        private const double TimeResolution = 0.1;

        private static List<TestCase> cases = new List<TestCase>()
        {
            new TestCase()
            {
                Base = new TimeInterval(0.9, 4.7),
                Query = new TimeInterval(0.0, 1.8),
                Criteria = new List<Func<IntervalIntersection, bool>>()
                {
                    (intersection => intersection.Overlap.Exists),
                    (intersection => intersection.Overlap.Origin == IntervalOrigin.IntersectingIntervals),
                    (intersection => intersection.Overlap.Interval.StartTime == 0.9),
                    (intersection => intersection.Overlap.Interval.EndTime == 1.8),
                    (intersection => intersection.Left.Exists),
                    (intersection => intersection.Left.Origin == IntervalOrigin.QueryInterval),
                    (intersection => intersection.Left.Interval.StartTime == 0.0),
                    (intersection => intersection.Left.Interval.EndTime == 0.9),
                    (intersection => intersection.Right.Exists),
                    (intersection => intersection.Right.Origin == IntervalOrigin.BaseInterval),
                    (intersection => intersection.Right.Interval.StartTime == 1.8),
                    (intersection => intersection.Right.Interval.EndTime == 4.7),
                }
            },
            new TestCase()
            {
                Base = new TimeInterval(0.0, 4.7), 
                Query = new TimeInterval(0.0, 1.8),
                Criteria = new List<Func<IntervalIntersection, bool>>()
                {
                    (intersection => intersection.Overlap.Exists),
                    (intersection => intersection.Overlap.Origin == IntervalOrigin.IntersectingIntervals),
                    (intersection => intersection.Overlap.Interval.StartTime == 0.0),
                    (intersection => intersection.Overlap.Interval.EndTime == 1.8),
                    (intersection => !intersection.Left.Exists),
                    (intersection => intersection.Right.Exists),
                    (intersection => intersection.Right.Origin == IntervalOrigin.BaseInterval),
                    (intersection => intersection.Right.Interval.StartTime == 1.8),
                    (intersection => intersection.Right.Interval.EndTime == 4.7),
                }
            },
            new TestCase()
            {
                Base = new TimeInterval(0.0, 1.5), 
                Query = new TimeInterval(0.0, 1.8),
                Criteria = new List<Func<IntervalIntersection, bool>>()
                {
                    (intersection => intersection.Overlap.Exists),
                    (intersection => intersection.Overlap.Origin == IntervalOrigin.IntersectingIntervals),
                    (intersection => intersection.Overlap.Interval.StartTime == 0.0),
                    (intersection => intersection.Overlap.Interval.EndTime == 1.5),
                    (intersection => !intersection.Left.Exists),
                    (intersection => intersection.Right.Exists),
                    (intersection => intersection.Right.Origin == IntervalOrigin.QueryInterval),
                    (intersection => intersection.Right.Interval.StartTime == 1.5),
                    (intersection => intersection.Right.Interval.EndTime == 1.8),
                }
            },
            new TestCase()
            {
                Base = new TimeInterval(0.9, 2.0), 
                Query = new TimeInterval(3.0, 5.0),
                Criteria = new List<Func<IntervalIntersection, bool>>()
                {
                    (intersection => !intersection.Overlap.Exists),
                    (intersection => intersection.Left.Exists),
                    (intersection => intersection.Left.Origin == IntervalOrigin.BaseInterval),
                    (intersection => intersection.Left.Interval.StartTime == 0.9),
                    (intersection => intersection.Left.Interval.EndTime == 2.0),
                    (intersection => intersection.Right.Exists),
                    (intersection => intersection.Right.Origin == IntervalOrigin.QueryInterval),
                    (intersection => intersection.Right.Interval.StartTime == 3.0),
                    (intersection => intersection.Right.Interval.EndTime == 5.0),
                }
            },
            new TestCase()
            {
                Base = new TimeInterval(4.0, 6.0), 
                Query = new TimeInterval(1.0, 2.0),
                Criteria = new List<Func<IntervalIntersection, bool>>()
                {
                    (intersection => !intersection.Overlap.Exists),
                    (intersection => intersection.Left.Exists),
                    (intersection => intersection.Left.Origin == IntervalOrigin.QueryInterval),
                    (intersection => intersection.Left.Interval.StartTime == 1.0),
                    (intersection => intersection.Left.Interval.EndTime == 2.0),
                    (intersection => intersection.Right.Exists),
                    (intersection => intersection.Right.Origin == IntervalOrigin.BaseInterval),
                    (intersection => intersection.Right.Interval.StartTime == 4.0),
                    (intersection => intersection.Right.Interval.EndTime == 6.0),
                }
            },
            new TestCase()
            {
                Base = new TimeInterval(1.0, 6.0), 
                Query = new TimeInterval(2.0, 3.0),
                Criteria = new List<Func<IntervalIntersection, bool>>()
                {
                    (intersection => intersection.Overlap.Exists),
                    (intersection => intersection.Overlap.Origin == IntervalOrigin.IntersectingIntervals),
                    (intersection => intersection.Overlap.Interval.StartTime == 2.0),
                    (intersection => intersection.Overlap.Interval.EndTime == 3.0),
                    (intersection => intersection.Left.Exists),
                    (intersection => intersection.Left.Origin == IntervalOrigin.BaseInterval),
                    (intersection => intersection.Left.Interval.StartTime == 1.0),
                    (intersection => intersection.Left.Interval.EndTime == 2.0),
                    (intersection => intersection.Right.Exists),
                    (intersection => intersection.Right.Origin == IntervalOrigin.BaseInterval),
                    (intersection => intersection.Right.Interval.StartTime == 3.0),
                    (intersection => intersection.Right.Interval.EndTime == 6.0),
                }
            },
            new TestCase()
            {
                Base = new TimeInterval(3.0, 4.0), 
                Query = new TimeInterval(1.0, 5.0),
                Criteria = new List<Func<IntervalIntersection, bool>>()
                {
                    (intersection => intersection.Overlap.Exists),
                    (intersection => intersection.Overlap.Origin == IntervalOrigin.IntersectingIntervals),
                    (intersection => intersection.Overlap.Interval.StartTime == 3.0),
                    (intersection => intersection.Overlap.Interval.EndTime == 4.0),
                    (intersection => intersection.Left.Exists),
                    (intersection => intersection.Left.Origin == IntervalOrigin.QueryInterval),
                    (intersection => intersection.Left.Interval.StartTime == 1.0),
                    (intersection => intersection.Left.Interval.EndTime == 3.0),
                    (intersection => intersection.Right.Exists),
                    (intersection => intersection.Right.Origin == IntervalOrigin.QueryInterval),
                    (intersection => intersection.Right.Interval.StartTime == 4.0),
                    (intersection => intersection.Right.Interval.EndTime == 5.0),
                }
            },
        };

        [Test]
        public void Test([ValueSource(nameof(cases))] TestCase testCase)
        {
            IntervalIntersection intersection = new IntervalIntersection(in testCase.Base, in testCase.Query, 0.1);
            for (var index = 0; index < testCase.Criteria.Count; index++)
            {
                var criterion = testCase.Criteria[index];
                Assert.IsTrue(criterion.Invoke(intersection), $"Criterion {index} failed.");
            }
        }
    }
}
