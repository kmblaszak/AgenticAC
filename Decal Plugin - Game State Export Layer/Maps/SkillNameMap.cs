using System.Collections.Generic;

namespace CharacterTracker.PacketTrackers
{
    public static class SkillNameMap
    {
        private static readonly Dictionary<int, string> Skills =
            new Dictionary<int, string>
            {
                { 1, "Axe" },
                { 2, "Bow" },
                { 3, "Crossbow" },
                { 4, "Dagger" },
                { 5, "Mace" },
                { 6, "MeleeDefense" },
                { 7, "MissileDefense" },
                { 9, "Spear" },
                { 10, "Staff" },
                { 11, "Sword" },
                { 12, "ThrownWeapons" },
                { 13, "Unarmed" },

                { 14, "ArcaneLore" },
                { 15, "MagicDefense" },
                { 16, "ManaConversion" },

                { 18, "ItemTinkering" },
                { 19, "AssessPerson" },
                { 20, "Deception" },
                { 21, "Healing" },
                { 22, "Jump" },
                { 23, "Lockpick" },
                { 24, "Run" },

                { 27, "AssessCreature" },

                { 28, "WeaponTinkering" },
                { 29, "ArmorTinkering" },
                { 30, "MagicItemTinkering" },

                { 31, "CreatureEnchantment" },
                { 32, "ItemEnchantment" },
                { 33, "LifeMagic" },
                { 34, "WarMagic" },

                { 35, "Leadership" },
                { 36, "Loyalty" },

                { 37, "Fletching" },
                { 38, "Alchemy" },
                { 39, "Cooking" },
                { 40, "Salvaging" },

                { 41, "TwoHandedCombat" },
                { 42, "Gearcraft" },

                { 43, "VoidMagic" },

                { 44, "HeavyWeapons" },
                { 45, "LightWeapons" },
                { 46, "FinesseWeapons" },
                { 47, "MissileWeapons" },
                { 48, "Shield" },
                { 49, "DualWield" },

                { 50, "Recklessness" },
                { 51, "SneakAttack" },
                { 52, "DirtyFighting" },

                { 54, "Summoning" }
            };

        public static bool TryGetSkillName(int id, out string name)
        {
            return Skills.TryGetValue(id, out name);
        }

        public static string GetName(int skillId)
        {
            if (Skills.TryGetValue(skillId, out string name))
            {
                return name;
            }

            return $"UnknownSkill_{skillId}";
        }
    }
}