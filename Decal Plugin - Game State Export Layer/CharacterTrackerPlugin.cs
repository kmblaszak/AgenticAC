using System;
using System.IO;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CharacterTracker
{
    [FriendlyName("CharacterTracker")]
    public class CharacterTrackerPlugin : FilterBase
    {
        private readonly string logFile = @"C:\CharacterTracker_Test.txt";
        private readonly string jsonFile = @"C:\CharacterTracker.json";

        private DateTime lastTrackerUpdate = DateTime.MinValue;

        private readonly HashSet<CharFilterSkillType> unsupportedSkills =
            new HashSet<CharFilterSkillType>
            {
                CharFilterSkillType.Gearcraft,
                CharFilterSkillType.Axe,
                CharFilterSkillType.Bow,
                CharFilterSkillType.Crossbow,
                CharFilterSkillType.Dagger,
                CharFilterSkillType.Mace,
                CharFilterSkillType.Spear,
                CharFilterSkillType.Staff,
                CharFilterSkillType.Sword,
                CharFilterSkillType.ThrownWeapons,
                CharFilterSkillType.Unarmed
            };

        protected override void Startup()
        {
            File.AppendAllText(
                logFile,
                "\r\n============================\r\n" +
                "Startup fired: " + DateTime.Now + "\r\n"
            );

            CoreManager.Current.FilterInitComplete += FilterInitComplete;
        }


        protected override void Shutdown()
        {
            try
            {
                CoreManager.Current.FilterInitComplete -= FilterInitComplete;

                if (CoreManager.Current.CharacterFilter != null)
                {
                    CoreManager.Current.CharacterFilter.LoginComplete -= LoginComplete;
                }

                CoreManager.Current.RenderFrame -= RenderFrame;
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    logFile,
                    "Shutdown ERROR:\r\n" +
                    ex.ToString() +
                    "\r\n"
                );
            }


            File.AppendAllText(
                logFile,
                "Shutdown fired: " + DateTime.Now + "\r\n"
            );
        }


        private void FilterInitComplete(object sender, EventArgs e)
        {
            File.AppendAllText(
                logFile,
                "FilterInitComplete fired\r\n"
            );


            if (CoreManager.Current.CharacterFilter != null)
            {
                CoreManager.Current.CharacterFilter.LoginComplete += LoginComplete;

                File.AppendAllText(
                    logFile,
                    "LoginComplete subscribed\r\n"
                );
            }

        }


        private void LoginComplete(object sender, EventArgs e)
        {
            try
            {
                File.AppendAllText(
                    logFile,
                    "LOGIN COMPLETE FIRED\r\n"
                );

                CoreManager.Current.Actions.AddChatText(
                    "[CharacterTrackerPlugin] Loaded",
                    5
                );


                CoreManager.Current.RenderFrame += RenderFrame;

                File.AppendAllText(
                    logFile,
                    "RenderFrame subscribed\r\n"
                );


                File.AppendAllText(
                    logFile,
                    "Calling initial WriteCurrentPosition\r\n"
                );


                // This runs on the Decal thread
                WriteCurrentPosition();


                File.AppendAllText(
                    logFile,
                    "Initial WriteCurrentPosition completed\r\n"
                );
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    logFile,
                    "LoginComplete ERROR:\r\n" +
                    ex.ToString() +
                    "\r\n"
                );
            }
        }


        private void RenderFrame(object sender, EventArgs e)
        {
            try
            {
                if ((DateTime.Now - lastTrackerUpdate).TotalSeconds < 1)
                {
                    return;
                }

                lastTrackerUpdate = DateTime.Now;

                WriteCurrentPosition();
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    logFile,
                    "RenderFrame ERROR:\r\n" +
                    ex.ToString() +
                    "\r\n"
                );
            }
        }

        private void WriteCurrentPosition()
        {
            uint characterId =
                (uint)CoreManager.Current.CharacterFilter.Id;


            string characterName =
                CoreManager.Current.CharacterFilter.Name;


            string server =
                CoreManager.Current.CharacterFilter.Server;


            var playerObject =
                CoreManager.Current.WorldFilter[(int)characterId];


            if (playerObject == null)
            {
                return;
            }


           var coords = playerObject.Coordinates();


            if (coords == null)
            {
                return;
            }


            VitalState vitals = GetVitals();

            CharacterInfoState info = GetCharacterInfo();

            AttributeState attributes = GetAttributes();

            SkillsState skills = GetSkills();


            CharacterState state = new CharacterState
            {
               CharacterId = characterId,

                Name = characterName,

                Server = server,

                Position = new PositionState
               {
                    NorthSouth = coords.NorthSouth,
                    EastWest = coords.EastWest
                },

                Vitals = vitals,

                CharacterInfo = info,

                Attributes = attributes,

                Skills = skills,

                Timestamp = DateTime.Now.ToString("o")
           };


            string json = JsonConvert.SerializeObject(
                state,
                Formatting.Indented
            );


            File.WriteAllText(
                jsonFile,
                json
            );
        }


        private VitalState GetVitals()
        {
            var character = CoreManager.Current.CharacterFilter;

            var health = character.Vitals[CharFilterVitalType.Health];
            var mana = character.Vitals[CharFilterVitalType.Mana];
            var stamina = character.Vitals[CharFilterVitalType.Stamina];

            return new VitalState
            {
                HealthCurrent = health.Current,
                HealthMaximum = health.Buffed,
                HealthBase = health.Base,
                HealthBonus = health.Bonus,

                ManaCurrent = mana.Current,
                ManaMaximum = mana.Buffed,
                ManaBase = mana.Base,
                ManaBonus = mana.Bonus,

                StaminaCurrent = stamina.Current,
                StaminaMaximum = stamina.Buffed,
                StaminaBase = stamina.Base,
               StaminaBonus = stamina.Bonus
            };
        }

        private CharacterInfoState GetCharacterInfo()
        {
            var character = CoreManager.Current.CharacterFilter;

            return new CharacterInfoState
            {
                Level = character.Level,

                TotalXP = character.TotalXP,

                XPToNextLevel = character.XPToNextLevel,

                UnassignedXP = character.UnassignedXP,

                Vitae = character.Vitae,

                Deaths = character.Deaths,

                Burden = character.Burden,

                BurdenUnits = character.BurdenUnits
            };
        }     

        private AttributeState GetAttributes()
        {
            var character = CoreManager.Current.CharacterFilter;

            var strength = character.Attributes[CharFilterAttributeType.Strength];
            var endurance = character.Attributes[CharFilterAttributeType.Endurance];
            var quickness = character.Attributes[CharFilterAttributeType.Quickness];
            var coordination = character.Attributes[CharFilterAttributeType.Coordination];
            var focus = character.Attributes[CharFilterAttributeType.Focus];
            var self = character.Attributes[CharFilterAttributeType.Self];

            return new AttributeState
            {
                Strength = new AttributeValue
                {
                    Base = strength.Base,
                    Buffed = strength.Buffed,
                    Creation = strength.Creation,
                    XP = strength.Exp
                },

                Endurance = new AttributeValue
                {
                    Base = endurance.Base,
                    Buffed = endurance.Buffed,
                    Creation = endurance.Creation,
                    XP = endurance.Exp
                },

                Quickness = new AttributeValue
                {
                    Base = quickness.Base,
                    Buffed = quickness.Buffed,
                    Creation = quickness.Creation,
                    XP = quickness.Exp
                },

                Coordination = new AttributeValue
                {
                    Base = coordination.Base,
                    Buffed = coordination.Buffed,
                    Creation = coordination.Creation,
                    XP = coordination.Exp
                },

                Focus = new AttributeValue
                {
                   Base = focus.Base,
                    Buffed = focus.Buffed,
                    Creation = focus.Creation,
                    XP = focus.Exp
                },

                Self = new AttributeValue
                {
                    Base = self.Base,
                    Buffed = self.Buffed,
                    Creation = self.Creation,
                    XP = self.Exp
                }
            };
        }

        private SkillsState GetSkills()
        {
            SkillsState skillsState = new SkillsState();

            var character = CoreManager.Current.CharacterFilter;


            File.AppendAllText(
                logFile,
                "Skill count test starting\r\n"
            );


            foreach (CharFilterSkillType skillType in Enum.GetValues(typeof(CharFilterSkillType)))
            {
                if (unsupportedSkills.Contains(skillType))
                {
                    continue;
                }

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
                catch (Exception ex)
                {
                    File.AppendAllText(
                        logFile,
                        "Skill ERROR " + skillType + ":\r\n" +
                        ex.ToString() +
                        "\r\n"
                    );
                }
            }


            return skillsState;
        }

        private TrainingState ConvertTraining(TrainingType training)
        {
            switch (training)
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