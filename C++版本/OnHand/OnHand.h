#ifndef ONHAND_H
#define ONHAND_H

#include <vector>
#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\Poker\Poker.h"

class OnHand {
public:
    std::vector<Poker> porkers;
    OnHand(const std::vector<Poker>& porkers);
    const std::vector<Poker>& getPorkers() const;
    void setPorkers(const std::vector<Poker>& porkers);
};

#endif  // ONHAND_H
