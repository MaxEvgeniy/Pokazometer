#ifndef GLOBALDEFAULT_H
#define GLOBALDEFAULT_H

#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include "cmsis_os2.h"

#define DHCP_SOCKET			0
#define DNS_SOCKET			1
#define MQTT_SOCKET			2
#define MQTT_MESSAGE_LENGTH	20
#define MQTT_ADDRESS_LENGTH	50

#define THERMOSENSORS_COUNT_MAX	10 // Максимальное количество дачикой DS18B20 подключаемое к шине

#define BUZZERALARM			1
#define ALARMMODESET		2

typedef struct {
	uint32_t NumOfMess;
	uint8_t Length;
	char Message[MQTT_MESSAGE_LENGTH];
	char Address[MQTT_ADDRESS_LENGTH];
} MyMessage;

typedef struct
{
	uint16_t Frequency; // Требуемая частота звучания
	uint16_t Prescaler; // Предделитель для расчета ШИМа таймера
	uint16_t Period; // Период для расчета ШИМа таймера
	uint16_t Pulse; // Заполнение. Громкость 100% при заполнении на 50%
}Note; // Нота



void SensorDrive(void); // Чтение датчиков
void AlarmDrive(void); // Управление сиреной
void BuzzerDrive(void); // Управление пъезодинамиком

void W5500_Select(void); // Активировать сетевой чип
void W5500_Unselect(void); // Дезактивировать сетевой чип
void W5500_ReadBuff(uint8_t* buff, uint16_t len); // Читаем буфер сетевого чипа
void W5500_WriteBuff(uint8_t* buff, uint16_t len); // Заполняем буфер сетевого чипа
void Callback_IPAssigned(void); // Отклик при назначении адреса
void Callback_IPConflict(void); // Отклик при ошибке совпадения адресов
void W5500Init(void); // Инициализация элемента связи
void Initialization(void); // Инициализация
void ErrorsReserch(void); // Обработка ошибок

#endif