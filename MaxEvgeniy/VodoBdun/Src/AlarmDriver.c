#include "GlobalHeader.h"
#include "stm32f1xx_hal.h"
#include "main.h"

extern uint16_t adc;

bool AlarmActive = false; // Если активен- сирена воет

void AlarmDrive(void)
{
	if (AlarmActive == true)
	{
		
	}
	else
	{
		
	}
	
}
