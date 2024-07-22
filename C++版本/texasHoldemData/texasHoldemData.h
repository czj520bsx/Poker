#ifndef TEXASHOLDEMDATA_H
#define TEXASHOLDEMDATA_H

#include <vector>
#include <cstdlib>
#include <array>
#include <string>
#include <algorithm>
#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\Poker\Poker.h"
#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\OnHand\OnHand.h"


class TexasHoldemData {
    static const int SMALLBLINDAMOUNT = 50; //小盲前注
    static const int BIGBLINDAMOUNT = 100;  //大盲前注
    std::vector<int> unUsedPks;
    int startPos;
    std::vector<int> onTableMoneys;        //桌面死钱
    std::vector<bool> bAlive;
    std::vector<PorkersCombineType> porkersTypes;

public:
    static int iPlayerCount;                   //玩家数量
    int bonus;                          //筹码量
    int iAliveCount;
    std::vector<int> moneys;
    std::vector<int> thisTurnMoneys;
    std::vector<OnHand> onHands;
    int blindpos;
    std::vector<Poker> CommonPorkers;
   
    TexasHoldemData(int ipc, const std::vector<int>& initMoney);
    void Abort();
    void Init(int ipc, const std::vector<int>& initMoney);
    [[nodiscard]] const std::vector<OnHand>& getOnHands() const;
    void Preflop();
    void Preflop_After(const std::vector<Poker>& pks);
    void PreflopMoneyInit(int ipc);
    void Flop();
    void Flop_After(const std::vector<Poker>& pks);
    void Turn();
    void Turn_After(const Poker& pk);
    void River();
    void River_After(const Poker& pk);
    Poker RandomGetAPorker();
    void NewGame();
    void Call(int n);
    Poker RandomPopAPorker();
    void AddThisTurnMoneyToBonus();
    void Fold(int n);
    void Allin(int n);
    void Check(int n);
    void Bet(int n, int amount);
    void Raise(int n, int amount);
    int EndGame();
    void CalcPorkersType();
    int JudgeWhoWin(int x, int y);
    int JudgeWin(int x, int y);
    void CalcPorkersCombineTypeByPorkers(int x);
    PorkersCombineType GetPorkersCombineTypeByPksAndHs(std::vector<int> pksa, std::vector<int> hsa);
};
#endif  // TEXASHOLDEMDATA_H
