using System.Collections.Generic;

namespace CharacterTracker.Maps
{
    public static class SkillFormulaMap
    {
        private static readonly Dictionary<int, string> formulas =
            new Dictionary<int, string>
            {
                { 1,  "( Strength + Coordination ) / 3" },   // Axe
                { 2,  "( Coordination ) / 2" },              // Bow
                { 3,  "( Coordination ) / 2" },              // Crossbow
                { 4,  "( Quickness + Coordination ) / 3" },  // Dagger
                { 5,  "( Strength + Coordination ) / 3" },   // Mace
                { 9,  "( Strength + Coordination ) / 3" },   // Spear
                { 10, "( Strength + Coordination ) / 3" },   // Staff
                { 11, "( Strength + Coordination ) / 3" },   // Sword
                { 12, "( Coordination ) / 2" },              // Thrown Weapons
                { 13, "( Strength + Coordination ) / 3" },   // Unarmed
                { 42, "( Focus + Coordination ) / 2" },      // Gearcraft
                { 6,  "( Quickness + Coordination ) / 3" },
                { 7,  "( Quickness + Coordination ) / 5" },
                { 14, "( Focus ) / 3" },
                { 15, "( Self + Focus ) / 7" },
                { 16, "( Focus + Self ) / 6" },
                { 18, "( Focus + Coordination ) / 2" },
                { 19, "Unknown" },
                { 20, "Unknown" },
                { 21, "( Focus + Coordination ) / 3" },
                { 22, "( Strength + Coordination ) / 2" },
                { 23, "( Coordination + Focus ) / 3" },
                { 24, "( Quickness ) / 1" },
                { 27, "Unknown" },
                { 28, "( Focus + Strength ) / 2" },
                { 29, "( Focus + Endurance ) / 2" },
                { 30, "( Focus ) / 1" },
                { 31, "( Focus + Self ) / 4" },
                { 32, "( Focus + Self ) / 4" },
                { 33, "( Focus + Self ) / 4" },
                { 34, "( Focus + Self ) / 4" },
                { 35, "Unknown" },
                { 36, "Unknown" },
                { 37, "( Coordination + Focus ) / 3" },
                { 38, "( Coordination + Focus ) / 3" },
                { 39, "( Coordination + Focus ) / 3" },
                { 40, "Unknown" },
                { 41, "( Strength + Coordination ) / 3" },
                { 43, "( Focus + Self ) / 4" },
                { 44, "( Strength + Coordination ) / 3" },
                { 45, "( Strength + Coordination ) / 3" },
                { 46, "( Quickness + Coordination ) / 3" },
                { 47, "( Coordination ) / 2" },
                { 48, "( Strength + Coordination ) / 2" },
                { 49, "( Coordination + Coordination ) / 3" },
                { 50, "( Strength + Quickness ) / 3" },
                { 51, "( Coordination + Quickness ) / 3" },
                { 52, "( Strength + Coordination ) / 3" },
                { 54, "( Endurance + Self ) / 3" }
            };


        public static string GetFormula(int skillId)
        {
            if (formulas.ContainsKey(skillId))
            {
                return formulas[skillId];
            }

            return "Unknown";
        }
    }
}