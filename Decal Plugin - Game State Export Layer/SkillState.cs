namespace CharacterTracker
{
    public class SkillState
    {
        public SkillType Type { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public bool Known { get; set; }

        public string Formula { get; set; }

        public TrainingState Training { get; set; }

        public SkillValue Value { get; set; } = new SkillValue();
    }
}