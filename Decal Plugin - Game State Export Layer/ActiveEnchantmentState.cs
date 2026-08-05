namespace CharacterTracker
{
    public class ActiveEnchantmentState
    {
        public int SpellId { get; set; }

        public string SpellName { get; set; }

        public double Adjustment { get; set; }

        public int Affected { get; set; }

        public string AffectedName { get; set; }

        public int AffectedMask { get; set; }

        public double Duration { get; set; }

        public double Remaining { get; set; }

        public int Family { get; set; }

        public int Layer { get; set; }
    }
}