/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2020 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under Ultimate Liberty license
  * SLA0044, the "License"; You may not use this file except in compliance with
  * the License. You may obtain a copy of the License at:
  *                             www.st.com/SLA0044
  *
  ******************************************************************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "cmsis_os.h"
#include "usb_device.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include "stdbool.h" 
#include "GLCD.h"                                                               // Р В Р’В Р Р†Р вЂљ?Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В±Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’ВµР В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° Р В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚вЂќР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В Р РЏ
#include "RussianText.h"                                                        // Р В Р’В Р РЋРЎвЂєР В Р’В Р вЂ™Р’ВµР В Р’В Р РЋРІР‚СњР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ў Р В Р’В Р РЋРІР‚вЂќР В Р’В Р РЋРІР‚Сћ Р В Р Р‹Р В РІР‚С™Р В Р Р‹Р РЋРІР‚СљР В Р Р‹Р В РЎвЂњР В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚?  
#include "BME280.h"
#include "DS18B20.h"
/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */
////////////////////////////////Р В Р’В Р РЋРІР‚вЂќР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В Р РЏ///////////////////////////////////
bool DisplayWorkPlace = false;
bool InitComplete = false; // Р В Р’В Р РЋРІР‚вЂќР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В Р РЏ Р В Р’В Р РЋРІР‚вЂќР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В Р вЂ°Р В Р Р‹Р В РІР‚в„– Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°
bool LCDInitComplete = false; // Р В Р’В Р РЋРІР‚вЂќР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В Р РЏ Р В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚вЂќР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В Р РЏ Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°

uint32_t BME280TimeCount = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚Сњ Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В¶Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚? Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° BME280
uint32_t BME280TimeCountOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В¶Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚? Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° BME280
uint32_t BME280TimeCountMAX = 1000; // Р В Р’В Р РЋРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р’В Р РЋРІР‚СњР В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚?Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ°Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В¶Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚? Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° BME280

float TemperFloat = 0; // Р В Р’В Р РЋРЎвЂєР В Р’В Р вЂ™Р’ВµР В Р’В Р РЋ?Р В Р’В Р РЋРІР‚вЂќР В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р РЋРІР‚СљР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°
float TemperFloatOLD = 0;

float PressureFloat = 0; // Р В Р’В Р Р†Р вЂљРЎСљР В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ
float PressureFloatOLD = 0;

float HumidityFloat = 0; // Р В Р’В Р Р†Р вЂљРІвЂћСћР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В Р вЂ°
float HumidityFloatOLD = 0;

/////////////////////////////////Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚?///////////////////////////////////////
/*
bool Key1Press=true;                                                            // Р В Р’В Р вЂ™Р’В¤Р В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚Сњ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚вЂњР В Р’В Р РЋРІР‚Сћ Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В°
uint32_t TimeKey1Press=0;
*/
uint32_t MainFrequency = 48000000; // Р В Р’В Р вЂ™Р’В§Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В° Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В¦Р В Р Р‹Р Р†Р вЂљРІвЂћвЂ“ Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р’В Р Р†РІР‚С›РІР‚вЂњР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В° Р В Р’В Р В РІР‚В  Р В Р’В Р Р†Р вЂљРЎС™Р В Р Р‹Р Р†Р вЂљР’В     

float Frequency = 0; // Р В Р’В Р вЂ™Р’В§Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°        
float FrequencyOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљРІвЂћвЂ“ 

uint16_t Prescaler = 4800 - 1; // Р В Р’В Р РЋРЎСџР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р СћРІР‚?Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ°
uint16_t PrescalerOLD = 4800 - 1; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚вЂќР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р СћРІР‚?Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р РЏ

uint16_t DeltaPrescaler = 500; // Р В Р’В ?Р В Р’В Р вЂ™Р’В·Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚вЂќР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р СћРІР‚?Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р РЏ

uint16_t FastTimerCount = 0;
uint16_t FastTimerCountOLD = 0;

uint16_t Key1PressCount = 0; // Р В Р’В Р РЋРІвЂћСћР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В°
uint16_t Key1PressCountOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’В° Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В°                                            

uint8_t short_state = 0;
uint8_t long_state = 0;
uint32_t time_key1 = 0;

////////////////////////////////Р В Р’В Р РЋРІР‚С”Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В±Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ/////////////////////////////////////
char FloatChars[32]; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В РЎвЂњР В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В» Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р РЏ Р В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В±Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В Р РЏ
uint8_t ScreensCol = 4; // Р В РЎв„ўР В РЎвЂўР В Р’В»Р В РЎвЂ?Р РЋРІР‚РЋР В Р’ВµР РЋР С“Р РЋРІР‚С™Р В Р вЂ Р В РЎвЂў Р В РўвЂ?Р В РЎвЂўР РЋР С“Р РЋРІР‚С™Р РЋРЎвЂњР В РЎвЂ”Р В Р вЂ¦Р РЋРІР‚в„–Р РЋРІР‚В¦ Р В РЎвЂќ Р В РЎвЂ?Р РЋР С“Р В РЎвЂ”Р В РЎвЂўР В Р’В»Р РЋР Р‰Р В Р’В·Р В РЎвЂўР В Р вЂ Р В Р’В°Р В Р вЂ¦Р В РЎвЂ?Р РЋР вЂ№ Р РЋР РЉР В РЎвЂќР РЋР вЂљР В Р’В°Р В Р вЂ¦Р В РЎвЂўР В Р вЂ 
uint8_t ActiveScreen = 0; // Р В РЎСљР В РЎвЂўР В РЎ?Р В Р’ВµР РЋР вЂљ Р В Р’В°Р В РЎвЂќР РЋРІР‚С™Р В РЎвЂ?Р В Р вЂ Р В Р вЂ¦Р В РЎвЂўР В РЎвЂ“Р В РЎвЂў Р РЋР РЉР В РЎвЂќР РЋР вЂљР В Р’В°Р В Р вЂ¦Р В Р’В°

/////////////////////////////////Р В Р’В Р РЋРІвЂћСћР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р РЋРІР‚СљР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°/////////////////////////////////////
uint8_t Key1State = 0; // Р В Р’В Р В Р вЂ№Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1
uint8_t Key1StateOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1

uint8_t Key2State = 0; // Р В Р’В Р В Р вЂ№Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1                                                            
uint8_t Key2StateOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1

uint8_t Key3State = 0; // Р В Р’В Р В Р вЂ№Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1
uint8_t Key3StateOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1

uint8_t Key4State = 0; // Р В Р’В Р В Р вЂ№Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1
uint8_t Key4StateOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1

uint8_t Key5State = 0; // Р В Р’В Р В Р вЂ№Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1
uint8_t Key5StateOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1

uint8_t Key6State = 0; // Р В Р’В Р В Р вЂ№Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1
uint8_t Key6StateOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р РЏР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р РЋРІР‚? 1

uint8_t Key_Count = 10; // Р В Р’В Р Р†Р вЂљРІР‚СњР В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В¶Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚? Р В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ў Р В Р’В Р СћРІР‚?Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В±Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В·Р В Р’В Р РЋРІР‚вЂњР В Р’В Р вЂ™Р’В°

///////////////////////////////Р В Р’В Р вЂ™Р’В Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™///////////////////////////////////////
uint16_t ExpenditureLitres = 0; // Р В Р’В Р РЋРІР‚С”Р В Р’В Р вЂ™Р’В±Р В Р Р‹Р Р†Р вЂљР’В°Р В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚? Р В Р’В Р В РІР‚В  Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В¦
uint16_t ExpenditureLitresOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В±Р В Р Р‹Р Р†Р вЂљР’В°Р В Р’В Р вЂ™Р’ВµР В Р’В Р РЋРІР‚вЂњР В Р’В Р РЋРІР‚Сћ Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’В° Р В Р’В Р В РІР‚В  Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В¦
float ExpenditureLiMin = 0; // Р В Р’В Р вЂ™Р’В Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚? Р В Р’В Р В РІР‚В  Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В¦ Р В Р’В Р В РІР‚В  Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В¦Р В Р Р‹Р РЋРІР‚СљР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р РЋРІР‚Сљ
float ExpenditureLiMinOLD = 0; // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В·Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚? Р В Р’В Р В РІР‚В  Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В¦ Р В Р’В Р В РІР‚В  Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В¦Р В Р Р‹Р РЋРІР‚СљР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р РЋРІР‚Сљ


float KoefExpend = 6.6; // Р В Р’В Р РЋРІвЂћСћР В Р’В Р РЋРІР‚СћР В Р Р‹Р В Р Р‰Р В Р Р‹Р Р†Р вЂљРЎвЂєР В Р Р‹Р Р†Р вЂљРЎвЂєР В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р Р‹Р Р†Р вЂљРЎв„ў Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В° Р В Р’В Р РЋРІвЂћСћР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»-Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В±Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В  Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В° Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™

extern float Temp[MAXDEVICES_ON_THE_BUS];
float OLDTemp[MAXDEVICES_ON_THE_BUS];
/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
I2C_HandleTypeDef hi2c1;

RTC_HandleTypeDef hrtc;

TIM_HandleTypeDef htim1;
TIM_HandleTypeDef htim13;

UART_HandleTypeDef huart2;

SRAM_HandleTypeDef hsram1;

/* Definitions for MainTask */
osThreadId_t MainTaskHandle;
const osThreadAttr_t MainTask_attributes = {
	.name = "MainTask",
	.stack_size = 200 * 4,
	.priority = (osPriority_t) osPriorityNormal,
};
/* Definitions for DisplayTask */
osThreadId_t DisplayTaskHandle;
const osThreadAttr_t DisplayTask_attributes = {
	.name = "DisplayTask",
	.stack_size = 200 * 4,
	.priority = (osPriority_t) osPriorityNormal1,
};
/* Definitions for KeyboardTask */
osThreadId_t KeyboardTaskHandle;
const osThreadAttr_t KeyboardTask_attributes = {
	.name = "KeyboardTask",
	.stack_size = 200 * 4,
	.priority = (osPriority_t) osPriorityNormal2,
};
/* USER CODE BEGIN PV */

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_FSMC_Init(void);
static void MX_RTC_Init(void);
static void MX_TIM1_Init(void);
static void MX_TIM13_Init(void);
static void MX_I2C1_Init(void);
static void MX_USART2_UART_Init(void);
void StartMainTask(void *argument);
void StartDisplayTask(void *argument);
void StartKeyboardTask(void *argument);

/* USER CODE BEGIN PFP */

/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
	/* USER CODE BEGIN 1 */

	/* USER CODE END 1 */

	/* MCU Configuration--------------------------------------------------------*/

	/* Reset of all peripherals, Initializes the Flash interface and the Systick. */
	HAL_Init();

	/* USER CODE BEGIN Init */

	/* USER CODE END Init */

	/* Configure the system clock */
	SystemClock_Config();

	/* USER CODE BEGIN SysInit */

	/* USER CODE END SysInit */

	/* Initialize all configured peripherals */
	MX_GPIO_Init();
	MX_FSMC_Init();
	MX_RTC_Init();
	MX_TIM1_Init();
	MX_TIM13_Init();
	MX_I2C1_Init();
	MX_USART2_UART_Init();
	/* USER CODE BEGIN 2 */

	/* USER CODE END 2 */

	/* Init scheduler */
	osKernelInitialize();

	/* USER CODE BEGIN RTOS_MUTEX */
	  /* add mutexes, ... */
	/* USER CODE END RTOS_MUTEX */

	/* USER CODE BEGIN RTOS_SEMAPHORES */
	  /* add semaphores, ... */
	/* USER CODE END RTOS_SEMAPHORES */

	/* USER CODE BEGIN RTOS_TIMERS */
	  /* start timers, add new ones, ... */
	/* USER CODE END RTOS_TIMERS */

	/* USER CODE BEGIN RTOS_QUEUES */
	  /* add queues, ... */
	/* USER CODE END RTOS_QUEUES */

	/* Create the thread(s) */
	/* creation of MainTask */
	MainTaskHandle = osThreadNew(StartMainTask, NULL, &MainTask_attributes);

	/* creation of DisplayTask */
	DisplayTaskHandle = osThreadNew(StartDisplayTask, NULL, &DisplayTask_attributes);

	/* creation of KeyboardTask */
	KeyboardTaskHandle = osThreadNew(StartKeyboardTask, NULL, &KeyboardTask_attributes);

	/* USER CODE BEGIN RTOS_THREADS */
	  /* add threads, ... */
	/* USER CODE END RTOS_THREADS */

	/* USER CODE BEGIN RTOS_EVENTS */
	/* add events, ... */
	/* USER CODE END RTOS_EVENTS */

	/* Start scheduler */
	osKernelStart();
	/* We should never get here as control is now taken by the scheduler */
	/* Infinite loop */
	/* USER CODE BEGIN WHILE */
	while (1)
	{
		/* USER CODE END WHILE */

		/* USER CODE BEGIN 3 */
	}
	/* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
	RCC_OscInitTypeDef RCC_OscInitStruct = { 0 };
	RCC_ClkInitTypeDef RCC_ClkInitStruct = { 0 };
	RCC_PeriphCLKInitTypeDef PeriphClkInit = { 0 };

	/** Initializes the RCC Oscillators according to the specified parameters
	* in the RCC_OscInitTypeDef structure.
	*/
	RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_LSI | RCC_OSCILLATORTYPE_HSE;
	RCC_OscInitStruct.HSEState = RCC_HSE_ON;
	RCC_OscInitStruct.HSEPredivValue = RCC_HSE_PREDIV_DIV1;
	RCC_OscInitStruct.HSIState = RCC_HSI_ON;
	RCC_OscInitStruct.LSIState = RCC_LSI_ON;
	RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
	RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSE;
	RCC_OscInitStruct.PLL.PLLMUL = RCC_PLL_MUL6;
	if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
	{
		Error_Handler();
	}

	/** Initializes the CPU, AHB and APB buses clocks
	*/
	RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK | RCC_CLOCKTYPE_SYSCLK
	                            | RCC_CLOCKTYPE_PCLK1 | RCC_CLOCKTYPE_PCLK2;
	RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
	RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
	RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV2;
	RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

	if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_1) != HAL_OK)
	{
		Error_Handler();
	}
	PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_RTC | RCC_PERIPHCLK_USB;
	PeriphClkInit.RTCClockSelection = RCC_RTCCLKSOURCE_LSI;
	PeriphClkInit.UsbClockSelection = RCC_USBCLKSOURCE_PLL;
	if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
	{
		Error_Handler();
	}
}

/**
  * @brief I2C1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_I2C1_Init(void)
{

	/* USER CODE BEGIN I2C1_Init 0 */

	/* USER CODE END I2C1_Init 0 */

	/* USER CODE BEGIN I2C1_Init 1 */

	/* USER CODE END I2C1_Init 1 */
	hi2c1.Instance = I2C1;
	hi2c1.Init.ClockSpeed = 100000;
	hi2c1.Init.DutyCycle = I2C_DUTYCYCLE_2;
	hi2c1.Init.OwnAddress1 = 0;
	hi2c1.Init.AddressingMode = I2C_ADDRESSINGMODE_7BIT;
	hi2c1.Init.DualAddressMode = I2C_DUALADDRESS_DISABLE;
	hi2c1.Init.OwnAddress2 = 0;
	hi2c1.Init.GeneralCallMode = I2C_GENERALCALL_DISABLE;
	hi2c1.Init.NoStretchMode = I2C_NOSTRETCH_DISABLE;
	if (HAL_I2C_Init(&hi2c1) != HAL_OK)
	{
		Error_Handler();
	}
	/* USER CODE BEGIN I2C1_Init 2 */

	/* USER CODE END I2C1_Init 2 */

}

/**
  * @brief RTC Initialization Function
  * @param None
  * @retval None
  */
static void MX_RTC_Init(void)
{

	/* USER CODE BEGIN RTC_Init 0 */

	/* USER CODE END RTC_Init 0 */

	RTC_TimeTypeDef sTime = { 0 };
	RTC_DateTypeDef DateToUpdate = { 0 };

	/* USER CODE BEGIN RTC_Init 1 */

	/* USER CODE END RTC_Init 1 */

	/** Initialize RTC Only
	*/
	hrtc.Instance = RTC;
	hrtc.Init.AsynchPrediv = RTC_AUTO_1_SECOND;
	hrtc.Init.OutPut = RTC_OUTPUTSOURCE_ALARM;
	if (HAL_RTC_Init(&hrtc) != HAL_OK)
	{
		Error_Handler();
	}

	/* USER CODE BEGIN Check_RTC_BKUP */
    
	/* USER CODE END Check_RTC_BKUP */

	/** Initialize RTC and set the Time and Date
	*/
	sTime.Hours = 0x0;
	sTime.Minutes = 0x0;
	sTime.Seconds = 0x0;

	if (HAL_RTC_SetTime(&hrtc, &sTime, RTC_FORMAT_BCD) != HAL_OK)
	{
		Error_Handler();
	}
	DateToUpdate.WeekDay = RTC_WEEKDAY_MONDAY;
	DateToUpdate.Month = RTC_MONTH_JANUARY;
	DateToUpdate.Date = 0x1;
	DateToUpdate.Year = 0x0;

	if (HAL_RTC_SetDate(&hrtc, &DateToUpdate, RTC_FORMAT_BCD) != HAL_OK)
	{
		Error_Handler();
	}
	/* USER CODE BEGIN RTC_Init 2 */

	/* USER CODE END RTC_Init 2 */

}

/**
  * @brief TIM1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM1_Init(void)
{

	/* USER CODE BEGIN TIM1_Init 0 */

	/* USER CODE END TIM1_Init 0 */

	TIM_MasterConfigTypeDef sMasterConfig = { 0 };

	/* USER CODE BEGIN TIM1_Init 1 */

	/* USER CODE END TIM1_Init 1 */
	htim1.Instance = TIM1;
	htim1.Init.Prescaler = 4800 - 1;
	htim1.Init.CounterMode = TIM_COUNTERMODE_DOWN;
	htim1.Init.Period = 9999;
	htim1.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
	htim1.Init.RepetitionCounter = 0;
	htim1.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
	if (HAL_TIM_OnePulse_Init(&htim1, TIM_OPMODE_SINGLE) != HAL_OK)
	{
		Error_Handler();
	}
	sMasterConfig.MasterOutputTrigger = TIM_TRGO_ENABLE;
	sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_ENABLE;
	if (HAL_TIMEx_MasterConfigSynchronization(&htim1, &sMasterConfig) != HAL_OK)
	{
		Error_Handler();
	}
	/* USER CODE BEGIN TIM1_Init 2 */

	/* USER CODE END TIM1_Init 2 */

}

/**
  * @brief TIM13 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM13_Init(void)
{

	/* USER CODE BEGIN TIM13_Init 0 */

	/* USER CODE END TIM13_Init 0 */

	/* USER CODE BEGIN TIM13_Init 1 */

	/* USER CODE END TIM13_Init 1 */
	htim13.Instance = TIM13;
	htim13.Init.Prescaler = 4800 - 1;
	htim13.Init.CounterMode = TIM_COUNTERMODE_UP;
	htim13.Init.Period = 65535 - 1;
	htim13.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
	htim13.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
	if (HAL_TIM_Base_Init(&htim13) != HAL_OK)
	{
		Error_Handler();
	}
	if (HAL_TIM_OnePulse_Init(&htim13, TIM_OPMODE_SINGLE) != HAL_OK)
	{
		Error_Handler();
	}
	/* USER CODE BEGIN TIM13_Init 2 */

	/* USER CODE END TIM13_Init 2 */

}

/**
  * @brief USART2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART2_UART_Init(void)
{

	/* USER CODE BEGIN USART2_Init 0 */

	/* USER CODE END USART2_Init 0 */

	/* USER CODE BEGIN USART2_Init 1 */

	/* USER CODE END USART2_Init 1 */
	huart2.Instance = USART2;
	huart2.Init.BaudRate = 115200;
	huart2.Init.WordLength = UART_WORDLENGTH_8B;
	huart2.Init.StopBits = UART_STOPBITS_1;
	huart2.Init.Parity = UART_PARITY_NONE;
	huart2.Init.Mode = UART_MODE_TX_RX;
	huart2.Init.HwFlowCtl = UART_HWCONTROL_NONE;
	huart2.Init.OverSampling = UART_OVERSAMPLING_16;
	if (HAL_HalfDuplex_Init(&huart2) != HAL_OK)
	{
		Error_Handler();
	}
	/* USER CODE BEGIN USART2_Init 2 */

	/* USER CODE END USART2_Init 2 */

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
	GPIO_InitTypeDef GPIO_InitStruct = { 0 };
	/* USER CODE BEGIN MX_GPIO_Init_1 */
	/* USER CODE END MX_GPIO_Init_1 */

	  /* GPIO Ports Clock Enable */
	__HAL_RCC_GPIOE_CLK_ENABLE();
	__HAL_RCC_GPIOC_CLK_ENABLE();
	__HAL_RCC_GPIOA_CLK_ENABLE();
	__HAL_RCC_GPIOD_CLK_ENABLE();
	__HAL_RCC_GPIOB_CLK_ENABLE();

	/*Configure GPIO pin Output Level */
	HAL_GPIO_WritePin(GPIOE, KeyPulse02_Pin | KeyPulse01_Pin, GPIO_PIN_RESET);

	/*Configure GPIO pin Output Level */
	HAL_GPIO_WritePin(GPIOB, BME280_Power_Pin | TFT_LED_ADJ_Pin, GPIO_PIN_SET);

	/*Configure GPIO pins : KeyScan01_Pin KeyScan02_Pin KeyScan03_Pin */
	GPIO_InitStruct.Pin = KeyScan01_Pin | KeyScan02_Pin | KeyScan03_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	HAL_GPIO_Init(GPIOE, &GPIO_InitStruct);

	/*Configure GPIO pins : KeyPulse02_Pin KeyPulse01_Pin */
	GPIO_InitStruct.Pin = KeyPulse02_Pin | KeyPulse01_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
	HAL_GPIO_Init(GPIOE, &GPIO_InitStruct);

	/*Configure GPIO pin : TickIncom_Pin */
	GPIO_InitStruct.Pin = TickIncom_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
	GPIO_InitStruct.Pull = GPIO_PULLDOWN;
	HAL_GPIO_Init(TickIncom_GPIO_Port, &GPIO_InitStruct);

	/*Configure GPIO pin : DHT22_Data_Pin */
	GPIO_InitStruct.Pin = DHT22_Data_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	HAL_GPIO_Init(DHT22_Data_GPIO_Port, &GPIO_InitStruct);

	/*Configure GPIO pin : Schetchik_Pin */
	GPIO_InitStruct.Pin = Schetchik_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
	GPIO_InitStruct.Pull = GPIO_PULLDOWN;
	HAL_GPIO_Init(Schetchik_GPIO_Port, &GPIO_InitStruct);

	/*Configure GPIO pin : BME280_Power_Pin */
	GPIO_InitStruct.Pin = BME280_Power_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_HIGH;
	HAL_GPIO_Init(BME280_Power_GPIO_Port, &GPIO_InitStruct);

	/*Configure GPIO pin : TFT_LED_ADJ_Pin */
	GPIO_InitStruct.Pin = TFT_LED_ADJ_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
	HAL_GPIO_Init(TFT_LED_ADJ_GPIO_Port, &GPIO_InitStruct);

	/* USER CODE BEGIN MX_GPIO_Init_2 */
	/* USER CODE END MX_GPIO_Init_2 */
}

/* FSMC initialization function */
static void MX_FSMC_Init(void)
{

	/* USER CODE BEGIN FSMC_Init 0 */

	/* USER CODE END FSMC_Init 0 */

	FSMC_NORSRAM_TimingTypeDef Timing = { 0 };

	/* USER CODE BEGIN FSMC_Init 1 */

	/* USER CODE END FSMC_Init 1 */

	/** Perform the SRAM1 memory initialization sequence
	*/
	hsram1.Instance = FSMC_NORSRAM_DEVICE;
	hsram1.Extended = FSMC_NORSRAM_EXTENDED_DEVICE;
	/* hsram1.Init */
	hsram1.Init.NSBank = FSMC_NORSRAM_BANK1;
	hsram1.Init.DataAddressMux = FSMC_DATA_ADDRESS_MUX_DISABLE;
	hsram1.Init.MemoryType = FSMC_MEMORY_TYPE_SRAM;
	hsram1.Init.MemoryDataWidth = FSMC_NORSRAM_MEM_BUS_WIDTH_16;
	hsram1.Init.BurstAccessMode = FSMC_BURST_ACCESS_MODE_DISABLE;
	hsram1.Init.WaitSignalPolarity = FSMC_WAIT_SIGNAL_POLARITY_LOW;
	hsram1.Init.WrapMode = FSMC_WRAP_MODE_DISABLE;
	hsram1.Init.WaitSignalActive = FSMC_WAIT_TIMING_BEFORE_WS;
	hsram1.Init.WriteOperation = FSMC_WRITE_OPERATION_ENABLE;
	hsram1.Init.WaitSignal = FSMC_WAIT_SIGNAL_DISABLE;
	hsram1.Init.ExtendedMode = FSMC_EXTENDED_MODE_DISABLE;
	hsram1.Init.AsynchronousWait = FSMC_ASYNCHRONOUS_WAIT_DISABLE;
	hsram1.Init.WriteBurst = FSMC_WRITE_BURST_DISABLE;
	/* Timing */
	Timing.AddressSetupTime = 15;
	Timing.AddressHoldTime = 15;
	Timing.DataSetupTime = 255;
	Timing.BusTurnAroundDuration = 15;
	Timing.CLKDivision = 16;
	Timing.DataLatency = 17;
	Timing.AccessMode = FSMC_ACCESS_MODE_A;
	/* ExtTiming */

	if (HAL_SRAM_Init(&hsram1, &Timing, NULL) != HAL_OK)
	{
		Error_Handler();
	}

	/** Disconnect NADV
	*/

	__HAL_AFIO_FSMCNADV_DISCONNECTED();

	/* USER CODE BEGIN FSMC_Init 2 */

	/* USER CODE END FSMC_Init 2 */
}

/* USER CODE BEGIN 4 */

void FrecMethodA(void)                                                         // Р В Р’В ?Р В Р’В Р вЂ™Р’В·Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљРІвЂћвЂ“ Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚вЂќР В Р Р‹Р РЋРІР‚СљР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ°Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В . Р В Р’В Р РЋРЎСџР В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ў Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’В° Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚вЂќР В Р Р‹Р РЋРІР‚СљР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ°Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В  Р В Р’В Р В РІР‚В  Р В Р Р‹Р В РЎвЂњР В Р’В Р вЂ™Р’ВµР В Р’В Р РЋРІР‚СњР В Р Р‹Р РЋРІР‚СљР В Р’В Р В РІР‚В¦Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’Вµ
{
	FastTimerCount = __HAL_TIM_GET_COUNTER(&htim13); // Р В Р’В Р Р†Р вЂљРІР‚СњР В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р В РІР‚В  Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р Р‹Р Р†Р вЂљ?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В±Р В Р Р‹Р Р†Р вЂљРІвЂћвЂ“Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚вЂњР В Р’В Р РЋРІР‚Сћ Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р’В Р Р†РІР‚С›РІР‚вЂњР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°
	///////////////////////Р В Р’В Р вЂ™Р’В Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ў Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљРІвЂћвЂ“///////////////////////////////////////////
	if (FastTimerCount > 0)
	{
		Frequency = (MainFrequency * 60) / (FastTimerCount*(htim13.Init.Prescaler + 1));  
		/*
		if(FastTimerCount>3000)
		  {
		  Prescaler=Prescaler-DeltaPrescaler;                                         // Р В Р’В Р В РІвЂљВ¬Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р Р‹Р В Р вЂ°Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’ВµР В Р’В Р РЋ? Р В Р’В Р РЋРІР‚вЂќР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р СћРІР‚?Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ° Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В° Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р РЋРІР‚Сљ
		  if(Prescaler>65535){Prescaler=65535;};
 
		  htim13.Init.Prescaler = Prescaler;
		  if (HAL_TIM_Base_Init(&htim13) != HAL_OK){Error_Handler();};
		  if (HAL_TIM_OnePulse_Init(&htim13, TIM_OPMODE_SINGLE) != HAL_OK){Error_Handler();};
		  };
  
		  if(FastTimerCount<1000)
		  {
		  Prescaler=Prescaler-DeltaPrescaler;                                         // Р В Р’В Р В РІвЂљВ¬Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’ВµР В Р’В Р РЋ? Р В Р’В Р РЋРІР‚вЂќР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р СћРІР‚?Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ° Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В° Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р Р‹Р В Р вЂ°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р РЋРІР‚Сљ
		  if(Prescaler<100){Prescaler=100;};
		  htim13.Init.Prescaler = Prescaler;
		  if (HAL_TIM_Base_Init(&htim13) != HAL_OK){Error_Handler();};
		  if (HAL_TIM_OnePulse_Init(&htim13, TIM_OPMODE_SINGLE) != HAL_OK){Error_Handler();};
		  };
		  */
	}
	////////////////////////////////////////////////////////////////////////////////
	__HAL_TIM_SET_COUNTER(&htim13, 0x0000); // Р В Р’В Р РЋРІР‚С”Р В Р’В Р вЂ™Р’В±Р В Р’В Р В РІР‚В¦Р В Р Р‹Р РЋРІР‚СљР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В Р вЂ° Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚Сњ Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р’В Р Р†РІР‚С›РІР‚вЂњР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°
	HAL_TIM_Base_Start_IT(&htim13);   
}

void Flowmeter(void)                                                           // Р В Р’В Р вЂ™Р’В Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™
{
	FastTimerCount = __HAL_TIM_GET_COUNTER(&htim13); // Р В Р’В Р Р†Р вЂљРІР‚СњР В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р В РІР‚В  Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р Р‹Р Р†Р вЂљ?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’Вµ Р В Р’В Р вЂ™Р’В±Р В Р Р‹Р Р†Р вЂљРІвЂћвЂ“Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚вЂњР В Р’В Р РЋРІР‚Сћ Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р’В Р Р†РІР‚С›РІР‚вЂњР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°
	///////////////////////Р В Р’В Р вЂ™Р’В Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ў Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљРІвЂћвЂ“///////////////////////////////////////////
	if (FastTimerCount > 0)
	{
		Frequency = (MainFrequency) / (FastTimerCount*(htim13.Init.Prescaler + 1));  
		ExpenditureLiMin = Frequency / KoefExpend; // Р В Р’В Р РЋРЎв„ўР В Р’В Р РЋРІР‚вЂњР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р’В Р В РІР‚В¦Р В Р Р‹Р Р†Р вЂљРІвЂћвЂ“Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?
	}
	ExpenditureLitres =  (uint16_t) round(Key1PressCount / KoefExpend); // Р В Р’В Р РЋРІР‚С”Р В Р’В Р вЂ™Р’В±Р В Р Р‹Р Р†Р вЂљР’В°Р В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚? 

	////////////////////////////////////////////////////////////////////////////////
	__HAL_TIM_SET_COUNTER(&htim13, 0x0000); // Р В Р’В Р РЋРІР‚С”Р В Р’В Р вЂ™Р’В±Р В Р’В Р В РІР‚В¦Р В Р Р‹Р РЋРІР‚СљР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В Р вЂ° Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚Сњ Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В°Р В Р’В Р Р†РІР‚С›РІР‚вЂњР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°
	HAL_TIM_Base_Start_IT(&htim13);   
}

void KeyBoardRead(void)
{
	uint8_t Changing = 0;
	uint8_t TestKey1, TestKey2, TestKey3, TestKey4, TestKey5, TestKey6;  
	TestKey1 = 0; 
	TestKey2 = 0;
	TestKey3 = 0;
	TestKey4 = 0;
	TestKey5 = 0;
	TestKey6 = 0;

	HAL_GPIO_WritePin(GPIOE, KeyPulse01_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(GPIOE, KeyPulse02_Pin, GPIO_PIN_SET);

	osDelay(1);

	if (HAL_GPIO_ReadPin(GPIOE, KeyScan01_Pin) == GPIO_PIN_SET){TestKey1 = 1; }
	else{TestKey1 = 0; }
	if (HAL_GPIO_ReadPin(GPIOE, KeyScan02_Pin) == GPIO_PIN_SET){TestKey2 = 1; }
	else{TestKey2 = 0; }
	if (HAL_GPIO_ReadPin(GPIOE, KeyScan03_Pin) == GPIO_PIN_SET){TestKey3 = 1; }
	else{TestKey3 = 0; }
	
	HAL_GPIO_WritePin(GPIOE, KeyPulse01_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOE, KeyPulse02_Pin, GPIO_PIN_RESET);

	osDelay(1);
	if (HAL_GPIO_ReadPin(GPIOE, KeyScan01_Pin) == GPIO_PIN_SET){TestKey4 = 1; }
	else{TestKey4 = 0; }
	;
	if (HAL_GPIO_ReadPin(GPIOE, KeyScan02_Pin) == GPIO_PIN_SET){TestKey5 = 1; }
	else{TestKey5 = 0; }
	;
	if (HAL_GPIO_ReadPin(GPIOE, KeyScan03_Pin) == GPIO_PIN_SET){TestKey6 = 1; }
	else{TestKey6 = 0; }

	if (TestKey1 != Key1State || TestKey2 != Key2State || TestKey3 != Key3State ||
	    TestKey4 != Key4State || TestKey5 != Key5State || TestKey6 != Key6State)
	  {Changing = 1; }
	else
	  {Changing = 0; }

	if (Changing == 1)
	{
		HAL_GPIO_WritePin(GPIOE, KeyPulse01_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOE, KeyPulse02_Pin, GPIO_PIN_SET);
		osDelay(10);
  
		if (HAL_GPIO_ReadPin(GPIOE, KeyScan01_Pin) == GPIO_PIN_SET){Key1State = 1; }
		else{Key1State = 0; }
		if (HAL_GPIO_ReadPin(GPIOE, KeyScan02_Pin) == GPIO_PIN_SET){Key2State = 1; }
		else{Key2State = 0; }
		if (HAL_GPIO_ReadPin(GPIOE, KeyScan03_Pin) == GPIO_PIN_SET){Key3State = 1; }
		else{Key3State = 0; }

		HAL_GPIO_WritePin(GPIOE, KeyPulse01_Pin, GPIO_PIN_SET);
		HAL_GPIO_WritePin(GPIOE, KeyPulse02_Pin, GPIO_PIN_RESET);
		osDelay(1);

		if (HAL_GPIO_ReadPin(GPIOE, KeyScan01_Pin) == GPIO_PIN_SET){Key4State = 1; }
		else{Key4State = 0; }
		;
		if (HAL_GPIO_ReadPin(GPIOE, KeyScan02_Pin) == GPIO_PIN_SET){Key5State = 1; }
		else{Key5State = 0; }
		;
		if (HAL_GPIO_ReadPin(GPIOE, KeyScan03_Pin) == GPIO_PIN_SET){Key6State = 1; }
		else{Key6State = 0; }
		;  

		////////////////////////////////////////////////////////////////////////////////
		/*
		  if(Key1State==1)
		    {DisplayFontRect(0,30, ALIGN_LEFT,"Key1 ON", (char*)Myriad_Pro_16, Black, Green);}
		  else
		    {DisplayFontRect(0,30, ALIGN_LEFT,"Key1 OFF", (char*)Myriad_Pro_16, Black, Green);}

		  if(Key2State==1)
		    {DisplayFontRect(0,50, ALIGN_LEFT,"Key2 ON", (char*)Myriad_Pro_16, Black, Green);}
		  else
		    {DisplayFontRect(0,50, ALIGN_LEFT,"Key2 OFF", (char*)Myriad_Pro_16, Black, Green);}  
  
		    if(Key3State==1)
		    {DisplayFontRect(0,70, ALIGN_LEFT,"Key3 ON", (char*)Myriad_Pro_16, Black, Green);}
		  else
		    {DisplayFontRect(0,70, ALIGN_LEFT,"Key3 OFF", (char*)Myriad_Pro_16, Black, Green);}
  
		    if(Key4State==1)
		    {DisplayFontRect(0,90, ALIGN_LEFT,"Key4 ON", (char*)Myriad_Pro_16, Black, Green);}
		  else
		    {DisplayFontRect(0,90, ALIGN_LEFT,"Key4 OFF", (char*)Myriad_Pro_16, Black, Green);}
  
		 if(Key5State==1)
		  {DisplayFontRect(0,110, ALIGN_LEFT,"Key5 ON", (char*)Myriad_Pro_16, Black, Green);}
		else
		  {DisplayFontRect(0,110, ALIGN_LEFT,"Key5 OFF", (char*)Myriad_Pro_16, Black, Green);}
  
		 if(Key6State==1)
		  {DisplayFontRect(0,130, ALIGN_LEFT,"Key6 ON", (char*)Myriad_Pro_16, Black, Green);}
		else
		  {DisplayFontRect(0,130, ALIGN_LEFT,"Key6 OFF", (char*)Myriad_Pro_16, Black, Green);}  
		  */
		  /////////////////////////////////////////////////////////////////////////////////
  
		if (Key1State == 1)                                                              // "Enter"
		{         
     
		}
		if (Key2State == 1)                                                              // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° Р В Р’В Р В РІР‚В Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р Р‹Р Р†Р вЂљР’В¦
		{

		}
		if (Key3State == 1)                                                              // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° Р В Р’В Р В РІР‚В Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·
		{

		}
		if (Key4State == 1)                                                              // Р В Р’В Р Р†Р вЂљРІвЂћСћР В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В»/Р В Р’В Р Р†Р вЂљРІвЂћСћР В Р Р‹Р Р†Р вЂљРІвЂћвЂ“Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В».
		{
 
		}
		if (Key5State == 1)                                                              // Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚вЂќР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ
		{ 
			ActiveScreen++; // Р В Р в‚¬Р В Р вЂ Р В Р’ВµР В Р’В»Р В РЎвЂ?Р РЋРІР‚РЋР В РЎвЂ?Р В Р вЂ Р В Р’В°Р В Р’ВµР В РЎ? Р В Р вЂ¦Р В РЎвЂўР В РЎ?Р В Р’ВµР РЋР вЂљ Р В Р’В°Р В РЎвЂќР РЋРІР‚С™Р В РЎвЂ?Р В Р вЂ Р В Р вЂ¦Р В РЎвЂўР В РЎвЂ“Р В РЎвЂў Р РЋР РЉР В РЎвЂќР РЋР вЂљР В Р’В°Р В Р вЂ¦Р В Р’В°
			if (ActiveScreen >= ScreensCol){ActiveScreen = 0; }
			DisplayWorkPlace = false;
		}
		if (Key6State == 1)                                                              //  Р В Р’В Р В Р вЂ№Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ
		{
			ActiveScreen--; // Р В Р’В Р В РІвЂљВ¬Р В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦Р В Р Р‹Р В Р вЂ°Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’ВµР В Р’В Р РЋ? Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™ Р В Р’В Р вЂ™Р’В°Р В Р’В Р РЋРІР‚СњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚вЂњР В Р’В Р РЋРІР‚Сћ Р В Р Р‹Р В Р Р‰Р В Р’В Р РЋРІР‚СњР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°
			if (ActiveScreen <= 0){ActiveScreen = ScreensCol - 1; }
			;                            // Р В Р’В Р Р†Р вЂљРЎС›Р В Р Р‹Р В РЎвЂњР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚? Р В Р’В Р В РІР‚В Р В Р Р‹Р Р†Р вЂљРІвЂћвЂ“Р В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋ? Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В° Р В Р’В Р РЋРІР‚вЂњР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’В°Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР’В Р В Р Р‹Р Р†Р вЂљРІвЂћвЂ“- Р В Р’В Р РЋРІР‚вЂќР В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В°Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’ВµР В Р’В Р РЋ? Р В Р’В Р В РІР‚В  Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р Р†Р вЂљР’В  
			DisplayWorkPlace = false;
		}
		Changing = 0;
	}
}
/* USER CODE END 4 */

/* USER CODE BEGIN Header_StartMainTask */
/**
  * @brief  Function implementing the MainTask thread.
  * @param  argument: Not used 
  * @retval None
  */
/* USER CODE END Header_StartMainTask */
void StartMainTask(void *argument)
{
	/* init code for USB_DEVICE */
	MX_USB_DEVICE_Init();
	/* USER CODE BEGIN 5 */
  
	uint32_t ms = 0;
	uint8_t key1_state = 0;
  
	///////////////////////////////////////////////////////////////
	get_ROMid();
	////////////////////////////////////////////////////////////////
	 /* Infinite loop */
	for (;;)
	{
		if (InitComplete == false)
		{
			LCD_Init(); // Р В Р’В Р РЋРІР‚вЂќР В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’В·Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљР’В Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В Р РЏ Р В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚?Р В Р Р‹Р В РЎвЂњР В Р’В Р РЋРІР‚вЂќР В Р’В Р вЂ™Р’В»Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В Р РЏ
			BME280_Init();
			if (LCDInitComplete == true){InitComplete = true; }
			;
		}
		else
		{
			switch (ActiveScreen)                                                                                                                
			{
			case 0:                                                               
				{ 
					ms = HAL_GetTick();
					key1_state = HAL_GPIO_ReadPin(TickIncom_GPIO_Port, TickIncom_Pin);
					if (key1_state == 0 && !short_state && (ms - time_key1) > 50)
					{
						short_state = 1;
						long_state = 0;
						time_key1 = ms;
					}
					else
					{
						//if(key1_state == 0 && !long_state && (ms - time_key1) > 2000) 
						if (key1_state == 0 && !long_state && (ms - time_key1) > 3000)
						{
							long_state = 1;
							// Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р Р†РІР‚С›РІР‚вЂњР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В° Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В¦Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ
						}
						else if (key1_state == 1 && short_state && (ms - time_key1) > 5) 
						{
							short_state = 0;
							time_key1 = ms;
							if (!long_state)                                                         
							{
								FrecMethodA();
								Key1PressCount++; // Р В Р’В Р В РІвЂљВ¬Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В Р вЂ° Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° 
							}
							;
						}
						;
					}
					;
					break;
				}
				; 
			case 1:                                                              
				{
					ms = HAL_GetTick();
					key1_state = HAL_GPIO_ReadPin(TickIncom_GPIO_Port, TickIncom_Pin);
					if (key1_state == 0 && !short_state && (ms - time_key1) > 100)        
						//if(key1_state == 0 && !short_state && (ms - time_key1) > 50)
					{
						short_state = 1;
						long_state = 0;
						time_key1 = ms;
					}
					else
					{
						if (key1_state == 0 && !long_state && (ms - time_key1) > 2000) 
						{
							long_state = 1;
							// Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’ВµР В Р’В Р Р†РІР‚С›РІР‚вЂњР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В° Р В Р’В Р СћРІР‚?Р В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р’В Р В РІР‚В¦Р В Р’В Р В РІР‚В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’Вµ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚?Р В Р’В Р вЂ™Р’Вµ
						}
						//else if(key1_state == 1 && short_state && (ms - time_key1) > 5) 
						else if(key1_state == 1 && short_state && (ms - time_key1) > 25) 
						{
							short_state = 0;
							time_key1 = ms;
							if (!long_state)                                                         
							{  
								Flowmeter(); // Р В Р’В Р вЂ™Р’В Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљР’В¦Р В Р’В Р РЋРІР‚СћР В Р’В Р СћРІР‚?Р В Р’В Р РЋРІР‚СћР В Р’В Р РЋ?Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РІР‚С™
								Key1PressCount++; // Р В Р’В Р В РІвЂљВ¬Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В Р вЂ° Р В Р’В Р РЋРІР‚СњР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ Р В Р’В Р В РІР‚В¦Р В Р’В Р вЂ™Р’В°Р В Р’В Р вЂ™Р’В¶Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚?Р В Р’В Р Р†РІР‚С›РІР‚вЂњ Р В Р’В Р РЋ?Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚?Р В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’В° 
							}
							;
						}
						;
					}
					;
					break;
				}
				;
			case 2:                                                              
				{
					if (BME280TimeCount == 0)
					{
						////////////////////////////////////////////////////////////////////////////////
						BME280_ReadTemperature();            
						TemperFloat = round(TemperFloat * 100) / 100;
						osDelay(1);
          
						BME280_ReadPressure();
						PressureFloat = round(PressureFloat * 100) / 100;
						osDelay(1);
          
						BME280_ReadHumidity();
						HumidityFloat = round(HumidityFloat * 10) / 10;
						osDelay(1);
          
						////////////////////////////////////////////////////////////////////////////////            
						BME280TimeCount = BME280TimeCountMAX;       
					}
					;          
					break;
				}
				;      
			default:                                                                      
				{
				}
				; 
			}
		}
		;       
		osDelay(1);
	}
	;
	/* USER CODE END 5 */
}

/* USER CODE BEGIN Header_StartDisplayTask */
/**
* @brief Function implementing the DisplayTask thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartDisplayTask */
void StartDisplayTask(void *argument)
{
	/* USER CODE BEGIN StartDisplayTask */

	  /* Infinite loop */
	for (;;)
	{
		if (InitComplete == true)
		{
			if (DisplayWorkPlace == false)
			{
				switch (ActiveScreen)
				{
				case 0:
					{
						LCD_Clear(Black);
						DisplayFontRect(100, 0, ALIGN_LEFT, Title1, (char*)Myriad_Pro_16, Black, Green);
						DisplayFontRect(0, 40, ALIGN_LEFT, Func1, (char*)Myriad_Pro_16, Black, Green);
						DisplayFontRect(0, 60, ALIGN_LEFT, Func2, (char*)Myriad_Pro_16, Black, Green);
						////////////////////////////////////////////////////////////////////////////////////
						// DisplayFontRect(0,100, ALIGN_LEFT,"Prescaler: ", (char*)Myriad_Pro_16, Black, Green);
						// DisplayFontRect(0,120, ALIGN_LEFT,"Ticks: ", (char*)Myriad_Pro_16, Black, Green);
						////////////////////////////////////////////////////////////////////////////////////
						break;
					}
					;
				case 1:
					{
						LCD_Clear(Black);
						DisplayFontRect(100, 0, ALIGN_LEFT, Title2, (char*)Myriad_Pro_16, Black, Green);
						DisplayFontRect(0, 40, ALIGN_LEFT, Func3, (char*)Myriad_Pro_16, Black, Green);
						DisplayFontRect(0, 60, ALIGN_LEFT, Func4, (char*)Myriad_Pro_16, Black, Green);

						break;
					}
					;

				case 2:
					{
						LCD_Clear(Black);
						TemperFloatOLD = 0;
						HumidityFloatOLD = 0;
						PressureFloatOLD = 0;
						DisplayFontRect(10, 0, ALIGN_LEFT, Title3, (char*)Myriad_Pro_16, Black, Green);
						DisplayFontRect(0, 40, ALIGN_LEFT, Func5, (char*)Myriad_Pro_16, Black, Green);
						DisplayFontRect(0, 60, ALIGN_LEFT, Func6, (char*)Myriad_Pro_16, Black, Green);
						DisplayFontRect(0, 80, ALIGN_LEFT, Func7, (char*)Myriad_Pro_16, Black, Green);
						break;
					}
				case 3:
					{
						LCD_Clear(Black);
						DisplayFontRect(10, 0, ALIGN_LEFT, Title4, (char*)Myriad_Pro_16, Black, Green);
						break;
					} // Р В ?Р В Р’В·Р В РЎ?Р В Р’ВµР РЋР вЂљР В Р’ВµР В Р вЂ¦Р В РЎвЂ?Р В Р’Вµ Р РЋРІР‚С™Р В Р’ВµР В РЎ?Р В РЎвЂ”Р В Р’ВµР РЋР вЂљР В Р’В°Р РЋРІР‚С™Р РЋРЎвЂњР РЋР вЂљР РЋРІР‚в„– Р В РўвЂ?Р В Р’В°Р РЋРІР‚С™Р РЋРІР‚РЋР В РЎвЂ?Р В РЎвЂќР В Р’В°Р В РЎ?Р В РЎвЂ? DS18B20
				default:
					{

					}
					;


				}
				DisplayWorkPlace = true;
			} // Р В РЎвЂєР РЋРІР‚С™Р РЋР вЂљР В РЎвЂ?Р РЋР С“Р В РЎвЂўР В Р вЂ Р В РЎвЂќР В Р’В° Р РЋР С“Р РЋРІР‚С™Р В Р’В°Р РЋРІР‚С™Р В РЎвЂ?Р РЋРІР‚РЋР В Р вЂ¦Р РЋРІР‚в„–Р РЋРІР‚В¦ Р В РЎвЂ”Р В РЎвЂўР В Р’В·Р В РЎвЂ?Р РЋРІР‚В Р В РЎвЂ?Р В РІвЂћвЂ“ Р В Р вЂ¦Р В Р’В° Р РЋР РЉР В РЎвЂќР РЋР вЂљР В Р’В°Р В Р вЂ¦Р В Р’Вµ
			switch (ActiveScreen)
			{
			case 0:
				{
					if (Frequency != FrequencyOLD)
					{
						sprintf(FloatChars, "%5.0f", Frequency);
						DisplayFontRect(120, 40, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						FrequencyOLD = Frequency;
					}
					;

					if (Key1PressCount != Key1PressCountOLD)
					{
						sprintf(FloatChars, "%.0d", Key1PressCount);
						DisplayFontRect(130, 60, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green); // Р В Р’В Р РЋРІвЂћСћР В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚?Р В Р Р‹Р Р†Р вЂљР Р‹Р В Р’В Р вЂ™Р’ВµР В Р Р‹Р В РЎвЂњР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ Р В Р’В Р РЋРІР‚СћР В Р’В Р вЂ™Р’В±Р В Р’В Р РЋРІР‚СћР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋРІР‚СћР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В 
						Key1PressCountOLD = Key1PressCount;
					}
					;
					/*
					if(Prescaler!=PrescalerOLD)
					{
					sprintf(FloatChars,"%.0d",Prescaler);
					DisplayFontRect(150,100, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
					PrescalerOLD=Prescaler;
					};

					if(FastTimerCount!=FastTimerCountOLD)
					{
					sprintf(FloatChars,"%.0d",FastTimerCount);
					DisplayFontRect(150,120, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
					FastTimerCountOLD=FastTimerCount;
					};
					*/
					break;
				}
			case 1:
				{
					if (ExpenditureLiMin != ExpenditureLiMinOLD)
					{
						sprintf(FloatChars, "%5.2f", ExpenditureLiMin);
						DisplayFontRect(120, 40, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						ExpenditureLiMinOLD = ExpenditureLiMin;
					}
					if (ExpenditureLitres != ExpenditureLitresOLD)
					{
						sprintf(FloatChars, "%.0d", ExpenditureLitres);
						DisplayFontRect(130, 60, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						ExpenditureLitresOLD = ExpenditureLitres;
					}
					break;
				}
			case 2:
				{
					if (TemperFloat != TemperFloatOLD)
					{
						sprintf(FloatChars, "%3.1f", TemperFloat);
						DisplayFontRect(200, 40, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						TemperFloatOLD = TemperFloat;
					}
					if (HumidityFloat != HumidityFloatOLD)
					{
						sprintf(FloatChars, "%3.1f", HumidityFloat);
						DisplayFontRect(180, 60, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						HumidityFloatOLD = HumidityFloat;
					}
					if (PressureFloat != PressureFloatOLD)
					{
						sprintf(FloatChars, "%3.2f", PressureFloat);
						DisplayFontRect(240, 80, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						PressureFloatOLD = PressureFloat;
					}
					break;
				}

			case 3:
				{
					get_Temperature();
					
					if (OLDTemp[0] != Temp[0])
					{
						sprintf(FloatChars, "1: %3.1f", Temp[0]);
						DisplayFontRect(10, 40, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[0] = Temp[0];
					}
					
					if (OLDTemp[1] != Temp[1])
					{
						sprintf(FloatChars, "2: %3.1f", Temp[1]);
						DisplayFontRect(10, 80, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[1] = Temp[1];
					}
					
					if (OLDTemp[2] != Temp[2])
					{
						sprintf(FloatChars, "3: %3.1f", Temp[2]);
						DisplayFontRect(10, 120, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[2] = Temp[2];
					}
					
					if (OLDTemp[3] != Temp[3])
					{
						sprintf(FloatChars, "4: %3.1f", Temp[3]);
						DisplayFontRect(10, 160, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[3] = Temp[3];
					}
					if (OLDTemp[4] != Temp[4])
					{
						sprintf(FloatChars, "5: %3.1f", Temp[4]);
						DisplayFontRect(110, 40, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[4] = Temp[4];
					}
					if (OLDTemp[5] != Temp[5])
					{
						sprintf(FloatChars, "6: %3.1f", Temp[5]);
						DisplayFontRect(110, 80, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[5] = Temp[5];
					}
					if (OLDTemp[6] != Temp[6])
					{
						sprintf(FloatChars, "7: %3.1f", Temp[6]);
						DisplayFontRect(110, 120, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[6] = Temp[6];
					}
					if (OLDTemp[7] != Temp[7])
					{
						sprintf(FloatChars, "8: %3.1f", Temp[7]);
						DisplayFontRect(110, 160, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[7] = Temp[7];
					}
					if (OLDTemp[8] != Temp[8])
					{
						sprintf(FloatChars, "9: %3.1f", Temp[8]);
						DisplayFontRect(210, 40, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[8] = Temp[8];
					}
					if (OLDTemp[9] != Temp[9])
					{
						sprintf(FloatChars, "10: %3.1f", Temp[9]);
						DisplayFontRect(210, 80, ALIGN_LEFT, FloatChars, (char*)Myriad_Pro_16, Black, Green);
						OLDTemp[9] = Temp[9];
					}
					
					osDelay(100);
					break;
				}
			default:
				{

				}
				;
			} // Р В РЎвЂєР РЋРІР‚С™Р РЋР вЂљР В РЎвЂ?Р РЋР С“Р В РЎвЂўР В Р вЂ Р В РЎвЂќР В Р’В° Р В РЎвЂ”Р В Р’ВµР РЋР вЂљР В Р’ВµР В РЎ?Р В Р’ВµР В Р вЂ¦Р В Р вЂ¦Р РЋРІР‚в„–Р РЋРІР‚В¦ Р В РЎвЂ”Р В РЎвЂўР В Р’В·Р В РЎвЂ?Р РЋРІР‚В Р В РЎвЂ?Р В РІвЂћвЂ“ Р В Р вЂ¦Р В Р’В° Р РЋР РЉР В РЎвЂќР РЋР вЂљР В Р’В°Р В Р вЂ¦Р В Р’Вµ
			osDelay(1);
		}
		else
		{
			osDelay(1);
		}
	}
	/* USER CODE END StartDisplayTask */
}

/* USER CODE BEGIN Header_StartKeyboardTask */
/**
* @brief Function implementing the KeyboardTask thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartKeyboardTask */
void StartKeyboardTask(void *argument)
{
	/* USER CODE BEGIN StartKeyboardTask */
	  /* Infinite loop */
	for (;;)
	{
		if (InitComplete == true)
		{ 
			KeyBoardRead();
			osDelay(10);
		}
		else
		{
			osDelay(1);
		}
		osDelay(1);
	}
	/* USER CODE END StartKeyboardTask */
}

/**
  * @brief  Period elapsed callback in non blocking mode
  * @note   This function is called  when TIM14 interrupt took place, inside
  * HAL_TIM_IRQHandler(). It makes a direct call to HAL_IncTick() to increment
  * a global variable "uwTick" used as application time base.
  * @param  htim : TIM handle
  * @retval None
  */
void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim)
{
	/* USER CODE BEGIN Callback 0 */

	/* USER CODE END Callback 0 */
	if (htim->Instance == TIM14) {
		HAL_IncTick();
	}
	/* USER CODE BEGIN Callback 1 */

	/* USER CODE END Callback 1 */
}

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
	/* USER CODE BEGIN Error_Handler_Debug */
	  /* User can add his own implementation to report the HAL error return state */

	/* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
	/* USER CODE BEGIN 6 */
	  /* User can add his own implementation to report the file name and line number,
	     tex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
    /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */
