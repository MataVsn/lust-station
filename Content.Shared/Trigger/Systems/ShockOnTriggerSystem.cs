using Content.Shared.Electrocution;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

namespace Content.Shared.Trigger.Systems;

public sealed class ShockOnTriggerSystem : XOnTriggerSystem<ShockOnTriggerComponent>
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedElectrocutionSystem _electrocution = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    protected override void OnTrigger(Entity<ShockOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (ent.Comp.Cooldown > TimeSpan.Zero &&
            _timing.CurTime < ent.Comp.PreviousActivation + ent.Comp.Cooldown)
        {
            return;
        }

        ent.Comp.PreviousActivation = _timing.CurTime;
        Dirty(ent);

        // Override the normal target if we target the container
        if (ent.Comp.TargetContainer)
        {
            // shock whoever is wearing this clothing item
            if (!_container.TryGetContainingContainer(ent.Owner, out var container))
                return;

            target = container.Owner;
        }

        _electrocution.TryDoElectrocution(target, null, ent.Comp.Damage, ent.Comp.Duration, true, ignoreInsulation: true);
        args.Handled = true;
    }
}
