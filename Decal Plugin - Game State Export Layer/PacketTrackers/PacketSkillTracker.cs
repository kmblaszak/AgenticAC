using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CharacterTracker.PacketTrackers
{
    /// <summary>
    /// Maintains the latest skill information received from network packets.
    /// This class is only a data store for now.
    /// </summary>
    public class PacketSkillTracker
    {
        private readonly Dictionary<int, PacketSkill> skills =
            new Dictionary<int, PacketSkill>();

        public IReadOnlyDictionary<int, PacketSkill> Skills => skills;

        public void UpdateSkill(PacketSkill skill)
        {
            skills[skill.SkillId] = skill;
        }

        public bool TryGetSkill(int skillId, out PacketSkill skill)
        {
            return skills.TryGetValue(skillId, out skill);
        }

        public void DumpSkills(string logFile)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine();
            output.AppendLine("============================");
            output.AppendLine("PACKET SKILL TRACKER DUMP");
            output.AppendLine("============================");


            foreach (var skill in skills.Values)
            {
                output.AppendLine(
                    $"SkillId={skill.SkillId} " +
                    $"Raised={skill.Raised} " +
                    $"XP={skill.XP} " +
                    $"Bonus={skill.Bonus} " +
                    $"State={skill.State}");
            }


            File.AppendAllText(
                logFile,
                output.ToString());
        }        

        public void Clear()
        {
            skills.Clear();
        }
    }

    /// <summary>
    /// Represents one skill exactly as it appears in the incoming packet.
    /// These names intentionally match the packet until we fully understand them.
    /// </summary>
    public class PacketSkill
    {
        /// <summary>
        /// Packet key (matches the skill enum value).
        /// Example: 6 = Melee Defense
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// Packet field: raised
        /// Known to match Decal Increment.
        /// </summary>
        public int Raised { get; set; }

        /// <summary>
        /// Packet field: state
        /// 0 = Unusable
        /// 1 = Untrained
        /// 2 = Trained
        /// 3 = Specialized
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Packet field: xp
        /// Known to match Decal XP.
        /// </summary>
        public int XP { get; set; }

        /// <summary>
        /// Packet field: bonus
        /// Known to match Decal Bonus.
        /// </summary>
        public int Bonus { get; set; }

        /// <summary>
        /// Packet field: diff
        /// Purpose still unknown.
        /// </summary>
        public int Diff { get; set; }

        /// <summary>
        /// Packet field: unknown2
        /// Purpose still unknown.
        /// </summary>
        public double Unknown2 { get; set; }
    }
}