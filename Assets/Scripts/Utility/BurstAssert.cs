using System;

namespace Utility
{
    public static class BurstAssert
    {
        public static void IsTrue(bool truth)
        {
            #if UNITY_ASSERTIONS
            if (!truth)
            {
                throw new Exception( "Truth Assertion failed");
            }
            #endif
        }
        
        public static void IsNotTrue(bool notTruth)
        {
            #if UNITY_ASSERTIONS
            if (notTruth)
            {
                throw new Exception( "Not truth Assertion failed");
            }
            #endif
        }
    }
}