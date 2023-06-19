#include "GlobalHeader.h"
#include "stm32f1xx_hal.h"
#include "main.h"

extern uint16_t adc;

bool AlarmActive = false; // Если активен- сирена воет

void AlarmDrive(void)
{
	if (AlarmActive == true)
	{
		HAL_GPIO_WritePin(LightAlarm_GPIO_Port, LightAlarm_Pin, GPIO_PIN_SET);
	}
	else
	{
		HAL_GPIO_WritePin(LightAlarm_GPIO_Port, LightAlarm_Pin, GPIO_PIN_RESET);
	}
	
}
