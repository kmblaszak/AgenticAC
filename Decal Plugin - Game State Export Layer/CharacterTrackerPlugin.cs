using System;
using System.IO;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Newtonsoft.Json;
using System.Collections.Generic;
using CharacterTracker.Extractors;
using CharacterTracker.PacketTrackers;
using CharacterTracker.Maps;
using CharacterTracker.Calculators;
using CharacterTracker.GameData;
using Decal.Interop.SpellFilter;
using Newtonsoft.Json.Linq;

namespace CharacterTracker
{
    [FriendlyName("CharacterTracker")]
    public class CharacterTrackerPlugin : FilterBase
    {
        private readonly string logFile = @"C:\CharacterTracker_SkillPackets_Test.txt";
        private readonly string jsonFile = @"C:\CharacterTracker.json";

        private DateTime lastTrackerUpdate = DateTime.MinValue;
        private SkillExtractor skillExtractor;
        private PacketSkillTracker packetSkillTracker;
        private SkillPacketListener skillPacketListener;
        private AttributeState attributeState;

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
                CharFilterSkillType.ThrownWeapons,
                CharFilterSkillType.Unarmed,
                CharFilterSkillType.Sword
            };
        
        protected override void Startup()
        {
            skillExtractor = new SkillExtractor(logFile);

            packetSkillTracker = new PacketSkillTracker();

            skillPacketListener =
                new SkillPacketListener(
                    logFile,
                    packetSkillTracker,
                    () => GetAttributes());

            skillPacketListener.Start();


            File.AppendAllText(
                logFile,
                "\r\n============================\r\n" +
                "Startup fired: " + DateTime.Now + "\r\n"
            );


            // Load spell database
            try
            {
                int spellCount = SpellNameMap.Load(
                    @"C:\Agentic AC\Decal Plugin - Game State Export Layer\GameData\spells.json"
                );

                File.AppendAllText(
                    logFile,
                    "Spell database loaded successfully\r\n" +
                    $"Loaded spell count: {spellCount}\r\n"
                );
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    logFile,
                    "Spell database load failed:\r\n" +
                    ex +
                    "\r\n"
                );
            }


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
                /*
                messageLogger?.Stop();
                */

                skillPacketListener?.Stop();
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

                DumpEnchantments();

                CoreManager.Current.Actions.AddChatText(
                    "[CharacterTrackerPlugin] Loaded",
                    5
                );


                attributeState = GetAttributes();


                CoreManager.Current.RenderFrame += RenderFrame;


                File.AppendAllText(
                    logFile,
                    "RenderFrame subscribed\r\n"
                );


                File.AppendAllText(
                    logFile,
                    "Calling initial WriteCurrentPosition\r\n"
                );


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

            SkillsState skills = GetPacketSkills();

            List<ActiveEnchantmentState> activeEnchantments =
                GetActiveEnchantments();


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

                ActiveEnchantments = activeEnchantments,

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


        private SkillsState GetPacketSkills()
        {
            SkillsState skillsState = new SkillsState();

            foreach (PacketSkill packetSkill in packetSkillTracker.Skills.Values)
            {
                AttributeState currentAttributes = GetAttributes();

                double attributeValue =
                    SkillFormulaEvaluator.CalculateAttributeValue(
                        packetSkill.SkillId,
                        currentAttributes);


                int baseValue =
                    (int)Math.Round(
                        packetSkill.Raised +
                        attributeValue +
                        packetSkill.Bonus);


                int enchantmentBonus =
                    GetSkillEnchantmentBonus(
                        packetSkill.SkillId);


                int buffedValue =
                    baseValue + enchantmentBonus;


                bool specialized =
                    packetSkill.State == 3;


                long experienceToNextSkillPoint =
                    SkillExperienceTable.GetXpToNextSkillPoint(
                        packetSkill.XP,
                        specialized);


                double percentToNextSkillPoint =
                    SkillExperienceTable.GetPercentToNextSkillPoint(
                        packetSkill.XP,
                        specialized);


                SkillState skillState = new SkillState
                {
                    Type = (SkillType)packetSkill.SkillId,

                    Name = SkillNameMap.GetName(packetSkill.SkillId),

                    ShortName = SkillNameMap.GetName(packetSkill.SkillId),

                    Known = true,

                    Formula = SkillFormulaMap.GetFormula(packetSkill.SkillId),

                    Training = (TrainingState)packetSkill.State,

                    Value = new SkillValue
                    {
                        Base = baseValue,

                        Bonus = packetSkill.Bonus,

                        Buffed = buffedValue,

                        EnchantmentBonus = enchantmentBonus,

                        Current = buffedValue,

                        Experience = packetSkill.XP,

                        Increment = packetSkill.Raised,

                        Diff = packetSkill.Diff,

                        ExperienceToNextSkillPoint = experienceToNextSkillPoint,

                        PercentToNextSkillPoint = percentToNextSkillPoint
                    }
                };


                skillsState.Skills[(SkillType)packetSkill.SkillId] = skillState;
            }


            return skillsState;
        } 

        private int GetSkillEnchantmentBonus(int skillId)
        {
            int bonus = 0;

            foreach (EnchantmentWrapper enchant in CoreManager.Current.CharacterFilter.Enchantments)
            {
                if (enchant.Affected == skillId)
                {
                    bonus += (int)Math.Round(enchant.Adjustment);
                }
            }

            return bonus;
        }        

        private List<ActiveEnchantmentState> GetActiveEnchantments()
        {
            List<ActiveEnchantmentState> enchantments =
                new List<ActiveEnchantmentState>();


            foreach (EnchantmentWrapper enchant in 
                CoreManager.Current.CharacterFilter.Enchantments)
            {
                string spellName = "Unknown";

                if (SpellNameMap.TryGetSpellName(
                    enchant.SpellId,
                    out string name))
                {
                    spellName = name;
                }


                string affectedName = "Unknown";

                if (SkillNameMap.TryGetSkillName(
                    enchant.Affected,
                    out string skillName))
                {
                    affectedName = skillName;
                }


        enchantments.Add(
            new ActiveEnchantmentState
            {
                SpellId = enchant.SpellId,

                SpellName = spellName,

                Adjustment = enchant.Adjustment,

                Affected = enchant.Affected,

                AffectedName = affectedName,

                AffectedMask = enchant.AffectedMask,

                Duration = enchant.Duration,

                Remaining = enchant.TimeRemaining,

                Family = enchant.Family,

                Layer = enchant.Layer
            });
            }


            return enchantments;
        }        

        private void DumpEnchantments()
        {
            File.AppendAllText(
                logFile,
                "\r\n===== ACTIVE ENCHANTMENTS =====\r\n");

            SpellsClass spells = new SpellsClass();

            foreach (EnchantmentWrapper enchant in CoreManager.Current.CharacterFilter.Enchantments)
            {
                string affectedName = "Unknown";

                if (SkillNameMap.TryGetSkillName(enchant.Affected, out string skillName))
                {
                    affectedName = skillName;
                }


                string spellName = "Unknown";

                if (SpellNameMap.TryGetSpellName(
                        enchant.SpellId,
                        out string name))
                {
                    spellName = name;
                }               


                File.AppendAllText(
                    logFile,
                    $"SpellId: {enchant.SpellId}\r\n" +
                    $"Spell Name: {spellName}\r\n" +
                    $"Adjustment: {enchant.Adjustment}\r\n" +
                    $"Affected: {enchant.Affected}\r\n" +
                    $"Affected Name: {affectedName}\r\n" +
                    $"AffectedMask: {enchant.AffectedMask}\r\n" +
                    $"Duration: {enchant.Duration}\r\n" +
                    $"Remaining: {enchant.TimeRemaining}\r\n" +
                    $"Family: {enchant.Family}\r\n" +
                    $"Layer: {enchant.Layer}\r\n" +
                    "----------------------------\r\n");
            }
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