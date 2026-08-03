using System;
using System.IO;
using System.Text;
using Decal.Adapter;
using CharacterTracker.Maps;
using CharacterTracker.Calculators;

namespace CharacterTracker.PacketTrackers
{
    public class SkillPacketListener
    {
        private readonly string logFile;
        private readonly PacketSkillTracker skillTracker;
        private readonly Func<AttributeState> getAttributes;


        public SkillPacketListener(
            string logFile,
            PacketSkillTracker skillTracker,
            Func<AttributeState> getAttributes)
        {
            this.logFile = logFile;
            this.skillTracker = skillTracker;
            this.getAttributes = getAttributes;
        }


        public void Start()
        {
            CoreManager.Current.MessageProcessed += OnMessageProcessed;

            File.AppendAllText(
                logFile,
                "SkillPacketListener started\r\n");
        }


        public void Stop()
        {
            CoreManager.Current.MessageProcessed -= OnMessageProcessed;
        }



        private void OnMessageProcessed(
            object sender,
            MessageProcessedEventArgs e)
        {
            try
            {
                if (e.Message.Type != 0xF7B0)
                {
                    return;
                }


                int eventId = -1;


                for (int i = 0; i < e.Message.Count; i++)
                {
                    try
                    {
                        if (e.Message.Name(i) == "event")
                        {
                            eventId =
                                Convert.ToInt32(
                                    e.Message.Value<object>(i));

                            break;
                        }
                    }
                    catch
                    {
                    }
                }



                File.AppendAllText(
                    logFile,
                    $"FOUND F7B0 | " +
                    $"Time={DateTime.Now:HH:mm:ss.fff} | " +
                    $"Fields={e.Message.Count} | " +
                    $"Event={eventId}\r\n");



                if (eventId != 19)
                {
                    return;
                }



                StringBuilder output = new StringBuilder();


                output.AppendLine();
                output.AppendLine("================================================");
                output.AppendLine(
                    $"SKILL STATE PACKET | {DateTime.Now:HH:mm:ss.fff}");
                output.AppendLine(
                    $"Message Type: 0x{e.Message.Type:X4}");
                output.AppendLine(
                    $"Event: {eventId}");
                output.AppendLine();



                MessageStruct properties = null;

                int characterId = 0;
                int sequence = 0;



                for (int i = 0; i < e.Message.Count; i++)
                {
                    try
                    {
                        string name =
                            e.Message.Name(i);


                        object value =
                            e.Message.Value<object>(i);



                        switch(name)
                        {
                            case "character":

                                characterId =
                                    Convert.ToInt32(value);

                                break;


                            case "sequence":

                                sequence =
                                    Convert.ToInt32(value);

                                break;


                            case "properties":

                                properties =
                                    (MessageStruct)value;

                                break;
                        }
                    }
                    catch
                    {
                    }
                }



                output.AppendLine(
                    $"Character: {characterId}");

                output.AppendLine(
                    $"Sequence: {sequence}");

                output.AppendLine();



                MessageStruct vectors = null;



                for (int i = 0; i < e.Message.Count; i++)
                {
                    try
                    {
                        if (e.Message.Name(i) == "vectors")
                        {
                            vectors =
                                e.Message.Value<MessageStruct>(i);

                            break;
                        }
                    }
                    catch
                    {
                    }
                }



                if (vectors == null)
                {
                    output.AppendLine(
                        "NO VECTORS STRUCT FOUND");

                    File.AppendAllText(
                        logFile,
                        output.ToString());

                    return;
                }



                MessageStruct skills = null;



                for (int i = 0; i < vectors.Count; i++)
                {
                    if (vectors.Name(i) == "skills")
                    {
                        skills =
                            vectors.Value<MessageStruct>(i);

                        break;
                    }
                }



                if (skills == null)
                {
                    output.AppendLine(
                        "NO SKILLS STRUCT FOUND");

                    File.AppendAllText(
                        logFile,
                        output.ToString());

                    return;
                }



                output.AppendLine(
                    "ACTIVE SKILLS");

                output.AppendLine(
                    "------------------------------");



                int found = 0;



                for (int i = 0; i < skills.Count; i++)
                {
                    MessageStruct skillEntry =
                        skills.Value<MessageStruct>(i);



                    int skillId = -1;
                    int increment = 0;
                    int xp = 0;
                    int bonus = 0;
                    int diff = 0;
                    int state = 0;



                    for (int x = 0; x < skillEntry.Count; x++)
                    {
                        string field =
                            skillEntry.Name(x);


                        object value =
                            skillEntry.Value<object>(x);



                        if (field == "key")
                        {
                            skillId =
                                Convert.ToInt32(value);
                        }


                        else if (field == "value")
                        {
                            MessageStruct skillData =
                                (MessageStruct)value;



                            for (int y = 0; y < skillData.Count; y++)
                            {
                                string dataField =
                                    skillData.Name(y);


                                object dataValue =
                                    skillData.Value<object>(y);



                                switch(dataField)
                                {
                                    case "raised":

                                        increment =
                                            Convert.ToInt32(dataValue);

                                        break;


                                    case "xp":

                                        xp =
                                            Convert.ToInt32(dataValue);

                                        break;


                                    case "bonus":

                                        bonus =
                                            Convert.ToInt32(dataValue);

                                        break;


                                    case "diff":

                                        diff =
                                            Convert.ToInt32(dataValue);

                                        break;


                                    case "state":

                                        state =
                                            Convert.ToInt32(dataValue);

                                        break;
                                }
                            }
                        }
                    }



                    if (skillId < 0)
                    {
                        continue;
                    }



                    found++;

                    skillTracker.UpdateSkill(
                        new PacketSkill
                        {
                            SkillId = skillId,
                            Raised = increment,
                            XP = xp,
                            Bonus = bonus,
                            Diff = diff,
                            State = state
                        });

                    File.AppendAllText(
                        logFile,
                        $"TRACKER UPDATE | SkillId={skillId} | " +
                        $"Name={SkillNameMap.GetName(skillId)} | " +
                        $"Raised={increment} | " +
                        $"XP={xp} | " +
                        $"Bonus={bonus} | " +
                        $"State={state}\r\n");    

                    string skillName =
                        SkillNameMap.GetName(skillId);


                    string stateName =
                        SkillStateMap.GetName(state);

                    string formula =
                        SkillFormulaMap.GetFormula(skillId);


                    AttributeState currentAttributes =
                        getAttributes?.Invoke();


                    double attributeValue =
                        SkillFormulaEvaluator.CalculateAttributeValue(
                            skillId,
                            currentAttributes);



                    int baseValue =
                        (int)Math.Round(
                            increment +
                            attributeValue +
                            bonus);



                    output.AppendLine();

                    output.AppendLine(
                        $"Skill : {skillName}");

                    output.AppendLine(
                        $"  Type       : {skillId}");

                    output.AppendLine(
                        $"  Name       : {skillName}");

                    output.AppendLine(
                        $"  ShortName  : {skillName}");

                    output.AppendLine(
                        $"  Known      : True");

                    output.AppendLine(
                        $"  Formula   : {formula}");                        

                    output.AppendLine(
                        $"  Training   : {state} ({stateName})");

                    output.AppendLine(
                        $"  Value:");

                    output.AppendLine(
                        $"    Base       : {baseValue:F2}");

                    output.AppendLine(
                        $"    Bonus      : {bonus}");

                    output.AppendLine(
                        $"    Buffed     : {baseValue:F2}");

                    output.AppendLine(
                        $"    Current    : 0");

                    output.AppendLine(
                        $"    Experience : {xp}");

                    output.AppendLine(
                        $"    Increment : {increment}");

                   // output.AppendLine(
                      //  $"    Diff       : {diff}");
                }



                output.AppendLine();

                output.AppendLine(
                    $"Total Active Skills: {found}");

                output.AppendLine(
                    "================================================");



            File.AppendAllText(
                logFile,
                output.ToString());


            skillTracker.DumpSkills(logFile);
            }
            catch(Exception ex)
            {
                File.AppendAllText(
                    logFile,
                    "SkillPacketListener ERROR:\r\n" +
                    ex +
                    "\r\n");
            }
        }
    }
}