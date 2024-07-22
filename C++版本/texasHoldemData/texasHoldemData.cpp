#include "texasholdemdata.h"
#include<time.h>


const std::vector<OnHand>& TexasHoldemData::getOnHands() const {
    return onHands;
}

TexasHoldemData::TexasHoldemData(int ipc, const std::vector<int>& initMoney){
    blindpos = 0;
    srand(time(NULL));
    Init(ipc, initMoney);
}

void TexasHoldemData::Abort() {
    for (int i = 0; i < iPlayerCount; i++) {
        moneys[i] += onTableMoneys[i];
    }
}

void TexasHoldemData::Init(int ipc, const std::vector<int>& initMoney) {
    bonus = 0;
    unUsedPks.clear();
    for (int i = 0; i < 52; i++) {
        unUsedPks.push_back(i);
    }
    onHands.clear();
    CommonPorkers.clear();
    iPlayerCount = ipc;
    moneys = initMoney;
    onTableMoneys.clear();
    thisTurnMoneys.clear();
    bAlive.resize(ipc, true);
    iAliveCount = ipc;
    for (int i = 0; i < ipc; i++) {
        onTableMoneys.push_back(0);
        thisTurnMoneys.push_back(0);
    }
    porkersTypes.clear();
}

void TexasHoldemData::Preflop() {
    for (int i = 0; i < iPlayerCount; i++) {
        std::vector<Poker> porkers;
        for (int j = 0; j < 2; j++) {
            Poker pk = RandomPopAPorker();
            porkers.emplace_back(pk);
        }
        OnHand oh(porkers);
        onHands.push_back(oh);

    }
}

void TexasHoldemData::Preflop_After(const std::vector<Poker>& pks) {
    for (int k = 0; k < 2; k++) {
        int pkIndex = pks[k].getDs() * 4 + pks[k].getHs();
        unUsedPks.erase(std::remove(unUsedPks.begin(), unUsedPks.end(), pkIndex), unUsedPks.end());
    }
    OnHand xoh(pks);
    onHands.push_back(xoh);
    for (int i = 1; i < iPlayerCount; i++) {
        std::vector<Poker> porkers;
        for (int j = 0; j < 2; j++) {
            Poker pk = RandomPopAPorker();
            porkers.push_back(pk);
        }
        OnHand oh(porkers);
        onHands.push_back(oh);
    }
}

void TexasHoldemData::PreflopMoneyInit(int ipc) {
    thisTurnMoneys[blindpos] = SMALLBLINDAMOUNT;
    thisTurnMoneys[(blindpos + 1) % 2] = BIGBLINDAMOUNT;
    moneys[blindpos] -= SMALLBLINDAMOUNT;
    onTableMoneys[blindpos] = SMALLBLINDAMOUNT;
    moneys[(blindpos + 1) % ipc] -= BIGBLINDAMOUNT;
    onTableMoneys[(blindpos + 1) % ipc] = BIGBLINDAMOUNT;
    startPos = (blindpos + 1) % ipc;
}

void TexasHoldemData::Flop() {
    for (int i = 0; i < 3; i++) {
        Poker pk = RandomPopAPorker();
        CommonPorkers.push_back(pk);
    }
}

void TexasHoldemData::Flop_After(const std::vector<Poker>& pks) {
    for (int k = 0; k < 3; k++) {
        if (std::find(unUsedPks.begin(), unUsedPks.end(), pks[k].getDs() * 4 + pks[k].getHs()) != unUsedPks.end()) {
            unUsedPks.erase(std::remove(unUsedPks.begin(), unUsedPks.end(), pks[k].getDs() * 4 + pks[k].getHs()), unUsedPks.end());
        }
        CommonPorkers.push_back(pks[k]);
    }
}


void TexasHoldemData::Turn() {
    Poker pk = RandomPopAPorker();
    CommonPorkers.push_back(pk);
}

void TexasHoldemData::Turn_After(const Poker& pk) {
    unUsedPks.erase(std::remove(unUsedPks.begin(), unUsedPks.end(), pk.getDs() * 4 + pk.getHs()), unUsedPks.end());
    CommonPorkers.push_back(pk);

}

void TexasHoldemData::River() {
    Poker pk = RandomPopAPorker();
    CommonPorkers.push_back(pk);
}

void TexasHoldemData::River_After(const Poker& pk) {
    unUsedPks.erase(std::remove(unUsedPks.begin(), unUsedPks.end(), pk.getDs() * 4 + pk.getHs()), unUsedPks.end());
    CommonPorkers.push_back(pk);
}

Poker TexasHoldemData::RandomGetAPorker() {
    int r = 0;
    for (int i = 1; i <= unUsedPks.size(); i++) {
        int j = rand() % i;
        if (j < 1) {
            r = unUsedPks[i - 1];
        }
    }
    unUsedPks.erase(unUsedPks.begin() + r);
    return Poker(r % 4, r / 4);
}

Poker TexasHoldemData::RandomPopAPorker() {
    Poker r = RandomGetAPorker();
    return r;
}

void TexasHoldemData::NewGame() {
    Init(iPlayerCount, moneys);
    PreflopMoneyInit(iPlayerCount);
    Preflop();
}

void TexasHoldemData::Call(int n) {
    int ca = thisTurnMoneys[(n - 1 + iPlayerCount) % iPlayerCount] - thisTurnMoneys[n];
    onTableMoneys[n] += ca;
    moneys[n] -= ca;
    thisTurnMoneys[n] += ca;
}

void TexasHoldemData::AddThisTurnMoneyToBonus() {
    for (int i = 0; i < thisTurnMoneys.size(); i++) {
        bonus += thisTurnMoneys[i];
        thisTurnMoneys[i] = 0;
    }
}

void TexasHoldemData::Fold(int n) {
    bAlive[n] = false;
    iAliveCount--;
}

void TexasHoldemData::Allin(int n) {
    int max = 0;
    int imax = 0;
    for (int i = 0; i < moneys.size(); i++) {
        if (i != n && bAlive[i]) {
            int t = moneys[i] + thisTurnMoneys[i];
            if (t > max) {
                max = t;
                imax = i;
            }
        }
    }
    int x = moneys[n] + thisTurnMoneys[n];
    int ca = max < x ? moneys[imax] + thisTurnMoneys[imax] - thisTurnMoneys[n] : moneys[n];
    onTableMoneys[n] += ca;
    moneys[n] -= ca;
    thisTurnMoneys[n] += ca;
}

void TexasHoldemData::Check(int n) {
    // No implementation required
}

void TexasHoldemData::Bet(int n, int amount) {
    moneys[n] -= amount;
    thisTurnMoneys[n] += amount;
    onTableMoneys[n] += amount;
}

void TexasHoldemData::Raise(int n, int amount) {
    moneys[n] += thisTurnMoneys[n] - amount;
    onTableMoneys[n] -= thisTurnMoneys[n] - amount;
    thisTurnMoneys[n] = amount;
}

int TexasHoldemData::EndGame() {
    AddThisTurnMoneyToBonus();
    int win = 0;
    int c = 0;
    int j = 0;
    for (int i = 0; i < iPlayerCount; i++) {
        if (bAlive[i]) {
            c++;
            j = i;
        }
    }
    if (c == 1) {
        win = j;
        moneys[win] += bonus;
    }
    else if (c > 1) {
        int bcc = 0;
        std::vector<int> iwinbc(iPlayerCount, 0);
        std::vector<int> ibc(iPlayerCount, 0);
        std::vector<int> bc(iPlayerCount, 0);
        std::vector<int> totm(iPlayerCount, 0);
        for (int i = 0; i < iPlayerCount; i++) {
            totm[i] = onTableMoneys[i];
        }
        for (int i = iPlayerCount - 1; i > 0; i--) {
            int min = INT_MAX;
            int imin = 0;
            for (j = 0; j <= i; j++) {
                if (totm[j] > 0 && totm[j] < min) {
                    min = totm[j];
                    imin = j;
                }
            }
            for (j = 0; j < iPlayerCount; j++) {
                if (onTableMoneys[j] >= min) {
                    onTableMoneys[j] -= min;
                    bc[bcc] += min;
                    ibc[bcc] += 1 << j;
                }
            }
            int t = totm[imin];
            totm[imin] = totm[i];
            totm[i] = t;
            bcc++;
        }
        for (int i = 0; i < iPlayerCount; i++) {
            //计算
            CalcPorkersCombineTypeByPorkers(i);
        }
        for (int i = 0; i < bcc; i++) {
            int itypemax = -1;
            for (int k = 0; k < iPlayerCount; k++) {
                if (ibc[i] & (1 << k)) {
                    if (itypemax == -1) {
                        itypemax = k;
                        iwinbc[i] = 1 << k;
                    }
                    else {
                        int ret = JudgeWin(k, itypemax);
                        if (ret == -1) {
                            itypemax = k;
                            iwinbc[i] = 1 << k;
                        }
                        else if (ret == 0) {
                            iwinbc[i] += 1 << k;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < bcc; i++) {
            int wc = 0;
            for (int k = 0; k < iPlayerCount; k++) {
                if (iwinbc[i] & (1 << k)) {
                    wc++;
                }
            }
            for (int k = 0; k < iPlayerCount; k++) {
                if (iwinbc[i] & (1 << k)) {
                    int ta = bc[i] / wc;
                    moneys[k] += ta;
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

void TexasHoldemData::CalcPorkersType() {
    for (int i = 0; i < iPlayerCount; i++) {
        CalcPorkersCombineTypeByPorkers(i);
    }
}

int TexasHoldemData::JudgeWhoWin(int x, int y) {
    return JudgeWin(x, y);
}

int TexasHoldemData::JudgeWin(int x, int y) {
    const PorkersCombineType& pcta = reinterpret_cast<PorkersCombineType &&>(porkersTypes[x]);
    const PorkersCombineType& pctb = reinterpret_cast<PorkersCombineType &&>(porkersTypes[y]);
    int r[] = { 1, 2, 2, 5, 1, 3, 3, 4, 5, 0 };
    if (pcta.GetType() == pctb.GetType()) {
        std::vector<int> pctaPts = pcta.GetPts();
        std::vector<int> pctbPts = pctb.GetPts();
        for (int i = 0; i < r[pcta.GetType()]; i++) {
            if (pctaPts[i] == pctbPts[i]) {
                continue;
            } else {
                return pctaPts[i] > pctbPts[i] ? -1 : 1;
            }
        }
    } else {
        return pcta.GetType() < pctb.GetType() ? -1 : 1;
    }
    return 0;
}

PorkersCombineType TexasHoldemData::GetPorkersCombineTypeByPksAndHs(std::vector<int> pksa, std::vector<int> hsa)
{
    std::vector<int> pts(5);
// 同花顺判定
    for (int i = 12; i >= 3; i--) {
        int hs = hsa[i];
        int j = 0;
        for (j = i - 1; j >= i - 4; j--) {
            // 检测有无连续点数，连续点数有无同花
            if (pksa[(j + 13) % 13] == 0 || pksa[j + 1] == 0 || (hsa[(j + 13) % 13] & hs) == 0) {
                break;
            } else {
                hs &= hsa[(j + 13) % 13];
            }
        }
        if (j < i - 4) {
            // 是同花顺
            pts[0] = i - 4;
            return PorkersCombineType(static_cast<int>(TypeName::THS), pts);
        }
    }


    // 四条判定
    bool four_a = false;
    bool four_b = false;
    for (int i = 12; i >= 0; i--){
        if (pksa[i] == 4 && !four_a){
            pts[0] = i;
            four_a = true;
        }else if (pksa[i] > 0 && !four_b){
            pts[1] = i;
            four_b = true;
        }
        if (four_a && four_b){
            break;
        }
    }
    if (four_a){
        return PorkersCombineType(static_cast<int>(TypeName::SIT), pts);
    }

    // 葫芦判定
    bool fullhouse_a = false;
    bool fullhouse_b = false;
    for (int i = 12; i >= 0; i--)
    {
        if (pksa[i] == 3 && !fullhouse_a)
        {
            pts[0] = i;
            fullhouse_a = true;
        }
        else if (pksa[i] > 1 && !fullhouse_b)
        {
            pts[1] = i;
            fullhouse_b = true;
        }
        if (fullhouse_a && fullhouse_b)
        {
            break;
        }
    }
    if (fullhouse_a && fullhouse_b){
        return PorkersCombineType(static_cast<int>(TypeName::HL), pts);
    }

    // 同花判定
    std::vector<int> hsCounts(4);
    for (int i = 12; i >= 0; i--){
        for (int j = 0; j < 4; j++){
            int x = 1 << j;
            if ((hsa[i] & x) == x){
                hsCounts[j]++;
            }
        }
    }
    int k = 0;
    for (; k < 4; k++){
        if (hsCounts[k] > 4){
            int x = 1 << k;
            int n = 0;
            for (int m = 12; m >= 0 && n < 5; m--){
                if ((hsa[m] & x) == x){
                    pts[n++] = m;
                }
            }
            break;
        }
    }
    if (k < 4){
        return PorkersCombineType(static_cast<int>(TypeName::TH), pts);
    }

    // 顺子判定
    for (int i = 12; i >= 3; i--){
        int j = 0;
        for (j = i - 1; j >= i - 4; j--){
            // 检测有无连续点数
            if (pksa[(j + 13) % 13] == 0 || pksa[j + 1] == 0){
                break;
            }
        }
        if (j < i - 4){
            // 是顺子
            pts[0] = i - 4;
            return PorkersCombineType(static_cast<int>(TypeName::SZ), pts);
        }
    }

    // 三条判定
    bool three_a = false;
    bool three_b = false;
    bool three_c = false;
    for (int i = 12; i >= 0; i--)
    {
        if (pksa[i] == 3 && !three_a){
            pts[0] = i;
            three_a = true;
        }else if (pksa[i] > 0 && !three_b){
            pts[1] = i;
            three_b = true;
        }else if (pksa[i] > 0 && !three_c){
            pts[2] = i;
            three_c = true;
        }
        if (three_a && three_b && three_c){
            break;
        }
    }
    if (three_a){
        return PorkersCombineType(static_cast<int>(TypeName::SANT), pts);
    }

    // 两对判定
    bool two_a = false;
    bool two_b = false;
    bool two_c = false;
    for (int i = 12; i >= 0; i--){
        if (pksa[i] == 2 && !two_a){
            pts[0] = i;
            two_a = true;
        }else if (pksa[i] == 2 && !two_b){
            pts[1] = i;
            two_b = true;
        }else if (pksa[i] > 0 && !two_c){
            pts[2] = i;
            two_c = true;
        }
        if (two_a && two_b && two_c)
        {
            break;
        }
    }
    if (two_a && two_b){
        return PorkersCombineType(static_cast<int>(TypeName::LD), pts);
    }

    // 对子判定
    bool pair_a = false;
    bool pair_b = false;
    bool pair_c = false;
    bool pair_d = false;
    for (int i = 12; i >= 0; i--){
        if (pksa[i] == 2 && !pair_a){
            pts[0] = i;
            pair_a = true;
        }else if (pksa[i] > 0 && !pair_b){
            pts[1] = i;
            pair_b = true;
        }else if (pksa[i] > 0 && !pair_c){
            pts[2] = i;
            pair_c = true;
        }else if (pksa[i] > 0 && !pair_d){
            pts[3] = i;
            pair_d = true;
        }
        if (pair_a && pair_b && pair_c && pair_d){
            break;
        }
    }
    if (pair_a){
        return PorkersCombineType(static_cast<int>(TypeName::DZ), pts);
    }

    // 高牌判定
    bool a = false;
    bool b = false;
    bool c = false;
    bool d = false;
    bool e = false;
    for (int i = 12; i >= 0; i--){
        if (pksa[i] > 0 && !a){
            pts[0] = i;
            a = true;
        }else if (pksa[i] > 0 && !b){
            pts[1] = i;
            b = true;
        }else if (pksa[i] > 0 && !c){
            pts[2] = i;
            c = true;
        }else if (pksa[i] > 0 && !d){
            pts[3] = i;
            d = true;
        }else if (pksa[i] > 0 && !e){
            pts[4] = i;
            e = true;
        }
        if (a && b && c && d && e)
        {
            break;
        }
    }
    return PorkersCombineType(static_cast<int>(TypeName::GP), pts);
}

void TexasHoldemData::CalcPorkersCombineTypeByPorkers(int x){
    if (!bAlive[x]) {
        std::vector<int> pts(5);
        PorkersCombineType pct(static_cast<int>(TypeName::FOLD), pts);
        porkersTypes.emplace_back(pct);
        return;
    }


    Poker* a = onHands[x].porkers.data();
    std::vector<int> pksa(13);
    std::vector<int> hsa(13);

    for (int i = 0; i < 2; i++){
        pksa[a[i].getDs()]++;
        hsa[a[i].getDs()] |= 1 << a[i].getHs();
    }

    for (int j = 0; j < CommonPorkers.size(); j++){
        Poker t = reinterpret_cast<Poker &&>(CommonPorkers[j]);
        pksa[t.getDs()]++;
        hsa[t.getDs()] |= 1 << t.getHs();
    }

    PorkersCombineType pcta = GetPorkersCombineTypeByPksAndHs(pksa, hsa);
    porkersTypes.insert(porkersTypes.begin() + x, pcta);
}





