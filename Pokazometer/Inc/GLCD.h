/****************************************Copyright (c)**************************************************                         
**
**                                 http://www.powermcu.com
**
**--------------File Info-------------------------------------------------------------------------------
** File name:			GLCD.h
** Descriptions:		STM32 FSMC TFT²Ù×÷º¯Êý¿â
**
**------------------------------------------------------------------------------------------------------
** Created by:			poweravr
** Created date:		2010-11-7
** Version:				1.0
** Descriptions:		The original version
**
**------------------------------------------------------------------------------------------------------
** Modified by:			
** Modified date:	
** Version:
** Descriptions:		
********************************************************************************************************/

#ifndef __GLCD_H 
#define __GLCD_H


/* Includes ------------------------------------------------------------------*/
#include "board.h"

#include <stdint.h>

typedef struct
{
  uint8_t Add;
  uint16_t Data;
}sInitCmd;

 typedef struct {
     const uint16_t *data;
     uint16_t width;
     uint16_t height;
     uint8_t dataSize;
     } tImage;

//#include "Small_Fonts_7_Bold.c"

void LCD_Init(void);

/* Private define ------------------------------------------------------------*/
#define HY32D              8989

//#define LCD_XSIZE          240
//#define LCD_YSIZE          320
#define LCD_XSIZE          320
#define LCD_YSIZE          240
#define LCD_CONTROLLER     HY32D
#define LCD_BITSPERPIXEL   16
#define LCD_FIXEDPALETTE   565
//#define LCD_SWAP_RB        1
/* Private define ------------------------------------------------------------*/

/* LCD Registers */
#define R0             0x00
#define R1             0x01
#define R2             0x02
#define R3             0x03
#define R4             0x04
#define R5             0x05
#define R6             0x06
#define R7             0x07
#define R8             0x08
#define R9             0x09
#define R10            0x0A
#define R12            0x0C
#define R13            0x0D
#define R14            0x0E
#define R15            0x0F
#define R16            0x10
#define R17            0x11
#define R18            0x12
#define R19            0x13
#define R20            0x14
#define R21            0x15
#define R22            0x16
#define R23            0x17
#define R24            0x18
#define R25            0x19
#define R26            0x1A
#define R27            0x1B
#define R28            0x1C
#define R29            0x1D
#define R30            0x1E
#define R31            0x1F
#define R32            0x20
#define R33            0x21
#define R34            0x22
#define R36            0x24
#define R37            0x25
#define R40            0x28
#define R41            0x29
#define R43            0x2B
#define R45            0x2D
#define R48            0x30
#define R49            0x31
#define R50            0x32
#define R51            0x33
#define R52            0x34
#define R53            0x35
#define R54            0x36
#define R55            0x37
#define R56            0x38
#define R57            0x39
#define R59            0x3B
#define R60            0x3C
#define R61            0x3D
#define R62            0x3E
#define R63            0x3F
#define R64            0x40
#define R65            0x41
#define R66            0x42
#define R67            0x43
#define R68            0x44
#define R69            0x45
#define R70            0x46
#define R71            0x47
#define R72            0x48
#define R73            0x49
#define R74            0x4A
#define R75            0x4B
#define R76            0x4C
#define R77            0x4D
#define R78            0x4E
#define R79            0x4F
#define R80            0x50
#define R81            0x51
#define R82            0x52
#define R83            0x53
#define R96            0x60
#define R97            0x61
#define R106           0x6A
#define R118           0x76
#define R128           0x80
#define R129           0x81
#define R130           0x82
#define R131           0x83
#define R132           0x84
#define R133           0x85
#define R134           0x86
#define R135           0x87
#define R136           0x88
#define R137           0x89
#define R139           0x8B
#define R140           0x8C
#define R141           0x8D
#define R143           0x8F
#define R144           0x90
#define R145           0x91
#define R146           0x92
#define R147           0x93
#define R148           0x94
#define R149           0x95
#define R150           0x96
#define R151           0x97
#define R152           0x98
#define R153           0x99
#define R154           0x9A
#define R157           0x9D
#define R192           0xC0
#define R193           0xC1
#define R229           0xE5

/* LCD color */
#define Alfa           0xffff
#define White          0xF79E
#define Black          0x0000
#define Grey           0xF7DE
#define Blue           0x001F
#define Blue2          0x051F
#define Red            RGB565CONVERT(255,0,0)
#define Magenta        0xF81F
#define Green          RGB565CONVERT(0,255,0)
#define Cyan           0x7FFF
#define Yellow         RGB565CONVERT(255,255,0)

#define HZ_LIB         1
#define ASCII_LIB      1

#define RGB565CONVERT(red, green, blue) (int) (((red >> 3) << 11) | ((green >> 2) << 5) | (blue >> 3))

#define WINDOW_HEADER RGB565CONVERT(49,82,123)
#define BACKGROUND  RGB565CONVERT(219,219,219)
#define MAIN_WINDOW_TEXT RGB565CONVERT(49,82,123)

#define PALLETE_BLUE  0
#define PALLETE_GREEN  1

#define MENU_HEADER RGB565CONVERT(247,151,37)

#define   COLOR_MENU_ITEM       RGB565CONVERT(49,82,123)
#define   COLOR_MENU_SELECTED   RGB565CONVERT(58,173,49)
#define   COLOR_MENU_EDIT       RGB565CONVERT(58,173,49)

/* Private function prototypes -----------------------------------------------*/
void LCD_Initializtion(void);
void LCD_BackLight_Init(void); 
void LCD_Clear(uint16_t Color);	
uint16_t LCD_BGR2RGB(uint16_t color);
////////////////////////////////FunctionalState LCD_BackLight( uint8_t percent);
uint16_t LCD_GetPoint(uint16_t Xpos,uint16_t Ypos);
void LCD_SetPoint(uint16_t Xpos,uint16_t Ypos,uint16_t point);
void LCD_DrawLine(int x1, int y1, int x2, int y2,uint16_t bkColor);
void LCD_SetWindows(uint16_t xStart,uint16_t yStart,uint16_t xLong,uint16_t yLong);
void LCD_DrawPicture(uint16_t StartX,uint16_t StartY,uint16_t EndX,uint16_t EndY,uint16_t *pic);
void LCD_DrawPictureLCDImageConverter(uint16_t StartX,uint16_t StartY,tImage pic,uint16_t color);
void GUI_Text(uint16_t Xpos, uint16_t Ypos, uint8_t *str,uint16_t Color, uint16_t bkColor);
void PutChar(unsigned short Xpos,unsigned short Ypos,unsigned char c,unsigned short charColor,unsigned short bkColor);
void PutChinese(uint16_t Xpos,uint16_t Ypos,uint8_t *str,uint16_t Color,uint16_t bkColor);
void GUI_Chinese(uint16_t Xpos, uint16_t Ypos, uint8_t *str,uint16_t Color, uint16_t bkColor);


// void LCD_SetReg(void);

void GUI_Text_Vertical(uint16_t Xpos, uint16_t Ypos, uint8_t *str,uint16_t Color, uint16_t bkColor);
void PutChar_Vertical(unsigned short Xpos,unsigned short Ypos,unsigned char c,unsigned short charColor,unsigned short bkColor);
void GUI_Text2_Vertical(uint16_t Xpos, uint16_t Ypos, uint8_t *str,uint16_t Color, uint16_t bkColor);
void PutChar2_Vertical(unsigned short Xpos,unsigned short Ypos,unsigned char c,unsigned short charColor,unsigned short bkColor);
void GUI_Text4_Vertical(uint16_t Xpos, uint16_t Ypos, uint8_t *str,uint16_t Color, uint16_t bkColor);
void PutChar4_Vertical(unsigned short Xpos,unsigned short Ypos,unsigned char c,unsigned short charColor,unsigned short bkColor);

/*extern  const unsigned char Small_Fonts_5[];
extern  const unsigned char Small_Fonts_7[];
extern  const unsigned char Tahoma_10[];
extern  const unsigned char Tahoma_12[];
*/

////////////////////////////////////////////////////////////////////////////////
extern  const unsigned char Myriad_Pro_16[];
////////////////////////////////////////////////////////////////////////////////

#define ALIGN_LEFT      0
#define ALIGN_RIGHT     1
#define ALIGN_CENTER    2

void DisplaySymbol(int x, int y, int width, int height,  char* ptr, int color);
void DisplaySymbolRect(int x, int y, int width, int height,  char* ptr, int background, int color);
void DisplaySymbol_16(int x, int y,  int width, int height, const unsigned char* ptr, const unsigned int* colors, unsigned short background);
void DisplaySymbol_256(int x, int y, unsigned short* ptr);

int DisplayBigDigit(int x, int y, int z, char align, char* str, char pallete);
int DisplaySmallDigit(int x, int y, char align, char* str, char pallete);
int DisplayDigit(int x, int y, char align, char* str, char pallete);


#define MAX_STR_LEN     64
int GetTextExtent(char* str, char* font);

int DisplayFontStr(int x, int y, char align, char* str,  char* font,int color);
int DisplayFontRect(int x, int y, char align, char* str,  char* font, int background, int color);

int VFDPrint ( const char *str,  const int x, const int y
              , const int size_x, const int size_y);


void LCD_DrawInvertFrame(int x, int y, int width, int height, int color);
void VFD_DrawLine(int x, int y, int width, int height);
void LCD_ClearRect(int x, int y, int width, int height, int color);


void VFD_DrawWindow(int x, int y, int width, int height, char* header);
void VFD_DrawTopWindow(int x, int y, int width, int height);
void LCD_DrawSingleFrame(int x, int y, int width, int height, unsigned short color);

void VFD_Line(int x1, int y1, int x2, int y2);
void VFD_MoveTo(int x, int y);
void VFD_LineTo(int x, int y);


#endif 
/*********************************************************************************************************
      END FILE
*********************************************************************************************************/





