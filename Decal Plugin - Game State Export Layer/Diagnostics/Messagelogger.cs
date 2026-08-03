using System;
using System.IO;
using System.Text;
using Decal.Adapter;

namespace CharacterTracker.Diagnostics
{
    public class MessageLogger
    {
        private readonly string logFile;

        public MessageLogger(string logFile)
        {
            this.logFile = logFile;
        }


        public void Start()
        {
            CoreManager.Current.MessageProcessed += OnMessageProcessed;
        }


        public void Stop()
        {
            CoreManager.Current.MessageProcessed -= OnMessageProcessed;
        }


        private string Indent(int depth)
        {
            return new string(' ', depth * 2);
        }



        private void OnMessageProcessed(object sender, MessageProcessedEventArgs e)
        {
            try
            {
                StringBuilder dump = new StringBuilder();

                dump.AppendLine();
                dump.AppendLine("============================");
                dump.AppendLine($"Message Type: {e.Message.Type}");
                dump.AppendLine($"Count: {e.Message.Count}");


                for (int i = 0; i < e.Message.Count; i++)
                {
                    DumpField(
                        e.Message,
                        i,
                        0,
                        dump
                    );
                }


                string dumpText = dump.ToString();


                //
                // SEARCH FOR POSSIBLE SKILL DATA
                //
                if (dumpText.Contains("value = 212"))
                {
                    File.AppendAllText(
                        logFile,
                        "\r\n\r\n******** POSSIBLE SKILL VALUE FOUND ********\r\n" +
                        dumpText +
                        "\r\n******** END POSSIBLE SKILL VALUE ********\r\n"
                    );
                }


                //
                // SEARCH FOR EXACT KEY/VALUE MATCH
                // This is what we want:
                // key = 13
                // value = 212
                //
                if (dumpText.Contains("key = 13") &&
                    dumpText.Contains("value = 215"))
                {
                    File.AppendAllText(
                        logFile,
                        "\r\n\r\n******** POSSIBLE UNARMED SKILL CONFIRMATION ********\r\n" +
                        dumpText +
                        "\r\n******** END POSSIBLE UNARMED CONFIRMATION ********\r\n"
                    );
                }


                // Normal logging
                File.AppendAllText(
                    logFile,
                    dumpText
                );

            }
            catch(Exception ex)
            {
                File.AppendAllText(
                    logFile,
                    "MESSAGE LOGGER ERROR:\r\n" +
                    ex +
                    "\r\n"
                );
            }
        }



        private void DumpField(
            Message message,
            int index,
            int depth,
            StringBuilder dump)
        {
            try
            {
                string indent = Indent(depth);

                string name = message.Name(index);

                object value;

                try
                {
                    value = message.Value<object>(index);
                }
                catch
                {
                    value = "VALUE READ FAILED";
                }


                dump.AppendLine(
                    $"{indent}Field {index}: {name} = {value}"
                );


                if (value is MessageStruct structValue)
                {
                    dump.AppendLine(
                        $"{indent}BEGIN MESSAGE STRUCT"
                    );


                    for (int x = 0; x < structValue.Count; x++)
                    {
                        DumpStructField(
                            structValue,
                            x,
                            depth + 1,
                            dump
                        );
                    }


                    dump.AppendLine(
                        $"{indent}END MESSAGE STRUCT"
                    );
                }
            }
            catch(Exception ex)
            {
                dump.AppendLine(
                    $"{Indent(depth)}FIELD {index} ERROR:\r\n{ex}"
                );
            }
        }



        private void DumpStructField(
            MessageStruct messageStruct,
            int index,
            int depth,
            StringBuilder dump)
        {
            try
            {
                string indent = Indent(depth);

                string name = messageStruct.Name(index);


                object value;

                try
                {
                    value = messageStruct.Value<object>(index);
                }
                catch
                {
                    value = "VALUE READ FAILED";
                }


                dump.AppendLine(
                    $"{indent}Struct Field {index}: {name} = {value}"
                );


                if (value is MessageStruct nested)
                {
                    dump.AppendLine(
                        $"{indent}BEGIN NESTED STRUCT"
                    );


                    for (int x = 0; x < nested.Count; x++)
                    {
                        DumpStructField(
                            nested,
                            x,
                            depth + 1,
                            dump
                        );
                    }


                    dump.AppendLine(
                        $"{indent}END NESTED STRUCT"
                    );
                }
            }
            catch(Exception ex)
            {
                dump.AppendLine(
                    $"{Indent(depth)}STRUCT FIELD {index} ERROR:\r\n{ex}"
                );
            }
        }
    }
}