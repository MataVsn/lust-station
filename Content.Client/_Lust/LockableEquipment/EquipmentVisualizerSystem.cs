using Content.Shared._Lust.LockableEquipment;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._Lust.LockableEquipment;

public sealed class EquipmentVisualizerSystem : VisualizerSystem<EquipmentContainerComponent>
{
    private static readonly string[] LockableLayers =
    {
        "lockable_under",
        "lockable_normal",
        "lockable_over",
        "lockable_chest",
        "lockable_underpants",
    };

    protected override void OnAppearanceChange(EntityUid uid, EquipmentContainerComponent comp, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var sprite = args.Sprite;

        if (!AppearanceSystem.TryGetData<EquipmentVisualData>(uid, EquipmentVisuals.VisualData, out var visualData, args.Component) ||
            visualData == null ||
            string.IsNullOrEmpty(visualData.Layer))
        {
            // No data - hide all known lockable layers to avoid stale visuals
            // from a prior device installation.
            foreach (var key in LockableLayers)
            {
                if (!SpriteSystem.LayerMapTryGet((uid, sprite), key, out var layer, false))
                    continue;

                SpriteSystem.LayerSetVisible((uid, sprite), layer, false);
            }

            return;
        }

        var layerIdx = SpriteSystem.LayerMapReserve((uid, sprite), visualData.Layer);

        if (!visualData.Visible ||
            string.IsNullOrEmpty(visualData.RsiPath) ||
            string.IsNullOrEmpty(visualData.State))
        {
            SpriteSystem.LayerSetVisible((uid, sprite), layerIdx, false);
            return;
        }

        SpriteSystem.LayerSetRsi((uid, sprite), layerIdx, new ResPath(visualData.RsiPath));
        SpriteSystem.LayerSetRsiState((uid, sprite), layerIdx, visualData.State);
        SpriteSystem.LayerSetVisible((uid, sprite), layerIdx, true);
    }
}
