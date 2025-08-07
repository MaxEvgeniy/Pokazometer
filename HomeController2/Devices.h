#ifndef DEVICES_H
#define DEVICES_H

#include "stm32f1xx_hal.h"
#include "stdint.h"
#include "stdbool.h"

//-----------------------Основная плата-----------------------------------
enum NamesOfBoards
{
	MajorMain = 0x01,
	// Плата для тестирования
	BathLighter,
	// Управление светом в ванной
	RooomRooom,
	// Управление светом в комнате
	Ventor // Плавное управление вентилятором
}; // Наименование плат
typedef struct
{
	uint8_t Address; // Адрес платы (Должен быть уникален)
	char Name[20]; // Имя платы (Может повторяться)
	uint8_t State; // Cостояние платы
	uint8_t LifeSeconds; // Время работы. Секунды
	uint8_t LifeMinutes; // Время работы. Минуты
	uint8_t LifeHours; // Время работы. Часы
	uint16_t LifeDays; // Время работы. Дни
	uint8_t LifeYears; // Время работы. Годы
}Board; // Описание платы с камнем
enum BoardStates
{
	BoardOnLine = 0x01,
	// Плата в сети, работает нормально
	BoardStateUnknown,
	// Состояние платы не отпределено
	BoardOffLine,
	// Плата отключена
	BoardInAlarm,
	// Плата в состоянии тревоги
	BoardError // Плата в состоянии ошибки
}; // Список состояний платы
enum CodeOfReturn
{
	ResultOK = 0,
	// Задача выполнена
	ResultError = 1 // Задача не выполнена
}; // Список ответов функций после работы
typedef struct
{
	uint8_t EventID; // Номер события
	bool Active; // Выполнять или нет
	uint32_t LastTime; // Время последнего запуска
	uint32_t TimePeriod; // Временная пауза между запусками
	uint32_t NextStartTime; // Время следующего запуска
	uint8_t(*Event)(void); // Событие
}RegularEvents; // Регулярно выполняемые события

//-----------------------Переключатель-----------------------------------
typedef struct
{
	uint8_t Address; // Адрес переключателя
	uint16_t Pin; // Пин переключателя
	GPIO_TypeDef* Port; // Порт переключателя
	uint8_t GPIOSwitchState; // Текущее состояние переключателя
	bool Locked; // Блокировка переключателя
	bool PostingState; // Разрешение на отправку состояния по MQTT
	bool Reversed; // Инвертированный. Вместо выключения- включается и наоборот
	uint16_t DelayOnCount; // Счетчик задержки
	uint16_t DelayOn; // Задержка на включение после выключения в микросекундах
	uint16_t ColOfSwitches; // Cчетчик переключений
	uint8_t LifeSeconds; // Общее время работы. Секунды
	uint8_t LifeMinutes; // Общее работы. Минуты
	uint8_t LifeHours; // Общее работы. Часы
	uint16_t LifeDays; // Общее работы. Дни
	uint8_t LifeYears; // Общее работы. Годы
	uint8_t WorkTimeSeconds; // Время работы. Секунды
	uint8_t WorkTimeMinutes; // Время работы. Минуты
	uint8_t WorkTimeHours; // Время работы. Часы
	uint16_t WorkTimeDays; // Время работы. Дни
	uint8_t WorkTimeYears; // Время работы. Годы
	uint32_t MaxWorkTime; // Максимально дозволенное время работы без выключения в секундах
	uint32_t MaxLifeTime; // Максимальный срок службы в секундах
}GPIOSwitch; // Описание переключателя
enum GPIOSwitchStates
{
	GPIOSwitchIsOn = 0x01,
	// Переключатель включен
	GPIOSwitchIsOff,
	// Переключатель выключен
	GPIOSwitchUnknown // Состояние переключателя неизвестно
}; // Список состояний переключателя

void GPIOSwitchOn(GPIOSwitch *GPIOSwitchIN); // Включить переключатель
void GPIOSwitchOff(GPIOSwitch *GPIOSwitchIN); // Выключить переключатель
void GPIOSwitchToggle(GPIOSwitch *GPIOSwitchIN); // Переключить переключатель

//---------------------------------Кнопка-----------------------------------
typedef struct
{
	uint8_t Address; // Адрес кнопки
	uint16_t Pin; // Пин кнопки
	GPIO_TypeDef* Port; // Порт кнопки
	uint8_t State; // Состояние кнопки
	uint16_t HotPointTime; // Счетчик времени до контрольной точки
	uint8_t HotPoint; // Счетчик пройденных контрольных точек
	bool FirstPointState; // Положение кнопки при прохождении первой контрольной точки
	bool SecondPointState; // Положение кнопки при прохождении второй контрольной точки
	bool FinalPointState; // Положение кнопки при прохождении последней контрольной точки
	uint8_t Result; // Результирующее положение кнопки
	uint8_t ShortClickScenario; // Номер сценария по короткому нажатию
	uint8_t ButtonLongPressScenario; // Номер сценария по длинному нажатию
	uint8_t DoubleClickScenario; // Номер сценария по двойному нажатию
	uint8_t ButtonHoldScenario; // Номер сценария по удержанию
	bool PostingState; // Разрешение на отправку состояния по MQTT
	uint8_t LastResult; // Последнее зарегистрированное положение кнопки
	bool Locked; // Блокировка кнопки
	uint16_t ColOfPressed; // Количество нажатий на кнопку
}Button; // Описание кнопки
enum ButtonStates
{
	ButtonIsPress = 0x01,
	// Кнопка нажата
	ButtonIsNoPress,
	// Кнопка не нажата
}; // Список состояний кнопки
enum ButtonResults
{
	ButtonUp = 0x01,
	// Кнопка не нажималась
	ButtonShortClick,
	// Короткое нажатие
	ButtonLongPress,
	// Длинное нажатие
	ButtonDoubleClick,
	// Двойное нажатие
	ButtonHold,
	// Удерживание
}; // Список результирующих положений кнопки
enum MovieSensorStates
{
	MovieIsRegistred = 0x01,
	// Движение зарегистрировано
	NoMovie,
	// Движения нет
}; // Список состояний датчика движения
uint8_t ReadButton(Button ButKey); // Считать кнопку

//---------------------------------Датчик давления, влажности и температуры-----------------------------------
typedef struct
{
	uint8_t Address; // Адрес датчика давления
	float TemperFloat; // Значение температуры
	float PressureFloat; // Значение давления
	float HumidityFloat; // Значение влажности
	bool Update; // Указатель на обновление данных с датчика
	bool PostingState; // Разрешение на отправку состояния по MQTT
}BME280Sensor; // Описание датчика давления

//---------------------------------Датчик движения-------------------------------------------------------------
typedef struct
{
	uint8_t Address; // Адрес датчика движения
	uint16_t Pin; // Пин датчика движения
	GPIO_TypeDef* Port; // Порт датчика движения
	uint8_t State; // Состояние датчика
	uint8_t LastResult; // Последнее зарегистрированное состояние датчика
	bool Locked; // Блокировка датчика
	bool PostingState; // Разрешение на отправку состояния по MQTT
	uint16_t ColOfPressed; // Количество срабатывания датчика
}MovingSensor; // Описание датчика движения
bool ReadMovingSensor(MovingSensor *MovSensor); // Считать датчик движения

//---------------------------------Датчик приближения------------------------------------------------------------
typedef struct
{
	uint8_t Address; // Адрес датчика приближения
	uint16_t Pin; // Пин датчика приближения
	GPIO_TypeDef* Port; // Порт датчика приближения
}ProximitySensor; // Описание датчика приближения
bool ReadProximitySensor(ProximitySensor ProxSensor); // Считать датчик приближения


//---------------------------------ШИМ генератор--------------------------------------------------------------
typedef struct
{
	uint8_t Address; // Адрес генератора ШИМ
	bool Active; // Флажок, показывающий включен шим или нет
	uint32_t Value; // Текущее заполнение. ШИМ
	uint32_t PWMTarget; // Требуемое значение заполнения ШИМ
	uint32_t PWMChangeStep; // Шаг изменения заполнения ШИМ
	uint32_t PWMMax; // Максимальная величина заполнения ШИМ
	uint32_t PWMMin; // Минимальная величина заполнения ШИМ
	TIM_HandleTypeDef Timer; // Опорный таймер ШИМ
	uint32_t Channel; // Номер канала генератора ШИМ
}PWMGenerator; // Шим генератор

//---------------------------------Датчик температуры--------------------------------------------------------
typedef struct
{
	uint8_t Address; // Адрес датчика
	uint8_t MACAddress[8]; // Вшитый адрес термодатчика
	float Temperature; // Значение температуры с датчика
	bool DataIsValid; // Достоверность данных
	bool PostingState; // Разрешение на отправку состояния по MQTT
}Ds18b20Sensor; // Адрес датчика температуры

//-----------------------------------Процедуры------------------------------------------------------------
void InitPWM(PWMGenerator Generator, uint8_t Value); // Инициализация генератора ШИМ
void GPIOSwitchOff(GPIOSwitch *GPIOSwitchIN); // Выключить переключатель
void GPIOSwitchOn(GPIOSwitch *GPIOSwitchIN); // Включить переключатель
void GPIOSwitchToggle(GPIOSwitch *GPIOSwitchIN); // Переключить переключатель
uint8_t ReadButton(Button ButKey); // Считать кнопку
bool ReadMovingSensor(MovingSensor *MovSensor); // Считать датчик движения
bool ReadProximitySensor(ProximitySensor ProxSensor); // Считать датчик приближения
#endif
