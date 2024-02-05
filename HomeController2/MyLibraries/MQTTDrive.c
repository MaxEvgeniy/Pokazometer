#include "MQTTDrive.h"
#include "GlobalDefault.h"
#include <stdbool.h>
#include <stdarg.h>
#include <cmsis_os2.h>
#include "mqtt_interface.h"
#include "wizchip_conf.h"
#include "dhcp.h"
#include "dns.h"
#include "socket.h"
#include "MQTTClient.h"

#define BUFFER_SIZE	2048

extern bool InitializationDone;
extern uint8_t W5500InitFlag;
uint8_t InitializationMQTTDone = 0b00011111; // Флажок инициализации клиента MQTT. При прохождении операций инициализации поэтапно обнуляется. 0- инициализировано
Network n;
MQTTClient c;
uint8_t buf[100];
uint8_t targetIP[4] = { 192, 168, 1, 159 }; //IPброкераMQTT
char SubString[] = "/#"; //подписаться на всё
uint8_t tempBuffer[BUFFER_SIZE]; // Receive Buffer
uint32_t PublicatedMessageCount = 0; // Счетчик количества отправленных сообщений
MQTTMessage* message;
MQTTMessage PublicMessage; // Сформированное сообщение для публикации через MQTT
char MyMQTTMessage[MQTT_MESSAGE_LENGTH]; // Строка с сообщением для отправки по MQTT
char MyMQTTAddress[MQTT_ADDRESS_LENGTH]; // Строка с адресом для отправки по MQTT
int8_t len = 0; // Для обработки строк и сообщений
MyMessage PubMessage; // Контейнер для обрабатываемых сообщений для помещения в очередь перед отправкой
MyMessage MessageFromQueue; // Контейнер для обрабатываемых сообщений для помещения в очередь перед отправкой
uint8_t mqtt_flag;
va_list ap;
int8_t len;
extern wiz_NetInfo net_info;
//wiz_NetInfo net_info;
uint16_t mes_id;
uint32_t dhcp_counter;
uint8_t mqtt_push_counter;
extern volatile bool ip_assigned;
extern uint8_t TestTimerCounter;
uint8_t IPAssignedTimerCounter = 0;
uint8_t DHCPState;
extern uint8_t dns[4];
extern uint8_t dns_buffer[1024];
bool DNSIsConfig = false; // Флаг о завершении конфигурации DNS
MyMessage PublicateMessage;
extern osMessageQueueId_t MQTTMessQueueHandle;
	
void messageArrived(MessageData* md)
{
	message = md->message;
	for (uint8_t i = 0; i < md->topicName->lenstring.len; i++)
		putchar(*(md->topicName->lenstring.data + i));
	//UART_Printf(" (%.*s)\r\n", (int32_t) message->payloadlen, (char*) message->payload);
}
int8_t str_printf(char *StrBuff, uint8_t BuffLen, const char *args, ...)
{
	va_start(ap, args);
	len = vsnprintf(StrBuff, BuffLen, args, ap);
	va_end(ap);
	return len;
}
void msTick_Handler(void)
{
	MilliTimer_Handler();

	if (++dhcp_counter >= 500)  // 1000 - секунда //25максимум скорости
	
	{
		dhcp_counter = 0;
		DHCP_time_handler(); // Функция для обработки тайм-аутов DHCP

		if (++mqtt_push_counter >= 10)  // Каждые 10 секунд
		{
			mqtt_push_counter = 0;
			mqtt_flag = 1;
		}
	}
}
void MQTTInit(void)
{
	// Ищем узел брокера, устанавливаем соединение с ним
	n.my_socket = 0;
	
	NewNetwork(&n, MQTT_SOCKET);
	InitializationMQTTDone = InitializationMQTTDone & 0b11111110;
	if (ConnectNetwork(&n, targetIP, 1883) == SOCK_OK)
	{
		InitializationMQTTDone = InitializationMQTTDone & 0b11111101;
		MQTTClientInit(&c, &n, 1000, buf, 100, tempBuffer, BUFFER_SIZE);
		// Подлючаемся к брокеру MQTT
		MQTTPacket_connectData data = MQTTPacket_connectData_initializer;
		data.willFlag = 0;
		data.MQTTVersion = 4; //3;
		data.clientID.cstring = (char*) "w5500-client";
		data.username.cstring = "username";
		data.password.cstring = "";
		data.keepAliveInterval = 60;
		data.cleansession = 1;
		InitializationMQTTDone = InitializationMQTTDone & 0b11111011;
		
		if (MQTTConnect(&c, &data) == SUCCESSS)
		{
			InitializationMQTTDone = InitializationMQTTDone & 0b11110111;
			if (MQTTSubscribe(&c, SubString, QOS0, messageArrived) == SUCCESSS)
			{
				InitializationMQTTDone = InitializationMQTTDone & 0b11101111;
			}
		}
	}
}
void MQTTDriver(void)
{
	if (W5500InitFlag == 0)
	{
		if (!ip_assigned)
		{
			if (IPAssignedTimerCounter != TestTimerCounter)
			{
				DHCPState = DHCP_run();
				IPAssignedTimerCounter = TestTimerCounter;
			} // Если секунда не прошла с момента последней попытки
			DNSIsConfig = false; // Параметры DNS нужно обновить
		} // Если IP адрес не назначен
		else
		{
			if (DNSIsConfig == false)
			{
				getIPfromDHCP(net_info.ip);
				getGWfromDHCP(net_info.gw);
				getSNfromDHCP(net_info.sn);
				getDNSfromDHCP(dns);
				wizchip_setnetinfo(&net_info); // Calling wizchip_setnetinfo()
				DNS_init(DNS_SOCKET, dns_buffer); // Calling DNS_init()...
				DNSIsConfig = true; // Параметры DNS актуальны
			} // Если параметры DNS нужно обновить
			
			if (InitializationMQTTDone != 0)
			{
				InitializationMQTTDone = 0b00011111;
				MQTTInit();
				if (InitializationMQTTDone == 0)
				{
					ip_assigned = false;
					osDelay(1000);
				}
			}
			else
			{
				if (MQTTYield(&c, 1000) == SUCCESSS)
				{
					// Приём топиков и поддержание соединения с брокером
					msTick_Handler();
				}
				else
				{
					InitializationMQTTDone = 0b00011111;
				}
			}
		}
	} // Если W5500 инициализирован
	else
	{
		W5500Init();
		DNSIsConfig = false; // Параметры DNS нужно обновить
	} // Если W5500 не инициализирован
	if (InitializationMQTTDone == 0)
	{
		if (osMessageQueueGetCount(MQTTMessQueueHandle) > 0)
		{
			osMessageQueueGet(MQTTMessQueueHandle, &MessageFromQueue, NULL, 100); //osWaitForever);
			PublicMessage.qos = QOS0;
			PublicMessage.id = MessageFromQueue.NumOfMess;
			PublicMessage.payloadlen = MessageFromQueue.Length;
			PublicMessage.payload = MessageFromQueue.Message;
			//MQTTPublish(&c, "homebridge/from/TestClient", &PublicMessage);
			MQTTPublish(&c, MessageFromQueue.Address, &PublicMessage);
		} // Если очередь сообщений не пустая- отправляем сообщение
	} // Если передача по MQTT инициализирована
	osDelay(1);
}
void MQTTMessageQueueEdd(void)
{
	uint8_t CharCount = 0; // Счетчик символов
	bool EndOfString = false; // Флажок о достижении конца строки
	
	for (uint8_t i = 0; i < MQTT_MESSAGE_LENGTH; i++)
	{
		if (EndOfString == false)
		{
			PubMessage.Message[i] = MyMQTTMessage[i];
			CharCount++;
			if (PubMessage.Message[i] == '\n')
			{
				EndOfString = true;
			}
		}
		else
		{
			PubMessage.Message[i] = '\n';
		}
	}
		
	PubMessage.NumOfMess = PublicatedMessageCount++;
	PubMessage.Length = CharCount;
	
	EndOfString = false; // Флажок о достижении конца строки
	
	for (uint8_t i = 0; i < MQTT_ADDRESS_LENGTH; i++)
	{
		if (EndOfString == false)
		{
			PubMessage.Address[i] = MyMQTTAddress[i];
			if (PubMessage.Address[i] == '\n')
			{
				PubMessage.Address[i] = 0;
				EndOfString = true;
			}
		}
		else
		{
			PubMessage.Address[i] = 0;
		}
	}
	
	if (PublicatedMessageCount >= 0xFFFFFFFE)
	{
		PublicatedMessageCount = 0;
	}
	
	osMessageQueuePut(MQTTMessQueueHandle, &PubMessage, NULL, 100); // osWaitForever);
}
