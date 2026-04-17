using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.Serialization;

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
                set.UnionWith(new[] { "грудь", "ляжки", "попа", "яйца", "член", "вагина", "анал" });
                if (tagSet?.Contains("NudeBottom") == true)
                    set = new() { "грудь" };
                if (tagSet?.Contains("NudeTop") == true)
                    set = new() { "ляжки", "попа", "яйца", "член", "вагина", "анал" };
                if (tagSet?.Contains("CommandSuit") == true)
                    set = new() { "грудь", "ляжки", "попа", "вагина", "анал" };
                break;

            case "outerClothing":
                set.UnionWith(new[] { "грудь", "ляжки", "попа", "яйца", "член", "вагина", "анал" });
                if (tagSet?.Contains("NudeBottom") == true)
                    set = new() { "грудь" };
                if (tagSet?.Contains("NudeFull") == true)
                    set.Clear();
                if (tagSet?.Contains("FullCovered") == true)
                    set = new()
                    {
                        "щёки", "губы", "шея", "уши", "волосы", "рот", "грудь", "ступни", "ляжки", "попа",
                        "яйца", "член", "вагина", "анал", "лицо", "хвост", "ладони", "гладкие перчатки"
                    };
                if (tagSet?.Contains("FullBodyOuter") == true)
                    set = new()
                    {
                        "грудь", "ступни", "ляжки", "попа", "яйца", "член", "вагина", "анал",
                        "шея", "ладони", "гладкие перчатки"
                    };
                break;

            case "pants":
                set.UnionWith(new[] { "яйца", "член", "вагина", "анал" });
                break;

            case "locked-equipment":
                if (tagSet?.Contains("ChastityBelt") == true)
                    set.UnionWith(new[] { "яйца", "член", "вагина", "клетка" });
                break;

            case "head":
                set.UnionWith(new[] { "волосы" });
                if (tagSet?.Contains("TopCovered") == true)
                    set = new() { "уши", "волосы" };
                if (tagSet?.Contains("FullCovered") == true)
                    set = new() { "уши", "волосы", "рот", "лицо", "губы", "щёки" };
                break;

            case "gloves":
                set.UnionWith(new[] { "ладони", "гладкие перчатки" });
                if (tagSet?.Contains("SmoothGloves") == true)
                    set = new() { "ладони" };
                if (tagSet?.Contains("Ring") == true)
                    set.Clear();
                break;

            case "neck":
                set.UnionWith(new[] { "шея" });
                if (tagSet?.Contains("OpenNeck") == true)
                    set.Clear();
                break;

            case "mask":
                set.UnionWith(new[] { "рот" });
                if (tagSet?.Contains("FaceCovered") == true)
                    set = new() { "рот", "щёки", "лицо" };
                break;

            case "bra":
                set.UnionWith(new[] { "грудь" });
                break;

            case "socks":
                set.UnionWith(new[] { "ступни" });
                break;

            case "shoes":
                set.UnionWith(new[] { "носки", "ступни" });
                break;
        }

        return set;
    }
}
