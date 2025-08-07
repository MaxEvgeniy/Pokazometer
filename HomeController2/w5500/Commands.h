#ifndef Commands_H
#define Commands_H

char CommRestartSTM[8]=         "Restart";                                      // Перезапуск процессора

char CommSwitchOn[9]=           "SwitchOn";                                     // Включение переключателя
char CommSwitchOff[10]=         "SwitchOff";                                    // Выключение переключателя
char CommSwitchToggle[10]=      "SwitchTog";                                    // Выключение переключателя

char CommSetTime[8]=            "SetTime";                                      // Установка времени
char CommSetDate[8]=            "SetDate";                                      // Установка даты

#ifdef BathLighter2Board 
char CommMainLight[10]=         "MLight";                                       // Управление основным освещением
char CommVent[5]=               "Vent";                                         // Управление вентиляцией
char CommRestartBME280[10]=     "ResBME280";                                    // Перезапуск датчика давления
#endif 

#endif