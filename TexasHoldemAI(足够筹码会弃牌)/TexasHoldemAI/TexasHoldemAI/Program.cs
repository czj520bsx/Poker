using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace TexasHoldemAI
{
    class Program
    {
        private const int simulateCount = 100000;
        private static bool allinflag = false;
        private static bool[] said;
        private static int allallinflag = 0;
        private static double PreflopWinProbCalc(TexasHoldemData thdata)
        {
            Porker first = ((OnHand)thdata.OnHands[0]).Porkers[0];
            Porker second = ((OnHand)thdata.OnHands[0]).Porkers[1];
            Porker t = null;
            if (first.Ds < second.Ds)
            {
                t = first;
                first = second;
                second = t;
            }
            DataSet_ThData.preflopstatisticsDataTable preflopstatistics = new DataSet_ThData.preflopstatisticsDataTable();
            DataSet_ThDataTableAdapters.preflopstatisticsTableAdapter dta = new DataSet_ThDataTableAdapters.preflopstatisticsTableAdapter();
            dta.FillByPrimaryKey(preflopstatistics, thdata.IPlayerCount, first.Ds, second.Ds, first.Hs == second.Hs ? 1 : 0);
            return preflopstatistics.Rows.Count > 0 ? (double)preflopstatistics.Rows[0]["winprobability"] : 0;
        }
        private static double FlopWinProbCalc(TexasHoldemData td)
        {
            int wc = 0;
            TexasHoldemData thdata = new TexasHoldemData(td.IPlayerCount, null);
            for (int w = 0; w < simulateCount; w++)
            {
                thdata.Init(thdata.IPlayerCount, null);
                Porker[] pks = new Porker[3];
                pks[0] = (Porker)td.CommonPorkers[0];
                pks[1] = (Porker)td.CommonPorkers[1];
                pks[2] = (Porker)td.CommonPorkers[2];
                thdata.Flop_After(pks);
                thdata.Preflop_After(((OnHand)td.OnHands[0]).Porkers);
                thdata.Turn();
                thdata.River();
                thdata.CalcPorkersType();
                int ret = 0;
                int j = 0;
                for (j = 1; j < thdata.IPlayerCount; j++)
                {
                    if (thdata.JudgeWhoWin(0, j) > 0)
                    {
                        ret = 1;
                        break;
                    }
                }
                if (j == thdata.IPlayerCount)
                {
                    ret = -1;
                }
                if (ret < 0)
                {
                    wc++;
                }
            }
            return (double)wc / simulateCount;
        }
        private static double TurnWinProbCalc(TexasHoldemData td)
        {
            int wc = 0;
            TexasHoldemData thdata = new TexasHoldemData(td.IPlayerCount, null);
            for (int w = 0; w < simulateCount; w++)
            {
                thdata.Init(thdata.IPlayerCount, null);
                Porker pk = (Porker)td.CommonPorkers[3];
                thdata.Turn_After(pk);
                Porker[] pks = new Porker[3];
                pks[0] = (Porker)td.CommonPorkers[0];
                pks[1] = (Porker)td.CommonPorkers[1];
                pks[2] = (Porker)td.CommonPorkers[2];
                thdata.Flop_After(pks);
                thdata.Preflop_After(((OnHand)td.OnHands[0]).Porkers);
                thdata.River();
                thdata.CalcPorkersType();
                int ret = 0;
                int j = 0;
                for (j = 1; j < thdata.IPlayerCount; j++)
                {
                    if (thdata.JudgeWhoWin(0, j) > 0)
                    {
                        ret = 1;
                        break;
                    }
                }
                if (j == thdata.IPlayerCount)
                {
                    ret = -1;
                }
                if (ret < 0)
                {
                    wc++;
                }
            }
            return (double)wc / simulateCount;
        }
        private static double RiverWinProbCalc(TexasHoldemData td)
        {
            int wc = 0;
            TexasHoldemData thdata = new TexasHoldemData(td.IPlayerCount, null);
            for (int w = 0; w < simulateCount; w++)
            {
                thdata.Init(thdata.IPlayerCount, null);
                Porker pk = (Porker)td.CommonPorkers[3];
                thdata.Turn_After(pk);
                pk = (Porker)td.CommonPorkers[4];
                thdata.River_After(pk);
                Porker[] pks = new Porker[3];
                pks[0] = (Porker)td.CommonPorkers[0];
                pks[1] = (Porker)td.CommonPorkers[1];
                pks[2] = (Porker)td.CommonPorkers[2];
                thdata.Flop_After(pks);
                thdata.Preflop_After(((OnHand)td.OnHands[0]).Porkers);
                thdata.CalcPorkersType();
                int ret = 0;
                int j = 0;
                for (j = 1; j < thdata.IPlayerCount; j++)
                {
                    if (thdata.JudgeWhoWin(0, j) > 0)
                    {
                        ret = 1;
                        break;
                    }
                }
                if (j == thdata.IPlayerCount)
                {
                    ret = -1;
                }
                if (ret < 0)
                {
                    wc++;
                }
            }
            return (double)wc / simulateCount;
        }
        private static int think(TexasHoldemData thdata, int cs, int iHumanActionCode, int iHumanAmount, out int amount)
        {
            amount = 0;
            int r = 0;
            Random rand = new Random();
            double p = 0;
            switch (cs)
            {
                case 0:
                    p = PreflopWinProbCalc(thdata);
                    break;
                case 1:
                    p = FlopWinProbCalc(thdata);
                    break;
                case 2:
                    p = TurnWinProbCalc(thdata);
                    break;
                case 3:
                    p = RiverWinProbCalc(thdata);
                    break;
            }
            double a = 0;
            if (p < 1)
            {
                int tttmm = 0; ;
                for (int i = 0; i < thdata.IPlayerCount; i++)
                {
                    if ((int)thdata.ThisTurnMoneys[i] > tttmm)
                    {
                        tttmm = (int)thdata.ThisTurnMoneys[i];
                    }
                }
                a = (thdata.Bonus + tttmm * thdata.IAliveCount) * p / (2 - p - p * thdata.IAliveCount / 2);
            }
            else
            {
                a = (int)thdata.Moneys[0];
            }
            switch (iHumanActionCode)
            {
                //NA
                case 0:
                    if (a >= (int)thdata.Moneys[0])
                    {
                        if (p >= 0.75)
                        {
                            r = 4;
                            amount = (int)thdata.Moneys[0];
                        }
                        else
                        {
                            amount = (int)((thdata.Bonus + 100 * thdata.IAliveCount) * p / (2 - p - p * thdata.IAliveCount / 2));
                            if (amount < (int)thdata.Moneys[0] && amount >= 100)
                            {
                                r = 3;
                            }
                            else r = 5;
                        }
                    }
                    else if (a >= 100)
                    {
                        r = 3;
                        amount = (int)a;
                    }
                    else
                    {
                        r = 5;
                    }
                    break;
                //CHECK
                case 5:
                    if (a >= (int)thdata.Moneys[0])
                    {
                        if (p >= 0.6)
                        {
                            r = 4;
                            amount = (int)thdata.Moneys[0];
                        }
                        else
                        {
                            amount = (int)((thdata.Bonus + 100 * thdata.IAliveCount) * p / (2 - p - p * thdata.IAliveCount / 2));
                            if (amount < (int)thdata.Moneys[0] && amount >= 100)
                            {
                                r = 3;
                            }
                            else r = 2;
                        }
                    }
                    else if (a >= 100)
                    {
                        r = 3;
                        amount = (int)a;
                    }
                    else
                    {
                        r = 2;
                    }
                    break;
                //RAISE
                case 3:
                    if (a >= (int)thdata.Moneys[0])
                    {
                        if (p >= 0.7)
                        {
                            r = 4;
                            amount = (int)thdata.Moneys[0];
                        }
                        else r = 6;
                    }
                    else if (a >= iHumanAmount * 2)
                    {
                        r = 3;
                        amount = (int)a;
                    }
                    else if (a >= iHumanAmount)
                    {
                        r = 2;
                        amount = (int)a;
                    }
                    else
                    {
                        r = 6;
                    }
                    break;
                //CALL
                case 2:
                    if (cs == 0)
                    {
                        if (a >= (int)thdata.Moneys[0])
                        {
                            if (p > 0.7)
                            {
                                r = 4;
                                amount = (int)thdata.Moneys[0];
                            }
                            else
                            {
                                amount = (int)((thdata.Bonus + 100 * thdata.IAliveCount) * p / (2 - p - p * thdata.IAliveCount / 2));
                                if (amount < (int)thdata.Moneys[0] && amount >= 200)
                                {
                                    r = 3;
                                }
                                else r = 5;
                            }
                        }
                        else if (a >= 200)
                        {
                            r = 3;
                            amount = (int)a;
                        }
                        else
                        {
                            r = 5;
                        }
                    }
                    break;
                //ALLIN
                case 4:
                    if (a >= (int)thdata.Moneys[0])
                    {
                        if (allallinflag == 1)
                        {
                            if (p >= 0.664)
                            {
                                r = 2;
                                amount = (int)thdata.Moneys[0];
                            }
                            else r = 6;
                        }
                        else
                        {
                            if (p >= 0.79)
                            {
                                r = 2;
                                amount = (int)thdata.Moneys[0];
                            }
                            else r = 6;
                        }
                    }
                    else
                    {
                        r = 6;
                    }
                    break;
                //FOLD
                case 6:
                    break;
            }
            return r;
        }
        private static void SendAction(int r,int cs,int amount,Socket socket_send,ref TexasHoldemData td)//发送
        {
            byte[] buffer = new byte[1024];
            switch(r)
            {
                case 2:
                    {
                        td.Call(0);
                        string t = "call";
                        Console.WriteLine(t);
                        buffer = Encoding.UTF8.GetBytes(t);
                        socket_send.Send(buffer); 
                        break;
                    }
                case 3:
                    {
                        if (said[0])
                            td.Raise(0, amount);
                        else if (cs == 0 && td.Blindpos == 0)
                            td.Raise(0, amount);
                        else
                            td.Bet(0, amount);
                        string t = "raise " + amount;
                        Console.WriteLine(t);
                        buffer = Encoding.UTF8.GetBytes(t);
                        socket_send.Send(buffer); 
                        break;
                    }
                case 4:
                    {
                        allinflag = true;
                        td.Allin(0);
                        string t = "allin";
                        Console.WriteLine(t);
                        buffer = Encoding.UTF8.GetBytes(t);
                        socket_send.Send(buffer); 
                        break;
                    }
                case 5:
                    {
                        td.Check(0);
                        string t = "check";
                        Console.WriteLine(t);
                        buffer = Encoding.UTF8.GetBytes(t);
                        socket_send.Send(buffer); 
                        break;
                    }
                case 6:
                    {
                        td.Fold(0);
                        td.EndGame();
                        string t = "fold";
                        Console.WriteLine(t);
                        buffer = Encoding.UTF8.GetBytes(t);
                        socket_send.Send(buffer); 
                        break;
                    }
            }
        }
        static void Main(string[] args)
        {
            //Socket通信
            Socket socket_ai = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ipstr = Console.ReadLine();
            IPAddress ip = IPAddress.Parse(ipstr);
            IPEndPoint endpoint = new IPEndPoint(ip, 10001);
            try
            {
                socket_ai.Connect(endpoint);
                Console.WriteLine("连接成功！");
            }
            catch (SocketException)
            {
                Console.WriteLine("连接失败！");
                return;
            }
            //获取数据
            ArrayList moneys=new ArrayList();
            for (int i = 0; i < 2; i++)
            {
                moneys.Add(20000);
            }
            TexasHoldemData td = new TexasHoldemData(2, moneys);
            int cs=0;
            int r;
            int amount = 0;
            int iHumanActionCode;
            int iHumanAmount;
            int BIGBLIND = 100;
            int earnchips = 0;
            int WinFlag = 0;
            int NumOfGames = 0;
            int allincount = 0;
            do
            {
                moneys = new ArrayList();
                for (int i = 0; i < 2; i++)
                {
                    moneys.Add(20000);
                }
                amount = 0;
                byte[] buffer2 = new byte[1024];
                socket_ai.Blocking = true;
                socket_ai.Receive(buffer2);
                string receive = Encoding.UTF8.GetString(buffer2);
                Console.WriteLine(receive);
                string[] temp = receive.Split(new char[5] { '|', '<', ',', '>' ,' '}, StringSplitOptions.RemoveEmptyEntries);
                if (temp[0].Trim('\0').Equals("name"))
                {
                    string t = "Boyi Texas Hold'em";
                    byte[] buffer = new byte[1024];
                    buffer = Encoding.UTF8.GetBytes(t);
                    socket_ai.Send(buffer);
                    Console.WriteLine(t);
                }
                else if (temp[0].Equals("preflop"))
                {
                    
                    allinflag = false;
                    td.Init(2, moneys);
                    cs = 0;
                    said = new bool[2];
                    Porker[] porker1 = { new Porker(Convert.ToInt32(temp[2]), Convert.ToInt32(temp[3])), new Porker(Convert.ToInt32(temp[4]), Convert.ToInt32(temp[5])) };
                    OnHand oh = new OnHand(porker1);
                    td.OnHands.Add(oh);
                    if (temp[1].Equals("SMALLBLIND"))
                    {
                        td.Blindpos = 0;
                        td.PreflopMoneyInit(2);
                        iHumanActionCode = 3;
                        iHumanAmount = BIGBLIND;
                        //td.Bet(1, iHumanAmount);
                        if (WinFlag == 1)
                        {
                            r = 6;
                        }
                        else
                        {
                            r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                        }
                        SendAction(r,cs, amount, socket_ai, ref td);
                        said[0] = true;
                    }
                    else
                    {
                        td.Blindpos = 1;
                        td.PreflopMoneyInit(2);
                    }
                }
                else if (temp[0].Equals("flop"))
                {
                    cs = 1;
                    said = new bool[2];
                    td.CommonPorkers.Add(new Porker(Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2])));
                    td.CommonPorkers.Add(new Porker(Convert.ToInt32(temp[3]), Convert.ToInt32(temp[4])));
                    td.CommonPorkers.Add(new Porker(Convert.ToInt32(temp[5]), Convert.ToInt32(temp[6])));
                    if (td.Blindpos == 1)
                    {
                        iHumanActionCode = 0;
                        iHumanAmount = 0;
                        if (WinFlag == 1)
                        {
                            r = 6;
                        }
                        else
                        {
                            r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                        }
                        SendAction(r,cs, amount, socket_ai, ref td);
                        said[0] = true;
                    }
                }
                else if (temp[0].Equals("turn"))
                {
                    cs = 2;
                    said = new bool[2];
                    td.CommonPorkers.Add(new Porker(Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2])));
                    if (td.Blindpos == 1)
                    {
                        iHumanActionCode = 0;
                        iHumanAmount = 0;
                        if (WinFlag == 1)
                        {
                            r = 6;
                        }
                        else
                        {
                            r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                        }
                        SendAction(r,cs, amount, socket_ai, ref td);
                        said[0] = true;
                    }
                }
                else if (temp[0].Equals("river"))
                {
                    cs = 3;
                    said = new bool[2];
                    td.CommonPorkers.Add(new Porker(Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2])));
                    if (td.Blindpos == 1)
                    {
                        iHumanActionCode = 0;
                        iHumanAmount = 0;
                        if (WinFlag == 1)
                        {
                            r = 6;
                        }
                        else
                        {
                            r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                        }
                        SendAction(r,cs, amount, socket_ai, ref td);
                        said[0] = true;
                    }
                }
                else if (temp[0].Equals("bet"))
                {
                    said[1] = true;
                    iHumanActionCode = 3;
                    iHumanAmount = Convert.ToInt32(temp[1]);
                    td.Bet(1, iHumanAmount);
                    if (WinFlag == 1)
                    {
                        r = 6;
                    }
                    else
                    {
                        r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                    }
                    SendAction(r, cs, amount, socket_ai, ref td);
                    said[0] = true;
                }
                else if (temp[0].Trim('\0').Equals("call"))
                {
                    said[1] = true;
                    iHumanActionCode = 2;
                    iHumanAmount = (int)td.ThisTurnMoneys[0];
                    td.Call(1);
                    if (said[0] == false)
                    {
                        if (WinFlag == 1)
                        {
                            r = 6;
                        }
                        else
                        {
                            r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                        }
                        SendAction(r, cs, amount, socket_ai, ref td);
                        said[0] = true;
                    }
                }
                else if (temp[0].Equals("raise"))
                {
                    iHumanActionCode = 3;
                    iHumanAmount = Convert.ToInt32(temp[1]);
                    if (said[1])
                        td.Raise(1, iHumanAmount);
                    else if (cs == 0 && td.Blindpos == 1)
                        td.Raise(1, iHumanAmount);
                    else
                        td.Bet(1, iHumanAmount);
                    said[1] = true;
                    if (WinFlag == 1)
                    {
                        r = 6;
                    }
                    else
                    {
                        r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                    }
                    SendAction(r, cs, amount, socket_ai, ref td);
                    said[0] = true;
                }
                else if (temp[0].Trim('\0').Equals("allin"))
                {
                    if (allallinflag == 0)
                    {
                        if (cs == 0) allincount++;
                        if (allincount >= 20) allallinflag = 1;
                    }
                    said[1] = true;
                    td.Allin(1);
                    iHumanActionCode = 4;
                    iHumanAmount = (int)td.ThisTurnMoneys[1];
                    if (allinflag == false)
                    {
                        if (WinFlag == 1)
                        {
                            r = 6;
                        }
                        else
                        {
                            r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                        }
                        SendAction(r, cs, amount, socket_ai, ref td);
                        said[0] = true;
                    }
                }
                else if (temp[0].Trim('\0').Equals("check"))
                {
                    said[1] = true;
                    iHumanActionCode = 5;
                    iHumanAmount = 0;
                    td.Check(1);
                    if (said[0] == false)
                    {
                        if (WinFlag == 1)
                        {
                            r = 6;
                        }
                        else
                        {
                            r = think(td, cs, iHumanActionCode, iHumanAmount, out amount);
                        }
                        SendAction(r, cs, amount, socket_ai, ref td);
                        said[0] = true;
                    }
                }
                else if (temp[0].Trim('\0').Equals("fold"))
                {
                    said[1] = true;
                    td.Fold(1);
                    td.EndGame();
                }
                else if (temp[0].Equals("earnChips"))
                {
                    earnchips += Convert.ToInt32(temp[1]);
                    NumOfGames++;
                    if (WinFlag == 0)
                    {
                        int shouldc = 0;
                        if ((70 - NumOfGames) % 2 == 0)
                            shouldc = ((70 - NumOfGames) / 2) * 150;
                        else
                        {
                            if (td.Blindpos == 1)
                            {
                                shouldc = ((69 - NumOfGames) / 2) * 150 + 50;
                            }
                            else
                            {
                                shouldc = ((69 - NumOfGames) / 2) * 150 + 100;
                            }
                        }
                        if (earnchips > shouldc) WinFlag = 1;
                    }
                }
            } while (true);
            
        }
    }
}
