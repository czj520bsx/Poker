using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemAI
{
    class Porker
    {
        int hs;
        int ds;
        public Porker(int hs, int ds)
        {
            this.Hs = hs;
            this.Ds = ds;
        }

        public int Hs { get { return hs; } set { hs = value; } }
        public int Ds { get { return ds; } set { ds = value; } }
    }
}
