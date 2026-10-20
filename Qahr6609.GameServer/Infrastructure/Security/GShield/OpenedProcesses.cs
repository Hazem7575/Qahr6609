using GShield;
using Conquer6609.Client;
using Conquer6609.Game.MsgLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    using Conquer6609.Client;
    using Conquer6609.Game.MsgLoader;
    using Conquer6609.ServerSockets;
    using System;
    using System.IO;
    using System.Text;

    public class OpenedProcesses
    {
        // A client reports its running processes between Start and Finish; never keep more than this per client.
        public const int MaxProcessesPerClient = 512;

        [Packet((ushort)Types.PacketTypes.CMsgOpenedProcesses)]
        public static void Process(GameClient client, Packet stream)
        {
            try
            {
                stream.Seek(0);
                byte[] buffer = stream.ReadBytes(stream.Size);
                using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
                {
                    br.BaseStream.Seek(4, SeekOrigin.Current);
                    Types.ProcessesType action = (Types.ProcessesType)br.ReadInt16();

                    if (action == Types.ProcessesType.Start)
                        client.OpenedProcesses.Clear();
                    else if (action == Types.ProcessesType.Insert)
                        InsertProcess(client, br);
                    else if (action == Types.ProcessesType.Finish)
                        SaveAndCheck(client);
                }
            }
            catch (Exception ex) { Console.WriteLine("[Process] Error: " + ex.Message); }
        }
        private static void InsertProcess(GameClient client, BinaryReader br)
        {
            short len = br.ReadInt16();
            if (len <= 0 || len > 128) return;
            string name = Encoding.UTF8.GetString(br.ReadBytes(128), 0, len).TrimEnd('\0');
            string[] p = name.Split(new[] { '|' }, 2);
            if (p.Length < 2) return;

            string exe = p[1].Trim().ToLowerInvariant();
            string md5 = br.ReadInt64().ToString("X16").ToLowerInvariant();
            uint sec = (uint)br.ReadInt64();
            uint mem = (uint)br.ReadInt64();

            if (client.OpenedProcesses.Count >= MaxProcessesPerClient)
                return;
            // "|" can't be part of a field: the old "exe|md5|sec|mem" string skipped such entries, keep doing so
            if (exe.IndexOf('|') >= 0)
                return;
            client.OpenedProcesses.Add(new QueryProcess
            {
                ExeName = exe,
                MD5_Hash = md5,
                SectionXxHash = sec,
                SectionMemXxHash = mem
            });
        }

        private static void SaveAndCheck(GameClient client)
        {
            string name = string.IsNullOrEmpty(client.Player.Name) ? "Player" : client.Player.Name;
            string safe = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
            string file = Path.Combine("Loader_Logs", "PlayerProcesses", $"{safe} [UID {client.Player.UID}] - {DateTime.Now:yyyy-MM-dd HH-mm-ss}.txt");

            var lines = new List<string>
    {
        "════════════════════════════════════════════════════════════════",
        $"Player : {name} | UID : {client.Player.UID} | Time : {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
        "════════════════════════════════════════════════════════════════",
        $"{"Process",-35} {"MD5",-34} {"SecHash",-10} {"MemHash",-10} Status",
        "────────────────────────────────────────────────────────────────────"
    };

            bool banned = false;
            var bannedProcesses = new List<string>();

            foreach (var qp in client.OpenedProcesses)
            {
                bool isBanned = QueryProcess.ContainOnList(qp, out string bn);
                if (isBanned)
                {
                    banned = true;
                    bannedProcesses.Add($"{qp.ExeName} ({bn})");
                }

                lines.Add($"{qp.ExeName,-35} {qp.MD5_Hash,-34} {qp.SectionXxHash,-10} {qp.SectionMemXxHash,-10} {(isBanned ? "BANNED → " + bn : "Allowed")}");
            }

            lines.Add("════════════════════════════════════════════════════════════════");

            if (banned || client.ForceProcessLog)
            {
                ShieldLogWriter.WriteFile(file, lines);// written by the HazemShield log thread
                client.ForceProcessLog = false;
                Console.WriteLine("[HazemShield] Process log saved > {0} <", Path.GetFileName(file));
            }

            if (banned)
            {
                string bannedList = string.Join(", ", bannedProcesses);

                HazemShield.Detected(client, $"Program : {bannedList}");
            }

            client.OpenedProcesses.Clear();
        }
    }
}