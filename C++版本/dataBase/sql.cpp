#include"sql.h"

TableManager::TableManager(){
    //初始化
    mysql_init(&mysql);
    //设置字符编码
    mysql_options(&mysql, MYSQL_SET_CHARSET_NAME, "GBK");
    if (mysql_real_connect(&mysql, host, user, pw, database_name, port, NULL, 0) == NULL){
        std::cout << "错误原因:" << mysql_error(&mysql) << std::endl;
        std::cout << "数据库连接失败" << std::endl;
        exit(-1);
    }
    else{
        std::cout << "数据库连接成功" << std::endl;
    }

}
TableManager::~TableManager(){
    mysql_close(&mysql);          //关闭与mysql的连接
}

double TableManager::get_data(int playercount, double preflopfirst, double preflopsecond, double preflopsamecolor) {
    vector<T_data>data_List;
    char sql[1024];
    T_data data;
    //构造sql语句
    sprintf(sql, "select *from  data ");
    if (mysql_query(&mysql, sql)){
        fprintf(stderr, "Failed to selete data:Error:%s\n", mysql_error(&mysql));
        return {};
    }
    //将查询到的结果集储存到res中
    MYSQL_RES* res = mysql_store_result(&mysql);
    MYSQL_ROW row;
    //获取每一行的数据
    while ((row = mysql_fetch_row(res))){
        data.playercount = atoi(row[0]);
        data.preflopfirst = strtod(row[1], NULL);
        data.preflopsecond = strtod(row[2], NULL);
        data.preflopsamecolor = strtod(row[3], NULL);
        data.historycount = strtod(row[4], NULL);
        data.winprobability = strtod(row[5], NULL);
        data_List.push_back(data);
    }
    for (int i = 0; i < data_List.size(); i++){
        if (playercount == data_List[i].playercount && preflopfirst == data_List[i].preflopfirst &&
            preflopsecond == data_List[i].preflopsecond && preflopsamecolor == data_List[i].preflopsamecolor){
            return data_List[i].winprobability;
        }
    }
}

