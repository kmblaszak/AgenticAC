using System.Collections.Generic;

namespace CharacterTracker
{
    public class EnchantmentsState
    {
        public List<ActiveEnchantmentState> ActiveEnchantments { get; set; }

        public EnchantmentsState()
        {
            ActiveEnchantments = new List<ActiveEnchantmentState>();
        }
    }
}