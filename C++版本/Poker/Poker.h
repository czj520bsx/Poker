#ifndef PORKER_H
#define PORKER_H
#include <vector>

class Poker {
    int hs;
    int ds;

public:
    Poker(int hs ,int ds);
    [[nodiscard]] int getHs() const;
    void setHs(int hs);
    [[nodiscard]] int getDs() const;
    void setDs(int ds);
};

enum class TypeName
{
    THS,  //同花顺
    SIT, //四条
    HL, //葫芦
    TH, // 同花
    SZ, //顺子
    SANT, //三条
    LD, //两对
    DZ, //对子
    GP, //高牌
    FOLD  //弃牌
};

class PorkersCombineType
{
private:
    int type;
    std::vector<int> pts;

public:
    PorkersCombineType(int type, const std::vector<int>& pts);
    [[nodiscard]] int GetType() const;
    void SetType(int type);
    [[nodiscard]] std::vector<int> GetPts() const;
    void SetPts(const std::vector<int> pts);
};

#endif  // PORKER_H


