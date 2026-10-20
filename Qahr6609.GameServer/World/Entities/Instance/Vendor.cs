using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
   public class Vendor
    {
       public static Extensions.Counter VendorCounter = new Extensions.Counter(100000);

       public const byte MaxItems = 20;

       public class VendorItem
       {
           public Game.MsgServer.MsgItemView.ActionMode CostType;
           public Game.MsgServer.MsgGameItem DataItem;
           public long AmountCost;

          
       }
       public Client.GameClient Owner;
       public ConcurrentDictionary<uint, VendorItem> Items;
       public Game.MsgServer.MsgMessage HalkMeesaje = null;
       public SobNpc VendorNpc;
       public uint VendorUID;

       public bool InVending;

       public Vendor(Client.GameClient client)
       {
           Items = new ConcurrentDictionary<uint, VendorItem>();
           Owner = client;
       }
       public unsafe  void CreateVendor(ServerSockets.Packet stream)
       {
           if (InVending) return;

           VendorUID = VendorCounter.Next;

           VendorNpc = new SobNpc();
           VendorNpc.ObjType = MapObjectType.SobNpc;
           VendorNpc.OwnerVendor = Owner;
           VendorNpc.Name = Owner.Player.Name;
           VendorNpc.UID = VendorUID;
           VendorNpc.Mesh = SobNpc.StaticMesh.Vendor;
           VendorNpc.Type = Flags.NpcType.Booth;
           VendorNpc.Map = Owner.Player.Map;
           VendorNpc.X = (ushort)(Owner.Player.X + 1);
           VendorNpc.Y = Owner.Player.Y;

           Owner.Map.View.EnterMap<Role.IMapObj>(VendorNpc);

           foreach (var IObj in Owner.Player.View.Roles(MapObjectType.Player))
           {
               Role.Player screenObj = IObj as Role.Player;
               screenObj.View.CanAdd(VendorNpc,true,stream);
           }
           Owner.Player.Send(VendorNpc.GetArray(stream,false));
           InVending = true;
       }
       public unsafe void StopVending(ServerSockets.Packet stream)
       {
           if (InVending)
           {

               ActionQuery actione = new ActionQuery()
               {
                   ObjId = VendorUID,
                   Type = ActionType.RemoveEntity
               };
               Owner.Player.View.SendView(stream.ActionCreate(&actione), true);

               Items.Clear();
               Owner.Map.View.LeaveMap<Role.IMapObj>(VendorNpc);
               InVending = false;
               Owner.MyVendor = null;
           }
       }
       public bool AddItem(Game.MsgServer.MsgGameItem DataItem, Game.MsgServer.MsgItemView.ActionMode CostType, ulong Price)
       {
           // Any bound item (Bound != 0) can never be sold through a booth.
           if (DataItem.Bound != 0 || DataItem.Inscribed == 1 || DataItem.Locked != 0 || DataItem.ITEM_ID == 750000)
               return false;
           // Price is validated on the full 64-bit value against the limit of its currency.
           long Amout;
           long max;
           if (CostType == Game.MsgServer.MsgItemView.ActionMode.CPs)
               max = Conquer6609.Security.Guard.MaxCpsAmount;
           else if (CostType == Game.MsgServer.MsgItemView.ActionMode.Gold)
               max = Conquer6609.Security.Guard.MaxMoneyAmount;
           else
               return false;
           if (!Conquer6609.Security.Guard.ValidAmount(Price, max, out Amout))
           {
               Conquer6609.Security.Guard.Audit("vendor-price", Owner.Player.UID, Owner.Player.Name, "rejected price " + Price + " type " + CostType);
               return false;
           }

           if (Items.Count == MaxItems)
               return false;
           if (!Items.ContainsKey(DataItem.UID))
           {
               VendorItem VItem = new VendorItem();
               VItem.DataItem = DataItem;
               VItem.CostType = CostType;
               VItem.AmountCost = Amout;
               Items.TryAdd(DataItem.UID, VItem);
               return true;
           }
           return false;
       }
   }
}
