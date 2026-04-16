using Content.Shared.Humanoid;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;

namespace Content.Shared._Lust.LockableEquipment;

public sealed class SexEquipRestrictionSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SexEquipRestrictionComponent, BeingEquippedAttemptEvent>(OnBeingEquipped);
    }

    private void OnBeingEquipped(Entity<SexEquipRestrictionComponent> ent, ref BeingEquippedAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<HumanoidAppearanceComponent>(args.EquipTarget, out var humanoid))
            return;

        if (ent.Comp.AllowedSex.Contains(humanoid.Sex))
            return;

        args.Cancel();

        _popup.PopupClient(
            Loc.GetString("sex-equip-restriction-blocked"),
            args.EquipTarget,
            args.Equipee);
    }
}
