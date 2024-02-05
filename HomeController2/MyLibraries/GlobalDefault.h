#ifndef GLOBALDEFAULT_H
#define GLOBALDEFAULT_H

#include <stdio.h>
#include <stdint.h>

#define DHCP_SOCKET			0
#define DNS_SOCKET			1
#define MQTT_SOCKET			2
#define MQTT_MESSAGE_LENGTH	20
#define MQTT_ADDRESS_LENGTH	50

typedef struct {
	uint32_t NumOfMess;
	uint8_t Length;
	char Message[MQTT_MESSAGE_LENGTH];
	char Address[MQTT_ADDRESS_LENGTH];
} MyMessage;

void W5500_Select(void); // Активировать сетевой чип
void W5500_Unselect(void); // Дезактивировать сетевой чип
void W5500_ReadBuff(uint8_t* buff, uint16_t len); // Читаем буфер сетевого чипа
void W5500_WriteBuff(uint8_t* buff, uint16_t len); // Заполняем буфер сетевого чипа
void Callback_IPAssigned(void); // Отклик при назначении адреса
void Callback_IPConflict(void); // Отклик при ошибке совпадения адресов
void W5500Init(void); // Инициализация элемента связи
void Initialization(void); // Инициализация

#endif