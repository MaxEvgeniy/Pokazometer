#ifndef MQTTDRIVER_H
#define MQTTDRIVER_H

#include "MQTTClient.h"

void MQTTInit(void); // Инициализация протокола MQTT
void MQTTDriver(void); // Управление передачей данных MQTT
void messageArrived(MessageData* md); // Обработчик сообщений MQTT, удовлетворяющих подпискам клиента
int8_t str_printf(char *StrBuff, uint8_t BuffLen, const char *args, ...); // Записывает форматированную строку в буфер StrBuff
void MQTTMessageQueueEdd(void); // Добавляет в очередь сообщения для отправки по MQTT

#endif
