#include "GlobalHeader.h"
#include "main.h"

extern bool AlarmActive; // Если активен- сирена воет
extern bool AlarmActiveOLD; // Если активен- сирена воет

void AlarmDrive(void)
{
	if (AlarmActiveOLD != AlarmActive)
	{
		if (AlarmActive == true)
		{
			HAL_GPIO_WritePin(LightAlarm_GPIO_Port, LightAlarm_Pin, GPIO_PIN_SET);
			////////////////////////////////////////////////////////////////////////
			SoundPlay(BUZZERALARM, 1, 1);
			////////////////////////////////////////////////////////////////////////
		}
		else
		{
			HAL_GPIO_WritePin(LightAlarm_GPIO_Port, LightAlarm_Pin, GPIO_PIN_RESET);
		}
		
		AlarmActiveOLD = AlarmActive;
	}
}
