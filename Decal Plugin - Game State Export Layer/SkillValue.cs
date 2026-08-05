namespace CharacterTracker
{
    public class SkillValue
    {
        public int Base { get; set; }

        public int Bonus { get; set; }

        public int Buffed { get; set; }

        public int EnchantmentBonus { get; set; }

        public int Current { get; set; }

        public int Experience { get; set; }

        public int Increment { get; set; }

        public int Diff { get; set; }

        public long ExperienceToNextSkillPoint { get; set; }

        public double PercentToNextSkillPoint { get; set; }
    }
}