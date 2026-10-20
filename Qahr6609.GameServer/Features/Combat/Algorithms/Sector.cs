using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.Algoritms
{
    public class Sector
    {
        private int attackerX, attackerY, attackX, attackY;
        private int degree, sectorsize, leftside, rightside;
        private int distance;
        private bool addextra;

        public Sector(int attackerX, int attackerY, int attackX, int attackY)
        {
            this.attackerX = attackerX;
            this.attackerY = attackerY;
            this.attackX = attackX;
            this.attackY = attackY;
            this.degree = (int)Role.Core.GetAngle((ushort)attackerX, (ushort)attackX, (ushort)attackerY, (ushort)attackY);
            this.addextra = false;
        }

        public void Arrange(int sectorsize, int distance)
        {
            this.distance = Math.Min(distance, 14);
            this.sectorsize = sectorsize;
            this.leftside = this.degree - (sectorsize / 2);
            if (this.leftside < 0)
                this.leftside += 360;
            this.rightside = this.degree + (sectorsize / 2);
            if (this.leftside < this.rightside || this.rightside - this.leftside != this.sectorsize)
            {
                this.rightside += 360;
                this.addextra = true;
            }
        }


        public bool Inside(int X, int Y)
        {
            if (Role.Core.GetDistance((ushort)X, (ushort)Y, (ushort)attackerX, (ushort)attackerY) <= distance)
            {
                int degree = (int)Role.Core.GetAngle((ushort)attackerX, (ushort)X, (ushort)attackerY, (ushort)Y);
                if (this.addextra)
                    degree += 360;
                if (degree >= this.leftside && degree <= this.rightside)
                    return true;
            }
            return false;
        }
    }
}
