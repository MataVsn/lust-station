using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.Serialization;

namespace Content.Shared._Sunrise.InteractionsPanel.Data.Conditions;

[Serializable, NetSerializable, DataDefinition]
public sealed partial class BodyAreaTagCondition : IAppearCondition
{
    private static readonly string[] CoveredTorso =
    {
        "грудь",
        "ляжки",
        "попа",
        "шея",
        "член",
        "вагина",
        "анал"
    };

    private static readonly string[] CoveredLowerBody =
    {
        "член",
        "вагина",
        "анал"
    };

    private static readonly string[] CoveredHead =
    {
        "волосы"
    };

    private static readonly string[] CoveredHeadTop =
    {
        "уши",
        "волосы"
    };

    private static readonly string[] CoveredFullFace =
    {
        "уши",
        "волосы",
        "рот",
        "лицо",
        "губы",
        "щёки"
    };

    private static readonly string[] CoveredGloves =
    {
        "ладони",
        "гладкие перчатки"
    };

    private static readonly string[] CoveredNeck =
    {
        "шея"
    };

    private static readonly string[] CoveredMask =
    {
        "рот"
    };

    private static readonly string[] CoveredFace =
    {
        "рот",
        "щёки",
        "лицо"
    };

    private static readonly string[] CoveredBra =
    {
        "грудь"
    };

    private static readonly string[] CoveredSocks =
    {
        "ступни"
    };

    private static readonly string[] CoveredShoes =
    {
        "носки",
        "ступни"
    };

    private static readonly string[] CoveredFullBody =
    {
        "щёки",
        "губы",
        "шея",
        "уши",
        "волосы",
        "рот",
        "грудь",
        "ступни",
        "ляжки",
        "попа",
        "член",
        "вагина",
        "анал",
        "лицо",
        "хвост",
        "ладони",
        "гладкие перчатки"
    };

    private static readonly string[] CoveredOuterFullBody =
    {
        "грудь",
        "ступни",
        "ляжки",
        "попа",
        "член",
        "вагина",
        "анал",
        "шея",
        "ладони",
        "гладкие перчатки"
    };

    private static readonly string[] CoveredChastity =
    {
        "член",
        "вагина",
        "анал",
        "клетка"
    };

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

            foreach (var ent in container.ContainedEntities)
            {
                entMan.TryGetComponent<TagComponent>(ent, out var tags);
                result.UnionWith(GetCategoriesBySlotAndTags(slot, tags));
            }
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
                set.UnionWith(CoveredTorso);

                if (tagSet?.Contains("NudeBottom") == true)
                    return new HashSet<string> { "грудь" };

                if (tagSet?.Contains("NudeTop") == true)
                    return new HashSet<string> { "ляжки", "попа", "шея", "член", "вагина", "анал" };

                if (tagSet?.Contains("CommandSuit") == true)
                    return new HashSet<string> { "грудь", "ляжки", "попа", "член", "вагина", "анал" };

                break;

            case "outerClothing":
                set.UnionWith(CoveredTorso);

                if (tagSet?.Contains("NudeBottom") == true)
                    return new HashSet<string> { "грудь" };

                if (tagSet?.Contains("NudeFull") == true)
                    return set.ClearAndReturn();

                if (tagSet?.Contains("FullCovered") == true)
                    return new HashSet<string>(CoveredFullBody);

                if (tagSet?.Contains("FullBodyOuter") == true)
                    return new HashSet<string>(CoveredOuterFullBody);

                break;

            case "pants":
                set.UnionWith(CoveredLowerBody);
                break;

            case "locked-equipment":
                if (tagSet?.Contains("ChastityBelt") == true)
                    set.UnionWith(CoveredChastity);
                break;

            case "head":
                set.UnionWith(CoveredHead);

                if (tagSet?.Contains("TopCovered") == true)
                    return new HashSet<string>(CoveredHeadTop);

                if (tagSet?.Contains("FullCovered") == true)
                    return new HashSet<string>(CoveredFullFace);

                break;

            case "gloves":
                set.UnionWith(CoveredGloves);

                if (tagSet?.Contains("SmoothGloves") == true)
                    return new HashSet<string> { "ладони" };

                if (tagSet?.Contains("Ring") == true)
                    return set.ClearAndReturn();

                break;

            case "neck":
                set.UnionWith(CoveredNeck);

                if (tagSet?.Contains("OpenNeck") == true)
                    return set.ClearAndReturn();

                break;

            case "mask":
                set.UnionWith(CoveredMask);

                if (tagSet?.Contains("FaceCovered") == true)
                    return new HashSet<string>(CoveredFace);

                break;

            case "bra":
                set.UnionWith(CoveredBra);
                break;

            case "socks":
                set.UnionWith(CoveredSocks);
                break;

            case "shoes":
                set.UnionWith(CoveredShoes);
                break;
        }

        return set;
    }
}

internal static class BodyAreaTagConditionExtensions
{
    public static HashSet<string> ClearAndReturn(this HashSet<string> set)
    {
        set.Clear();
        return set;
    }
}
