namespace CharacterTracker
{
    public class CharacterState
    {
        public uint CharacterId { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public string Server { get; set; }

        public long Timestamp { get; set; }
    }
}