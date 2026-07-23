namespace CharacterTracker
{
    public class CharacterState
    {
        public uint CharacterId { get; set; }

        public string Name { get; set; }

        public string Server { get; set; }


        public PositionState Position { get; set; }

        public VitalState Vitals { get; set; }

        public CharacterInfoState Info { get; set; }


        public long Timestamp { get; set; }
    }
}