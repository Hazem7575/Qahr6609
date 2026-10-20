using Conquer6609.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Conquer6609.Client;
using Conquer6609.Game.MsgServer;
using Conquer6609.WindowsAPI;
using Project2D.Database;


namespace Conquer6609
{
    // Game-side part of the old GM panel (Co_Panle): the [GM] whisper history, the per-skill extra damage
    // multipliers from table panelattack (LoadAttack / ExtraPanleDamge) and their Update. It is always built.
    // The DevExpress form itself (Co_Panle.cs + Co_Panle.Designer.cs + Co_Panle.resx) is only built when the
    // DevExpress v21.2 dlls are in libs\ (GameServer.csproj, constant HAS_DEVEXPRESS).
    public partial class Co_Panle
    {
        // Whisper history per player for the chat page. Written from the game's socket thread (MsgMessage) and
        // read / changed by this form's UI thread, so everything here is thread-safe and capped.
        public class Clientt
        {
            public const int MaxLines = 50;
            public const int MaxLineLength = 255;
            private readonly System.Collections.Concurrent.ConcurrentQueue<string> lines = new System.Collections.Concurrent.ConcurrentQueue<string>();
            public volatile bool Seen = true;

            /// <summary>Adds one line (cut to MaxLineLength); only the last MaxLines lines are kept.</summary>
            public void Add(string line)
            {
                if (line == null)
                    line = "";
                if (line.Length > MaxLineLength)
                    line = line.Substring(0, MaxLineLength);
                lines.Enqueue(line);
                string dropped;
                while (lines.Count > MaxLines && lines.TryDequeue(out dropped)) { }
            }

            /// <summary>Snapshot of the history, oldest first.</summary>
            public string[] Lines { get { return lines.ToArray(); } }
        }
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Clientt> Clients = new System.Collections.Concurrent.ConcurrentDictionary<string, Clientt>();

        public static volatile bool NewMessage = false;

        /// <summary>Adds a line to a player's history (any thread). notify = let the chat page refresh.</summary>
        public static void AddChatLine(string player, string line, bool notify = true)
        {
            if (string.IsNullOrEmpty(player))
                return;
            Clients.GetOrAdd(player, name => new Clientt()).Add(line);
            if (notify)
                NewMessage = true;
        }
        public static bool On = false;

        public static void WelcomeMessage(Client.GameClient user, Game.MsgNpc.Npc npc, ServerSockets.Packet stream)
        {
            var msg = new MsgMessage("السلام عليكم انا مدير العبه ازاى اقدر اساعد حضرتك؟ من فضلك ارسل رسالتك وانتظر رد الجى ام شكرا ", 1371006, user.Player.Name, "[GM]", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Whisper);
            using (var rec = new ServerSockets.RecycledPacket())
                Program.SendGlobalPackets.Enqueue(msg.GetArray(rec.GetStream()));
        }


        #region Ninja
        #region TwofoldBlade
          public static uint TwofoldBladeSkilUID = 0;
        public static double TwofoldBladePlayers;
        public static void TwofoldBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "6000");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                TwofoldBladeSkilUID = r.ReadUInt32("SkilUID");
                TwofoldBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BloodyScythe
        public static uint BloodyScytheSkilUID = 0;
        public static double BloodyScythePlayers;
        public static void BloodyScytheTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11170");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                BloodyScytheSkilUID = r.ReadUInt32("SkilUID");
                BloodyScythePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region CounterKill
        public static uint CounterKillSkilUID = 0;
        public static double CounterKillPlayers;
        public static void CounterKillTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "6003");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                CounterKillSkilUID = r.ReadUInt32("SkilUID");
                CounterKillPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ShurikenV
        public static uint ShurikenVSkilUID = 0;
        public static double ShurikenVPlayers;
        public static void ShurikenVTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "6010");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ShurikenVSkilUID = r.ReadUInt32("SkilUID");
                ShurikenVPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region GapingWounds
        public static uint GapingWoundsSkilUID = 0;
        public static double GapingWoundsPlayers;
        public static void GapingWoundsTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11230");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                GapingWoundsSkilUID = r.ReadUInt32("SkilUID");
                GapingWoundsPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region TwilightDance
        public static uint TwilightDanceUID = 0;
        public static double TwilightDancePlayers;
        public static void TwilightDanceTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12070");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                TwilightDanceUID = r.ReadUInt32("SkilUID");
                TwilightDancePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SuperTwBlade
        public static uint SuperTwBladeUID = 0;
        public static double SuperTwBladePlayers;
        public static void SuperTwBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12080");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SuperTwBladeUID = r.ReadUInt32("SkilUID");
                SuperTwBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FatalSpin
        public static uint FatalSpinUID = 0;
        public static double FatalSpinPlayers;
        public static void FatalSpinTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12110");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FatalSpinUID = r.ReadUInt32("SkilUID");
                FatalSpinPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Monk
        #region RadiantPalm
        public static uint RadiantPalmSkilUID =0;
        public static double RadiantPalmPlayers;

        public static void RadiantPalmTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "10381");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                RadiantPalmSkilUID = r.ReadUInt32("SkilUID");
                RadiantPalmPlayers = r.ReadDouble("Damge");
;
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region WhirlwindKick
        public static uint WhirlwindKickSkilUID = 0;
        public static double WhirlwindKickPlayers;
        public static void WhirlwindKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "10415");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                WhirlwindKickSkilUID = r.ReadUInt32("SkilUID");
                WhirlwindKickPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region TripleAttack
        public static uint TripleAttackSkilUID = 0;
        public static double TripleAttackPlayers;
        public static void TripleAttackTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "10490");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                TripleAttackSkilUID = r.ReadUInt32("SkilUID");
                TripleAttackPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Oblivion
        public static uint OblivionSkilUID = 0;
        public static double OblivionPlayers;
        public static void OblivionTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "10390");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                OblivionSkilUID = r.ReadUInt32("SkilUID");
                OblivionPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region WrathoftheEmperor
        public static uint WrathoftheEmperorSkilUID = 0;
        public static double WrathoftheEmperorPlayers;
        public static void WrathoftheEmperorTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12570");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                WrathoftheEmperorSkilUID = r.ReadUInt32("SkilUID");
                WrathoftheEmperorPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region InfernalEcho
        public static uint InfernalEchoSkillUID = 0;
        public static double InfernalEchoPlayers;
        public static void InfernalEchoTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12550");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                InfernalEchoSkillUID = r.ReadUInt32("SkilUID");
                InfernalEchoPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Strike
        public static uint StrikeSkilUID = 0;
        public static double StrikePlayers;
        public static void StrikeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12600");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                StrikeSkilUID = r.ReadUInt32("SkilUID");
                StrikePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Warrior
        #region SpeedGun
        public static uint SpeedGunSkilUID = 0;
        public static double SpeedGunPlayers;
        public static void SpeedGunTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1260");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SpeedGunSkilUID = r.ReadUInt32("SkilUID");
                SpeedGunPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Snow
        public static uint SnowSkilUID = 0;
        public static double SnowPlayers;
        public static void SnowTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "5010");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SnowSkilUID = r.ReadUInt32("SkilUID");
                SnowPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Boom
        public static uint BoomSkilUID = 0;
        public static double BoomPlayers;
        public static void BoomTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "5040");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                BoomSkilUID = r.ReadUInt32("SkilUID");
                BoomPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Seizer
        public static uint SeizerSkilUID = 0;
        public static double SeizerPlayers;
        public static void SeizerTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "7000");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SeizerSkilUID = r.ReadUInt32("SkilUID");
                SeizerPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ViperFang
        public static uint ViperFangSkilUID = 0;
        public static double ViperFangPlayers;
        public static void ViperFangTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11005");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ViperFangSkilUID = r.ReadUInt32("SkilUID");
                ViperFangPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Penetration
        public static uint PenetrationSkilUID = 0;
        public static double PenetrationPlayers;
        public static void PenetrationTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1290");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                PenetrationSkilUID = r.ReadUInt32("SkilUID");
                PenetrationPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonTail
        public static uint DragonTailSkilUID = 0;
        public static double DragonTailPlayers;
        public static void DragonTailTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11000");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                DragonTailSkilUID = r.ReadUInt32("SkilUID");
                DragonTailPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Celestial
        public static uint CelestialSkilUID = 0;
        public static double CelestialPlayers;
        public static void CelestialTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "7030");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                CelestialSkilUID = r.ReadUInt32("SkilUID");
                CelestialPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ChargingVortex
        public static uint ChargingVortexUID = 0;
        public static double ChargingVortexPlayers;
        public static void ChargingVortexTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11190");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ChargingVortexUID = r.ReadUInt32("SkilUID");
                ChargingVortexPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ScarofEarth
        public static uint ScarofEarthSkilUID = 0;
        public static double ScarofEarthPlayers;
        public static void ScarofEarthTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12670");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ScarofEarthSkilUID = r.ReadUInt32("SkilUID");
                ScarofEarthPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region WaveofBlood
        public static uint WaveofBloodSkilUID = 0;
        public static double WaveofBloodPlayers;
        public static void WaveofBloodTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12690");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                WaveofBloodSkilUID = r.ReadUInt32("SkilUID");
                WaveofBloodPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ManiacDance
        public static uint ManiacDanceSkilUID = 0;
        public static double ManiacDancePlayers;
        public static void ManiacDanceTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12700");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ManiacDanceSkilUID = r.ReadUInt32("SkilUID");
                ManiacDancePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Pirate
        #region BladeTempest
        public static uint BladeTempestSkilUID = 0;
        public static double BladeTempestPlayers;
        public static void BladeTempestTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11110");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                BladeTempestSkilUID = r.ReadUInt32("SkilUID");
                BladeTempestPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region GaleBomb
        public static uint GaleBombSkilUID = 0;
        public static double GaleBombPlayers;
        public static void GaleBombTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11070");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                GaleBombSkilUID = r.ReadUInt32("SkilUID");
                GaleBombPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Windstorm
        public static uint WindstormSkilUID = 0;
        public static double WindstormPlayers;
        public static void WindstormTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11140");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                WindstormSkilUID = r.ReadUInt32("SkilUID");
                WindstormPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ScurvyBomb
        public static uint ScurvyBombSkilUID = 0;
        public static double ScurvyBombPlayers;
        public static void ScurvyBombTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11040");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ScurvyBombSkilUID = r.ReadUInt32("SkilUID");
                ScurvyBombPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region CannonBarrage
        public static uint CannonBarrageSkilUID = 0;
        public static double CannonBarragePlayers;
        public static void CannonBarrageTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11050");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                CannonBarrageSkilUID = r.ReadUInt32("SkilUID");
                CannonBarragePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region KrakensRevenge
        public static uint KrakensRevengeSkilUID = 0;
        public static double KrakensRevengePlayers;
        public static void KrakensRevengeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11100");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                KrakensRevengeSkilUID = r.ReadUInt32("SkilUID");
                KrakensRevengePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Trojan
        #region FastBlade
        public static uint FastBladeSkilUID = 0;
        public static double FastBladePlayers;
        public static void FastBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1045");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FastBladeSkilUID = r.ReadUInt32("SkilUID");
                FastBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ScentSword
        public static uint ScentSwordSkilUID = 0;
        public static double ScentSwordPlayers;
        public static void ScentSwordTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1046");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ScentSwordSkilUID = r.ReadUInt32("SkilUID");
                ScentSwordPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Rage
        public static uint RageSkilUID = 0;
        public static double RagePlayers;
        public static void RageTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "7020");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                RageSkilUID = r.ReadUInt32("SkilUID");
                RagePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Phoenix
        public static uint PhoenixSkilUID = 0;
        public static double PhoenixPlayers;

        public static void PhoenixTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "5030");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                PhoenixSkilUID = r.ReadUInt32("SkilUID");
                PhoenixPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Hercules
        public static uint HerculesSkilUID = 0;
        public static double HerculesPlayers;
        public static void HerculesTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1115");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                HerculesSkilUID = r.ReadUInt32("SkilUID");
                HerculesPlayers = r.ReadDouble("Damge");
               
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonWhirl
        public static uint DragonWhirlSkilUID = 0;
        public static double DragonWhirlPlayers;
        public static void DragonWhirlTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "10315");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                DragonWhirlSkilUID = r.ReadUInt32("SkilUID");
                DragonWhirlPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FatalCross
        public static uint FatalCrossSkilUID = 0;
        public static double FatalCrossPlayers;
        public static void FatalCrossTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11980");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FatalCrossSkilUID = r.ReadUInt32("SkilUID");
                FatalCrossPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region MortalStrike
        public static uint MortalStrikeSkilUID = 0;
        public static double MortalStrikePlayers;
        public static void MortalStrikeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11990");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                MortalStrikeSkilUID = r.ReadUInt32("SkilUID");
                MortalStrikePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Fire
        #region Thunder
        public static uint ThunderSkilUID = 0;
        public static double ThunderPlayers;
        public static void ThunderTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1000");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ThunderSkilUID = r.ReadUInt32("SkilUID");
                ThunderPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Fire
        public static uint FireSkilUID = 0;
        public static double FirePlayers;
        public static void FireTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1001");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FireSkilUID = r.ReadUInt32("SkilUID");
                FirePlayers = r.ReadDouble("Damge");


            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Tornado
        public static uint TornadoSkilUID = 0;
        public static double TornadoPlayers;
        public static void TornadoTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1002");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                TornadoSkilUID = r.ReadUInt32("SkilUID");
                TornadoPlayers = r.ReadDouble("Damge");
               
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Bomb
        public static uint BombSkilUID = 0;
        public static double BombPlayers;
        public static void BombTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1160");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                BombSkilUID = r.ReadUInt32("SkilUID");
                BombPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ChainBolt
        public static uint ChainBoltSkilUID = 0;
        public static double ChainBoltPlayers;
        public static void ChainBoltTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "10309");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ChainBoltSkilUID = r.ReadUInt32("SkilUID");
                ChainBoltPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireRing
        public static uint FireRingSkilUID = 0;
        public static double FireRingPlayers;
        public static void FireRingTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1150");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FireRingSkilUID = r.ReadUInt32("SkilUID");
                FireRingPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireMeteor
        public static uint FireMeteorSkilUID = 0;
        public static double FireMeteorPlayers;
        public static void FireMeteorTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1180");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FireMeteorSkilUID = r.ReadUInt32("SkilUID");
                FireMeteorPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireCircle
        public static uint FireCircleSkilUID = 0;
        public static double FireCirclePlayers;
        public static void FireCircleTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1120");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FireCircleSkilUID = r.ReadUInt32("SkilUID");
                FireCirclePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireofHell
        public static uint FireofHellSkilUID = 0;
        public static double FireofHellPlayers;
        public static void FireofHellTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1165");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                FireofHellSkilUID = r.ReadUInt32("SkilUID");
                FireofHellPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region HeavenBlade
        public static uint HeavenBladeSkilUID = 0;
        public static double HeavenBladePlayers;
        public static void HeavenBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "10310");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                HeavenBladeSkilUID = r.ReadUInt32("SkilUID");
                HeavenBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion

        #endregion
        #region Archer
        #region Scatter
        public static uint ScatterSkilUID = 0;
        public static double ScatterPlayers;
        public static void ScatterTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "8001");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ScatterSkilUID = r.ReadUInt32("SkilUID");
                ScatterPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region RapidFire
        public static uint RapidFireSkilUID = 0;
        public static double RapidFirePlayers;
        public static void RapidFireTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "8000");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                RapidFireSkilUID = r.ReadUInt32("SkilUID");
                RapidFirePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DaggerStorm
        public static uint DaggerStormSkilUID = 0;
        public static double DaggerStormPlayers;
        public static void DaggerStormTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11600");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                DaggerStormSkilUID = r.ReadUInt32("SkilUID");
                DaggerStormPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BladeFlurry
        public static uint BladeFlurrySkilUID = 0;
        public static double BladeFlurryPlayers;
        public static void BladeFlurryTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11610");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                BladeFlurrySkilUID = r.ReadUInt32("SkilUID");
                BladeFlurryPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BlisteringWave
        public static uint BlisteringWaveSkilUID = 0;
        public static double BlisteringWavePlayers;
        public static void BlisteringWaveTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11650");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                BlisteringWaveSkilUID = r.ReadUInt32("SkilUID");
                BlisteringWavePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region MortalWound
        public static uint MortalWoundSkilUID = 0;
        public static double MortalWoundPlayers;
        public static void MortalWoundTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "11660");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                MortalWoundSkilUID = r.ReadUInt32("SkilUID");
                MortalWoundPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Water
        #region Vulcano
        public static uint VulcanoSkilUID = 0;
        public static double VulcanoPlayers;
        public static void VulcanoTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1125");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                VulcanoSkilUID = r.ReadUInt32("SkilUID");
                VulcanoPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Lightning
        public static uint LightningSkilUID = 0;
        public static double LightningPlayers;
        public static void LightningTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "1010");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                LightningSkilUID = r.ReadUInt32("SkilUID");
                LightningPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SpeedLightning
        public static uint SpeedLightningSkilUID = 0;
        public static double SpeedLightningPlayers;
        public static void SpeedLightningTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "5001");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SpeedLightningSkilUID = r.ReadUInt32("SkilUID");
                SpeedLightningPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Prevade
        public static uint PrevadeSkilUID = 0;
        public static double PrevadePlayers;
        public static void PrevadeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "3090");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                PrevadeSkilUID = r.ReadUInt32("SkilUID");
                PrevadePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SearingTouch
        public static uint SearingTouchSkilUID = 0;
        public static double SearingTouchPlayers;
        public static void SearingTouchTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12400");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SearingTouchSkilUID = r.ReadUInt32("SkilUID");
                SearingTouchPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region DragonWarrer
        #region CrackingSwipe
        public static uint CrackingSwipeSkilUID = 0;
        public static double CrackingSwipePlayers;
        public static void CrackingSwipeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12160");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                CrackingSwipeSkilUID = r.ReadUInt32("SkilUID");
                CrackingSwipePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SplittingSwipe
        public static uint SplittingSwipeSkilUID = 0;
        public static double SplittingSwipePlayers;
        public static void SplittingSwipeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12170");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SplittingSwipeSkilUID = r.ReadUInt32("SkilUID");
                SplittingSwipePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SpeedKick
        public static uint SpeedKickSkilUID = 0;
        public static double SpeedKickPlayers;
        public static void SpeedKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12120");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SpeedKickSkilUID = r.ReadUInt32("SkilUID");
                SpeedKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ViolentKick
        public static uint ViolentKickSkilUID = 0;
        public static double ViolentKickPlayers;
        public static void ViolentKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12130");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ViolentKickSkilUID = r.ReadUInt32("SkilUID");
                ViolentKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region StormKick
        public static uint StormKickSkilUID = 0;
        public static double StormKickPlayers;
        public static void StormKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12140");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                StormKickSkilUID = r.ReadUInt32("SkilUID");
                StormKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonPunch
        public static uint DragonPunchSkilUID = 0;
        public static double DragonPunchPlayers;
        public static void DragonPunchTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12240");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                DragonPunchSkilUID = r.ReadUInt32("SkilUID");
                DragonPunchPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonCyclone
        public static uint DragonCycloneSkilUID = 0;
        public static double DragonCyclonePlayers;
        public static void DragonCycloneTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12290");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                DragonCycloneSkilUID = r.ReadUInt32("SkilUID");
                DragonCyclonePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonFury
        public static uint DragonFurySkilUID = 0;
        public static double DragonFuryPlayers;
        public static void DragonFuryTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12300");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                DragonFurySkilUID = r.ReadUInt32("SkilUID");
                DragonFuryPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region AirKick
        public static uint AirKickSkilUID = 0;
        public static double AirKickPlayers;
        public static void AirKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12320");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                AirKickSkilUID = r.ReadUInt32("SkilUID");
                AirKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region AirSweep
        public static uint AirSweepSkilUID = 0;
        public static double AirSweepPlayers;
        public static void AirSweepTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12330");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                AirSweepSkilUID = r.ReadUInt32("SkilUID");
                AirSweepPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonSlash
        public static uint DragonSlashSkilUID = 0;
        public static double DragonSlashPlayers;
        public static void DragonSlashTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12350");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                DragonSlashSkilUID = r.ReadUInt32("SkilUID");
                DragonSlashPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region WindWalker
        #region TripleBlasts
        public static uint TripleBlastsSkilUID = 0;
        public static double TripleBlastsPlayers;
        public static void TripleBlastsTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12850");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                TripleBlastsSkilUID = r.ReadUInt32("SkilUID");
                TripleBlastsPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Omnipotence
        public static uint OmnipotenceSkilUID = 0;
        public static double OmnipotencePlayers;
        public static void OmnipotenceTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12860");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                OmnipotenceSkilUID = r.ReadUInt32("SkilUID");
                OmnipotencePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SwirlingStorm
        public static uint SwirlingStormSkilUID = 0;
        public static double SwirlingStormPlayers;
        public static void SwirlingStormTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12890");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                SwirlingStormSkilUID = r.ReadUInt32("SkilUID");
                SwirlingStormPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region RageofWar
        public static uint RageofWarSkilUID = 0;
        public static double RageofWarPlayers;
        public static void RageofWarTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12930");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                RageofWarSkilUID = r.ReadUInt32("SkilUID");
                RageofWarPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BurntFrost
        public static uint BurntFrostSkilUID = 0;
        public static double BurntFrostPlayers;
        public static void BurntFrostTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12940");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                BurntFrostSkilUID = r.ReadUInt32("SkilUID");
                BurntFrostPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region AngerofStomper
        public static uint AngerofStomperSkilUID = 0;
        public static double AngerofStomperPlayers;
        public static void AngerofStomperTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12980");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                AngerofStomperSkilUID = r.ReadUInt32("SkilUID");
                AngerofStomperPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Thundercloud
        public static uint ThundercloudSkilUID = 0;
        public static double ThundercloudPlayers;
        public static void ThundercloudTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "12840");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ThundercloudSkilUID = r.ReadUInt32("SkilUID");
                ThundercloudPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ThundercloudAttack
        public static uint ThundercloudAttackSkilUID = 0;
        public static double ThundercloudAttackPlayers;
        public static void ThundercloudAttackTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack").Where("SkilUID", "13190");
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                ThundercloudAttackSkilUID = r.ReadUInt32("SkilUID");
                ThundercloudAttackPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
     
        #endregion
        public static void LoadAttack()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("panelattack"))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {

                    #region Ninja
                    TwofoldBladeTime();
                    CounterKillTime();
                    BloodyScytheTime();
                    ShurikenVTime();
                    GapingWoundsTime();
                    TwilightDanceTime();
                    SuperTwBladeTime();
                    FatalSpinTime();
                    #endregion
                    #region Monk
                    RadiantPalmTime();
                    WhirlwindKickTime();
                    TripleAttackTime();
                    OblivionTime();
                    WrathoftheEmperorTime();
                    StrikeTime();
                    InfernalEchoTime();
                    #endregion
                    #region Warrior
                    SpeedGunTime();
                    SnowTime();
                    BoomTime();
                    SeizerTime();
                    ViperFangTime();
                    PenetrationTime();
                    DragonTailTime();
                    CelestialTime();
                    ChargingVortexTime();
                    ScarofEarthTime();
                    WaveofBloodTime();
                    ManiacDanceTime();
                    #endregion
                    #region Pirate
                    BladeTempestTime();
                    GaleBombTime();
                    WindstormTime();
                    ScurvyBombTime();
                    CannonBarrageTime();
                    KrakensRevengeTime();
                    #endregion
                    #region Trojan
                    FastBladeTime();
                    ScentSwordTime();
                    PhoenixTime();
                    RageTime();
                    HerculesTime();
                    DragonWhirlTime();
                    FatalCrossTime();
                    MortalStrikeTime();
                    #endregion
                    #region Taoist fire
                    ThunderTime();
                    FireTime();
                    TornadoTime();
                    BombTime();
                    ChainBoltTime();
                    FireRingTime();
                    FireMeteorTime();
                    FireCircleTime();
                    FireofHellTime();
                    HeavenBladeTime();
                    #endregion
                    #region Archer
                    ScatterTime();
                    RapidFireTime();
                    DaggerStormTime();
                    BladeFlurryTime();
                    BlisteringWaveTime();
                    MortalWoundTime();
                    #endregion
                    #region Water
                    VulcanoTime();
                    LightningTime();
                    SpeedLightningTime();
                    PrevadeTime();
                    SearingTouchTime();
                    #endregion
                    #region DragonWarrer
                    CrackingSwipeTime();
                    SplittingSwipeTime();
                    SpeedKickTime();
                    ViolentKickTime();
                    StormKickTime();
                    DragonPunchTime();
                    DragonCycloneTime();
                    DragonFuryTime();
                    AirKickTime();
                    AirSweepTime();
                    DragonSlashTime();
                    #endregion
                    #region Wind Wailker
                    TripleBlastsTime();
                    OmnipotenceTime();
                    SwirlingStormTime();
                    RageofWarTime();
                    BurntFrostTime();
                    AngerofStomperTime();
                    ThundercloudTime();
                    ThundercloudAttackTime();
                    #endregion 
                    return;
                }
            }
           
        }

        public static void ExtraPanleDamge(Database.MagicType.Magic DBSpell, MsgSpellAnimation.SpellObj Spell)
        {
            switch (DBSpell.ID)
            {
                case 12890:
                    {
                        Spell.Damage = (uint)(Spell.Damage *SwirlingStormPlayers);
                        break;
                    }
                case 11600:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DaggerStormPlayers);
                        break;
                    }
                case 12930:
                    {
                        Spell.Damage = (uint)(Spell.Damage * RageofWarPlayers);
                        break;
                    }
                case 11610:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BladeFlurryPlayers);
                        break;
                    }
                case 11650:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BlisteringWavePlayers);
                        break;
                    }
                case 11660:
                    {
                        Spell.Damage = (uint)(Spell.Damage * MortalWoundPlayers);
                        break;
                    }
                case 12840:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ThundercloudPlayers);
                        break;
                    }
                case 12940:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BurntFrostPlayers);
                        break;
                    }
                case 12980:
                    {
                        Spell.Damage = (uint)(Spell.Damage * AngerofStomperPlayers);
                        break;
                    }
              
                case 13190:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ThundercloudAttackPlayers);
                        break;
                    }
                case 12850:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TripleBlastsPlayers);
                        break;
                    }
                case 12680:
                    {
                        Spell.Damage = (uint)(Spell.Damage * StrikePlayers);
                        break;
                    }

                case 11980:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FatalCrossPlayers);
                        break;
                    }
                case 11990:
                    {
                        Spell.Damage = (uint)(Spell.Damage * MortalStrikePlayers);
                        break;
                    }
              
                case 12400:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SearingTouchPlayers);
                        break;
                    }
                case 12550:
                    {
                        Spell.Damage = (uint)(Spell.Damage * InfernalEchoPlayers);
                        break;
                    }
                case 12570:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WrathoftheEmperorPlayers);
                        break;
                    }
                case 12600:
                    {
                        Spell.Damage = (uint)(Spell.Damage * StrikePlayers);
                        break;
                    }
                case 12670:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScarofEarthPlayers);
                        break;
                    }
                case 12690:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WaveofBloodPlayers);
                        break;
                    }
                case 12700:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ManiacDancePlayers);
                        break;
                    }
               
                case 12130:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ViolentKickPlayers);
                        break;
                    }
                case 12140:
                    {
                        Spell.Damage = (uint)(Spell.Damage * StormKickPlayers);
                        break;
                    }
                case 12240:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonPunchPlayers);
                        break;
                    }
                case 12290:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonCyclonePlayers);
                        break;
                    }
                case 12300:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonFuryPlayers);
                        break;
                    }
                case 12320:
                    {
                        Spell.Damage = (uint)(Spell.Damage * AirKickPlayers);
                        break;
                    }
                case 12330:
                    {
                        Spell.Damage = (uint)(Spell.Damage * AirSweepPlayers);
                        break;
                    }
                case 12350:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonSlashPlayers);
                        break;
                    }
                case 12070:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TwilightDancePlayers);
                        break;
                    }
                case 12080:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SuperTwBladePlayers);
                        break;
                    }
                case 12110:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FatalSpinPlayers);
                        break;
                    }
                case 12160:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CrackingSwipePlayers);
                        break;
                    }
                case 12170:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SplittingSwipePlayers);
                        break;
                    }
                case 12120:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SpeedKickPlayers);
                        break;
                    }
                case 11100:
                    {
                        Spell.Damage = (uint)(Spell.Damage * KrakensRevengePlayers);
                        break;
                    }
                case 11110:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BladeTempestPlayers);
                        break;
                    }
                case 11070:
                    {
                        Spell.Damage = (uint)(Spell.Damage * GaleBombPlayers);
                        break;
                    }
                case 11140:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WindstormPlayers);
                        break;
                    }
                case 11040:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScurvyBombPlayers);
                        break;
                    }
                case 11050:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CannonBarragePlayers);
                        break;
                    }
                case 5040:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BoomPlayers);
                        break;
                    }
                case 7000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SeizerPlayers);
                        break;
                    }
                case 11005:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ViperFangPlayers);
                        break;
                    }
                case 1290:
                    {
                        Spell.Damage = (uint)(Spell.Damage * PenetrationPlayers);
                        break;
                    }
                case 5030:
                    {
                        Spell.Damage = (uint)(Spell.Damage * PhoenixPlayers);
                        break;
                    }
                case 10315:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonWhirlPlayers);
                        break;
                    }
                case 10381:
                    {
                        Spell.Damage = (uint)(Spell.Damage * RadiantPalmPlayers);
                        break;
                    }
                case 1045:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FastBladePlayers);
                        break;
                    }
                case 1046:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScentSwordPlayers);
                        break;
                    }
                case 1115:
                    {
                        Spell.Damage = (uint)(Spell.Damage * HerculesPlayers);
                        break;
                    }
                case 7020:
                    {
                        Spell.Damage = (uint)(Spell.Damage * RagePlayers);
                        break;
                    }
                case 11000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonTailPlayers);
                        break;
                    }
                case 7030:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CelestialPlayers);
                        break;
                    }
                case 11190:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ChargingVortexPlayers);
                        break;
                    }
                case 10415:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WhirlwindKickPlayers);
                        break;
                    }
                case 10490:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TripleAttackPlayers);
                        break;
                    }
                case 10390:
                    {
                        Spell.Damage = (uint)(Spell.Damage * OblivionPlayers);
                        break;
                    }
                case 1260:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SpeedGunPlayers);
                        break;
                    }
                case 5010:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SnowPlayers);
                        break;
                    }
                case 1000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ThunderPlayers);
                        break;
                    }
                case 1001:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FirePlayers);
                        break;
                    }
                case 1125:
                    {
                        Spell.Damage = (uint)(Spell.Damage * VulcanoPlayers);
                        break;
                    }
                case 1010:
                    {
                        Spell.Damage = (uint)(Spell.Damage * LightningPlayers);
                        break;
                    }
                case 5001:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SpeedLightningPlayers);
                        break;
                    }
                case 3090:
                    {
                        Spell.Damage = (uint)(Spell.Damage * PrevadePlayers);
                        break;
                    }
                case 6000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TwofoldBladePlayers);
                        break;
                    }
                case 11170:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BloodyScythePlayers);
                        break;
                    }
                case 6003:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CounterKillPlayers);
                        break;
                    }
                case 6010:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ShurikenVPlayers);
                        break;
                    }
                case 11230:
                    {
                        Spell.Damage = (uint)(Spell.Damage * GapingWoundsPlayers);
                        break;
                    }
                case 1002:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TornadoPlayers);
                        break;
                    }
                case 1160:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BombPlayers);
                        break;
                    }
                case 10309:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ChainBoltPlayers);
                        break;
                    }
                case 1150:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireRingPlayers);
                        break;
                    }
                case 1180:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireMeteorPlayers);
                        break;
                    }
                case 1120:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireCirclePlayers);
                        break;
                    }
                case 1165:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireofHellPlayers);
                        break;
                    }
                case 10310:
                    {
                        Spell.Damage = (uint)(Spell.Damage * HeavenBladePlayers);
                        break;
                    }
                case 8001:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScatterPlayers);
                        break;
                    }
                case 8000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * RapidFirePlayers);
                        break;
                    }
            }
        }

        public static void Update(uint SkillUID, double Damge)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
            {
                cmd.Update("panelattack").Set("Damge", Damge)
                .Where("SkilUID", SkillUID);
                if (cmd.Execute2() > 0)
                {
                }
            }
        }

    }
}
