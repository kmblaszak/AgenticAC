namespace CharacterTracker
{
    public class CharacterState
    {
        public uint CharacterId { get; set; }

        public string Name { get; set; }

        public string Server { get; set; }

        public PositionState Position { get; set; }

        public VitalState Vitals { get; set; }

        public CharacterInfoState CharacterInfo { get; set; }

        public AttributeState Attributes { get; set; }

        public SkillsState Skills { get; set; } = new SkillsState();

        public string Timestamp { get; set; }
    }
}