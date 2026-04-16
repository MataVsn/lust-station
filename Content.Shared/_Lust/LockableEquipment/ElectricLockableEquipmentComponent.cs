using System;

namespace Content.Shared._Lust.LockableEquipment;

/// <summary>
/// Adds a temporary activated icon pulse when a lockable device is triggered.
/// </summary>
[RegisterComponent]
[Access(typeof(ElectricLockableEquipmentSystem))]
public sealed partial class ElectricLockableEquipmentComponent : Component
{
    /// <summary>
    /// RSI state shown on the item itself while the device is actively shocking.
    /// This is useful when the device is visible in hands or inventory.
    /// </summary>
    [DataField]
    public string ActivatedIconState = "icon_activated";

    /// <summary>
    /// Optional RSI state shown on the wearer overlay while the device is actively shocking.
    /// If null, the equipped overlay stays on its normal sprite state.
    /// </summary>
    [DataField]
    public string? ActivatedEquippedState;

    /// <summary>
    /// How long the activated icon state stays visible after a trigger.
    /// </summary>
    [DataField]
    public TimeSpan ActivationDuration = TimeSpan.FromSeconds(0.35);

    /// <summary>
    /// Internal timer used to restore the default lockable icon state.
    /// </summary>
    [ViewVariables]
    public TimeSpan ActivatedUntil = TimeSpan.Zero;
}
