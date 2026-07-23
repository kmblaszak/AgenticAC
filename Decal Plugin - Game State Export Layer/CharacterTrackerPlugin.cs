using System;
using System.IO;
using System.Timers;
using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace CharacterTracker
{
    [FriendlyName("CharacterTracker")]
    public class CharacterTrackerPlugin : FilterBase
    {
        private readonly string logFile = @"C:\CharacterTracker_Test.txt";
        private readonly string jsonFile = @"C:\CharacterTracker.json";

        private Timer trackerTimer;


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


                if (trackerTimer != null)
                {
                    trackerTimer.Stop();
                    trackerTimer.Dispose();
                    trackerTimer = null;
                }
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

                if (trackerTimer != null)
                {
                    trackerTimer.Stop();
                    trackerTimer.Dispose();
                }


                trackerTimer = new Timer(1000);
                trackerTimer.Elapsed += TrackerTimer_Elapsed;
                trackerTimer.AutoReset = true;
                trackerTimer.Start();


                File.AppendAllText(
                    logFile,
                    "Tracker timer started\r\n"
                );

                // Write immediately instead of waiting 1 second
                WriteCurrentPosition();


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


        private void TrackerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                WriteCurrentPosition();
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    logFile,
                    "Timer ERROR:\r\n" +
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

            string json =
                "{\r\n" +
                "  \"CharacterId\": " + characterId + ",\r\n" +
                "  \"Name\": \"" + characterName + "\",\r\n" +
                "  \"Server\": \"" + server + "\",\r\n" +
                "  \"NorthSouth\": " + coords.NorthSouth + ",\r\n" +
                "  \"EastWest\": " + coords.EastWest + ",\r\n" +
                "  \"Vitals\": {\r\n" +
                "      \"HealthCurrent\": " + vitals.HealthCurrent + ",\r\n" +
                "      \"HealthMaximum\": " + vitals.HealthMaximum + ",\r\n" +
                "      \"HealthBase\": " + vitals.HealthBase + ",\r\n" +
                "      \"HealthBonus\": " + vitals.HealthBonus + ",\r\n" +
                "      \"ManaCurrent\": " + vitals.ManaCurrent + ",\r\n" +
                "      \"ManaMaximum\": " + vitals.ManaMaximum + ",\r\n" +
                "      \"ManaBase\": " + vitals.ManaBase + ",\r\n" +
                "      \"ManaBonus\": " + vitals.ManaBonus + ",\r\n" +
                "      \"StaminaCurrent\": " + vitals.StaminaCurrent + ",\r\n" +
                "      \"StaminaMaximum\": " + vitals.StaminaMaximum + ",\r\n" +
                "      \"StaminaBase\": " + vitals.StaminaBase + ",\r\n" +
                "      \"StaminaBonus\": " + vitals.StaminaBonus + "\r\n" +
                "  },\r\n" +
                "  \"CharacterInfo\": {\r\n" +
                "      \"Level\": " + info.Level + ",\r\n" +
                "      \"TotalXP\": " + info.TotalXP + ",\r\n" +
                "      \"XPToNextLevel\": " + info.XPToNextLevel + ",\r\n" +
                "      \"UnassignedXP\": " + info.UnassignedXP + ",\r\n" +
                "      \"Vitae\": " + info.Vitae + ",\r\n" +
                "      \"Deaths\": " + info.Deaths + ",\r\n" +
                "      \"Burden\": " + info.Burden + ",\r\n" +
                "      \"BurdenUnits\": " + info.BurdenUnits + "\r\n" +
                "  },\r\n" +
                "  \"Timestamp\": \"" + DateTime.Now.ToString("o") + "\"\r\n" +
                "}";


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
    }
}