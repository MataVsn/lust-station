using Content.Shared.Trigger;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

namespace Content.Shared._Lust.LockableEquipment;

/// <summary>
/// Handles temporary activated sprite pulses for electric lockable devices installed on a humanoid.
/// The cage entity itself is hidden inside a container; visual feedback is applied to the humanoid owner.
/// </summary>
public sealed class ElectricLockableEquipmentSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly LockableEquipmentSystem _lockable = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ElectricLockableEquipmentComponent, TriggerEvent>(OnTrigger);
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<ElectricLockableEquipmentComponent, LockableEquipmentComponent>();
        while (query.MoveNext(out var uid, out var electric, out var lockable))
        {
            if (electric.ActivatedUntil == TimeSpan.Zero || _timing.CurTime < electric.ActivatedUntil)
                continue;

            electric.ActivatedUntil = TimeSpan.Zero;
            _lockable.RefreshIconState((uid, lockable));

            if (TryGetHumanoid(uid, out var humanoid) &&
                TryComp(humanoid, out AppearanceComponent? humanoidAppearance))
            {
                var restoreData = new EquipmentVisualData(true, lockable.Layer, lockable.RsiPath, lockable.SpriteState);
                _appearance.SetData(humanoid, EquipmentVisuals.VisualData, restoreData, humanoidAppearance);
            }
        }
    }

    private void OnTrigger(Entity<ElectricLockableEquipmentComponent> ent, ref TriggerEvent args)
    {
        if (!TryComp(ent.Owner, out LockableEquipmentComponent? lockable))
            return;

        ent.Comp.ActivatedUntil = _timing.CurTime + ent.Comp.ActivationDuration;

        if (TryComp(ent.Owner, out AppearanceComponent? itemAppearance))
            _appearance.SetData(ent.Owner, EquipmentVisuals.IconState, ent.Comp.ActivatedIconState, itemAppearance);

        if (!TryGetHumanoid(ent.Owner, out var humanoid))
            return;

        if (!TryComp(humanoid, out AppearanceComponent? appearance))
            return;

        var equippedState = ent.Comp.ActivatedEquippedState ?? lockable.SpriteState;
        var activatedData = new EquipmentVisualData(true, lockable.Layer, lockable.RsiPath, equippedState);
        _appearance.SetData(humanoid, EquipmentVisuals.VisualData, activatedData, appearance);
    }

    private bool TryGetHumanoid(EntityUid device, out EntityUid humanoid)
    {
        humanoid = EntityUid.Invalid;

        if (!_container.TryGetContainingContainer(device, out var container))
            return false;

        if (!TryComp(container.Owner, out EquipmentContainerComponent? equipComp) ||
            container.ID != equipComp.ContainerId)
            return false;

        humanoid = container.Owner;
        return true;
    }
}
