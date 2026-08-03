using System.Collections.Generic;

namespace CharacterTracker.Maps
{
    public static class SkillStateMap
    {
        private static readonly Dictionary<int, string> states =
            new Dictionary<int, string>
            {
                { 0, "Unknown" },
                { 1, "Untrained" },
                { 2, "Trained" },
                { 3, "Specialized" }
            };


        public static string GetName(int state)
        {
            if (states.ContainsKey(state))
            {
                return states[state];
            }

            return "Unknown";
        }
    }
}