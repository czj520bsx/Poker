#pragma once
#include<vector>
#include <winsock2.h>
#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\texasHoldemData\texasHoldemData.h"

class Program {
public:
    static std::vector<bool>said;
    static int allinflag;

    void  static SendAction(int r, int cs, int amount, SOCKET socket_send,  TexasHoldemData td);
    int static think(TexasHoldemData thdata, int cs, int iHumanActionCode, int iHumanAmount, int amount);
    static int  oppshouldc ;
    static int earnchips;
private:
    static const  int simulateCount = 100000;
    static double PreflopWinProbCalc(TexasHoldemData thdata);
    static double FlopWinProbCalc(TexasHoldemData td);
    static double TurnWinProbCalc(TexasHoldemData td);
    static double RiverWinProbCalc(TexasHoldemData td);
};
;