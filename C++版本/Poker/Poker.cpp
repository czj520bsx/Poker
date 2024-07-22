#include "D:\c++学习\新建文件夹 (2)\计算机博弈大赛伯懿德州扑克\C++版本\Poker\Poker.h"


Poker::Poker(int hs ,int ds)
{
    this->hs = hs;
    this->ds = ds;
}

int Poker::getHs() const
{
    return hs;
}

void Poker::setHs(int hs)
{
    this->hs = hs;
}

int Poker::getDs() const
{
    return ds;
}

void Poker::setDs(int ds)
{
    this->ds = ds;
}

PorkersCombineType::PorkersCombineType(int type, const std::vector<int>& pts): type(type), pts(pts) {}


int PorkersCombineType::GetType() const
{
    return type;
}

void PorkersCombineType::SetType(int type)
{
    this->type = type;
}

std::vector<int> PorkersCombineType::GetPts() const
{
    return pts;
}

void PorkersCombineType::SetPts(const std::vector<int> pts)
{
    this->pts = pts;
}
