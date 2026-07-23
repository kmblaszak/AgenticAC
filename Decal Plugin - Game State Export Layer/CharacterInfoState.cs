namespace CharacterTracker
{
    public class CharacterInfoState
    {
        public int Level { get; set; }

        public long TotalXP { get; set; }

        public long XPToNextLevel { get; set; }

        public long UnassignedXP { get; set; }

        public int Vitae { get; set; }

        public int Deaths { get; set; }

        public int Burden { get; set; }

        public int BurdenUnits { get; set; }
    }
}