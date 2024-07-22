#include <iostream>
#include"Program.h"
#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\dataBase\sql.h"


//翻前胜率
double Program::PreflopWinProbCalc(TexasHoldemData thdata){
    Poker first = reinterpret_cast<const OnHand&>(thdata.getOnHands()[0]).porkers[0];
    Poker second = reinterpret_cast<const OnHand&>(thdata.getOnHands()[0]).porkers[1];
    Poker* t;
    if (first.getDs() < second.getDs()) {
        t = &first;
        first = second;
        second = *t;
    }
    return TableManager::get_data(TexasHoldemData::iPlayerCount,first.getDs(),
                                  second.getDs(),first.getHs() == second.getHs());
}

//翻牌胜率
double Program::FlopWinProbCalc(TexasHoldemData td){
    int wc = 0;
    TexasHoldemData thdata(td.iPlayerCount, std::vector<int>());
    for (int w = 0; w < simulateCount; w++) {
        thdata.Init(thdata.iPlayerCount, std::vector<int>());

        std::vector<Poker> pks = {
                Poker(td.CommonPorkers[0].getHs(), td.CommonPorkers[0].getDs()),
                Poker(td.CommonPorkers[1].getHs(), td.CommonPorkers[1].getDs()),
                Poker(td.CommonPorkers[2].getHs(), td.CommonPorkers[2].getDs()),
        };

        thdata.Flop_After(pks);
        thdata.Preflop_After(td.getOnHands()[0].porkers);
        thdata.Turn();
        thdata.River();
        thdata.CalcPorkersType();

        int ret = 0;
        int j;
        for (j = 1; j < thdata.iPlayerCount; j++) {
            if (thdata.JudgeWhoWin(0, j) > 0) {
                ret = 1;
                break;
            }
        }

        if (j == thdata.iPlayerCount) {
            ret = -1;
        }

        if (ret < 0) {
            wc++;
        }
    }
    return static_cast<double>(wc) / simulateCount;
}

//转牌胜率
double Program::TurnWinProbCalc(TexasHoldemData td){
    int wc = 0;
    TexasHoldemData thdata(td.iPlayerCount, std::vector<int>());

    for (int w = 0; w < simulateCount; w++) {
        thdata.Init(thdata.iPlayerCount, std::vector<int>());

        Poker index = td.CommonPorkers[3];

        thdata.Turn_After(index);

        std::vector<Poker> pks = {
                Poker(td.CommonPorkers[0].getHs(), td.CommonPorkers[0].getDs()),
                Poker(td.CommonPorkers[1].getHs(), td.CommonPorkers[1].getDs()),
                Poker(td.CommonPorkers[2].getHs(), td.CommonPorkers[2].getDs()),
        };


        thdata.Flop_After(pks);
        thdata.Preflop_After(td.getOnHands()[0].porkers);
        thdata.River();
        thdata.CalcPorkersType();

        int ret = 0 , j;
        for (j = 1; j < thdata.iPlayerCount; j++) {
            if (thdata.JudgeWhoWin(0, j) > 0) {
                ret = 1;
                break;
            }
        }

        if (j == thdata.iPlayerCount) {
            ret = -1;
        }

        if (ret < 0) {
            wc++;
        }
    }

    return static_cast<double>(wc) / simulateCount;
}

//河牌胜率
double Program::RiverWinProbCalc(TexasHoldemData td) {
    int wc = 0;
    TexasHoldemData thdata(td.iPlayerCount, std::vector<int>());

    for (int w = 0; w < simulateCount; w++) {
        thdata.Init(thdata.iPlayerCount, std::vector<int>());

        Poker index = td.CommonPorkers[3];

        thdata.Turn_After(index);

        index = td.CommonPorkers[4];

        thdata.River_After(index);

        std::vector<Poker> pks = {
                Poker(td.CommonPorkers[0].getHs(), td.CommonPorkers[0].getDs()),
                Poker(td.CommonPorkers[1].getHs(), td.CommonPorkers[1].getDs()),
                Poker(td.CommonPorkers[2].getHs(), td.CommonPorkers[2].getDs()),
        };

        thdata.Flop_After(pks);
        thdata.Preflop_After(td.getOnHands()[0].porkers);
        thdata.CalcPorkersType();

        int ret = 0 , j;
        for (j = 1; j < thdata.iPlayerCount; j++) {
            if (thdata.JudgeWhoWin(0, j) > 0) {
                ret = 1;
                break;
            }
        }

        if (j == thdata.iPlayerCount) {
            ret = -1;
        }

        if (ret < 0) {
            wc++;
        }
    }
    return static_cast<double>(wc) / simulateCount;
}


//行动树
int Program::think(TexasHoldemData thdata, int cs, int iHumanActionCode, int iHumanAmount, int amount) {
    amount = 0;
    int r = 0;
    double p = 0;

    switch (cs) {
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

    if (p < 1) {
        int tttmm = 0;
        for (int i = 0; i < thdata.iPlayerCount; i++) {
            if (static_cast<int>(thdata.thisTurnMoneys[i]) > tttmm) {
                tttmm = static_cast<int>(thdata.thisTurnMoneys[i]);
            }
        }

        a = (thdata.bonus + tttmm * thdata.iAliveCount) * p / (2 - p - p * thdata.iAliveCount / 2);
    } else {
        a = static_cast<int>(thdata.moneys[0]);
    }

    switch (iHumanActionCode) {
        case 0:
            if (a >= static_cast<int>(thdata.moneys[0])) {
                if (p >= 0.75) {
                    r = 4;
                    amount = static_cast<int>(thdata.moneys[0]);
                } else {
                    amount = static_cast<int>((thdata.bonus + 100 * thdata.iAliveCount) * p /(2 - p - p * thdata.iAliveCount / 2));
                    if (amount < static_cast<int>(thdata.moneys[0]) && amount >= 100) {
                        r = 3;
                    } else {
                        r = 5;
                    }
                }
            } else if (a >= 100) {
                r = 3;
                amount = static_cast<int>(a);
            } else {
                r = 5;
            }
            break;

            // CHECK
        case 5:
            if (a >= static_cast<int>(thdata.moneys[0])) {
                if (p >= 0.6) {
                    r = 4;
                    amount = static_cast<int>(thdata.moneys[0]);

                } else {
                    amount = static_cast<int>((thdata.bonus + 100 * thdata.iAliveCount) * p /(2 - p - p * thdata.iAliveCount / 2));
                    if (amount < static_cast<int>(thdata.moneys[0]) && amount >= 100) {
                        r = 3;
                    } else {
                        r = 2;
                    }
                }
            } else if (a >= 100) {
                r = 3;
                amount = static_cast<int>(a);
            } else {
                r = 2;
            }
            break;

            // RAISE
        case 3:
            if (a >= static_cast<int>(thdata.moneys[0])) {
                if (p >= 0.7) {
                    r = 4;
                    amount = static_cast<int>(thdata.moneys[0]);
                } else if (20000 - static_cast<int>(thdata.moneys[0]) - earnchips >= oppshouldc) {
                    r = 2;
                } else {
                    r = 6;
                }
            } else if (a >= iHumanAmount * 2) {
                r = 3;
                amount = static_cast<int>(a);
            } else if (a >= iHumanAmount) {
                r = 2;
                amount = static_cast<int>(a);
            } else if (20000 - static_cast<int>(thdata.moneys[0]) - earnchips >= oppshouldc) {
                r = 2;
            } else {
                r = 6;
            }
            break;

            // CALL
        case 2:
            if (cs == 0) {
                if (a >= static_cast<int>(thdata.moneys[0])) {
                    if (p > 0.7) {
                        r = 4;
                        amount = static_cast<int>(thdata.moneys[0]);
                    } else {
                        amount = static_cast<int>((thdata.bonus + 100 * thdata.iAliveCount) * p /
                                                  (2 - p - p * thdata.iAliveCount / 2));
                        if (amount < static_cast<int>(thdata.moneys[0]) && amount >= 200) {
                            r = 3;
                        } else {
                            r = 5;
                        }
                    }
                } else if (a >= 200) {
                    r = 3;
                    amount = static_cast<int>(a);
                } else {
                    r = 5;
                }
            }
            break;

            // ALLIN
        case 4:
            if (a >= static_cast<int>(thdata.moneys[0])) {
                if (allinflag == 1) {
                    if (p >= 0.664) {
                        r = 2;
                        amount = static_cast<int>(thdata.moneys[0]);
                    } else if (20000 - static_cast<int>(thdata.moneys[0]) - earnchips >= oppshouldc) {
                        r = 2;
                    } else {
                        r = 6;
                    }
                } else {
                    if (p >= 0.79) {
                        r = 2;
                        amount = static_cast<int>(thdata.moneys[0]);
                    } else if (20000 - static_cast<int>(thdata.moneys[0]) - earnchips >= oppshouldc) {
                        r = 2;
                    } else {
                        r = 6;
                    }
                }
            } else if (20000 - static_cast<int>(thdata.moneys[0]) - earnchips >= oppshouldc) {
                r = 2;
            } else {
                r = 6;
            }
            break;

            // FOLD
        case 6:
            break;
    }
    return r;
}

//和服务器的交互
void Program::SendAction(int r, int cs, int amount, SOCKET socket_send, TexasHoldemData td) {
    char buffer[1024];
    switch(r) {
        case 2: {
            td.Call(0);
            std::string t = "call";
            std::cout << t << std::endl;
            memcpy(buffer, t.c_str(), t.length());
            send(socket_send, buffer, t.length(), 0);
            break;
        }
        case 3: {
            if (said[0]){
                td.Raise(0, amount);
            }else if (cs == 0 && td.blindpos == 0){
                td.Raise(0, amount);
            }else{
                td.Bet(0, amount);
            }
            std::string t = "raise " + std::to_string(amount);
            std::cout << t << std::endl;
            memcpy(buffer, t.c_str(), t.length());
            send(socket_send, buffer, t.length(), 0);
            break;
        }
        case 4: {
            allinflag = true;
            td.Allin(0);
            std::string t = "allin";
            std::cout << t << std::endl;
            memcpy(buffer, t.c_str(), t.length());
            send(socket_send, buffer, t.length(), 0);
            break;
        }
        case 5: {
            td.Check(0);
            std::string t = "check";
            std::cout << t << std::endl;
            memcpy(buffer, t.c_str(), t.length());
            send(socket_send, buffer, t.length(), 0);
            break;
        }
        case 6: {
            td.Fold(0);
            td.EndGame();
            std::string t = "fold";
            std::cout << t << std::endl;
            memcpy(buffer, t.c_str(), t.length());
            send(socket_send, buffer, t.length(), 0);
            break;
        }
    }
}