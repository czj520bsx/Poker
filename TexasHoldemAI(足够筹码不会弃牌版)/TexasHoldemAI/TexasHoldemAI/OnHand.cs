using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemAI
{
    class OnHand
    {
        Porker[] porkers;
        public OnHand(Porker[] porkers)
        {
            this.Porkers = porkers;
        }

        public Porker[] Porkers { get { return porkers; } set { porkers = value; } }
    }
}
