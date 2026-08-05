using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CharacterTracker.GameData
{
    public static class SpellNameMap
    {
        private static Dictionary<int, string> spells =
            new Dictionary<int, string>();


        public static int Load(string filePath)
        {
            string json = File.ReadAllText(filePath);

            JObject root = JObject.Parse(json);

            JArray spellArray =
                (JArray)root["table"]["spellBaseHash"];

            if (spellArray == null)
            {
                throw new Exception(
                    "Could not find table.spellBaseHash array"
                );
            }


            foreach (JObject entry in spellArray)
            {
                int spellId = (int)entry["key"];

                JObject value = (JObject)entry["value"];

                string name = (string)value["name"];

                if (!string.IsNullOrEmpty(name))
                {
                    spells[spellId] = name;
                }
            }


            return spells.Count;
        }


        public static bool TryGetSpellName(int spellId, out string name)
        {
            return spells.TryGetValue(spellId, out name);
        }
    }
}

