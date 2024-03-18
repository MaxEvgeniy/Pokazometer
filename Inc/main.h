/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
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

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32f1xx_hal.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */

/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */

/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define KeyScan01_Pin GPIO_PIN_2
#define KeyScan01_GPIO_Port GPIOE
#define KeyScan02_Pin GPIO_PIN_3
#define KeyScan02_GPIO_Port GPIOE
#define KeyPulse02_Pin GPIO_PIN_4
#define KeyPulse02_GPIO_Port GPIOE
#define KeyPulse01_Pin GPIO_PIN_5
#define KeyPulse01_GPIO_Port GPIOE
#define TickIncom_Pin GPIO_PIN_2
#define TickIncom_GPIO_Port GPIOD
#define DHT22_Data_Pin GPIO_PIN_6
#define DHT22_Data_GPIO_Port GPIOD
#define Schetchik_Pin GPIO_PIN_3
#define Schetchik_GPIO_Port GPIOB
#define BME280_Power_Pin GPIO_PIN_4
#define BME280_Power_GPIO_Port GPIOB
#define TFT_LED_ADJ_Pin GPIO_PIN_5
#define TFT_LED_ADJ_GPIO_Port GPIOB
#define KeyScan03_Pin GPIO_PIN_0
#define KeyScan03_GPIO_Port GPIOE

/* USER CODE BEGIN Private defines */

/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */
