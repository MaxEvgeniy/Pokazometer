#include "GlobalHeader.h"
#include "stm32f1xx_hal.h"
#include "main.h"

uint16_t adc = 0;
bool ADCComplete = false;
extern bool AlarmActive;
extern ADC_HandleTypeDef hadc1;

void SensorDrive(void)
{
	if (ADCComplete == true)
	{
		ADCComplete = false; // Сбрасываем флажок о регистрации полученных значений
		HAL_ADC_Start_DMA(&hadc1, (uint32_t*)&adc, 2); // Запуск расчета АЦП
	} // Если АЦП выдал значение
	if (adc < 3000)
	{
		AlarmActive = true; // Включить тревогу
	}
	else
	{
		AlarmActive = false; // Выключить тревогу
	} // Если значение АЦП меньше заданного уровня
}
