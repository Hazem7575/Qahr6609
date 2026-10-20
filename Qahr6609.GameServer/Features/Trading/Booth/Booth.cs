using Conquer6609.Game.MsgServer;
using Conquer6609.Role;
using System;
using System.Collections.Generic;

namespace Conquer6609.Game
{
    public struct BoothItem
    {
        public MsgGameItem Item;
        public uint Cost;
        public MsgItemView.ActionMode Cost_Type;
    }
    public unsafe class Booth
    {
        public static Counter BoothCounter = new Counter(1) { Finish = 10000 };
        public static Dictionary<uint, Booth> Booths = new Dictionary<uint, Booth>();
        /// <summary>Guards <see cref="Booths"/> (player booths are added from packet handlers, the static ones by Booths.CreateBooths).</summary>
        public static readonly object SyncRoot = new Object();
        public Extensions.SafeDictionary<uint, BoothItem> ItemList;
        Client.GameClient Owner;
        public SobNpc Base;
        public Booth(Client.GameClient client, ServerSockets.Packet stream)
        {
            Owner = client;
            Owner.Player.Action = Role.Flags.ConquerAction.Sit;
            ItemList = new Extensions.SafeDictionary<uint, BoothItem>(20);
            Base = new SobNpc();
            lock (SyncRoot)
            {
                Base.UID = BoothCounter.Next;
                while (Booths.ContainsKey(Base.UID))
                    Base.UID = BoothCounter.Next;
                Booths.Add(Base.UID, this);
            }
            Base.Mesh = (SobNpc.StaticMesh)406;
            Base.Type = Flags.NpcType.Booth;
            Base.Name = Name;
            Base.Booth = this;
            Base.Map = client.Player.Map;
            Base.X = (ushort)(Owner.Player.X + 1);
            Base.Y = Owner.Player.Y;
            Owner.Map.View.EnterMap<Role.IMapObj>(Base);
            foreach (var IObj in Owner.Player.View.Roles(MapObjectType.Player))
            {
                Role.Player screenObj = IObj as Role.Player;
                screenObj.View.CanAdd(Base, true, stream);
            }
            Owner.Player.Send(Base.GetArray(stream, false));
            ActionQuery action = new ActionQuery()
            {
                dwParam = Base.UID,
                wParam1 = Base.X,
                wParam2 = Base.Y,
                Type = ActionType.StartVendor
            };
            Owner.Send(stream.ActionCreate(&action));
        }
        public Booth()
        {
            ItemList = new Extensions.SafeDictionary<uint, BoothItem>(20);
        }
        public string Name
        {
            get { return Owner.Player.Name; }
        }
        public static implicit operator SobNpc(Booth booth)
        {
            return booth.Base;
        }
    }
}