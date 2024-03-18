#ifndef _BOARD_H_
#define _BOARD_H_

//#pragma pack(1)

//////////////////////////////////////////////////////////#include "stm32f10x.h"
#include "stm32f103xg.h"


#define FLASH_BASE_ADDR               ((OS_U32)0x08000040)

#define HW_CFG_GPIO        GPIOE->CRL&=~(GPIO_CRL_MODE6|GPIO_CRL_CNF6);  
#define HW_CFG_PWM         GPIOE->CRL|=(GPIO_CRL_MODE6_0|GPIO_CRL_CNF6_1);//MODE 01-Output mode, max speed 10 MHz, CNF=01 Alternate function output Push-pull     ;   
#define HW_CFG_OUT         GPIOE->CRL |= GPIO_CRL_MODE6_0;//MODE 01-Output mode, max speed 10 MHz, CNF=0 General purpose output push-pull   
#define HW_SET_OUT_LOW     GPIOE->ODR&=~GPIO_ODR_ODR6;  
#define HW_SET_OUT_HIGH    GPIOE->ODR|=GPIO_ODR_ODR6;  
#define HW_PORT            (GPIOE->IDR&GPIO_ODR_ODR6)

/*********************************************************************/
#define SPI_RESET_PORT_CFG_GPIO            GPIOB->CRH &=~(GPIO_CRH_MODE12|GPIO_CRH_CNF12);  
#define SPI_RESET_PORT_CFG_OUT             GPIOB->CRH |=GPIO_CRH_MODE12_0;//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define SPI_RESET_SET_OUT_LOW              GPIOB->ODR&=~GPIO_ODR_ODR12
#define SPI_RESET_SET_OUT_HIGH             GPIOB->ODR|=GPIO_ODR_ODR12

#define SPI_CS_PORT_CFG_GPIO            GPIOD->CRH &=~(GPIO_CRH_MODE12|GPIO_CRH_CNF12|GPIO_CRH_MODE13|GPIO_CRH_CNF13);  
#define SPI_CS_PORT_CFG_OUT             GPIOD->CRH |=GPIO_CRH_MODE12_0|GPIO_CRH_MODE13_0;//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define SPI_CS_SET_OUT_LOW(a)           GPIOD->ODR&=~(0x01<<(12+a))
#define SPI_CS_SET_OUT_HIGH(a)          GPIOD->ODR|=(0x01<<(12+a))

#define SPI_SCK_PORT_CFG_GPIO            GPIOB->CRH &=~(GPIO_CRH_MODE13|GPIO_CRH_CNF13);  
#define SPI_SCK_PORT_CFG_OUT             GPIOB->CRH |=(GPIO_CRH_MODE13_0);//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define SPI_SCK_SET_OUT_LOW              GPIOB->ODR&=~GPIO_ODR_ODR13
#define SPI_SCK_SET_OUT_HIGH             GPIOB->ODR|=GPIO_ODR_ODR13

#define SPI_MISO_PORT_CFG_GPIO           GPIOB->CRH &=~(GPIO_CRH_MODE15|GPIO_CRH_CNF14);  
#define SPI_MISO_PORT_CFG_IN             GPIOB->CRH &=~ GPIO_CRH_MODE14;//MODE 00-Input mode, CNF=10 Input with pull-up / pull-down
#define SPI_MISO_PORT_CFG_PULL_1         GPIOB->CRH |=GPIO_CRH_CNF14_1;GPIOB->ODR|=GPIO_ODR_ODR14;//GPIOA->BSRR=GPIO_BSRR_BS6; //CNF=10: Input with pull-up / pull-down
#define SPI_MISO_PIN                     (GPIOB->IDR&GPIO_ODR_ODR14)


#define SPI_MOSI_PORT_CFG_GPIO            GPIOB->CRH &=~(GPIO_CRH_MODE15|GPIO_CRH_CNF15);  
#define SPI_MOSI_PORT_CFG_OUT             GPIOB->CRH |=(GPIO_CRH_MODE15_0);//MODE 01-Output mode, max speed 2 MHz, , CNF=01 General purpose output Open-drain
#define SPI_MOSI_SET_OUT_LOW              GPIOB->ODR&=~GPIO_ODR_ODR15
#define SPI_MOSI_SET_OUT_HIGH             GPIOB->ODR|=GPIO_ODR_ODR15


/*********************************************************************/
  /* PE.07(D4), PE.08(D5), PE.09(D6), PE.10(D7), PE.11(D8), PE.12(D9),
     PE.13(D10), PE.14(D11), PE.15(D12) */

#define LCD_PORT_E_CFG_GPIO           GPIOE->CRL &= ~(GPIO_CRL_CNF7|GPIO_CRL_MODE7);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF8|GPIO_CRH_MODE8);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF9|GPIO_CRH_MODE9);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF10|GPIO_CRH_MODE10);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF11|GPIO_CRH_MODE11);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF12|GPIO_CRH_MODE12);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF13|GPIO_CRH_MODE13);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF14|GPIO_CRH_MODE14);\
                                      GPIOE->CRH &= ~(GPIO_CRH_CNF15|GPIO_CRH_MODE15);
  
#define LCD_PORT_E_CFG_AF_PP          GPIOE->CRL |= (GPIO_CRL_CNF7_1|GPIO_CRL_MODE7);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF8_1|GPIO_CRH_MODE8);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF9_1|GPIO_CRH_MODE9);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF10_1|GPIO_CRH_MODE10);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF11_1|GPIO_CRH_MODE11);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF12_1|GPIO_CRH_MODE12);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF13_1|GPIO_CRH_MODE13);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF14_1|GPIO_CRH_MODE14);\
                                      GPIOE->CRH |= (GPIO_CRH_CNF15_1|GPIO_CRH_MODE15);

  /* PD.00(D2), PD.01(D3), PD.04(RD), PD.5(WR), PD.7(CS), PD.8(D13), PD.9(D14),
     PD.10(D15), PD.11(RS) PD.14(D0) PD.15(D1) */


#define LCD_PORT_D_CFG_GPIO           GPIOD->CRL &= ~(GPIO_CRL_CNF0|GPIO_CRL_MODE0);\
                                      GPIOD->CRL &= ~(GPIO_CRL_CNF1|GPIO_CRL_MODE1);\
                                      GPIOD->CRL &= ~(GPIO_CRL_CNF4|GPIO_CRL_MODE4);\
                                      GPIOD->CRL &= ~(GPIO_CRL_CNF5|GPIO_CRL_MODE5);\
                                      GPIOD->CRL &= ~(GPIO_CRL_CNF7|GPIO_CRL_MODE7);\
                                      GPIOD->CRH &= ~(GPIO_CRH_CNF8|GPIO_CRH_MODE8);\
                                      GPIOD->CRH &= ~(GPIO_CRH_CNF9|GPIO_CRH_MODE9);\
                                      GPIOD->CRH &= ~(GPIO_CRH_CNF10|GPIO_CRH_MODE10);\
                                      GPIOD->CRH &= ~(GPIO_CRH_CNF11|GPIO_CRH_MODE11);\
                                      GPIOD->CRH &= ~(GPIO_CRH_CNF14|GPIO_CRH_MODE14);\
                                      GPIOD->CRH &= ~(GPIO_CRH_CNF15|GPIO_CRH_MODE15);
  
#define LCD_PORT_D_CFG_AF_PP          GPIOD->CRL |= (GPIO_CRL_CNF0_1|GPIO_CRL_MODE0);\
                                      GPIOD->CRL |= (GPIO_CRL_CNF1_1|GPIO_CRL_MODE1);\
                                      GPIOD->CRL |= (GPIO_CRL_CNF4_1|GPIO_CRL_MODE4);\
                                      GPIOD->CRL |= (GPIO_CRL_CNF5_1|GPIO_CRL_MODE5);\
                                      GPIOD->CRL |= (GPIO_CRL_CNF7_1|GPIO_CRL_MODE7);\
                                      GPIOD->CRH |= (GPIO_CRH_CNF8_1|GPIO_CRH_MODE8);\
                                      GPIOD->CRH |= (GPIO_CRH_CNF9_1|GPIO_CRH_MODE9);\
                                      GPIOD->CRH |= (GPIO_CRH_CNF10_1|GPIO_CRH_MODE10);\
                                      GPIOD->CRH |= (GPIO_CRH_CNF11_1|GPIO_CRH_MODE11);\
                                      GPIOD->CRH |= (GPIO_CRH_CNF14_1|GPIO_CRH_MODE14);\
                                      GPIOD->CRH |= (GPIO_CRH_CNF15_1|GPIO_CRH_MODE15);
                                                

/*********************************************************************/

#//define SND_PORT_ENABLE    RCC->APB2ENR|=RCC_APB2ENR_IOPBEN
#define SND_CFG_GPIO       GPIOB->CRL&=~(GPIO_CRL_MODE7|GPIO_CRL_CNF7);  
#define SND_CFG_OUT        GPIOB->CRL|=GPIO_CRL_MODE7_0;//MODE 01-Output mode, max speed 10 MHz, CNF=0 General purpose output push-pull     ;  
#define SND_CFG_PWM        GPIOB->CRL|=(GPIO_CRL_MODE7_0|GPIO_CRL_CNF7_1);//MODE 01-Output mode, max speed 10 MHz, CNF=10 Alternate function output Push-pull     ;   

#define SND_SET_OUT_LOW    GPIOB->ODR&=~GPIO_ODR_ODR7 
#define SND_SET_OUT_HIGH   GPIOB->ODR|=GPIO_ODR_ODR7 
#define SND_TOGGLE         GPIOB->ODR^=GPIO_ODR_ODR7 


/*********************************************************************/
#define HEATER_SW_CFG_GPIO       GPIOB->CRL&=~(GPIO_CRL_MODE1|GPIO_CRL_CNF1);  
#define HEATER_SW_CFG_OUT        GPIOB->CRL |=(GPIO_CRL_MODE1_0);//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define HEATER_SW_SET_OUT_LOW    GPIOB->ODR&=~GPIO_ODR_ODR1
#define HEATER_SW_SET_OUT_HIGH   GPIOB->ODR|=GPIO_ODR_ODR1

#define HEATER_CFG_GPIO          GPIOB->CRL&=~(GPIO_CRL_MODE0|GPIO_CRL_CNF0);  
#define HEATER_CFG_PWM           GPIOB->CRL|=(GPIO_CRL_MODE0_0|GPIO_CRL_CNF0_1);//MODE 01-Output mode, max speed 10 MHz, CNF=10 Alternate function output Push-pull     ;   

/*********************************************************************/

#define FLOAT_HIGH_PORT_CFG_GPIO      GPIOB->CRL &=~(GPIO_CRL_MODE3|GPIO_CRL_CNF3);  
#define FLOAT_HIGH_PORT_CFG_IN        GPIOB->CRL &=~(GPIO_CRL_MODE3);//MODE 00: Input mode 

#define FLOAT_HIGH_PORT_CFG_PULL_1     GPIOB->CRL |=GPIO_CRL_CNF3_1;GPIOB->ODR|=GPIO_ODR_ODR3; //CNF=10: Input with pull-up / pull-down
#define FLOAT_HIGH_PORT_CFG_NO_PULL    GPIOB->CRL |=GPIO_CRL_CNF3_0;        //CNF=01: Floating input (reset state)
#define FLOAT_HIGH_PORT_CFG_PULL_0     GPIOB->CRL |=GPIO_CRL_CNF3_1;GPIOB->ODR&=~GPIO_ODR_ODR3;//CNF=10: Input with pull-up / pull-down

#define FLOAT_HIGH_PIN                 (GPIOB->IDR&GPIO_ODR_ODR3)

#define FLOAT_LOW_PORT_CFG_GPIO      GPIOB->CRL &=~(GPIO_CRL_MODE4|GPIO_CRL_CNF4);  
#define FLOAT_LOW_PORT_CFG_IN        GPIOB->CRL &=~(GPIO_CRL_MODE4);//MODE 00: Input mode 

#define FLOAT_LOW_PORT_CFG_PULL_1     GPIOB->CRL |=GPIO_CRL_CNF4_1;GPIOB->ODR|=GPIO_ODR_ODR4; //CNF=10: Input with pull-up / pull-down
#define FLOAT_LOW_PORT_CFG_NO_PULL    GPIOB->CRL |=GPIO_CRL_CNF4_0;        //CNF=01: Floating input (reset state)
#define FLOAT_LOW_PORT_CFG_PULL_0     GPIOB->CRL |=GPIO_CRL_CNF4_1;GPIOB->ODR&=~GPIO_ODR_ODR4;//CNF=10: Input with pull-up / pull-down

#define FLOAT_LOW_PIN                 (GPIOB->IDR&GPIO_ODR_ODR4)

/*********************************************************************/
#define CANCEL_KEY_CFG_GPIO       GPIOE->CRL &=~(GPIO_CRL_MODE1|GPIO_CRL_CNF1);  
#define CANCEL_KEY_CFG_IN         GPIOE->CRL &=~(GPIO_CRL_MODE1);//MODE 00: Input mode 
#define CANCEL_KEY_CFG_OUT        GPIOE->CRL |=(GPIO_CRL_MODE1_0);//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define CANCEL_KEY_CFG_PULL_1     GPIOE->CRL |=GPIO_CRL_CNF1_1;GPIOE->ODR|=GPIO_ODR_ODR1; //CNF=10: Input with pull-up / pull-down
#define CANCEL_KEY_CFG_NO_PULL    GPIOE->CRL |=GPIO_CRL_CNF1_0;        //CNF=01: Floating input (reset state)
#define CANCEL_KEY_CFG_PULL_0     GPIOE->CRL |=GPIO_CRL_CNF1_1;GPIOE->ODR&=~GPIO_ODR_ODR1;//CNF=10: Input with pull-up / pull-down
#define CANCEL_KEY_MASK           (1<<1)
#define CANCEL_KEY_IN             (GPIOE->IDR&GPIO_ODR_ODR1)

#define ENCODER_PIN               GPIOC->IDR

#define ENCODER_KEY_CFG_GPIO       GPIOC->CRL &=~(GPIO_CRL_MODE0|GPIO_CRL_CNF0);  
#define ENCODER_KEY_CFG_IN         GPIOC->CRL &=~(GPIO_CRL_MODE0);//MODE 00: Input mode 
#define ENCODER_KEY_CFG_OUT        GPIOC->CRL |=(GPIO_CRL_MODE0_1);//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define ENCODER_KEY_CFG_PULL_1     GPIOC->CRL |=GPIO_CRL_CNF0_1;GPIOC->ODR|=GPIO_ODR_ODR0; //CNF=10: Input with pull-up / pull-down
#define ENCODER_KEY_CFG_NO_PULL    GPIOC->CRL |=GPIO_CRL_CNF0_0;        //CNF=01: Floating input (reset state)
#define ENCODER_KEY_CFG_PULL_0     GPIOC->CRL |=GPIO_CRL_CNF0_1;GPIOC->ODR&=~GPIO_ODR_ODR0;//CNF=10: Input with pull-up / pull-down
/*
#define ENCODER_KEY_INT_RISING_ENABLE    IO2INTENR_bit.P2_13 = 1;
#define ENCODER_KEY_INT_FALING_ENABLE    IO2INTENF_bit.P2_13 = 1;
#define ENCODER_KEY_INT_RISING_DISABLE    IO2INTENR_bit.P2_13 = 0;
#define ENCODER_KEY_INT_FALING_DISABLE    IO2INTENF_bit.P2_13 = 0;

#define ENCODER_KEY_INT_CLEAR             IO2INTCLR_bit.P2_13 = 1;
#define ENCODER_KEY_INT_RISING_FLAG       IO2INTSTATR_bit.P2_13
#define ENCODER_KEY_INT_FALING_FLAG       IO2INTSTATF_bit.P2_13
*/
#define ENCODER_KEY_MASK                  (1<<0)
#define ENCODER_KEY_IN             (GPIOC->IDR&GPIO_ODR_ODR0)



#define ENCODER_CHA_CFG_GPIO       GPIOC->CRL &=~(GPIO_CRL_MODE2|GPIO_CRL_CNF2); 
#define ENCODER_CHA_CFG_IN         GPIOC->CRL &=~(GPIO_CRL_MODE2);//MODE 00: Input mode 
#define ENCODER_CHA_CFG_OUT        GPIOC->CRL |=(GPIO_CRL_MODE2_0);//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define ENCODER_CHA_CFG_PULL_1     GPIOC->CRL |=GPIO_CRL_CNF2_1;GPIOC->ODR|=GPIO_ODR_ODR2; //CNF=10: Input with pull-up / pull-down
#define ENCODER_CHA_CFG_NO_PULL    GPIOC->CRL |=GPIO_CRL_CNF2_0;        //CNF=01: Floating input (reset state)
#define ENCODER_CHA_CFG_PULL_0     GPIOC->CRL |=GPIO_CRL_CNF2_1;GPIOC->ODR&=~GPIO_ODR_ODR2;//CNF=10: Input with pull-up / pull-down
/*
#define ENCODER_CHA_INT_RISING_ENABLE    IO2INTENR_bit.P2_11 = 1;
#define ENCODER_CHA_INT_FALING_ENABLE    IO2INTENF_bit.P2_11 = 1;
#define ENCODER_CHA_INT_RISING_DISABLE    IO2INTENR_bit.P2_11 = 0;
#define ENCODER_CHA_INT_FALING_DISABLE    IO2INTENF_bit.P2_11 = 0;

#define ENCODER_CHA_INT_CLEAR             IO2INTCLR_bit.P2_11 = 1;
#define ENCODER_CHA_INT_RISING_FLAG       IO2INTSTATR_bit.P2_11
#define ENCODER_CHA_INT_FALING_FLAG       IO2INTSTATF_bit.P2_11
*/
#define ENCODER_CHA_IN             (GPIOC->IDR&GPIO_ODR_ODR2)
#define ENCODER_CHA_MASK                  (1<<2)

#define ENCODER_CHB_CFG_GPIO       GPIOC->CRL &=~(GPIO_CRL_MODE1|GPIO_CRL_CNF1); 
#define ENCODER_CHB_CFG_IN         GPIOC->CRL &=~(GPIO_CRL_MODE1);//MODE 00: Input mode 
#define ENCODER_CHB_CFG_OUT        GPIOC->CRL |=(GPIO_CRL_MODE1_0);//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define ENCODER_CHB_CFG_PULL_1     GPIOC->CRL |=GPIO_CRL_CNF1_1;GPIOC->ODR|=GPIO_ODR_ODR1; //CNF=10: Input with pull-up / pull-down
#define ENCODER_CHB_CFG_NO_PULL    GPIOC->CRL |=GPIO_CRL_CNF1_0;        //CNF=01: Floating input (reset state)
#define ENCODER_CHB_CFG_PULL_0     GPIOC->CRL |=GPIO_CRL_CNF1_1;GPIOC->ODR&=~GPIO_ODR_ODR1;//CNF=10: Input with pull-up / pull-down
/*
#define ENCODER_CHB_INT_RISING_ENABLE    IO2INTENR_bit.P2_12 = 1;
#define ENCODER_CHB_INT_FALING_ENABLE    IO2INTENF_bit.P2_12 = 1;
#define ENCODER_CHB_INT_RISING_DISABLE    IO2INTENR_bit.P2_12 = 0;
#define ENCODER_CHB_INT_FALING_DISABLE    IO2INTENF_bit.P2_12 = 0;

#define ENCODER_CHB_INT_CLEAR             IO2INTCLR_bit.P2_12 = 1;
#define ENCODER_CHB_INT_RISING_FLAG       IO2INTSTATR_bit.P2_12
#define ENCODER_CHB_INT_FALING_FLAG       IO2INTSTATF_bit.P2_12
*/
#define ENCODER_CHB_MASK                  (1<<1)
#define ENCODER_CHB_IN             (GPIOC->IDR&GPIO_ODR_ODR1)

/*********************************************************************/

#define MOTOR_IN_CFG_GPIO       GPIOC->CRL &=~(GPIO_CRL_MODE6|GPIO_CRL_CNF6); 
#define MOTOR_IN_CFG_IN         GPIOC->CRL &=~(GPIO_CRL_MODE6);//MODE 00: Input mode 
#define MOTOR_IN_CFG_PULL_1     GPIOC->CRL |=GPIO_CRL_CNF6_1;GPIOC->ODR|=GPIO_ODR_ODR6; //CNF=10: Input with pull-up / pull-down
#define MOTOR_IN_CFG_NO_PULL    GPIOC->CRL |=GPIO_CRL_CNF6_0;        //CNF=01: Floating input (reset state)
#define MOTOR_IN_CFG_PULL_0     GPIOC->CRL |=GPIO_CRL_CNF6_1;GPIOC->ODR&=~GPIO_ODR_ODR6;//CNF=10: Input with pull-up / pull-down

/*********************************************************************/

#define MOTOR_PWM_CFG_GPIO             GPIOB->CRL &=~(GPIO_CRL_MODE6|GPIO_CRL_CNF6);  
#define MOTOR_PWM_CFG_OUT              GPIOB->CRL |=GPIO_CRL_MODE6_0;//MODE 01-Output mode, max speed 10 MHz, CNF=0 General purpose output push-pull     
#define MOTOR_PWM_CFG_PWM              GPIOB->CRL|=(GPIO_CRL_MODE6_0|GPIO_CRL_CNF6_1);//MODE 01-Output mode, max speed 10 MHz, CNF=10 Alternate function output Push-pull     ;   

//#define MOTOR_PWM_SET_OUT_LOW          GPIOB->ODR&=~GPIO_ODR_ODR6;
//#define MOTOR_PWM_SET_OUT_HIGH         GPIOB->ODR|=GPIO_ODR_ODR6;

/*********************************************************************/

#define MOTOR_EN_CFG_GPIO             GPIOC->CRL &=~(GPIO_CRL_MODE3|GPIO_CRL_CNF3);  
#define MOTOR_EN_CFG_OUT              GPIOC->CRL |=GPIO_CRL_MODE3_0;//MODE 01-Output mode, max speed 10 MHz, CNF=0 General purpose output push-pull     
#define MOTOR_EN_SET_OUT_LOW          GPIOC->ODR&=~GPIO_ODR_ODR3;
#define MOTOR_EN_SET_OUT_HIGH         GPIOC->ODR|=GPIO_ODR_ODR3;

/*********************************************************************/
#define SAFETY_CFG_ADC             GPIOC->CRL&=~(GPIO_CRL_MODE4|GPIO_CRL_CNF4);//MODE 00: Input mode, CNF=00: Analog mode

#define SAFETY_CFG_GPIO       GPIOC->CRH &=~(GPIO_CRH_MODE13|GPIO_CRH_CNF13);  
#define SAFETY_CFG_IN         GPIOC->CRH &=~(GPIO_CRH_MODE13);//MODE 00: Input mode 
#define SAFETY_CFG_OUT        GPIOC->CRH |=(GPIO_CRH_MODE13_0);//MODE 01-Output mode, max speed 2 MHz, CNF=01 General purpose output Open-drain
#define SAFETY_CFG_PULL_1     GPIOC->CRH |=GPIO_CRH_CNF13_1;GPIOC->ODR|=GPIO_ODR_ODR13; //CNF=10: Input with pull-up / pull-down
#define SAFETY_CFG_NO_PULL    GPIOC->CRH |=GPIO_CRH_CNF13_0;        //CNF=01: Floating input (reset state)
#define SAFETY_CFG_PULL_0     GPIOC->CRH |=GPIO_CRH_CNF13_1;GPIOC->ODR&=~GPIO_ODR_ODR13;//CNF=10: Input with pull-up / pull-down
#define SAFETY_IN             (GPIOC->IDR&GPIO_ODR_ODR13)

#endif