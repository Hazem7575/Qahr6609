// OperatingActs module: the "Events" tab of the Welfare window (client ini\OperateActivity.ini, packets 1315/1316).
// Layouts from the 6609 client code (OperatingActs.cs). Activities 273-281 run per calendar month; prizes are the
// tiers of ServerDataBase\operating_prize.txt; claims/counters per player in <DataPath>\players\<uid>.txt.
// GM: @opact (re-send the list) | @opact points <273-276> <n> | @opact reset
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.OperatingActs
{
    public sealed class OperatingActsModule : ModuleBase
    {
        internal static OperatingActsModule Instance;
        public override string Name { get { return "OperatingActs"; } }

        public override void OnLoad()
        {
            Instance = this;
            OperatingActs.Load(DataPath);
            Log(OperatingActs.LoadInfo);
        }

        public override void OnLogin(Client.GameClient client) { OperatingActs.OnLogin(client); }
        public override void OnLogout(Client.GameClient client) { OperatingActs.OnLogout(client); }

        [GmCommand("opact", Role = CommandRole.GM, Help = "Events tab: @opact | points <273-276> <n> | reset", Usage = "@opact points 274 500")]
        static void OpactCommand(Client.GameClient client, string[] args) { OperatingActs.Gm(client, args); }
    }
}
