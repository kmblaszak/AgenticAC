using Decal.Adapter;
using Decal.Adapter.Wrappers;
using CharacterTracker.GameData;
using CharacterTracker.Maps;

namespace CharacterTracker.Extractors
{
    public class EnchantmentExtractor
    {
        public EnchantmentsState GetEnchantments()
        {
            EnchantmentsState state = new EnchantmentsState();

            foreach (EnchantmentWrapper enchant in CoreManager.Current.CharacterFilter.Enchantments)
            {
                string spellName = "Unknown";

                SpellNameMap.TryGetSpellName(
                    enchant.SpellId,
                    out spellName);


                string affectedName = "Unknown";

                if (SkillNameMap.TryGetSkillName(
                    enchant.Affected,
                    out string skillName))
                {
                    affectedName = skillName;
                }


                state.ActiveEnchantments.Add(
                    new ActiveEnchantmentState
                    {
                        SpellId = enchant.SpellId,
                        SpellName = spellName,
                        Adjustment = enchant.Adjustment,
                        Affected = enchant.Affected,
                        AffectedName = affectedName,
                        AffectedMask = enchant.AffectedMask,
                        Duration = enchant.Duration,
                        Remaining = enchant.TimeRemaining,
                        Family = enchant.Family,
                        Layer = enchant.Layer
                    });
            }


            return state;
        }
    }
}