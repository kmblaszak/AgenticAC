using System;
using System.Collections.Generic;
using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace CharacterTracker.Extractors
{
    public class SkillExtractor
    {
        private readonly string logFile;

        public SkillExtractor(string logFile)
        {
            this.logFile = logFile;
        }


        public SkillsState Extract()
        {
            SkillsState skillsState = new SkillsState();

            var character = CoreManager.Current.CharacterFilter;


            foreach (CharFilterSkillType skillType in Enum.GetValues(typeof(CharFilterSkillType)))
            {
                try
                {
                    var skill = character.Skills[skillType];


                    if (skill == null)
                    {
                        continue;
                    }


                    SkillState skillState = new SkillState
                    {
                        Type = (SkillType)skillType,

                        Name = skill.Name,

                        ShortName = skill.ShortName,

                        Known = skill.Known,

                        Formula = skill.Formula,

                        Training = ConvertTraining(skill.Training),

                        Value = new SkillValue
                        {
                            Base = skill.Base,

                            Bonus = skill.Bonus,

                            Buffed = skill.Buffed,

                            Current = skill.Current,

                            Experience = skill.XP,

                            Increment = skill.Increment
                        }
                    };


                    skillsState.Skills[(SkillType)skillType] = skillState;
                }
                catch(Exception ex)
                {
                    System.IO.File.AppendAllText(
                        logFile,
                        $"Skill extraction failed {skillType}\r\n{ex}\r\n"
                    );
                }
            }


            return skillsState;
        }


        private TrainingState ConvertTraining(TrainingType training)
        {
            switch(training)
            {
                case TrainingType.Untrained:
                    return TrainingState.Untrained;

                case TrainingType.Trained:
                    return TrainingState.Trained;

                case TrainingType.Specialized:
                    return TrainingState.Specialized;

                default:
                    return TrainingState.Unusable;
            }
        }
    }
}