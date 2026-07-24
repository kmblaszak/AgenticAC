using System.Collections.Generic;

namespace CharacterTracker
{
    public class SkillsState
    {
        public Dictionary<SkillType, SkillState> Skills { get; set; }
            = new Dictionary<SkillType, SkillState>();
    }
}