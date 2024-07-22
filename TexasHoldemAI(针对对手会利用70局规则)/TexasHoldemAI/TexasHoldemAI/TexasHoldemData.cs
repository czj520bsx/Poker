using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace TexasHoldemAI
{
    class TexasHoldemData
    {
        private const int SMALLBLINDAMOUNT = 50;
        private const int BIGBLINDAMOUNT = 100;

        private ArrayList unUsedPks;
        private ArrayList onHands;
        private ArrayList commonPorkers;
        private int iPlayerCount;
        private Random rand;
        private ArrayList moneys;
        private int blindpos;
        private int bonus;
        private int startPos;
        private ArrayList onTableMoneys;
        private ArrayList thisTurnMoneys;
        private bool[] bAlive;
        private int iAliveCount;
        private ArrayList porkersTypes;

        public ArrayList UnUsedPks { get {return unUsedPks;} set { unUsedPks = value;} }
        public ArrayList OnHands { get {return onHands;} set { onHands = value;} }
        public ArrayList Moneys { get {return  moneys;} set { moneys = value;} }
        public int Bonus { get {return  bonus;} set { bonus = value;} }
        public ArrayList CommonPorkers { get {return  commonPorkers;} set { commonPorkers = value;} }
        public int Blindpos { get {return blindpos;} set {blindpos = value;} }
        public ArrayList ThisTurnMoneys { get {return thisTurnMoneys;} set { thisTurnMoneys = value;} }
        public ArrayList PorkersTypes { get {return porkersTypes; }set { porkersTypes = value;} }
        public int IPlayerCount { get {return  iPlayerCount; }set { iPlayerCount = value;} }
        public int IAliveCount { get{return  iAliveCount;} set {iAliveCount = value;} }
        public ArrayList OnTableMoneys { get{return onTableMoneys;} set {onTableMoneys = value;} }

        public TexasHoldemData(int ipc, ArrayList initMoney)
        {
            Blindpos = 0;
            rand = new Random();
            Init(ipc, initMoney);
        }

        public void Abort()
        {
            for (int i = 0; i < IPlayerCount; i++)
            {
                moneys[i] = (int)moneys[i] + (int)OnTableMoneys[i];
            }
        }

        public void Init(int ipc, ArrayList initMoney)
        {
            Bonus = 0;
            UnUsedPks = new ArrayList();
            for (int i = 0; i < 52; i++)
            {
                UnUsedPks.Add(i);
            }
            OnHands = new ArrayList();
            CommonPorkers = new ArrayList();
            IPlayerCount = ipc;
            moneys = initMoney;
            OnTableMoneys = new ArrayList();
            ThisTurnMoneys = new ArrayList();
            bAlive = new bool[ipc];
            iAliveCount = ipc;
            for (int i = 0; i < ipc; i++)
            {
                OnTableMoneys.Add(0);
                ThisTurnMoneys.Add(0);
                bAlive[i] = true;
            }
            PorkersTypes = new ArrayList();
        }




        public void Preflop()
        {
            for (int i = 0; i < IPlayerCount; i++)
            {
                Porker[] porkers = new Porker[2];
                for (int j = 0; j < 2; j++)
                {
                    int pk = RandomPopAPorker();
                    porkers[j] = new Porker(pk % 4, pk / 4);
                }
                OnHand oh = new OnHand(porkers);
                OnHands.Add(oh);
            }
        }

        public void Preflop_After(Porker[] pks)
        {
            for (int k = 0; k < 2; k++)
            {
                UnUsedPks.Remove(pks[k].Ds * 4 + pks[k].Hs);
            }
            OnHand xoh = new OnHand(pks);
            OnHands.Add(xoh);
            for (int i = 1; i < IPlayerCount; i++)
            {
                Porker[] porkers = new Porker[2];
                for (int j = 0; j < 2; j++)
                {
                    int pk = RandomPopAPorker();
                    porkers[j] = new Porker(pk % 4, pk / 4);
                }
                OnHand oh = new OnHand(porkers);
                OnHands.Add(oh);
            }
        }

        public void PreflopMoneyInit(int ipc)
        {
            ThisTurnMoneys[Blindpos] = SMALLBLINDAMOUNT;
            ThisTurnMoneys[(Blindpos + 1) % 2] = BIGBLINDAMOUNT;
            moneys[Blindpos] = (int)moneys[Blindpos] - SMALLBLINDAMOUNT;
            OnTableMoneys[Blindpos] = SMALLBLINDAMOUNT;
            moneys[(Blindpos + 1) % ipc] = (int)moneys[(Blindpos + 1) % ipc] - BIGBLINDAMOUNT;
            OnTableMoneys[(Blindpos + 1) % ipc] = BIGBLINDAMOUNT;
            startPos = (Blindpos + 1) % ipc;
        }

        public void Flop()
        {
            for (int i = 0; i < 3; i++)
            {
                int pk = RandomPopAPorker();
                CommonPorkers.Add(new Porker(pk % 4, pk / 4));
            }
        }

        public void Flop_After(Porker[] pks)
        {
            for (int k = 0; k < 3; k++)
            {
                if (!UnUsedPks.Contains(pks[k].Ds * 4 + pks[k].Hs))
                {
                    int a = 0;
                    a++;
                }
                UnUsedPks.Remove(pks[k].Ds * 4 + pks[k].Hs);
                CommonPorkers.Add(pks[k]);
            }
        }

        //zhe 

        public void Turn()
        {
            int pk = RandomPopAPorker();
            CommonPorkers.Add(new Porker(pk % 4, pk / 4));
        }

        public void Turn_After(Porker pk)
        {
            UnUsedPks.Remove(pk.Ds * 4 + pk.Hs);
            CommonPorkers.Add(pk);
        }

        public void River()
        {
            int pk = RandomPopAPorker();
            CommonPorkers.Add(new Porker(pk % 4, pk / 4));
        }

        public void River_After(Porker pk)
        {
            UnUsedPks.Remove(pk.Ds * 4 + pk.Hs);
            CommonPorkers.Add(pk);
        }

        public int RandomGetAPorker()
        {
            int r = 0;
            int n = UnUsedPks.Count;
            for (int i = 1; i <= n; i++)
            {
                int j = rand.Next(i);
                if (j < 1)
                {
                    r = (int)UnUsedPks[i - 1];
                }
            }
            return r;
        }

        public int RandomPopAPorker()
        {
            int r = RandomGetAPorker();
            UnUsedPks.Remove(r);
            return r;
        }

        public void NewGame()
        {
            Init(IPlayerCount, moneys);
            PreflopMoneyInit(IPlayerCount);
            Preflop();
        }

        public void Call(int n)
        {
            int ca = (int)thisTurnMoneys[(n - 1 + IPlayerCount) % IPlayerCount] - (int)thisTurnMoneys[n];
            OnTableMoneys[n] = (int)OnTableMoneys[n] + ca;
            moneys[n] = (int)moneys[n] - ca;
            thisTurnMoneys[n] = (int)thisTurnMoneys[n] + ca;
        }

        public void AddThisTurnMoneyToBonus()
        {
            for (int i = 0; i < thisTurnMoneys.Count; i++)
            {
                Bonus += (int)thisTurnMoneys[i];
                thisTurnMoneys[i] = 0;
            }
        }

        public void Fold(int n)
        {
            bAlive[n] = false;
            iAliveCount--;
        }

        public void Allin(int n)
        {
            int max = 0;
            int imax = 0;
            for (int i = 0; i < moneys.Count; i++)
            {
                if (i != n && bAlive[i] == true)
                {
                    int t = (int)moneys[i] + (int)ThisTurnMoneys[i];
                    if (t > max)
                    {
                        max = t;
                        imax = i;
                    }
                }
            }
            int x = (int)moneys[n] + (int)ThisTurnMoneys[n];
            int ca = max < x ? (int)moneys[imax] + (int)thisTurnMoneys[imax] - (int)thisTurnMoneys[n] : (int)moneys[n];
            OnTableMoneys[n] = (int)OnTableMoneys[n] + ca;
            moneys[n] = (int)moneys[n] - ca;
            thisTurnMoneys[n] = (int)thisTurnMoneys[n] + ca;
        }

        public void Check(int n)
        {
            ;
        }

        public void Bet(int n, int amount)
        {
            moneys[n] = (int)moneys[n] - amount;
            thisTurnMoneys[n] = (int)thisTurnMoneys[n] + amount;
            OnTableMoneys[n] = (int)OnTableMoneys[n] + amount;
        }

        public void Raise(int n, int amount)
        {
            moneys[n] = (int)moneys[n] + (int)thisTurnMoneys[n] - amount;
            OnTableMoneys[n] = (int)OnTableMoneys[n] - (int)thisTurnMoneys[n] + amount;
            thisTurnMoneys[n] = amount;
        }

        public int EndGame()
        {
            AddThisTurnMoneyToBonus();
            int win = 0;
            int c = 0;
            int j = 0;
            for (int i = 0; i < bAlive.Length; i++)
            {
                if (bAlive[i] == true)
                {
                    c++;
                    j = i;
                }
            }
            if (c == 1)
            {
                win = j;
                Moneys[win] = (int)Moneys[win] + bonus;
            }
            else if (c > 1)
            {
                int bcc = 0;
                int[] iwinbc = new int[IPlayerCount];
                int[] ibc = new int[IPlayerCount];
                int[] bc = new int[IPlayerCount];
                int[] totm = new int[IPlayerCount];
                for (int i = 0; i < IPlayerCount; i++)
                {
                    totm[i] = (int)OnTableMoneys[i];
                }
                for (int i = IPlayerCount - 1; i > 0; i--)
                {
                    int min = int.MaxValue;
                    int imin = 0;
                    for (j = 0; j <= i; j++)
                    {
                        if (totm[j] > 0 && totm[j] < min)
                        {
                            min = totm[j];
                            imin = j;
                        }
                    }
                    for (j = 0; j < IPlayerCount; j++)
                    {
                        if ((int)OnTableMoneys[j] >= min)
                        {
                            OnTableMoneys[j] = (int)OnTableMoneys[j] - min;
                            bc[bcc] += min;
                            ibc[bcc] += 1 << j;
                        }
                    }
                    int t = totm[imin];
                    totm[imin] = totm[i];
                    totm[i] = t;
                    bcc++;
                }
                for (int i = 0; i < IPlayerCount; i++)
                {
                    CalcPorkersCombineTypeByPorkers(i);
                }
                for (int i = 0; i < bcc; i++)
                {
                    int itypemax = -1;
                    for (int k = 0; k < IPlayerCount; k++)
                    {
                        if ((ibc[i] & (1 << k)) > 0)
                        {
                            if (itypemax == -1)
                            {
                                itypemax = k;
                                iwinbc[i] = 1 << k;
                            }
                            else
                            {
                                int ret = JudgeWin(k, itypemax);
                                if (ret == -1)
                                {
                                    itypemax = k;
                                    iwinbc[i] = 1 << k;
                                }
                                else if (ret == 0)
                                {
                                    iwinbc[i] += 1 << k;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < bcc; i++)
                {
                    int wc = 0;
                    for (int k = 0; k < IPlayerCount; k++)
                    {
                        if ((iwinbc[i] & (1 << k)) > 0)
                        {
                            wc++;
                        }
                    }
                    for (int k = 0; k < IPlayerCount; k++)
                    {
                        if ((iwinbc[i] & (1 << k)) > 0)
                        {
                            int ta = bc[i] / wc;
                            Moneys[k] = (int)Moneys[k] + ta;
                            bc[i] -= ta;
                            wc--;
                        }
                    }
                }
                win = iwinbc[0];
            }
            bonus = 0;
            return win;
        }

        public void CalcPorkersType()
        {
            for (int i = 0; i < IPlayerCount; i++)
            {
                CalcPorkersCombineTypeByPorkers(i);
            }
        }

        public int JudgeWhoWin(int x, int y)
        {            
            return JudgeWin(x, y);
        }

        private int JudgeWin(int x, int y)
        {
            PorkersCombineType pcta = (PorkersCombineType)porkersTypes[x];
            PorkersCombineType pctb = (PorkersCombineType)porkersTypes[y];
            int[] r = {1, 2, 2, 5, 1, 3, 3, 4, 5, 0};
            if (pcta.Type == pctb.Type)
            {
                for (int i = 0; i < r[pcta.Type]; i++)
                {
                    if (pcta.Pts[i] == pctb.Pts[i])
                    {
                        continue;
                    }
                    else
                    {
                        return pcta.Pts[i] > pctb.Pts[i] ? -1 : 1;
                    }
                }
            }
            else
            {
                return pcta.Type < pctb.Type ? -1 : 1;
            }
            return 0;
        }

        private void CalcPorkersCombineTypeByPorkers(int x)
        {
            if (bAlive[x] == false)
            {
                PorkersCombineType pct = new PorkersCombineType((int)TypeName.FOLD, new int[5]);
                porkersTypes.Insert(x, pct);
                return;
            }
            Porker[] a = ((OnHand)onHands[x]).Porkers;
            int[] pksa = new int[13];
            int[] hsa = new int[13];
            for (int i = 0; i < 2; i++)
            {
                pksa[a[i].Ds]++;
                hsa[a[i].Ds] |= 1 << a[i].Hs;
            }
            for (int j = 0; j < commonPorkers.Count; j++)
            {
                Porker t = (Porker)CommonPorkers[j];
                pksa[t.Ds]++;
                hsa[t.Ds] |= 1 << t.Hs;
            }
            PorkersCombineType pcta = GetPorkersCombineTypeByPksAndHs(pksa, hsa);
            porkersTypes.Insert(x, pcta);
        }

        private PorkersCombineType GetPorkersCombineTypeByPksAndHs(int[] pksa, int[] hsa)
        {
            int[] pts = new int[5];
            //同花顺判定
            {
                for (int i = 12; i >= 3; i--)
                {
                    int hs = hsa[i];
                    int j = 0;
                    for (j = i - 1; j >= i - 4; j--)
                    {
                        //检测有无连续点数，连续点数有无同花
                        if (pksa[(j + 13) % 13] == 0 || pksa[j + 1] == 0 || (hsa[(j + 13) % 13] & hs) == 0)
                        {
                            break;
                        }
                        else
                        {
                            hs &= hsa[(j + 13) % 13];
                        }
                    }
                    if (j < i - 4)
                    {
                        //是同花顺
                        pts[0] = i - 4;
                        return new PorkersCombineType((int)TypeName.THS, pts);
                    }
                }
            }
            //四条判定
            {
                bool a = false;
                bool b = false;
                for (int i = 12; i >= 0; i--)
                {
                    if (pksa[i] == 4 && a == false)
                    {
                        pts[0] = i;
                        a = true;
                    }
                    else if (pksa[i] > 0 && b == false)
                    {
                        pts[1] = i;
                        b = true;
                    }
                    if (a == true && b == true)
                    {
                        break;
                    }
                }
                if (a == true)
                {
                    return new PorkersCombineType((int)TypeName.SIT, pts);
                }
            }
            //葫芦判定
            {
                bool a = false;
                bool b = false;
                for (int i = 12; i >= 0; i--)
                {
                    if (pksa[i] == 3 && a == false)
                    {
                        pts[0] = i;
                        a = true;
                    }
                    else if (pksa[i] > 1 && b == false)
                    {
                        pts[1] = i;
                        b = true;
                    }
                    if (a == true && b == true)
                    {
                        break;
                    }
                }
                if (a == true && b == true)
                {
                    return new PorkersCombineType((int)TypeName.HL, pts);
                }
            }
            //同花判定
            {
                int[] hsCounts = new int[4];
                for (int i = 12; i >= 0; i--)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int x = 1 << j;
                        if ((hsa[i] & x) == x)
                        {
                            hsCounts[j]++;
                        }
                    }
                }
                int k = 0;
                for (; k < 4; k++)
                {
                    if (hsCounts[k] > 4)
                    {
                        int x = 1 << k;
                        int n = 0;
                        for (int m = 12; m >= 0 && n < 5; m--)
                        {
                            if ((hsa[m] & x) == x)
                            {
                                pts[n++] = m;
                            }
                        }
                        break;
                    }
                }
                if (k < 4)
                {
                    return new PorkersCombineType((int)TypeName.TH, pts);
                }
            }
            //顺子判定
            {
                for (int i = 12; i >= 3; i--)
                {
                    int j = 0;
                    for (j = i - 1; j >= i - 4; j--)
                    {
                        //检测有无连续点数
                        if (pksa[(j + 13) % 13] == 0 || pksa[j + 1] == 0)
                        {
                            break;
                        }
                    }
                    if (j < i - 4)
                    {
                        //是顺子
                        pts[0] = i - 4;
                        return new PorkersCombineType((int)TypeName.SZ, pts);
                    }
                }
            }
            //三条判定
            {
                bool a = false;
                bool b = false;
                bool c = false;
                for (int i = 12; i >= 0; i--)
                {
                    if (pksa[i] == 3 && a == false)
                    {
                        pts[0] = i;
                        a = true;
                    }
                    else if (pksa[i] > 0 && b == false)
                    {
                        pts[1] = i;
                        b = true;
                    }
                    else if (pksa[i] > 0 && c == false)
                    {
                        pts[2] = i;
                        c = true;
                    }
                    if (a == true && b == true && c == true)
                    {
                        break;
                    }
                }
                if (a == true)
                {
                    return new PorkersCombineType((int)TypeName.SANT, pts);
                }
            }
            //两对判定
            {
                bool a = false;
                bool b = false;
                bool c = false;
                for (int i = 12; i >= 0; i--)
                {
                    if (pksa[i] == 2 && a == false)
                    {
                        pts[0] = i;
                        a = true;
                    }
                    else if (pksa[i] == 2 && b == false)
                    {
                        pts[1] = i;
                        b = true;
                    }
                    else if (pksa[i] > 0 && c == false)
                    {
                        pts[2] = i;
                        c = true;
                    }
                    if (a == true && b == true && c == true)
                    {
                        break;
                    }
                }
                if (a == true && b == true)
                {
                    return new PorkersCombineType((int)TypeName.LD, pts);
                }
            }
            //对子判定
            {
                bool a = false;
                bool b = false;
                bool c = false;
                bool d = false;
                for (int i = 12; i >= 0; i--)
                {
                    if (pksa[i] == 2 && a == false)
                    {
                        pts[0] = i;
                        a = true;
                    }
                    else if (pksa[i] > 0 && b == false)
                    {
                        pts[1] = i;
                        b = true;
                    }
                    else if (pksa[i] > 0 && c == false)
                    {
                        pts[2] = i;
                        c = true;
                    }
                    else if (pksa[i] > 0 && d == false)
                    {
                        pts[3] = i;
                        d = true;
                    }
                    if (a == true && b == true && c == true && d == true)
                    {
                        break;
                    }
                }
                if (a == true)
                {
                    return new PorkersCombineType((int)TypeName.DZ, pts);
                }
            }
            //高牌判定
            {
                bool a = false;
                bool b = false;
                bool c = false;
                bool d = false;
                bool e = false;
                for (int i = 12; i >= 0; i--)
                {
                    if (pksa[i] > 0 && a == false)
                    {
                        pts[0] = i;
                        a = true;
                    }
                    else if (pksa[i] > 0 && b == false)
                    {
                        pts[1] = i;
                        b = true;
                    }
                    else if (pksa[i] > 0 && c == false)
                    {
                        pts[2] = i;
                        c = true;
                    }
                    else if (pksa[i] > 0 && d == false)
                    {
                        pts[3] = i;
                        d = true;
                    }
                    else if (pksa[i] > 0 && e == false)
                    {
                        pts[4] = i;
                        e = true;
                    }
                    if (a == true && b == true && c == true && d == true && e == true)
                    {
                        break;
                    }
                }
                return new PorkersCombineType((int)TypeName.GP, pts);
            }
        }
    }

    public enum TypeName
    {
        THS,
        SIT,
        HL,
        TH,
        SZ,
        SANT,
        LD,
        DZ,
        GP,
        FOLD
    }

    public class PorkersCombineType
    {
        private int type;
        private int[] pts;
        public PorkersCombineType(int type, int[] pts)
        {
            this.Type = type;
            this.Pts = pts;
        }

        public int Type { get {return type;} set { type = value;} }
        public int[] Pts { get { return pts; } set { pts = value; } }
    }
}
