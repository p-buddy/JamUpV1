using AudioPlayBack;
using MonoBehaviours;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(StageClipsForPlaySystem))]
public class ScheduleClipsForPlaySystem : SystemBase
{
    protected override void OnUpdate()
    {
        GameManager.Instance.TryFetch(out IPlayBack playBack);
        if (playBack == null)
        {
            return;
        }
        
        Entities.WithoutBurst()
            .WithAll<ScheduleForPlayComponent>()
            .ForEach((in ClipAliasComponent clip,
                in PlayEventComponent eventDetails,
                in TrackAliasComponent trackAlias) =>
            {
                playBack.ScheduleClip(clip, eventDetails, trackAlias);
            }).Run();
    }
}