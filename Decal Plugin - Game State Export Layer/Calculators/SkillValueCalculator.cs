using System;

namespace CharacterTracker.Calculators
{
    public static class SkillValueCalculator
    {
        /// <summary>
        /// Calculates the displayed base skill value.
        ///
        /// Formula:
        ///
        /// Base = Increment + Attribute Contribution + Bonus
        ///
        /// Example:
        ///
        /// Melee Defense:
        ///
        /// Increment = 105
        /// Attributes = (160 Quickness + 160 Coordination) / 3
        ///             = 106.666
        /// Bonus = 10
        ///
        /// Total:
        /// 105 + 106.666 + 10 = 221.666
        ///
        /// Rounded:
        /// 222
        ///
        /// </summary>
        public static int CalculateBaseValue(
            int skillId,
            int increment,
            int bonus,
            AttributeState attributes)
        {
            double attributeValue =
                SkillFormulaEvaluator.CalculateAttributeValue(
                    skillId,
                    attributes);


            double total =
                increment +
                attributeValue +
                bonus;


            return (int)Math.Round(
                total,
                MidpointRounding.AwayFromZero);
        }



        /// <summary>
        /// Calculates the buffed value.
        ///
        /// Currently:
        ///
        /// Buffed = Base
        ///
        /// Future:
        /// This is where item buffs,
        /// spell buffs, and augmentation effects
        /// can be added.
        /// </summary>
        public static int CalculateBuffedValue(
            int baseValue,
            int buffModifier = 0)
        {
            return baseValue + buffModifier;
        }



        /// <summary>
        /// Calculates current skill value.
        ///
        /// Placeholder for future mechanics:
        /// - temporary debuffs
        /// - skill drains
        /// - other AC mechanics
        ///
        /// Current behavior:
        /// Current = Buffed
        /// </summary>
        public static int CalculateCurrentValue(
            int buffedValue,
            int currentModifier = 0)
        {
            return buffedValue + currentModifier;
        }
    }
}