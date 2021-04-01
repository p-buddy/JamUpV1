using Unity.Entities;

[UpdateAfter(typeof(ScheduleClipsForPlaySystem))]
public class CleanScheduledClipsSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
        Entities.WithAll<ScheduleForPlayComponent>().ForEach((Entity entity, int entityInQueryIndex) =>
        {
            ecb.AddComponent<ScheduledComponent>(entityInQueryIndex, entity);
            ecb.RemoveComponent<ScheduleForPlayComponent>(entityInQueryIndex, entity);
        }).ScheduleParallel();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}