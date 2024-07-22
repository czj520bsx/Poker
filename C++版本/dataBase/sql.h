#include <iostream>
#include<vector>
#include<string>
#include<mysql.h>
using namespace std;

typedef struct T_data
{
    int playercount;
    double preflopfirst;
    double preflopsecond;
    double preflopsamecolor;
    double historycount;
    double winprobability;

}T_data;
class TableManager{
    TableManager();
    ~TableManager();
public:
   /*
   static TableManager* GetInstance()//单例模式（项目确定只有一个实体）
    {
        static TableManager  TableManager;
        return &TableManager;
    }
   
   */
    //查表
    static double get_data(int playercount, double preflopfirst, double preflopsecond, double preflopsamecolor);
private:
    static MYSQL mysql;//数据库句柄
    const char* host = "127.0.0.1";
    const char* user = "martin";
    const char* pw = "2318bsx520";
    const char* database_name = "game_db";
    const int port = 3306;
};
