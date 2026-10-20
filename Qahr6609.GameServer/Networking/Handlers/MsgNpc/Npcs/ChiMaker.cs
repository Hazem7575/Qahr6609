using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {
        /// <summary>Chi Maker NPC: sets one chi attribute (medium value) for 1 CP. Same dialogs as the Chi Token item.</summary>
        [NpcAttribute(NpcID.ChiMaker)]
        public static void ChiMaker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog a = new Dialog(client, stream);
            if (ChiDialog.ShowMenu(a, Option))
                return;
            ChiDialog.Pick pick;
            if (ChiDialog.Picks.TryGetValue(Option, out pick))
                ChiDialog.SetAttribute(client, stream, a, Option, pick, ChiDialog.MakerRules);
        }
    }

    /// <summary>
    /// Data and logic of the Chi Maker NPC and the Chi Token item (NpcHandler.ChiToken). These were two generated
    /// ~10k-line switch statements; the menus, option ids, texts and per-option quirks below were extracted from them
    /// one to one (option -> stage/attribute/field), so the dialogs behave exactly as before.
    /// </summary>
    public static class ChiDialog
    {
        public sealed class Menu
        {
            public string Text;
            public string[] Labels;
            public byte[] Options;
        }
        public struct Pick
        {
            /// <summary>MsgChiInfo.ChiPowerType name (Dragon, Phoenix, Tiger, Turtle).</summary>
            public string Power;
            /// <summary>Role.Instance.Chi.ChiAttributeType name.</summary>
            public string Attribute;
            /// <summary>Attribute slot 0-3.</summary>
            public int Field;
        }
        /// <summary>How one of the two dialogs charges and words things.</summary>
        public sealed class Rules
        {
            /// <summary>true = costs one Chi Token (727236) and gives the max value; false = costs 1 CP and gives the medium value.</summary>
            public bool UsesToken;
            /// <summary>Options whose "not enough" text is the alternative one (MissingTextAlt instead of MissingText).</summary>
            public HashSet<byte> AltMissingText;
            public string MissingText;
            public string MissingTextAlt;
            /// <summary>Options that take the CP before the duplicate check as well (taken twice on success) - kept as it was.</summary>
            public HashSet<byte> EarlyCharge;
            /// <summary>Options that do not refresh ranking/status/client after the change - kept as it was.</summary>
            public HashSet<byte> NoRefresh;
        }

        public const uint MakerCost = 1;
        public const uint ChiTokenItem = 727236;

        public static readonly Rules MakerRules = new Rules
        {
            UsesToken = false,
            MissingText = "You do not have Chi Token",
            MissingTextAlt = "You don't have " + MakerCost + " cps ",
            AltMissingText = new HashSet<byte> { 21, 33, 45, 57, 69, 81, 93, 105, 117, 129, 141, 153, 165, 180, 192, 204 },
            EarlyCharge = new HashSet<byte> { 24, 36, 48, 60, 72, 84, 96, 108, 120, 132, 144, 156, 168, 183, 195, 207 },
            NoRefresh = new HashSet<byte>(),
        };

        public static readonly Rules TokenRules = new Rules
        {
            UsesToken = true,
            MissingText = "You do not have Chi Token",
            MissingTextAlt = "You don't have Chi Token",
            AltMissingText = new HashSet<byte> { 21, 33, 45, 57, 69, 81, 93, 105, 117, 129, 141, 153, 165, 180, 192, 204 },
            EarlyCharge = new HashSet<byte>(),
            NoRefresh = new HashSet<byte> { 141 },
        };

        private static Menu M(string text, params object[] labelAndOption)
        {
            var m = new Menu { Text = text, Labels = new string[labelAndOption.Length / 2], Options = new byte[labelAndOption.Length / 2] };
            for (int i = 0; i < m.Labels.Length; i++)
            {
                m.Labels[i] = (string)labelAndOption[i * 2];
                m.Options[i] = Convert.ToByte(labelAndOption[i * 2 + 1]);
            }
            return m;
        }
        private static Pick P(string power, string attribute, int field)
        {
            return new Pick { Power = power, Attribute = attribute, Field = field };
        }

        /// <summary>Menu dialogs by option id (all end with "#Thnx ." 255 and avatar 63).</summary>
        public static readonly Dictionary<byte, Menu> Menus = new Dictionary<byte, Menu>
        {
            { 0, M("Select Stage", "Dragon", 1, "Phoenix", 2, "Tiger", 3, "Turtle", 4, "#Thnx .", 255) },
            { 1, M("Select Attribute", "Attribute 1", 5, "Attribute 2", 6, "Attribute 3", 7, "Attribute 4", 8, "#Thnx .", 255) },
            { 2, M("Select Attribute", "Attribute 1", 9, "Attribute 2", 10, "Attribute 3", 11, "Attribute 4", 12, "#Thnx .", 255) },
            { 3, M("Select Attribute", "Attribute 1", 13, "Attribute 2", 14, "Attribute 3", 15, "Attribute 4", 16, "#Thnx .", 255) },
            { 4, M("Select Attribute", "Attribute 1", 17, "Attribute 2", 18, "Attribute 3", 19, "Attribute 4", 20, "#Thnx .", 255) },
            { 5, M("Select Power Attribute 1 ", "CriticalStrike.", 21, "Immunity.", 216, "MaxLife.", 22, "Attack.", 23, "Breakthrough.", 24, "MagicAttack.", 25, "SkillCriticalStrike.", 26, "AntiBreak", 27, "MagicDefense.", 28, "FinalAttack.", 29, "FinalMagicAttack.", 30, "FinalDefense.", 31, "FinalMagicDefense.", 32, "#Thnx .", 255) },
            { 6, M("Select Power Attribute 2 ", "CriticalStrike.", 33, "Immunity.", 217, "MaxLife.", 34, "Attack.", 35, "Breakthrough.", 36, "MagicAttack.", 37, "SkillCriticalStrike.", 38, "AntiBreak", 39, "MagicDefense.", 40, "FinalAttack.", 41, "FinalMagicAttack.", 42, "FinalDefense.", 43, "FinalMagicDefense.", 44, "#Thnx .", 255) },
            { 7, M("Select Power Attribute 3 ", "CriticalStrike.", 45, "Immunity.", 218, "MaxLife.", 46, "Attack.", 47, "Breakthrough.", 48, "MagicAttack.", 49, "SkillCriticalStrike.", 50, "AntiBreak", 51, "MagicDefense.", 52, "FinalAttack.", 53, "FinalMagicAttack.", 54, "FinalDefense.", 55, "FinalMagicDefense.", 56, "#Thnx .", 255) },
            { 8, M("Select Power Attribute 4 ", "CriticalStrike.", 57, "Immunity.", 219, "MaxLife.", 58, "Attack.", 59, "Breakthrough.", 60, "MagicAttack.", 61, "SkillCriticalStrike.", 62, "AntiBreak", 63, "MagicDefense.", 64, "FinalAttack.", 65, "FinalMagicAttack.", 66, "FinalDefense.", 67, "FinalMagicDefense.", 68, "#Thnx .", 255) },
            { 9, M("Select Power Attribute 1 ", "CriticalStrike.", 69, "Immunity.", 220, "MaxLife.", 70, "Attack.", 71, "Breakthrough.", 72, "MagicAttack.", 73, "SkillCriticalStrike.", 74, "AntiBreak", 75, "MagicDefense.", 76, "FinalAttack.", 77, "FinalMagicAttack.", 78, "FinalDefense.", 79, "FinalMagicDefense.", 80, "#Thnx .", 255) },
            { 10, M("Select Power Attribute 2 ", "CriticalStrike.", 81, "Immunity.", 221, "MaxLife.", 82, "Attack.", 83, "Breakthrough.", 84, "MagicAttack.", 85, "SkillCriticalStrike.", 86, "AntiBreak", 87, "MagicDefense.", 88, "FinalAttack.", 89, "FinalMagicAttack.", 90, "FinalDefense.", 91, "FinalMagicDefense.", 92, "#Thnx .", 255) },
            { 11, M("Select Power Attribute 2 ", "CriticalStrike.", 93, "Immunity.", 222, "MaxLife.", 94, "Attack.", 95, "Breakthrough.", 96, "MagicAttack.", 97, "SkillCriticalStrike.", 98, "AntiBreak", 99, "MagicDefense.", 100, "FinalAttack.", 101, "FinalMagicAttack.", 102, "FinalDefense.", 103, "FinalMagicDefense.", 104, "#Thnx .", 255) },
            { 12, M("Select Power Attribute 2 ", "CriticalStrike.", 105, "Immunity.", 223, "MaxLife.", 106, "Attack.", 107, "Breakthrough.", 108, "MagicAttack.", 109, "SkillCriticalStrike.", 110, "AntiBreak", 111, "MagicDefense.", 112, "FinalAttack.", 113, "FinalMagicAttack.", 114, "FinalDefense.", 115, "FinalMagicDefense.", 116, "#Thnx .", 255) },
            { 13, M("Select Power Attribute 1 ", "CriticalStrike.", 117, "Immunity.", 224, "MaxLife.", 118, "Attack.", 119, "Breakthrough.", 120, "MagicAttack.", 121, "SkillCriticalStrike.", 122, "AntiBreak", 123, "MagicDefense.", 124, "FinalAttack.", 125, "FinalMagicAttack.", 126, "FinalDefense.", 127, "FinalMagicDefense.", 128, "#Thnx .", 255) },
            { 14, M("Select Power Attribute 2 ", "CriticalStrike.", 129, "Immunity.", 225, "MaxLife.", 130, "Attack.", 131, "Breakthrough.", 132, "MagicAttack.", 133, "SkillCriticalStrike.", 134, "AntiBreak", 135, "MagicDefense.", 136, "FinalAttack.", 137, "FinalMagicAttack.", 138, "FinalDefense.", 139, "FinalMagicDefense.", 140, "#Thnx .", 255) },
            { 15, M("Select Power Attribute 2 ", "CriticalStrike.", 141, "Immunity.", 226, "MaxLife.", 142, "Attack.", 143, "Breakthrough.", 144, "MagicAttack.", 145, "SkillCriticalStrike.", 146, "AntiBreak", 147, "MagicDefense.", 148, "FinalAttack.", 149, "FinalMagicAttack.", 150, "FinalDefense.", 151, "FinalMagicDefense.", 152, "#Thnx .", 255) },
            { 16, M("Select Power Attribute 2 ", "CriticalStrike.", 153, "Immunity.", 227, "MaxLife.", 154, "Attack.", 155, "Breakthrough.", 156, "MagicAttack.", 157, "SkillCriticalStrike.", 158, "AntiBreak", 159, "MagicDefense.", 160, "FinalAttack.", 161, "FinalMagicAttack.", 162, "FinalDefense.", 163, "FinalMagicDefense.", 164, "#Thnx .", 255) },
            { 17, M("Select Power Attribute 1 ", "CriticalStrike.", 165, "Immunity.", 228, "MaxLife.", 166, "Attack.", 167, "Breakthrough.", 168, "MagicAttack.", 169, "SkillCriticalStrike.", 170, "AntiBreak", 171, "MagicDefense.", 172, "FinalAttack.", 173, "FinalMagicAttack.", 174, "FinalDefense.", 175, "FinalMagicDefense.", 176, "#Thnx .", 255) },
            { 18, M("Select Power Attribute 2 ", "CriticalStrike.", 180, "Immunity.", 229, "MaxLife.", 181, "Attack.", 182, "Breakthrough.", 183, "MagicAttack.", 184, "SkillCriticalStrike.", 185, "AntiBreak", 186, "MagicDefense.", 187, "FinalAttack.", 188, "FinalMagicAttack.", 189, "FinalDefense.", 190, "FinalMagicDefense.", 191, "#Thnx .", 255) },
            { 19, M("Select Power Attribute 2 ", "CriticalStrike.", 192, "Immunity.", 230, "MaxLife.", 193, "Attack.", 194, "Breakthrough.", 195, "MagicAttack.", 196, "SkillCriticalStrike.", 197, "AntiBreak", 198, "MagicDefense.", 199, "FinalAttack.", 200, "FinalMagicAttack.", 201, "FinalDefense.", 202, "FinalMagicDefense.", 203, "#Thnx .", 255) },
            { 20, M("Select Power Attribute 2 ", "CriticalStrike.", 204, "Immunity.", 231, "MaxLife.", 205, "Attack.", 206, "Breakthrough.", 207, "MagicAttack.", 208, "SkillCriticalStrike.", 209, "AntiBreak", 210, "MagicDefense.", 211, "FinalAttack.", 212, "FinalMagicAttack.", 213, "FinalDefense.", 214, "FinalMagicDefense.", 215, "#Thnx .", 255) },
        };

        /// <summary>Attribute choices by option id.</summary>
        public static readonly Dictionary<byte, Pick> Picks = new Dictionary<byte, Pick>
        {
            { 21, P("Dragon", "CriticalStrike", 0) },
            { 216, P("Dragon", "Immunity", 0) },
            { 22, P("Dragon", "MaxLife", 0) },
            { 23, P("Dragon", "AddAttack", 0) },
            { 24, P("Dragon", "Breakthrough", 0) },
            { 25, P("Dragon", "AddMagicAttack", 0) },
            { 26, P("Dragon", "SkillCriticalStrike", 0) },
            { 27, P("Dragon", "Counteraction", 0) },
            { 28, P("Dragon", "MagicDefense", 0) },
            { 29, P("Dragon", "FinalAttack", 0) },
            { 30, P("Dragon", "FinalMagicAttack", 0) },
            { 31, P("Dragon", "FinalDefense", 0) },
            { 32, P("Dragon", "FinalMagicDefense", 0) },
            { 33, P("Dragon", "CriticalStrike", 1) },
            { 217, P("Dragon", "Immunity", 1) },
            { 34, P("Dragon", "MaxLife", 1) },
            { 35, P("Dragon", "AddAttack", 1) },
            { 36, P("Dragon", "Breakthrough", 1) },
            { 37, P("Dragon", "AddMagicAttack", 1) },
            { 38, P("Dragon", "SkillCriticalStrike", 1) },
            { 39, P("Dragon", "Counteraction", 1) },
            { 40, P("Dragon", "MagicDefense", 1) },
            { 41, P("Dragon", "FinalAttack", 1) },
            { 42, P("Dragon", "FinalMagicAttack", 1) },
            { 43, P("Dragon", "FinalDefense", 1) },
            { 44, P("Dragon", "FinalMagicDefense", 1) },
            { 45, P("Dragon", "CriticalStrike", 2) },
            { 218, P("Dragon", "Immunity", 2) },
            { 46, P("Dragon", "MaxLife", 2) },
            { 47, P("Dragon", "AddAttack", 2) },
            { 48, P("Dragon", "Breakthrough", 2) },
            { 49, P("Dragon", "AddMagicAttack", 2) },
            { 50, P("Dragon", "SkillCriticalStrike", 2) },
            { 51, P("Dragon", "Counteraction", 2) },
            { 52, P("Dragon", "MagicDefense", 2) },
            { 53, P("Dragon", "FinalAttack", 2) },
            { 54, P("Dragon", "FinalMagicAttack", 2) },
            { 55, P("Dragon", "FinalDefense", 2) },
            { 56, P("Dragon", "FinalMagicDefense", 2) },
            { 57, P("Dragon", "CriticalStrike", 3) },
            { 219, P("Dragon", "Immunity", 3) },
            { 58, P("Dragon", "MaxLife", 3) },
            { 59, P("Dragon", "AddAttack", 3) },
            { 60, P("Dragon", "Breakthrough", 3) },
            { 61, P("Dragon", "AddMagicAttack", 3) },
            { 62, P("Dragon", "SkillCriticalStrike", 3) },
            { 63, P("Dragon", "Counteraction", 3) },
            { 64, P("Dragon", "MagicDefense", 3) },
            { 65, P("Dragon", "FinalAttack", 3) },
            { 66, P("Dragon", "FinalMagicAttack", 3) },
            { 67, P("Dragon", "FinalDefense", 3) },
            { 68, P("Dragon", "FinalMagicDefense", 3) },
            { 69, P("Phoenix", "CriticalStrike", 0) },
            { 220, P("Phoenix", "Immunity", 0) },
            { 70, P("Phoenix", "MaxLife", 0) },
            { 71, P("Phoenix", "AddAttack", 0) },
            { 72, P("Phoenix", "Breakthrough", 0) },
            { 73, P("Phoenix", "AddMagicAttack", 0) },
            { 74, P("Phoenix", "SkillCriticalStrike", 0) },
            { 75, P("Phoenix", "Counteraction", 0) },
            { 76, P("Phoenix", "MagicDefense", 0) },
            { 77, P("Phoenix", "FinalAttack", 0) },
            { 78, P("Phoenix", "FinalMagicAttack", 0) },
            { 79, P("Phoenix", "FinalDefense", 0) },
            { 80, P("Phoenix", "FinalMagicDefense", 0) },
            { 81, P("Phoenix", "CriticalStrike", 1) },
            { 221, P("Phoenix", "Immunity", 1) },
            { 82, P("Phoenix", "MaxLife", 1) },
            { 83, P("Phoenix", "AddAttack", 1) },
            { 84, P("Phoenix", "Breakthrough", 1) },
            { 85, P("Phoenix", "AddMagicAttack", 1) },
            { 86, P("Phoenix", "SkillCriticalStrike", 1) },
            { 87, P("Phoenix", "Counteraction", 1) },
            { 88, P("Phoenix", "MagicDefense", 1) },
            { 89, P("Phoenix", "FinalAttack", 1) },
            { 90, P("Phoenix", "FinalMagicAttack", 1) },
            { 91, P("Phoenix", "FinalDefense", 1) },
            { 92, P("Phoenix", "FinalMagicDefense", 1) },
            { 93, P("Phoenix", "CriticalStrike", 2) },
            { 222, P("Phoenix", "Immunity", 2) },
            { 94, P("Phoenix", "MaxLife", 2) },
            { 95, P("Phoenix", "AddAttack", 2) },
            { 96, P("Phoenix", "Breakthrough", 2) },
            { 97, P("Phoenix", "AddMagicAttack", 2) },
            { 98, P("Phoenix", "SkillCriticalStrike", 2) },
            { 99, P("Phoenix", "Counteraction", 2) },
            { 100, P("Phoenix", "MagicDefense", 2) },
            { 101, P("Phoenix", "FinalAttack", 2) },
            { 102, P("Phoenix", "FinalMagicAttack", 2) },
            { 103, P("Phoenix", "FinalDefense", 2) },
            { 104, P("Phoenix", "FinalMagicDefense", 2) },
            { 105, P("Phoenix", "CriticalStrike", 3) },
            { 223, P("Phoenix", "Immunity", 3) },
            { 106, P("Phoenix", "MaxLife", 3) },
            { 107, P("Phoenix", "AddAttack", 3) },
            { 108, P("Phoenix", "Breakthrough", 3) },
            { 109, P("Phoenix", "AddMagicAttack", 3) },
            { 110, P("Phoenix", "SkillCriticalStrike", 3) },
            { 111, P("Phoenix", "Counteraction", 3) },
            { 112, P("Phoenix", "MagicDefense", 3) },
            { 113, P("Phoenix", "FinalAttack", 3) },
            { 114, P("Phoenix", "FinalMagicAttack", 3) },
            { 115, P("Phoenix", "FinalDefense", 3) },
            { 116, P("Phoenix", "FinalMagicDefense", 3) },
            { 117, P("Tiger", "CriticalStrike", 0) },
            { 224, P("Tiger", "Immunity", 0) },
            { 118, P("Tiger", "MaxLife", 0) },
            { 119, P("Tiger", "AddAttack", 0) },
            { 120, P("Tiger", "Breakthrough", 0) },
            { 121, P("Tiger", "AddMagicAttack", 0) },
            { 122, P("Tiger", "SkillCriticalStrike", 0) },
            { 123, P("Tiger", "Counteraction", 0) },
            { 124, P("Tiger", "MagicDefense", 0) },
            { 125, P("Tiger", "FinalAttack", 0) },
            { 126, P("Tiger", "FinalMagicAttack", 0) },
            { 127, P("Tiger", "FinalDefense", 0) },
            { 128, P("Tiger", "FinalMagicDefense", 0) },
            { 129, P("Tiger", "CriticalStrike", 1) },
            { 225, P("Tiger", "Immunity", 1) },
            { 130, P("Tiger", "MaxLife", 1) },
            { 131, P("Tiger", "AddAttack", 1) },
            { 132, P("Tiger", "Breakthrough", 1) },
            { 133, P("Tiger", "AddMagicAttack", 1) },
            { 134, P("Tiger", "SkillCriticalStrike", 1) },
            { 135, P("Tiger", "Counteraction", 1) },
            { 136, P("Tiger", "MagicDefense", 1) },
            { 137, P("Tiger", "FinalAttack", 1) },
            { 138, P("Tiger", "FinalMagicAttack", 1) },
            { 139, P("Tiger", "FinalDefense", 1) },
            { 140, P("Tiger", "FinalMagicDefense", 1) },
            { 141, P("Tiger", "CriticalStrike", 2) },
            { 226, P("Tiger", "Immunity", 2) },
            { 142, P("Tiger", "MaxLife", 2) },
            { 143, P("Tiger", "AddAttack", 2) },
            { 144, P("Tiger", "Breakthrough", 2) },
            { 145, P("Tiger", "AddMagicAttack", 2) },
            { 146, P("Tiger", "SkillCriticalStrike", 2) },
            { 147, P("Tiger", "Counteraction", 2) },
            { 148, P("Tiger", "MagicDefense", 2) },
            { 149, P("Tiger", "FinalAttack", 2) },
            { 150, P("Tiger", "FinalMagicAttack", 2) },
            { 151, P("Tiger", "FinalDefense", 2) },
            { 152, P("Tiger", "FinalMagicDefense", 2) },
            { 153, P("Tiger", "CriticalStrike", 3) },
            { 227, P("Tiger", "Immunity", 3) },
            { 154, P("Tiger", "MaxLife", 3) },
            { 155, P("Tiger", "AddAttack", 3) },
            { 156, P("Tiger", "Breakthrough", 3) },
            { 157, P("Tiger", "AddMagicAttack", 3) },
            { 158, P("Tiger", "SkillCriticalStrike", 3) },
            { 159, P("Tiger", "Counteraction", 3) },
            { 160, P("Tiger", "MagicDefense", 3) },
            { 161, P("Tiger", "FinalAttack", 3) },
            { 162, P("Tiger", "FinalMagicAttack", 3) },
            { 163, P("Tiger", "FinalDefense", 3) },
            { 164, P("Tiger", "FinalMagicDefense", 3) },
            { 165, P("Turtle", "CriticalStrike", 0) },
            { 228, P("Turtle", "Immunity", 0) },
            { 166, P("Turtle", "MaxLife", 0) },
            { 167, P("Turtle", "AddAttack", 0) },
            { 168, P("Turtle", "Breakthrough", 0) },
            { 169, P("Turtle", "AddMagicAttack", 0) },
            { 170, P("Turtle", "SkillCriticalStrike", 0) },
            { 171, P("Turtle", "Counteraction", 0) },
            { 172, P("Turtle", "MagicDefense", 0) },
            { 173, P("Turtle", "FinalAttack", 0) },
            { 174, P("Turtle", "FinalMagicAttack", 0) },
            { 175, P("Turtle", "FinalDefense", 0) },
            { 176, P("Turtle", "FinalMagicDefense", 0) },
            { 180, P("Turtle", "CriticalStrike", 1) },
            { 229, P("Turtle", "Immunity", 1) },
            { 181, P("Turtle", "MaxLife", 1) },
            { 182, P("Turtle", "AddAttack", 1) },
            { 183, P("Turtle", "Breakthrough", 1) },
            { 184, P("Turtle", "AddMagicAttack", 1) },
            { 185, P("Turtle", "SkillCriticalStrike", 1) },
            { 186, P("Turtle", "Counteraction", 1) },
            { 187, P("Turtle", "MagicDefense", 1) },
            { 188, P("Turtle", "FinalAttack", 1) },
            { 189, P("Turtle", "FinalMagicAttack", 1) },
            { 190, P("Turtle", "FinalDefense", 1) },
            { 191, P("Turtle", "FinalMagicDefense", 1) },
            { 192, P("Turtle", "CriticalStrike", 2) },
            { 230, P("Turtle", "Immunity", 2) },
            { 193, P("Turtle", "MaxLife", 2) },
            { 194, P("Turtle", "AddAttack", 2) },
            { 195, P("Turtle", "Breakthrough", 2) },
            { 196, P("Turtle", "AddMagicAttack", 2) },
            { 197, P("Turtle", "SkillCriticalStrike", 2) },
            { 198, P("Turtle", "Counteraction", 2) },
            { 199, P("Turtle", "MagicDefense", 2) },
            { 200, P("Turtle", "FinalAttack", 2) },
            { 201, P("Turtle", "FinalMagicAttack", 2) },
            { 202, P("Turtle", "FinalDefense", 2) },
            { 203, P("Turtle", "FinalMagicDefense", 2) },
            { 204, P("Turtle", "CriticalStrike", 3) },
            { 231, P("Turtle", "Immunity", 3) },
            { 205, P("Turtle", "MaxLife", 3) },
            { 206, P("Turtle", "AddAttack", 3) },
            { 207, P("Turtle", "Breakthrough", 3) },
            { 208, P("Turtle", "AddMagicAttack", 3) },
            { 209, P("Turtle", "SkillCriticalStrike", 3) },
            { 210, P("Turtle", "Counteraction", 3) },
            { 211, P("Turtle", "MagicDefense", 3) },
            { 212, P("Turtle", "FinalAttack", 3) },
            { 213, P("Turtle", "FinalMagicAttack", 3) },
            { 214, P("Turtle", "FinalDefense", 3) },
            { 215, P("Turtle", "FinalMagicDefense", 3) },
        };

        public static bool ShowMenu(Dialog a, byte option)
        {
            Menu menu;
            if (!Menus.TryGetValue(option, out menu))
                return false;
            a.AddText(menu.Text);
            for (int i = 0; i < menu.Labels.Length; i++)
                a.AddOption(menu.Labels[i], menu.Options[i]);
            a.AddAvatar(63).FinalizeDialog();
            return true;
        }

        public static void SetAttribute(Client.GameClient client, ServerSockets.Packet stream, Dialog a, byte option, Pick pick, Rules rules)
        {
            uint ValeCps = MakerCost;
            if (rules.UsesToken ? client.Inventory.Contain(ChiTokenItem, 1) : client.Player.ConquerPoints >= ValeCps)
            {
                if (rules.EarlyCharge.Contains(option))
                    client.Player.ConquerPoints -= ValeCps;
                var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), pick.Power);
                var Power = client.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                var Attribute = (Role.Instance.Chi.ChiAttributeType)Enum.Parse(typeof(Role.Instance.Chi.ChiAttributeType), pick.Attribute);
                var Value = rules.UsesToken ? Role.Instance.Chi.MaxPower(Attribute) : Role.Instance.Chi.MedPower(Attribute);
                string FieldsName = pick.Attribute;
                string Text1 = Power.Fields[0].Item1.ToString();
                string Text2 = Power.Fields[1].Item1.ToString();
                string Text3 = Power.Fields[2].Item1.ToString();
                string Text4 = Power.Fields[3].Item1.ToString();
                if (Text1 == FieldsName || Text2 == FieldsName || Text3 == FieldsName || Text4 == FieldsName)
                {
                    a.AddText("Sorry you can't have multiple attributes")
                    .AddOption("My~bad~sorry.", 255)
                    .AddAvatar(63).FinalizeDialog();
                    return;
                }
                if (rules.UsesToken && !client.Inventory.Remove(ChiTokenItem, 1, stream))
                    return;
                Power.Fields[pick.Field] = Tuple.Create((Role.Instance.Chi.ChiAttributeType)Attribute, (int)((byte)Attribute * 10000 + Value));
                if (!rules.UsesToken)
                    client.Player.ConquerPoints -= ValeCps;
                if (rules.NoRefresh.Contains(option))
                    return;

                if (PowerType == MsgChiInfo.ChiPowerType.Dragon)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, Power);
                else if (PowerType == MsgChiInfo.ChiPowerType.Phoenix)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, Power);
                else if (PowerType == MsgChiInfo.ChiPowerType.Tiger)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, Power);
                else if (PowerType == MsgChiInfo.ChiPowerType.Turtle)
                    Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, Power);

                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Send);
                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade);
            }
            else
            {
                a.AddText(rules.AltMissingText.Contains(option) ? rules.MissingTextAlt : rules.MissingText)
                .AddOption("Let~me~think.", 255)
                .AddAvatar(63).FinalizeDialog();
            }
        }
    }
}
