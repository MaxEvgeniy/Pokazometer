#include "GlobalHeader.h"
#include "Buzzer.h"
#include "cmsis_os2.h"
#include <math.h>
#include "main.h"


extern TIM_HandleTypeDef htim2;

void SoundPlay(uint16_t nSound, uint8_t repeat, uint8_t pause)
{
	switch (nSound)
	{
	case BUZZERALARM:
		{
			SoundArray = AlarmSound;
			SoundArraySize = sizeof(AlarmSound) / 4;
			SoundRepeat = repeat;
			SoundPause = pause;
			SoundStep = 0;
			break;
		}
	default:
		{
			break;
		}
	}
}

void SoundSetPeriod(uint32_t period)
{
	HAL_TIM_PWM_Stop(&htim2, TIM_CHANNEL_1);
	
	if (period > 0)
	{
		htim2.Init.Prescaler = (uint32_t) round(1000000 / (2*period)) - 1;
		htim2.Init.Period = (uint32_t) round(1000000 / (period)) - 1;
		HAL_TIM_PWM_Start(&htim2, TIM_CHANNEL_1);
	}
	
	//	int cnt = 1000000 / period;
	//	HAL_TIM_PWM_Stop(&htim3, TIM_CHANNEL_1);
	//	htim3.Instance->CCR1 = cnt >> 1;
	//	htim3.Instance->ARR = cnt;
	//	if (period)
	//		HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
	
}

void SoundStop(void)
{
	SoundSetPeriod(0);
	SoundStep = 0;
	SoundArraySize = 0;
}

void BuzzerDrive(void)
{
	if (SoundArraySize>0)
	{
		SoundSetPeriod(SoundArray[SoundStep]);
		osDelay(50);
		SoundStep++;
		if (SoundStep >= SoundArraySize)
		{
			SoundSetPeriod(0);
			SoundStep = 0;
			if (SoundRepeat == 1)
			{
				SoundArraySize = 0;
			}
			else
			{
				if ((SoundRepeat > 0)&&(SoundRepeat != 255))
					SoundRepeat--;
				if (SoundPause)
					osDelay(SoundPause);
			}
		}
	}
}
