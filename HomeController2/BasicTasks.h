#ifndef BasicTasks_H/*ИМЯ ЗАГОЛОВОЧНОГО ФАЙЛА_H*/
#define BasicTasks_H/*ИМЯ ЗАГОЛОВОЧНОГО ФАЙЛА_H*/

#include "stdint.h"                                                             // Определение перемменных int
//#include "stdbool.h"                                                            // Определение булевских переменных
//#include "usb_device.h"                                                         // Определение устройства USB
//#include <stdarg.h>
//#include <stdio.h>
//#include <string.h>
//#include "stdint.h"                                                             // Определение перемменных int
//#include "w5500_driver.h"                                                       // Драйвер сетевой платы
//#include "ds18b20.h"                                                            // Датчик температуры ds18b20
//#include "onewire.h"                                                            // Сеть 1-Ware для ds18b20
//#include "MQTTClient.h"                                                         // Клиент сети MQTT
//#include "Devices.h"
//#include "Commands.h"

//bool InitializationDone=false;                                                  // Указатель на завершение инициализации
//
//extern RTC_HandleTypeDef hrtc;                                                  // Хендлер на счетчик времени
//RTC_TimeTypeDef BoardTime={0};                                                  // Время на плате
//RTC_DateTypeDef BoardDate={0};                                                  // Дата на плате
//

//
//uint16_t Timer1Ticks=0;                                                         // Тики таймера 2 
//uint16_t Timer1TicksOLD=0;                                                      // Старое значение тиков таймера 2 для блокировки повторного срабатывания 
//uint16_t Timer1TicksSec=0;                                                      // Отсчет секунд для тестовых задач 
//
//bool SendLogsFromUSB=false;                                                     // Флажок активации передачи логов по USB
//
//bool DataForTransmitReady=false;                                                // Флажок готовности данных к передаче по USB
//bool DataForReceiveReady=false;                                                 // Флажок о получении данных по USB
//extern USBD_HandleTypeDef  hUsbDeviceFS;                                        // Хендлер на USB устройство
//extern uint8_t dataToReceiveRep1[16];                                           // Массив принятых значений первого рапорта
//extern bool DataUSBCome;                                                        // Флаг наличия сообщения
//uint8_t USB_Transmit_Buffer[16];                                                // Контейнер для отправки данных по USB. Максимальная величина элемента= 256
//uint8_t USB_Receive_Buffer[16];                                                 // Контейнер для получения данных по USB
//
//bool USBLogsSend=true;//false;                                                  // Флажок отправки сообщений по USB
//
//bool AutonomousWork=false;                                                      // Флажок автономной работы платы
//

//

//////////////////////////////////////////////////////////////////////////////////
//
//////////////////////////////MQTT////////////////////////////////////////////////
//MQTTMessage* IncomingMessage;                                                   // Получаемые сообщения по MQTT
//bool IncomingMessageReady=false;                                                // Пришло новое сообщение
//
//MQTTMessage SendingMessage;                                                     // Отправляемые сообщения по MQTT
//bool SendingMessageReady=false;                                                 // Готовность отправляемого сообщения
//
//
////char SubString[50] = "/#";                                                        // Подписаться на все. Нужно заменять на необходимые подписки
//


//
//char AdrButtonSensor[] = "/ButtSens";                                           // Адрес MQTT сообщений группы сенсорных кнопок   
//
//char AdrBME280Sensor[] = "/BME280";                                             // Адрес MQTT сообщений датчика BME280 
//char AdrTempBME280Sensor[] = "/Temp";                                           // Адрес MQTT сообщений температуры датчика BME280  
//char AdrPressBME280Sensor[] = "/Press";                                         // Адрес MQTT сообщений давления датчика BME280  
//char AdrHumBME280Sensor[] = "/Hum";                                             // Адрес MQTT сообщений влажности датчика BME280
//
//char AdrDS18B20Sensor[] = "/TempDS18B20";                                       // Адрес MQTT сообщений от датчика температуры DS18B20
//

//
//char PostString[128] = ""; 
//char payload[50];
//

//
//extern char buffer_control[];                                                   // Сюда приходит входящее сообщение по MQTT
//extern uint32_t ui32_len_buffer_control;                                        // Длина сообщения
//
//#define CommandCount 10                                                         // Количество комманд в массиве
//#define SymbolCount 10                                                          // Количество букв в команде
//char DecodingCommands[CommandCount][SymbolCount];                               // Массив расшифрованных команд из присланной строки. Разделение символом "_"
//extern bool ErrorMQTT;                                                          // Наличие ошибки связи по MQTT
//
/////////////////////////////////Буфер для MQTT///////////////////////////////////
//typedef struct                                                                  // Описание датчика давления
//{
//uint8_t Index;                                                                  // Номер сообщения  
//char PostingString[128];                                                        // Строка адреса поста сообщения
//char Format[10];                                                                // Сообщение(или формат выводимого сообщения)
//float *Parametr;                                                                // Выводимый параметр 
//}TupeMQTTBuffer;
//
//uint8_t BufferMQTTCount=0;                                                      // Количество ождидающих записей в буфере
//uint8_t BufferMQTTMax=5;                                                        // Максимально возможное количество записей в буфере
//
//TupeMQTTBuffer MQTTBuffer[5];                                                   // Записи буфера MQTT                  
//////////////////////////////////////////////////////////////////////////////////
//
//////////////////////////////////DS18B20/////////////////////////////////////////
//extern OneWire_t        OneWire;
//extern uint8_t	        OneWireDevices;
//extern uint8_t          TempSensorCount;                                        // Количество термодатчиков
//extern uint16_t	        Ds18b20Timeout;                                         // Задержка для ожидания ответа датчика
////extern Ds18b20Sensor_t	ds18b20[_DS18B20_MAX_SENSORS];                          // Массив датчиков на шине
//
//uint8_t Ds18b20TryToFind=5;                                                     // Количество попыток поиска датчиков
//bool DS18B20Act=false;                                                          // Разрешение для чтения датчика. Для организации работы 
//uint16_t TimerTempSensorsDS18B20=0;                                             // Таймер чтения датчиков температуры (1= 0,5 секунды)
//////////////////////////////////////////////////////////////////////////////////
//
////////////////////////////////////BME280////////////////////////////////////////
//#define SEALEVELPRESSURE_HPA (1013.25)
//#define SEALEVELPRESSURE_PA (1013250)
//
//
//extern float TemperFloat;
//extern float PressureFloat;
//float KPressureFloat=0;
//extern float AltitudeFloat;
//extern float HumidityFloat;
//
//char MQTTAdressBME280[] = "/BME280";                                            // Адрес группы датчиков 
//char MQTTAdressPressBME280[] = "/Press";                                        // Адрес записи для давления
//char MQTTAdressTempBME280[] = "/Temp";                                          // Адре записи для температуры
//char MQTTAdressHumBME280[] = "/Hum";                                            // Адрес записи для влажности
//
//////////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////Опрос клавиатуры///////////////////////////////////
//uint16_t KeyBoardTimerMainCount=0;                                              // Основной таймер для опроса кнопок
//uint16_t KeyBoardTimerMainCountOLD=0;                                           // Старое значение основного таймера
//uint16_t KeyBoardTimerMainMax=100;//200;                                        // Максимальное значение основного таймера
////uint8_t KeyBoardTimerCount=0;                                                 // Таймер для опроса кнопок
////uint8_t KeyBoardTimerMax=100;                                                 // Максимальное значение таймера опроса кнопок
//
//uint8_t TimeFirstHotPoint=1;//30;                                               // Количество тиков таймера для достижения первой контрольной точки относительно стартовой.
//uint8_t TimeSecondHotPoint=3;//30;                                              // Количество тиков таймера для достижения второй контрольной точки относительно первой.
//uint8_t TimeFinalHotPoint=5;                                                   // Количество тиков таймера для достижения последней контрольной точки относительно второй.
//////////////////////////////////////////////////////////////////////////////////
//
//////////////////////////////////Ошибки//////////////////////////////////////////
//extern bool ErrorInitBME280;                                                   // Ошибка инициализации термодатчиков BME280
////uint32_t ErrorBME280=HAL_I2C_ERROR_NONE;                                      // Код ошибки в работе с датчиком BME280
////HAL_StatusTypeDef ErrorConnectBME280=HAL_OK;                                  // Код ошибки связи с датчиком BME280
//
//uint8_t ErrorMQTTCode=0;                                                        // Код ощибки связи по MQTT: 1- Нет связи по сети 
//int ErrorMQTTSubCode=0;                                                         // Код ощибки конкретнее.
//////////////////////////////////////////////////////////////////////////////////
//
//
//extern SPI_HandleTypeDef hspi2;                                                 // Хендлер связи по SPI с W5500
//
//uint16_t USBPausedCount;                                                        // Счетчик паузы для USB для стабильной передачи
//uint16_t USBPausedMax=100;                                                      // Максимальное значение сетчика паузы USB для стабильной передачи
//
//enum ComandsGroups                                                              // Перечень групп комманд
//{
//InformationGroup=0x01,                                                          // Группа информационных команд
//AsksGroup,                                                                      // Группа вопросов
//BoardGroup,                                                                     // Группа команд для платы
//GPIOBinarGroup,                                                                 // Группа команд для GPIO переключателей
//USBGroup                                                                        // Группа команд для USB связи
//};
//
//enum BoardCommandsGroup                                                         // Команды для платы
//{
//AutonomousWorkCommand=0x01                                                      // Команда активации автономной работы
//};
//
//enum USBCommandsGroup                                                           // Команды для платы
//{
//USBLogsSendCommand=0x01                                                         // Команда активации отправки логов по USB
//};
//
//enum AnswersGroups                                                              // Перечень групп ответов платы
//{
//MQTTConnectGroup=0x01,                                                          // Группа сообщений о связи
//
//};
//
//enum MQTTConnectGroup                                                           // Перечень групп сообщений о связи MQTT
//{
//Connected=0x01,                                                                 // Подключено или нет
//
//};
//
//enum YesNoAnswers                                                               // Универсальные ответы да и нет
//{
//Yes=0x01,                                                                       // Да
//No                                                                              // Нет
//};
//
//void SetTime(uint8_t Hours, uint8_t Minutes, uint8_t Seconds); // Установка времени
//void SetDate(uint8_t WeekDay, uint8_t Month, uint8_t Date, uint8_t Year); // Установка даты
void Initialisation(void); // Все, что нужно выполнить перед пуском- здесь
//void InitSensors(void); // Инициализация сенсоров датчиков
//void MemorySave(void); // Сохранения параметров в памяти
//void MemoryRead(void); // Чтение данных из памяти
//void Drive(void); // Основная работа
//void ReadSensors(void); // Чтение даных датчиков
//void USBSendData(void);                                                         // Прием данных по USB
//void USBReceiveData(void);                                                      // Отправка данных по USB
//void MessageUSBCook(uint8_t Mess0,
//	uint8_t Mess1,
//	uint8_t Mess2,
//	uint8_t Mess3,
//	uint8_t Mess4,
//	uint8_t Mess5,
//	uint8_t Mess6,
//	uint8_t Mess7,
//	uint8_t Mess8,
//	uint8_t Mess9,
//	uint8_t Mess10,
//	uint8_t Mess11,
//	uint8_t Mess12,
//	uint8_t Mess13,
//	uint8_t Mess14,
//	uint8_t Mess15); // Подготовка сообщения для отправки по USB
//void ReadCommandUSB(void); // Обработка полученной команды по USB
//void USBTweak(void); // Настройка работы USB по командам
//void BoardTweak(void); // Настройки общие. Платы и камня
////
////void MQTTInit(void);                                                            // Инициализация связи по MQTT
////void MQTTServise(void);                                                         // Работа связи по MQTT
//void ProcessingIncomigMessage(void); // Обработка входящих сообщений по MQTT
//void ButtonsListen(void); // Слушалка для кнопок
//void ButtonsReset(uint8_t NumderOfButton); // Сброс кнопки после регистрации нажатия
////
//////void DS18B20Drive(void);                                                      // Работа датчика DS18B20
//////bool Ds18b20Convert(void);                                                    // Получение значений с датчика DS18B20       
////
//void BME280Drive(void); // Работа датчика давления BMP280
////
////void ESP8266Drive(void);                                                        // Работа WiFi модуля
////
////void MessageMQTTCook(char PostingString[128], char Format[10], float *Parametr); // Собрать сообщение для отправки по MQTT
//void ReadCommandMQTT(void); // Чтение команд приходящих по MQTT
//void LifeCycle(void); // Счетчик времени работы переключателей и прочего
////void InitPWM(PWMGenerator Generator,uint8_t Value);                             // Инициализация ШИМ
//void RegularWork(void); // Отработка регулярных событий
//void BufferMQTTSending(void); // Отправка сообщений, содержащихся в буфере
#endif