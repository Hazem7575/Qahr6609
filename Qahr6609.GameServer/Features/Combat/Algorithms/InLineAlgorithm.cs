using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.Algoritms
{
    public class InLineAlgorithm
    {
        public struct coords
        {
            public int X;
            public int Y;

            public coords(double x, double y)
            {
                this.X = (int)x;
                this.Y = (int)y;
            }
        }
      
        bool Contains(List<coords> Coords, coords Check)
        {
            foreach (coords Coord in Coords)
                if (Coord.X == Check.X && Check.Y == Coord.Y)
                    return true;
            return false;
        }
        List<coords> LineCoords(ushort userx, ushort usery, ushort shotx, ushort shoty)
        {
            return linedda(userx, usery, shotx, shoty);
        }
        void Add(List<coords> Coords, int x, int y)
        {
            coords add = new coords((ushort)x, (ushort)y);
            if (!Coords.Contains(add))
                Coords.Add(add);
        }
        List<coords> linedda(int xa, int ya, int xb, int yb)
        {
            int dx = xb - xa, dy = yb - ya, steps, k;
            float xincrement, yincrement, x = xa, y = ya;

            if (Math.Abs(dx) > Math.Abs(dy))
                steps = Math.Abs(dx);
            else
                steps = Math.Abs(dy);

            xincrement = dx / (float)steps;
            yincrement = dy / (float)steps;
            List<coords> ThisLine = new List<coords>();
            if (Program.ProtectMapSpells.Contains(map.ID) && SpellID != 1045 && SpellID != 1046
                    && SpellID != 11980 && SpellID != 1260 && SpellID != 11000 && SpellID != 12350 && SpellID != 11005)
            {
                if (!map.IsFlagPresent((ushort)Math.Round(x), (ushort)Math.Round(y), Role.MapFlagType.Valid))
                    return ThisLine;
            }
            ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));

            for (k = 0; k < MaxDistance; k++)
            {
                x += xincrement;
                y += yincrement; 
                if (Program.ProtectMapSpells.Contains(map.ID) && SpellID != 1045 && SpellID != 1046
                    && SpellID != 11980 && SpellID != 1260 && SpellID != 11000 && SpellID != 12350 && SpellID != 11005)
                {
                    if (!map.IsFlagPresent((ushort)Math.Round(x), (ushort)Math.Round(y), Role.MapFlagType.Valid))
                        return ThisLine;
                }
                ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));
            }
            return ThisLine;
        }
        public List<coords> lcoords;
        public byte MaxDistance = 10;
        private Role.GameMap map;
        private ushort SpellID = 0;
        public InLineAlgorithm(ushort X1, ushort X2, ushort Y1, ushort Y2, Role.GameMap _map, byte MaxDistance, byte MaxRange, ushort spelldid = 0)
        {
            map = _map;

            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;

            this.MaxDistance = MaxDistance;
            SpellID = spelldid;
            lcoords = LineCoords(X1, Y1, X2, Y2);
       
        }
        public ushort X1 { get; set; }
        public ushort Y1 { get; set; }
        public ushort X2 { get; set; }
        public ushort Y2 { get; set; }
        public byte Direction { get; set; }

        public bool InLine(ushort X, ushort Y,byte Range, bool IncreaseRange = false)
        {
     
                int mydst = GetDistance((ushort)X1, (ushort)Y1, X, Y);

                if (mydst <= MaxDistance)
                {
                    if (Range == 0)
                    {
                        return Contains(lcoords, new coords(X, Y));
                    }
                    else
                        return InRange(X, Y, Range, lcoords, IncreaseRange);
                }
          
            return false;
        }
        public bool GetNewCoords(ref ushort X, ref ushort Y)
        {
            if (lcoords.Count > 0)
            {
                var coord = lcoords.Last();

                X = (ushort)coord.X;
                Y = (ushort)coord.Y;

                return true;
            }
         
            return false;
        }
        public bool InRange(ushort X, ushort Y, byte Range, List<coords> bas, bool IncreaseRange = false)
        {
            if (IncreaseRange)
            {
                uint _Range = Range;
                foreach (coords line in bas)
                {
                    byte distance = (byte)GetDistance((ushort)X, (ushort)Y, (ushort)line.X, (ushort)line.Y);
                    if (distance <= _Range)
                        return true;
                    if (_Range < 8)
                        _Range++;
                }
            }
            else
            {
                foreach (coords line in bas)
                {
                    byte distance = (byte)GetDistance((ushort)X, (ushort)Y, (ushort)line.X, (ushort)line.Y);
                    if (distance <= Range)
                        return true;
                }
            }
            return false;
        }

        public static Role.Flags.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);

            byte Dir = (byte)((7 - (Math.Floor(direction) / 45 % 8)) - 1 % 8);
            return (Role.Flags.ConquerAngle)(byte)((int)Dir % 8);
        }
        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            short x = 0;
            short y = 0;
            if (X >= X2)
            {
                x = (short)(X - X2);
            }
            else if (X2 >= X)
            {
                x = (short)(X2 - X);
            }
            if (Y >= Y2)
            {
                y = (short)(Y - Y2);
            }
            else if (Y2 >= Y)
            {
                y = (short)(Y2 - Y);
            }
            if (x > y)
                return x;
            else
                return y;
        }
    }
}
