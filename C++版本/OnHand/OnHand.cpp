#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\OnHand\OnHand.h"
#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\Poker\Poker.h"

OnHand::OnHand(const std::vector<Poker>& porkers) {
    this->porkers = porkers;
}

const std::vector<Poker>& OnHand::getPorkers() const {
    return porkers;
}

void OnHand::setPorkers(const std::vector<Poker>& porkers) {
    this->porkers = porkers;
}