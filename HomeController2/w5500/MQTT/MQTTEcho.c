#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include "main.h"
#include "FreeRTOS.h"
#include "task.h"
#include "queue.h"
#include "MQTTFreeRTOS.h"
#include "FreeRTOS.h"
#include "cmsis_os2.h"
#include "socket.h"
#include "MQTTClient.h"

#include "Devices.h"
////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////#define MQTT_TASK 1

extern Board BoardDrive;                                                        // Плата

extern bool SendingMessageReady;                                                // Готовность отправляемого сообщения
extern char PostString[50]; 
extern MQTTMessage SendingMessage;                                              // Отправляемые сообщения по MQTT
extern char payload[50];

extern	BaseType_t ReturnMQTTTask;
extern bool IncomingMessageReady;                                               // Флажок нового сообщения
extern char SubString[];                                                        // Строка подписки

MQTTMessage message;

#define LEN_BUFFER_CONTROL             100

uint32_t ui32_value_control = 0;
uint32_t ui32_len_buffer_control = 0;
char buffer_control[LEN_BUFFER_CONTROL]={0};
uint32_t is_process_data = false;

int rc2=0;

extern uint8_t ErrorMQTTCode;                                                   // Код ощибки связи по MQTT: 1- Нет связи по сети 
extern int ErrorMQTTSubCode;                                                    // Код ощибки конкретнее. 
bool ErrorMQTT=true;                                                            // Наличие ошибки связи по MQTT
bool ConnectAlive=false;                                                        // Наличие ошибки связи по MQTT
bool Subscribed=false;                                                          // Подписка выполнена
/////////////////////////////////////////////////////////////////////////////////

void free_buffer(char* buffer, uint16_t len)
{
  uint16_t i=0;
  for(i=0;i<len;i++)
  {
    buffer[i] = 0;    
  }
}

void messageArrived(MessageData* data)
{

//printf("Message arrived on topic %.*s: %.*s\n", data->topicName->lenstring.len, data->topicName->lenstring.data,data->message->payloadlen, data->message->payload);
//uint32_t M=0;
MQTTMessage* message = data->message;  
char* data2;

/*
for (M = 0; M < data->topicName->lenstring.len; M++)
  {
  putchar(*(data->topicName->lenstring.data + M));
  }; 
*/

if(ui32_len_buffer_control>LEN_BUFFER_CONTROL)
  {
  ui32_len_buffer_control = LEN_BUFFER_CONTROL;
  }
else
  {
  ui32_len_buffer_control = (int32_t)message->payloadlen;
  }; 

data2 = (char*)message->payload;
free_buffer(buffer_control,LEN_BUFFER_CONTROL);  
memcpy(buffer_control,data2,ui32_len_buffer_control);
  //printf(" Message: %s\r\n", buffer_control);
  //is_process_data = true;  

////////////////////////////////////////////////////////////////////////////////
//HAL_GPIO_WritePin(Led1_GPIO_Port,Led1_Pin,GPIO_PIN_SET);
//osDelay(200);
//HAL_GPIO_WritePin(Led1_GPIO_Port,Led1_Pin,GPIO_PIN_RESET);
////////////////////////////////////////////////////////////////////////////////
      
IncomingMessageReady=true;      
}

static void prvMQTTEchoTask(void *pvParameters)
{
static char             clientID[20]="Stm32_Mqtt_Bath";
//char             clientID[]={'S','T','M','3','2'};//BoardDrive.Name;

MQTTClient              client;
Network                 network;
network.my_socket=7;
//network.my_socket=8;
unsigned char           sendbuf[256], readbuf[512];
MQTTPacket_connectData  connectData = MQTTPacket_connectData_initializer;
pvParameters = 0;
/////////////////////////////////////////////////////////////////////////////////char address[4] = {192,168,1,100};
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
char address[4] = {192,168,1,8};
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/////////////////////////////////////////////////////////////////////////////////
while (1)
  {    
  if(ErrorMQTT==true)
    {  
    NetworkInit(&network);
    MQTTClientInit(&client, &network, 30000, sendbuf, sizeof(sendbuf), readbuf, sizeof(readbuf));  
    if ((rc2 = NetworkConnect(&network, address, 1883))!=SOCK_OK)
      {  
      ErrorMQTT=true;
      ErrorMQTTCode=1;
      ErrorMQTTSubCode=rc2;
      ConnectAlive=false;
      Subscribed=false;
      }
    else
      {
      ErrorMQTT=false;
      };    
    };
 
  if(ErrorMQTT==false & ConnectAlive==false)
    { 
    osDelay(500);
    connectData.MQTTVersion = 3;
    connectData.clientID.cstring = clientID;
    if ((rc2 = MQTTConnect(&client, &connectData)) != 0)
      {
      ErrorMQTT=true;
      ErrorMQTTCode=3;
      ErrorMQTTSubCode=rc2;
      ConnectAlive=false;
      Subscribed=false;       
      }
    else
      {
      ErrorMQTT=false;
      ConnectAlive=true; 
      };
    };

  if(ErrorMQTT==false & ConnectAlive==true & Subscribed==false)
    {
    if ((rc2 = MQTTSubscribe(&client, SubString, 2, messageArrived)) != 0)
      {
      ErrorMQTT=true;
      ErrorMQTTCode=4;
      ErrorMQTTSubCode=rc2;
      ConnectAlive=false;
      Subscribed=false;  
      }
    else
      {
      ConnectAlive=true;  
      ErrorMQTT=false;
      Subscribed=true;
      };       
    }; 
  
  if (ErrorMQTT==false & ConnectAlive==true & SendingMessageReady==true)      
    {  
    if ((rc2 = MQTTPublish(&client, PostString, &SendingMessage))!=SUCCESS2)    // Публикация сообщения
      {             
      ErrorMQTT=true;
      ErrorMQTTCode=5;
      ErrorMQTTSubCode=rc2;
      ConnectAlive=false;
      Subscribed=false; 
      }
    else  
      {
      ErrorMQTT=false; 
      ConnectAlive=true;        
      SendingMessageReady=false; 
      };  
    }; 
  
   if(ErrorMQTT==false & ConnectAlive==true & Subscribed==true)
    { 
    if ((rc2 = MQTTYield(&client, 1000)) != 0)
      {
      ErrorMQTT=true;
      ErrorMQTTCode=6;
      ErrorMQTTSubCode=rc2;
      ConnectAlive=false;
      Subscribed=false; 
      };  
    };
  }; 
}


void vStartMQTTTasks(uint16_t usTaskStackSize, UBaseType_t uxTaskPriority)
{
BaseType_t x = 0L;
ReturnMQTTTask=xTaskCreate(prvMQTTEchoTask,	                                                // The function that implements the task.
			"MQTTEcho0",			                        // Just a text name for the task to aid debugging.
			usTaskStackSize,	                                // The stack size is defined in FreeRTOSIPConfig.h. 
			(void *)x,		                                // The task parameter, not used in this case. 
			uxTaskPriority,		                                // The priority assigned to the task is defined in FreeRTOSConfig.h. 
			NULL);				                        // The task handle is not used. 
}