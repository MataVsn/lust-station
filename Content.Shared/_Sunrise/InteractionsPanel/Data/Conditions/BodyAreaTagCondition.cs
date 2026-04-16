using Robust.Shared.Containers;
using Robust.Shared.Serialization;
using Content.Shared.Tag;

namespace Content.Shared._Sunrise.InteractionsPanel.Data.Conditions;

[Serializable, NetSerializable, DataDefinition]
public sealed partial class BodyAreaTagCondition : IAppearCondition
{
    [DataField]
    public bool CheckInitiator { get; private set; }

    [DataField]
    public bool CheckTarget { get; private set; } = true;

    [DataField]
    public bool RequireExposed { get; private set; } = true;

    [DataField(required: true)]
    public HashSet<string> Categories { get; private set; } = new();

    public bool IsMet(EntityUid initiator, EntityUid target, EntityManager entityManager)
    {
        if (CheckInitiator && !CheckEntity(initiator, entityManager))
            return false;

        if (CheckTarget && !CheckEntity(target, entityManager))
            return false;

        return true;
    }

    private bool CheckEntity(EntityUid entity, EntityManager entMan)
    {
        if (!entMan.TryGetComponent<ContainerManagerComponent>(entity, out var inventory))
            return RequireExposed;

        var restricted = GetCoveredCategories(entMan, inventory);
        foreach (var category in Categories)
        {
            var isCovered = restricted.Contains(category);

            if (RequireExposed && isCovered)
                return false;

            if (!RequireExposed && !isCovered)
                return false;
        }

        return true;
    }

    private HashSet<string> GetCoveredCategories(EntityManager entMan, ContainerManagerComponent inventory)
    {
        var result = new HashSet<string>();

        foreach (var (slot, container) in inventory.Containers)
        {
            if (container.ContainedEntities.Count == 0)
                continue;

            var ent = container.ContainedEntities[0];
            entMan.TryGetComponent<TagComponent>(ent, out var tags);
            result.UnionWith(GetCategoriesBySlotAndTags(slot, tags));
        }

        return result;
    }

    private HashSet<string> GetCategoriesBySlotAndTags(string slot, TagComponent? tags)
    {
        var set = new HashSet<string>();
        var tagSet = tags?.Tags;

        switch (slot)
        {
            case "jumpsuit":
                set.UnionWith(new[] { "РіСЂСѓРґСЊ", "Р»СЏР¶РєРё", "РїРѕРїР°", "СЏР№С†Р°", "С‡Р»РµРЅ", "РІР°РіРёРЅР°", "Р°РЅР°Р»" });
                if (tagSet?.Contains("NudeBottom") == true)
                    set = new() { "РіСЂСѓРґСЊ" };
                if (tagSet?.Contains("NudeTop") == true)
                    set = new() { "Р»СЏР¶РєРё", "РїРѕРїР°", "СЏР№С†Р°", "С‡Р»РµРЅ", "РІР°РіРёРЅР°", "Р°РЅР°Р»" };
                if (tagSet?.Contains("CommandSuit") == true)
                    set = new() { "РіСЂСѓРґСЊ", "Р»СЏР¶РєРё", "РїРѕРїР°", "РІР°РіРёРЅР°", "Р°РЅР°Р»" };
                break;

            case "outerClothing":
                set.UnionWith(new[] { "РіСЂСѓРґСЊ", "Р»СЏР¶РєРё", "РїРѕРїР°", "СЏР№С†Р°", "С‡Р»РµРЅ", "РІР°РіРёРЅР°", "Р°РЅР°Р»" });
                if (tagSet?.Contains("NudeBottom") == true)
                    set = new() { "РіСЂСѓРґСЊ" };
                if (tagSet?.Contains("NudeFull") == true)
                    set.Clear();
                if (tagSet?.Contains("FullCovered") == true)
                    set = new() {
                        "С‰С‘РєРё", "РіСѓР±С‹", "С€РµСЏ", "СѓС€Рё", "РІРѕР»РѕСЃС‹", "СЂРѕС‚", "РіСЂСѓРґСЊ", "СЃС‚СѓРїРЅРё", "Р»СЏР¶РєРё", "РїРѕРїР°",
                        "СЏР№С†Р°", "С‡Р»РµРЅ", "РІР°РіРёРЅР°", "Р°РЅР°Р»", "Р»РёС†Рѕ", "С…РІРѕСЃС‚", "Р»Р°РґРѕРЅРё", "РіР»Р°РґРєРёРµ РїРµСЂС‡Р°С‚РєРё"
                    };
                if (tagSet?.Contains("FullBodyOuter") == true)
                    set = new() {
                        "РіСЂСѓРґСЊ", "СЃС‚СѓРїРЅРё", "Р»СЏР¶РєРё", "РїРѕРїР°", "СЏР№С†Р°", "С‡Р»РµРЅ", "РІР°РіРёРЅР°", "Р°РЅР°Р»", "С€РµСЏ", "Р»Р°РґРѕРЅРё", "РіР»Р°РґРєРёРµ РїРµСЂС‡Р°С‚РєРё"
                    };
                break;

            case "pants":
                set.UnionWith(new[] { "СЏР№С†Р°", "С‡Р»РµРЅ", "РІР°РіРёРЅР°", "Р°РЅР°Р»" });
                break;

            case "locked-equipment":
                if (tagSet?.Contains("ChastityBelt") == true)
                    set.UnionWith(new[] { "СЏР№С†Р°", "С‡Р»РµРЅ", "РІР°РіРёРЅР°", "Р°РЅР°Р»", "РєР»РµС‚РєР°" });
                break;

            case "head":
                set.UnionWith(new[] { "РІРѕР»РѕСЃС‹" });
                if (tagSet?.Contains("TopCovered") == true)
                    set = new() { "СѓС€Рё", "РІРѕР»РѕСЃС‹" };
                if (tagSet?.Contains("FullCovered") == true)
                    set = new() { "СѓС€Рё", "РІРѕР»РѕСЃС‹", "СЂРѕС‚", "Р»РёС†Рѕ", "РіСѓР±С‹", "С‰С‘РєРё" };
                break;

            case "gloves":
                set.UnionWith(new[] { "Р»Р°РґРѕРЅРё", "РіР»Р°РґРєРёРµ РїРµСЂС‡Р°С‚РєРё" });
                if (tagSet?.Contains("SmoothGloves") == true)
                    set = new() { "Р»Р°РґРѕРЅРё" };
                if (tagSet?.Contains("Ring") == true)
                    set.Clear();
                break;

            case "neck":
                set.UnionWith(new[] { "С€РµСЏ" });
                if (tagSet?.Contains("OpenNeck") == true)
                    set.Clear();
                break;

            case "mask":
                set.UnionWith(new[] { "СЂРѕС‚" });
                if (tagSet?.Contains("FaceCovered") == true)
                    set = new() { "СЂРѕС‚", "С‰С‘РєРё", "Р»РёС†Рѕ" };
                break;

            case "bra":
                set.UnionWith(new[] { "РіСЂСѓРґСЊ" });
                break;

            case "socks":
                set.UnionWith(new[] { "СЃС‚СѓРїРЅРё" });
                break;

            case "shoes":
                set.UnionWith(new[] { "РЅРѕСЃРєРё", "СЃС‚СѓРїРЅРё" });
                break;
        }

        return set;
    }
}
