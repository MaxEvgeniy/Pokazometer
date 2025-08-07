#include "Devices.h"
#include <string.h>
#include <stdio.h>
#include "BasicTasks.h"
#include "cmsis_os2.h"

uint8_t DelayForButtons = 10; //2; // Пауза опроса кнопки для подавления дребезга
uint8_t State1; // Первое состояние кнопки
uint8_t State2; // Второе состояние кнопки

extern Board BoardDrive; // Плата
extern char strAdr[5]; // Для перевода числового адреса в строку
extern char AddressOfPost[128]; // Для формирования адреса отправки сообщений
extern char BoardWarnings[]; // Адрес MQTT сообщений для предупреждений от платы
//extern char AdrButtonSensor[]; // Адрес MQTT сообщений группы сенсорных кнопок
//extern char AdrBME280Sensor[]; // Адрес MQTT сообщений датчика BME280
//extern char AdrTempBME280Sensor[]; // Адрес MQTT сообщений температуры датчика BME280
//extern char AdrPressBME280Sensor[]; // Адрес MQTT сообщений давления датчика BME280
//extern char AdrHumBME280Sensor[]; // Адрес MQTT сообщений влажности датчика BME280
extern char AdrGPIOSwitch[]; // Адрес группы GPIO переключателей

void GPIOSwitchOn(GPIOSwitch *GPIOSwitchIN) // Включить переключатель
{
	if (GPIOSwitchIN->Locked == true) // Если переключатель заблокирован
	{
		if (GPIOSwitchIN->PostingState == true) // Если отправка сообщений включена
		{
			for (int i = 0; i < 128; i++){AddressOfPost[i] = '\0'; }
			strcat(AddressOfPost, BoardDrive.Name); // Имя группы плат
			strcat(AddressOfPost, "/");
			sprintf(strAdr, "%d", BoardDrive.Address); // Адрес платы
			strcat(AddressOfPost, strAdr);
			strcat(AddressOfPost, BoardWarnings); // Имя предупреждений
			//MessageMQTTCook(AddressOfPost, "Switch %d blocked", GPIOSwitchIN->Address); // Отправляем сообщение о блокировке
		}
	}
	else
	{
		if (GPIOSwitchIN->DelayOnCount == 0)
		{
			if (GPIOSwitchIN->Reversed == true)
			{
				HAL_GPIO_WritePin(GPIOSwitchIN->Port, GPIOSwitchIN->Pin, GPIO_PIN_RESET); // Сбросить пин
			}
			else
			{
				HAL_GPIO_WritePin(GPIOSwitchIN->Port, GPIOSwitchIN->Pin, GPIO_PIN_SET); // Установить пин
			}
			GPIOSwitchIN->GPIOSwitchState = GPIOSwitchIsOn; // Выставить статус "Включено"
			GPIOSwitchIN->WorkTimeSeconds = 0; // Обнуляем время работы. Секунды
			GPIOSwitchIN->WorkTimeMinutes = 0; // Обнуляем время работы. Минуты
			GPIOSwitchIN->WorkTimeHours = 0; // Обнуляем время работы. Часы
			GPIOSwitchIN->WorkTimeDays = 0; // Обнуляем время работы. Дни
			GPIOSwitchIN->WorkTimeYears = 0; // Обнуляем время работы. Годы
			if (GPIOSwitchIN->PostingState == true) // Если отправка сообщений включена
			{
				for (int i = 0; i < 128; i++){AddressOfPost[i] = '\0'; }
				strcat(AddressOfPost, BoardDrive.Name); // Имя группы плат
				strcat(AddressOfPost, "/");
				sprintf(strAdr, "%d", BoardDrive.Address); // Адрес платы
				strcat(AddressOfPost, strAdr);
				strcat(AddressOfPost, AdrGPIOSwitch); // Имя группы переключателей
				strcat(AddressOfPost, "/");
				sprintf(strAdr, "%d", GPIOSwitchIN->Address); // Адрес переключателя
				strcat(AddressOfPost, strAdr);
				//////////////////////////////////////////////////////////////////////////MessageMQTTCook(AddressOfPost, "SwithOn %d",1); // Отправляем сообщение о включении
			}
		}
	}
}
void GPIOSwitchOff(GPIOSwitch *GPIOSwitchIN) // Выключить переключатель
{
	if (GPIOSwitchIN->Locked == true) // Если переключатель заблокирован
	{
		if (GPIOSwitchIN->PostingState == true) // Если отправка сообщений включена
		{
			for (int i = 0; i < 128; i++){AddressOfPost[i] = '\0'; }
			strcat(AddressOfPost, BoardDrive.Name); // Имя группы плат
			strcat(AddressOfPost, "/");
			sprintf(strAdr, "%d", BoardDrive.Address); // Адрес платы
			strcat(AddressOfPost, strAdr);
			strcat(AddressOfPost, BoardWarnings); // Имя предупреждений
			//////////////////////////////////////////////////////////////////////////////////MessageMQTTCook(AddressOfPost, "Switch %d blocked",GPIOSwitchIN->Address); // Отправляем сообщение о блокировке
		}
	}
	else
	{
		if (GPIOSwitchIN->Reversed == true)
		{
			HAL_GPIO_WritePin(GPIOSwitchIN->Port, GPIOSwitchIN->Pin, GPIO_PIN_SET); // Установить пин
		}
		else
		{
			HAL_GPIO_WritePin(GPIOSwitchIN->Port, GPIOSwitchIN->Pin, GPIO_PIN_RESET); // Сбросить пин
		}
		GPIOSwitchIN->GPIOSwitchState = GPIOSwitchIsOff; // Выставить статус "Выключено"
		GPIOSwitchIN->DelayOnCount = GPIOSwitchIN->DelayOn; // Включить задержку
		if (GPIOSwitchIN->PostingState == true) // Если отправка сообщений включена
		{
			for (int i = 0; i < 128; i++){AddressOfPost[i] = '\0'; }
			strcat(AddressOfPost, BoardDrive.Name); // Имя группы плат
			strcat(AddressOfPost, "/");
			sprintf(strAdr, "%d", BoardDrive.Address); // Адрес платы
			strcat(AddressOfPost, strAdr);
			strcat(AddressOfPost, AdrGPIOSwitch); // Имя группы переключателей
			strcat(AddressOfPost, "/");
			sprintf(strAdr, "%d", GPIOSwitchIN->Address); // Адрес переключателя
			strcat(AddressOfPost, strAdr);
//			MessageMQTTCook(AddressOfPost, "SwithOff %d", 0); // Отправляем сообщение о выключении
		}
	}
}
void GPIOSwitchToggle(GPIOSwitch *GPIOSwitchIN) // Переключить переключатель
{
	switch (GPIOSwitchIN->GPIOSwitchState) // Проверка статуса переключателя
	{
	case GPIOSwitchIsOn: // Включен
		{
			GPIOSwitchOff(GPIOSwitchIN); // Выключить
			break;
		}
	case GPIOSwitchIsOff: // Выключен
		{
			GPIOSwitchOn(GPIOSwitchIN); // Включить
			break;
		}
	case GPIOSwitchUnknown: // Неизвестно
		{
			GPIOSwitchOff(GPIOSwitchIN); // Выключить
			break;
		}
	default: // Если команда не входит ни в одну из групп
		{
			;//группа операторов;
		}
	}
}
uint8_t ReadButton(Button ButKey) // Считать кнопку
{
	uint8_t ResState; // Первое состояние, второе состояние, результирующее состояние
	
	ResState = ButtonIsNoPress;
	
	if (HAL_GPIO_ReadPin(ButKey.Port, ButKey.Pin) == GPIO_PIN_SET) // Если пин активен
	{State1 = ButtonIsPress; } // Состояние кнонки "нажата"
	else
	{State1 = ButtonIsNoPress; } // Состояние кнопки "не нажата"
	
	osDelay(DelayForButtons); // Задержка для подавления дребезга
	
	if (HAL_GPIO_ReadPin(ButKey.Port, ButKey.Pin) == GPIO_PIN_SET) // Если пин активен
	{
		State2 = ButtonIsPress;
		osDelay(1);
	} // Состояние кнонки "нажата"
	else
	{State2 = ButtonIsNoPress; } // Состояние кнопки "не нажата"
	
	if (State1 == ButtonIsPress && State2 == ButtonIsPress){ResState = ButtonIsPress; }
	
	return ResState;
}
bool ReadMovingSensor(MovingSensor *MovSensor) // Считать датчик движения
{
	bool ResState = false; // Результирующее состояние
	MovSensor->State = NoMovie;
	if (HAL_GPIO_ReadPin(MovSensor->Port, MovSensor->Pin) == GPIO_PIN_SET) // Если пин активен
	{
		MovSensor->State = MovieIsRegistred;
		ResState = true;
	} // Состояние кнонки "нажата"
	return ResState;
}
bool ReadProximitySensor(ProximitySensor ProxSensor) // Считать датчик приближения
{
	bool ResState = false; // Результирующее состояние
	if (HAL_GPIO_ReadPin(ProxSensor.Port, ProxSensor.Pin) == GPIO_PIN_SET) // Если пин активен
	{ResState = true; } // Состояние кнонки "нажата"
	return ResState;
}
void InitPWM(PWMGenerator Generator, uint8_t Value)
{
	TIM_OC_InitTypeDef sConfigOC;

	sConfigOC.OCMode = TIM_OCMODE_PWM1;
	sConfigOC.Pulse = Value;
	sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
	sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
	HAL_TIM_PWM_ConfigChannel(&Generator.Timer, &sConfigOC, Generator.Channel);
	HAL_TIM_PWM_Start(&Generator.Timer, Generator.Channel);
}
