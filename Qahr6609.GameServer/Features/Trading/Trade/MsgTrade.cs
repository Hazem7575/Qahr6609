using Conquer6609.Game.Conquer6609_Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static class MsgTrade
    {
        public enum TradeID : uint
        {
            RequestNewTrade = 0x01,
            RequestCloseTrade = 0x02,
            RequestAddItemToTrade = 0x06,
            RequestAddMoneyToTrade = 0x07,
            RequestAddConquerPointsToTrade = 0x0D,
            RequestCompleteTrade = 0x0A,
            RemoveItem = 11,
            ShowTradeWindow = 0x03,
            CloseTradeWindow = 0x05,
            DisplayMoney = 0x08,
            DisplayConquerPoints = 0x0C
        }

        public static unsafe void GetTrade(this ServerSockets.Packet stream, out ulong dwParam, out TradeID ID)
        {
            dwParam = stream.ReadUInt64();
            ID = (TradeID)stream.ReadUInt16();
        }

        public static unsafe ServerSockets.Packet TradeCreate(this ServerSockets.Packet stream, ulong dwParam, TradeID ID)
        {
            stream.InitWriter();

            stream.Write(dwParam);//4
  
            stream.Write((ushort)ID);//8


            stream.Finalize(GamePackets.Trade);
            return stream;
        }
     
        [PacketAttribute(GamePackets.Trade)]
        private static void HandlerTrade(Client.GameClient user, ServerSockets.Packet stream)
        {
          
            if (!user.Player.IsCheckedPass)
                return;
            ulong dwParam;
            TradeID ID;
            stream.GetTrade(out dwParam, out ID);

            lock (Role.Instance.Trade.SyncRoot)
            {
            switch (ID)
            {

                case TradeID.RequestNewTrade:
                    {
                        // A player can only be in one open trade at a time.
                        if (user.InTrade)
                        {
                            user.SendSysMesage("You are already in a trade.");
                            break;
                        }
                        // an item offered in a booth could also be traded, and sold twice when both completed at once
                        if (user.IsVendor)
                        {
                            user.SendSysMesage("Close your booth before trading.");
                            break;
                        }
                        if (dwParam > uint.MaxValue || (uint)dwParam == user.Player.UID)
                            break;

                        if (user.MyTrade == null)
                            user.MyTrade = new Role.Instance.Trade(user);


                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue((uint)dwParam, out obj, Role.MapObjectType.Player))
                        {
                            Client.GameClient Partner = user.MyTrade.Target = (obj as Role.Player).Owner;
                          
                            if (user.MyTrade.Target != null && Partner != user)
                            {
                                if (!user.MyTrade.Target.InTrade)
                                {

                                    user.Player.targetTrade = (uint)dwParam;
                                    if (Partner.Player.targetTrade == user.Player.UID)
                                    {
                                        if (Partner.Player.UID == dwParam)
                                        {


                                            // Both sides start from a fresh, empty escrow that points at each other.
                                            user.MyTrade = new Role.Instance.Trade(user);
                                            user.MyTrade.Target = Partner;
                                            Partner.MyTrade = new Role.Instance.Trade(Partner);
                                            Partner.MyTrade.Target = user;


                                            Partner.MyTrade.Target = user;
                                            Partner.MyTrade.WindowOpen = true;
                                            user.MyTrade.WindowOpen = true;
                                            Partner.MyTrade.Confirmed = false;
                                            user.MyTrade.Confirmed = false;


                                            user.Send(stream.TradeCreate(dwParam, TradeID.ShowTradeWindow));
                                            Partner.Send(stream.TradeCreate(user.Player.UID, TradeID.ShowTradeWindow));
                                        }
                                        else
                                        {
                                            user.SendSysMesage("Player already in a trade.");

                                        }
                                    }
                                    else
                                    {
                                        if (Game.MsgServer.BlackList.Blocks(Partner.Player.UID, user.Player.Name))
                                            break;//on the partner's black list

                                        Partner.Send(stream.PopupInfoCreate(user.Player.UID, Partner.Player.UID, user.Player.Level, user.Player.BattlePower));


                                        Partner.Send(stream.TradeCreate(user.Player.UID, TradeID.RequestNewTrade));

                                    }
                                }
                                else
                                {
                                    user.SendSysMesage("Player already in a trade.");

                                }
                            }
                        }
                        break;
                    }
                case TradeID.RequestCloseTrade:
                    {
                        if (user.InTrade)
                        {
                            user.MyTrade.CloseTrade();
                        }
                        break;
                    }
                case TradeID.RequestAddItemToTrade:
                    {
                        if (user.InTrade && user.MyTrade.IsMutual())
                        {
                            Game.MsgServer.MsgGameItem DataItem;
                            if (user.Inventory.TryGetItem((uint)dwParam, out DataItem))
                                user.MyTrade.AddItem(stream, (uint)dwParam, DataItem);
                        }
                        break;
                    }
                case TradeID.RequestAddConquerPointsToTrade:
                    {
                        if (user.InTrade && user.MyTrade.IsMutual())
                            user.MyTrade.AddConquerPoints(dwParam, stream);
                        break;
                    }
                case TradeID.RequestAddMoneyToTrade:
                    {
                        if (user.InTrade && user.MyTrade.IsMutual())
                            user.MyTrade.AddMoney(dwParam, stream);
                        break;
                    }
                case TradeID.RequestCompleteTrade:
                    {
                        if (user.InTrade)
                        {
                            // Completion requires both sides to still be paired with each other.
                            if (user.MyTrade.IsMutual())
                            {
                                user.MyTrade.Confirmed = true;
                                if (!user.MyTrade.Target.MyTrade.Confirmed)
                                {

                                    user.MyTrade.Target.Send(stream.TradeCreate(dwParam, TradeID.RequestCompleteTrade));
                                }
                                else
                                {


                                    Client.GameClient Partner = user.MyTrade.Target;

                                    user.Player.targetTrade = 0;
                                    Partner.Player.targetTrade = 0;


                                    user.Send(stream.TradeCreate(dwParam, TradeID.CloseTradeWindow));
                                    Partner.Send(stream.TradeCreate(dwParam, TradeID.CloseTradeWindow));


                                    bool Acceped = false;
                                    // Received CPs must stay within the account CP limit on both sides.
                                    bool cpsFit = (long)user.Player.ConquerPoints + Partner.MyTrade.ConquerPoints <= int.MaxValue
                                        && (long)Partner.Player.ConquerPoints + user.MyTrade.ConquerPoints <= int.MaxValue;
                                    if (cpsFit && user.Inventory.HaveSpace((byte)Partner.MyTrade.Items.Count))
                                    {
                                        if (Partner.MyTrade.ValidItems())
                                        {
                                            if (Partner.Inventory.HaveSpace((byte)user.MyTrade.Items.Count))
                                            {
                                                if (user.MyTrade.ValidItems())
                                                {
                                                    //add money and conquerpoints ------------------
                                                    user.Player.ConquerPoints += Partner.MyTrade.ConquerPoints;
                                                    user.Player.Money += Partner.MyTrade.Money;

                                                    user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);

                                                    if (user.MyTrade.Target.MyTrade.ConquerPoints > 0)
                                                    {
                                                        string Logs1 = "[TradeConquerPoints]" + user.MyTrade.Target.Player.Name + " Trade " + user.MyTrade.Target.MyTrade.ConquerPoints + " Cps: To " + user.Player.Name + "";
                                                        Logs.OnDequeue(Logs1);
                                                    }
                                                    if (user.MyTrade.Target.MyTrade.Money > 0)
                                                    {
                                                        string Logs1 = "[TradeMoney]" + user.MyTrade.Target.Player.Name + " Trade " + user.MyTrade.Target.MyTrade.Money + "  Money  To " + user.Player.Name + "";
                                                        Logs.OnDequeue(Logs1);
                                                    }
                                                    Partner.Player.ConquerPoints += user.MyTrade.ConquerPoints;
                                                    Partner.Player.Money += user.MyTrade.Money;

                                                    Partner.Player.SendUpdate(stream, Partner.Player.Money, MsgUpdate.DataType.Money);
                                                    if (user.MyTrade.ConquerPoints > 0)
                                                    {
                                                        string Logs1 = "[TradeConquerPoints]" + user.Player.Name + " Trade " + user.MyTrade.Target.MyTrade.ConquerPoints + " Cps: To " + user.MyTrade.Target.Player.Name + "";
                                                        Logs.OnDequeue(Logs1);
                                                    }
                                                    if (user.MyTrade.Money > 0)
                                                    {
                                                        string Logs1 = "[TradeMoney]" + user.Player.Name + " Trade " + user.MyTrade.Money + "  Money  To " + user.MyTrade.Target.Player.Name + "";
                                                        Logs.OnDequeue(Logs1);
                                                    }
                                                   
                                                   
                                                    //-------------------------

                                                    //add items ------------------
                                                    foreach (var item in Partner.MyTrade.Items.Values)
                                                    {
                                                        user.Inventory.Update(item, Role.Instance.AddMode.MOVE, stream);
                                                        Partner.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                                                        string Logss = "[TradeItem]" + user.MyTrade.Target.Player.Name + " Trade ID: " + item.ITEM_ID + " UID: " + item.UID + " Item Plus: " + item.Plus + " To " + user.Player.Name + "";
                                                        Logs.OnDequeue(Logss);
                                                    }
                                                    foreach (var item in user.MyTrade.Items.Values)
                                                    {
                                                        Partner.Inventory.Update(item, Role.Instance.AddMode.MOVE, stream);
                                                        user.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                                                        string Logss = "[TradeItem]" + user.Player.Name + " Trade ID: " + item.ITEM_ID + " UID: " + item.UID + " Item Plus: " + item.Plus + " To " + user.MyTrade.Target.Player.Name + "";
                                                       Logs.OnDequeue(Logss);
                                                    }

                                                    user.MyTrade.ClearEscrow();
                                                    Partner.MyTrade.ClearEscrow();
                                                    user.MyTrade = null;
                                                    Partner.MyTrade = null;
                                                    Acceped = true;
                                                }
                                            }
                                        }
                                    }
                                    if (!Acceped)
                                    {
                                        user.SendSysMesage("There was an error with the trade", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                        Partner.SendSysMesage("There was an error with the trade", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                        // Failed completion: return each side's escrow.
                                        if (user.MyTrade != null)
                                            user.MyTrade.CloseTrade();
                                    }
                                }
                            }
                        }
                        break;
                    }

            }
            }
        }
    }
}
