using System;

namespace CharacterTracker.Calculators
{
    public static class SkillFormulaEvaluator
    {
        public static double CalculateAttributeValue(
            int skillId,
            AttributeState attributes)
        {
            if (attributes == null)
            {
                return 0;
            }


            switch (skillId)
            {
                // Axe
                case 1:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Bow
                case 2:
                    return
                        attributes.Coordination.Buffed / 2.0;


                // Crossbow
                case 3:
                    return
                        attributes.Coordination.Buffed / 2.0;


                // Dagger
                case 4:
                    return (
                        attributes.Quickness.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Mace
                case 5:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Melee Defense
                case 6:
                    return (
                        attributes.Quickness.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Missile Defense
                case 7:
                    return (
                        attributes.Quickness.Buffed +
                        attributes.Coordination.Buffed
                    ) / 5.0;


                // Spear
                case 9:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Staff
                case 10:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Sword
                case 11:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Thrown Weapons
                case 12:
                    return
                        attributes.Coordination.Buffed / 2.0;


                // Unarmed Combat
                case 13:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Arcane Lore
                case 14:
                    return
                        attributes.Focus.Buffed / 3.0;


                // Magic Defense
                case 15:
                    return (
                        attributes.Self.Buffed +
                        attributes.Focus.Buffed
                    ) / 7.0;


                // Mana Conversion
                case 16:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Self.Buffed
                    ) / 6.0;


                // Item Tinkering
                case 18:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Coordination.Buffed
                    ) / 2.0;


                // Assess Person - Unknown
                case 19:
                    return 0;


                // Deception - Unknown
                case 20:
                    return 0;


                // Healing
                case 21:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Jump
                case 22:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 2.0;


                // Lockpick
                case 23:
                    return (
                        attributes.Coordination.Buffed +
                        attributes.Focus.Buffed
                    ) / 3.0;


                // Run
                case 24:
                    return
                        attributes.Quickness.Buffed / 1.0;


                // Assess Creature - Unknown
                case 27:
                    return 0;


                // Weapon Tinkering
                case 28:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Strength.Buffed
                    ) / 2.0;


                // Armor Tinkering
                case 29:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Endurance.Buffed
                    ) / 2.0;


                // Magic Item Tinkering
                case 30:
                    return
                        attributes.Focus.Buffed / 1.0;


                // Creature Enchantment
                case 31:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Self.Buffed
                    ) / 4.0;


                // Item Enchantment
                case 32:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Self.Buffed
                    ) / 4.0;


                // Life Magic
                case 33:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Self.Buffed
                    ) / 4.0;


                // War Magic
                case 34:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Self.Buffed
                    ) / 4.0;


                // Leadership - Unknown
                case 35:
                    return 0;


                // Loyalty - Unknown
                case 36:
                    return 0;


                // Fletching
                case 37:
                    return (
                        attributes.Coordination.Buffed +
                        attributes.Focus.Buffed
                    ) / 3.0;


                // Alchemy
                case 38:
                    return (
                        attributes.Coordination.Buffed +
                        attributes.Focus.Buffed
                    ) / 3.0;


                // Cooking
                case 39:
                    return (
                        attributes.Coordination.Buffed +
                        attributes.Focus.Buffed
                    ) / 3.0;


                // Salvaging - Unknown
                case 40:
                    return 0;


                // Two Handed Combat
                case 41:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Gearcraft
                case 42:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Coordination.Buffed
                    ) / 2.0;


                // Void Magic
                case 43:
                    return (
                        attributes.Focus.Buffed +
                        attributes.Self.Buffed
                    ) / 4.0;


                // Heavy Weapons
                case 44:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Light Weapons
                case 45:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Finesse Weapons
                case 46:
                    return (
                        attributes.Quickness.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Missile Weapons
                case 47:
                    return
                        attributes.Coordination.Buffed / 2.0;


                // Shield
                case 48:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 2.0;


                // Dual Wield
                case 49:
                    return (
                        attributes.Coordination.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Recklessness
                case 50:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Quickness.Buffed
                    ) / 3.0;


                // Sneak Attack
                case 51:
                    return (
                        attributes.Coordination.Buffed +
                        attributes.Quickness.Buffed
                    ) / 3.0;


                // Dirty Fighting
                case 52:
                    return (
                        attributes.Strength.Buffed +
                        attributes.Coordination.Buffed
                    ) / 3.0;


                // Summoning
                case 54:
                    return (
                        attributes.Endurance.Buffed +
                        attributes.Self.Buffed
                    ) / 3.0;


                default:
                    return 0;
            }
        }
    }
}