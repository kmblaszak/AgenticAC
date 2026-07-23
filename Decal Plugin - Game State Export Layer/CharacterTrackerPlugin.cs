using System;
using System.IO;
using System.Timers;
using Decal.Adapter;

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


            string json =
                "{\r\n" +
                "  \"CharacterId\": " + characterId + ",\r\n" +
                "  \"Name\": \"" + characterName + "\",\r\n" +
                "  \"Server\": \"" + server + "\",\r\n" +
                "  \"NorthSouth\": " + coords.NorthSouth + ",\r\n" +
                "  \"EastWest\": " + coords.EastWest + ",\r\n" +
                "  \"Timestamp\": \"" + DateTime.Now.ToString("o") + "\"\r\n" +
                "}";


            File.WriteAllText(
                jsonFile,
                json
            );
        }
    }
}