#ifndef GLOBALHEADER
#define GLOBALHEADER

#include "stdbool.h"
#include "stdint.h"

#define BUZZERALARM	1

void SensorDrive(void); // Чтение датчиков
void AlarmDrive(void); // Управление сиреной
void BuzzerDrive(void); // Управление пъезодинамиком
void SoundPlay(uint16_t nSound, uint8_t repeat, uint8_t pause); // Воспроизведение звука на пьезодинамике
void SoundStop(void); // Остановить воспроизведение звука на пьезодинамике
#endif
