using ECS.Components;
using ECS.Systems.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems
{
    public class CreateMergedClipsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Dependency = OnUpdate(Dependency);
        }

        private JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityQuery query = GetEntityQuery(typeof(ClipMicroSample));
            NativeArray<ClipMicroSample> allSamples = query.ToComponentDataArray<ClipMicroSample>(Allocator.TempJob);
            
            JobHandle handle = allSamples.SortJob(inputDeps);


            return handle;
        }
    }
}