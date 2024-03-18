#include "GLCD.h" 
#include "GLCD_Init.h" 
#include <math.h>
#include <string.h>
#include "stdbool.h"
//////////////////////////////////////////////////////////#include "stm32f10x.h" 

#include "stm32f103xg.h"

//#include "img.h"
//#include "task.h"

//#include "Small_Fonts_5.c"
//#include "Tahoma_10.c"
//#include "Myriad_Pro_16.c"
//#include "Tahoma_16.c"
//#include "Small_Fonts_7.c"
//#include "stm32f10x_it.h"

#define LCD_COLUMN_CNT          320
#define LCD_LINE_CNT            240


/* Private variables ---------------------------------------------------------*/
static uint16_t DeviceCode;

/* Private define ------------------------------------------------------------*/
#define LCD_REG              (*((volatile unsigned short *) 0x60000000)) /* RS = 0 */
#define LCD_RAM              (*((volatile unsigned short *) 0x60020000)) /* RS = 1 */

extern bool LCDInitComplete;                                                     // Ð˜Ð½Ð¸Ñ†Ð¸Ð°Ð»Ð¸Ð·Ð°Ñ†Ð¸Ñ Ð´Ð¸ÑÐ¿Ð»ÐµÑ Ð·Ð°Ð²ÐµÑ€ÑˆÐµÐ½Ð°
void LCD_WriteReg(uint8_t LCD_Reg,uint16_t LCD_RegValue);
uint16_t LCD_ReadReg(uint8_t LCD_Reg);
static void LCD_WriteRAM_Prepare(void);
void LCD_WriteRAM(uint16_t RGB_Code);
static void LCD_Delay(uint16_t nCount);

void LCD_Init(void)
{
    
  /* Enable FSMC clock */
  RCC->AHBENR &= ~(RCC_AHBENR_FSMCEN);//|RCC_AHBENR_SRAMEN|RCC_AHBENR_FLITFEN);
  
  AFIO->MAPR2 |= AFIO_MAPR2_FSMC_NADV_REMAP;

  LCD_PORT_E_CFG_GPIO;
  LCD_PORT_E_CFG_AF_PP;

  LCD_PORT_D_CFG_GPIO;
  LCD_PORT_D_CFG_AF_PP;  
   
/*----------------  FSMC Configuration ---------------------------------------*/  
/*----------------  Enable FSMC Bank1_SRAM Bank ------------------------------*/
  
//  BTCR
    
// 1 #define  FSMC_BCR1_MBKEN                     ((uint32_t)0x00000001)        /*!<Memory bank enable bit */
// 0 #define  FSMC_BCR1_MUXEN                     ((uint32_t)0x00000002)        /*!<Address/data multiplexing enable bit */

// 00 - SRAM #define  FSMC_BCR1_MTYP                      ((uint32_t)0x0000000C)        /*!<MTYP[1:0] bits (Memory type) */
//  -- #define  FSMC_BCR1_MTYP_0                    ((uint32_t)0x00000004)        /*!<Bit 0 */
//  -- #define  FSMC_BCR1_MTYP_1                    ((uint32_t)0x00000008)        /*!<Bit 1 */

// 01 -16 bits #define  FSMC_BCR1_MWID                      ((uint32_t)0x00000030)        /*!<MWID[1:0] bits (Memory data bus width) */
//    1 -- #define  FSMC_BCR1_MWID_0                    ((uint32_t)0x00000010)        /*!<Bit 0 */
//    0 -- #define  FSMC_BCR1_MWID_1                    ((uint32_t)0x00000020)        /*!<Bit 1 */

// 0 #define  FSMC_BCR1_FACCEN                    ((uint32_t)0x00000040)        /*!<Flash access enable */
// 0 #define  FSMC_BCR1_BURSTEN                   ((uint32_t)0x00000100)        /*!<Burst enable bit */
// 0 - Low #define  FSMC_BCR1_WAITPOL                   ((uint32_t)0x00000200)        /*!<Wait signal polarity bit */
// 0 #define  FSMC_BCR1_WRAPMOD                   ((uint32_t)0x00000400)        /*!<Wrapped burst mode support */
// 0 #define  FSMC_BCR1_WAITCFG                   ((uint32_t)0x00000800)        /*!<Wait timing configuration */
// 1 #define  FSMC_BCR1_WREN                      ((uint32_t)0x00001000)        /*!<Write enable bit */
// 0 #define  FSMC_BCR1_WAITEN                    ((uint32_t)0x00002000)        /*!<Wait enable bit */
// 0 #define  FSMC_BCR1_EXTMOD                    ((uint32_t)0x00004000)        /*!<Extended mode enable */
// 0 #define  FSMC_BCR1_CBURSTRW                  ((uint32_t)0x00080000)        /*!<Write burst enable */

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#define  FSMC_BTR1_ADDSET_1                  ((uint32_t)0x00000002)        /*!<Bit 1 */
#define  FSMC_BTR1_ADDSET_3                  ((uint32_t)0x00000008)        /*!<Bit 3 */
#define  FSMC_BTR1_DATAST_1                  ((uint32_t)0x00000200)        /*!<Bit 1 */
#define  FSMC_BTR1_DATAST_3                  ((uint32_t)0x00000800)        /*!<Bit 3 */ 
#define  FSMC_BTR1_BUSTURN_1                 ((uint32_t)0x00020000)        /*!<Bit 1 */
#define  FSMC_BTR1_BUSTURN_3                 ((uint32_t)0x00080000)        /*!<Bit 3 */
    
#define  FSMC_BCR1_MWID_0                    ((uint32_t)0x00000010)        /*!<Bit 0 */
#define  FSMC_BCR1_WREN                      ((uint32_t)0x00001000)        /*!<Write enable bit */
#define  FSMC_BCR1_MBKEN                     ((uint32_t)0x00000001)        /*!<Memory bank enable bit */
    
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// FSMC timing. Use BTRx where x is the sub-bank.
	FSMC_Bank1->BTCR[1] = (FSMC_BTR1_ADDSET_1 | FSMC_BTR1_ADDSET_3) | (FSMC_BTR1_DATAST_1 | FSMC_BTR1_DATAST_3) | (FSMC_BTR1_BUSTURN_1 | FSMC_BTR1_BUSTURN_3);
 
	// Bank1 NOR/SRAM control register configuration. Use BCRx where x is the sub-bank.
	FSMC_Bank1->BTCR[0] = FSMC_BCR1_MWID_0 | FSMC_BCR1_WREN | FSMC_BCR1_MBKEN;
        
  //FSMC_BTR1
// FSMC timing. Use BTRx where x is the sub-bank.  
//  FSMC_Bank1->BTCR[5] = (10<<0)|(5<<4)|(10<<8)|(0x01<<12)|(0x01<<16)|(0x01<<24);
  
RCC->AHBENR |= (RCC_AHBENR_FSMCEN|RCC_AHBENR_SRAMEN);//|RCC_AHBENR_SRAMEN|RCC_AHBENR_FLITFEN);
//  == #define  FSMC_BTR1_ADDSET                    ((uint32_t)0x0000000F)        /*!<ADDSET[3:0] bits (Address setup phase duration) */
//  0== #define  FSMC_BTR1_ADDSET_0                  ((uint32_t)0x00000001)        /*!<Bit 0 */
//  1== #define  FSMC_BTR1_ADDSET_1                  ((uint32_t)0x00000002)        /*!<Bit 1 */
//  0== #define  FSMC_BTR1_ADDSET_2                  ((uint32_t)0x00000004)        /*!<Bit 2 */
//  1== #define  FSMC_BTR1_ADDSET_3                  ((uint32_t)0x00000008)        /*!<Bit 3 */

//  == #define  FSMC_BTR1_ADDHLD                    ((uint32_t)0x000000F0)        /*!<ADDHLD[3:0] bits (Address-hold phase duration) */
//  == #define  FSMC_BTR1_ADDHLD_0                  ((uint32_t)0x00000010)        /*!<Bit 0 */
//  == #define  FSMC_BTR1_ADDHLD_1                  ((uint32_t)0x00000020)        /*!<Bit 1 */
//  == #define  FSMC_BTR1_ADDHLD_2                  ((uint32_t)0x00000040)        /*!<Bit 2 */
//  == #define  FSMC_BTR1_ADDHLD_3                  ((uint32_t)0x00000080)        /*!<Bit 3 */

//  == #define  FSMC_BTR1_DATAST                    ((uint32_t)0x0000FF00)        /*!<DATAST [3:0] bits (Data-phase duration) */
//  == #define  FSMC_BTR1_DATAST_0                  ((uint32_t)0x00000100)        /*!<Bit 0 */
//  == #define  FSMC_BTR1_DATAST_1                  ((uint32_t)0x00000200)        /*!<Bit 1 */
//  == #define  FSMC_BTR1_DATAST_2                  ((uint32_t)0x00000400)        /*!<Bit 2 */
//  == #define  FSMC_BTR1_DATAST_3                  ((uint32_t)0x00000800)        /*!<Bit 3 */

//  == #define  FSMC_BTR1_BUSTURN                   ((uint32_t)0x000F0000)        /*!<BUSTURN[3:0] bits (Bus turnaround phase duration) */
//  == #define  FSMC_BTR1_BUSTURN_0                 ((uint32_t)0x00010000)        /*!<Bit 0 */
//  == #define  FSMC_BTR1_BUSTURN_1                 ((uint32_t)0x00020000)        /*!<Bit 1 */
//  == #define  FSMC_BTR1_BUSTURN_2                 ((uint32_t)0x00040000)        /*!<Bit 2 */
//  == #define  FSMC_BTR1_BUSTURN_3                 ((uint32_t)0x00080000)        /*!<Bit 3 */

//  == #define  FSMC_BTR1_CLKDIV                    ((uint32_t)0x00F00000)        /*!<CLKDIV[3:0] bits (Clock divide ratio) */
//  == #define  FSMC_BTR1_CLKDIV_0                  ((uint32_t)0x00100000)        /*!<Bit 0 */
//  == #define  FSMC_BTR1_CLKDIV_1                  ((uint32_t)0x00200000)        /*!<Bit 1 */
//  == #define  FSMC_BTR1_CLKDIV_2                  ((uint32_t)0x00400000)        /*!<Bit 2 */
//  == #define  FSMC_BTR1_CLKDIV_3                  ((uint32_t)0x00800000)        /*!<Bit 3 */

//  == #define  FSMC_BTR1_DATLAT                    ((uint32_t)0x0F000000)        /*!<DATLA[3:0] bits (Data latency) */
//  == #define  FSMC_BTR1_DATLAT_0                  ((uint32_t)0x01000000)        /*!<Bit 0 */
//  == #define  FSMC_BTR1_DATLAT_1                  ((uint32_t)0x02000000)        /*!<Bit 1 */
//  == #define  FSMC_BTR1_DATLAT_2                  ((uint32_t)0x04000000)        /*!<Bit 2 */
//  == #define  FSMC_BTR1_DATLAT_3                  ((uint32_t)0x08000000)        /*!<Bit 3 */

// 00 - Mode A #define  FSMC_BTR1_ACCMOD                    ((uint32_t)0x30000000)        /*!<ACCMOD[1:0] bits (Access mode) */
//  == #define  FSMC_BTR1_ACCMOD_0                  ((uint32_t)0x10000000)        /*!<Bit 0 */
//  == #define  FSMC_BTR1_ACCMOD_1                  ((uint32_t)0x20000000)        /*!<Bit 1 */  
  
  
  LCD_Delay(200);  /* delay 50 ms */		
  DeviceCode = LCD_ReadReg(0x0000);		

  if(DeviceCode==0x9325 || DeviceCode==0x9328)	
  {
    for(int i=0; i<56; i++)
    {
      LCD_WriteReg(Init_9325[i].Add, Init_9325[i].Data);
      LCD_Delay(150);  
    }
  }
  else if(DeviceCode==0x9320 || DeviceCode==0x9300)
  {
    for(int i=0; i<38; i++)
    {
      LCD_WriteReg(Init_9320[i].Add, Init_9320[i].Data);
      LCD_Delay(100);  
    }
  }
  else if(DeviceCode==0x9331)
  {
    for(int i=0; i<50; i++)
    {
      LCD_WriteReg(Init_9331[i].Add, Init_9331[i].Data);
      LCD_Delay(120);  
    }
  }
  else if(DeviceCode==0x9919)
  {
    for(int i=0; i<30; i++)
    {
      LCD_WriteReg(Init_9919[i].Add, Init_9919[i].Data);
    }
  }
  else if(DeviceCode==0x1505)
  {
    for(int i=0; i<59; i++)
    {
      LCD_WriteReg(Init_1505[i].Add, Init_1505[i].Data);
      LCD_Delay(120);  
    }
  }
  else if(DeviceCode==0x8989)
  {
    for(int i=0; i<41; i++)
    {
      LCD_WriteReg(Init_8989[i].Add, Init_8989[i].Data);
      LCD_Delay(150);  
    }
  }
  LCD_Delay(150);  /* delay 50 ms */	
/////////////////////////////////////////
LCD_Clear(Black);
////////////////////////////////////////////////////////////////////////////////
/*
  LCD_DrawLine(0,0,159,0,Green);
  LCD_DrawLine(159,0,159,119,Green);
  LCD_DrawLine(159,119,0,119,Green);
  LCD_DrawLine(0,119,0,0,Green);
  
  LCD_DrawLine(161,0,319,0,Blue);
  LCD_DrawLine(319,0,320,119,Blue);
  LCD_DrawLine(319,119,161,119,Blue);
  LCD_DrawLine(161,119,161,0,Blue);
  
  LCD_DrawLine(0,121,159,121,Red);
  LCD_DrawLine(159,121,159,239,Red);
  LCD_DrawLine(159,239,0,239,Red);
  LCD_DrawLine(0,239,0,121,Red);
*/
////////////////////////////////////////////////////////////////////////////////
//LCD_DrawLine(10,10, 200,200,Red);
//DisplayFontRect(100, 100, ALIGN_RIGHT, "25 68 97", (char*)Myriad_Pro_16, Black, Green); 

LCDInitComplete=true;
}

//----------------------------------------------
void LCD_WriteReg(uint8_t LCD_Reg,uint16_t LCD_RegValue)
{
  LCD_REG = LCD_Reg; /* Write 16-bit Index, then Write Reg */
  LCD_RAM = LCD_RegValue;   /* Write 16-bit Reg */
}

//----------------------------
uint16_t LCD_ReadReg(uint8_t LCD_Reg)
{
  LCD_REG = LCD_Reg;				   /* Write 16-bit Index (then Read Reg) */
  return (LCD_RAM);	/* Read 16-bit Reg */
}

//----------------------------------------
static void LCD_WriteRAM_Prepare(void)
{
  LCD_REG = R34;
}

//------------------------------------
void LCD_WriteRAM(uint16_t RGB_Code)					 
{
  LCD_RAM = RGB_Code;  /* Write 16-bit GRAM Reg */
}

//----------------------------------------------
static uint16_t LCD_ReadRAM(void)
{
  volatile uint16_t dummy; 
  /* Write 16-bit Index (then Read Reg) */
  LCD_REG = R34; /* Select GRAM Reg */
  dummy = LCD_RAM; 	/* Read 16-bit Reg */
  return LCD_RAM;
}

//-------------------------------------------------------------
static void LCD_SetCursor(uint16_t Xpos,uint16_t Ypos)
{
  ///////////////Ïåðåâåðíóòîå èçîáðàæåíèå///////////////////////////////////////
  //Xpos=(uint16_t)LCD_COLUMN_CNT-Xpos-112;
  //Ypos=(uint16_t)LCD_LINE_CNT-Ypos+79;
  
  Xpos=LCD_COLUMN_CNT-Xpos-1;
  //Ypos=LCD_LINE_CNT-Ypos;
  ////////////////////////////////////////////////////////////////////////////// 
  if(DeviceCode==0x8989)
  {
    LCD_WriteReg(0x004e,Ypos); /* Row */
    LCD_WriteReg(0x004f,Xpos); /* Line */ 
  }
  else if(DeviceCode==0x9919)
  {
    LCD_WriteReg(0x004e,Xpos); /* Row */
    LCD_WriteReg(0x004f,Ypos); /* Line */
  }
  else
  {
    LCD_WriteReg(0x0020,Xpos); /* Row */
    LCD_WriteReg(0x0021,Ypos); /* Line */
  }
}

//--------------------------------------
static void LCD_Delay(uint16_t nCount)
{
  uint16_t TimingDelay; 
  while(nCount--)
  {
    for(TimingDelay=0;TimingDelay<10;TimingDelay++);
  }
}

/*  
void LCD_SetReg()
{
    LCD_WriteReg(0x0000,LCD.Reg30);    LCD_Delay(5);   
    LCD_WriteReg(0x0003,LCD.Reg31);    LCD_Delay(5); /// 0xA8A4 
    LCD_WriteReg(0x0030,LCD.Reg32);    LCD_Delay(5);
    LCD_WriteReg(0x0031,LCD.Reg33);    LCD_Delay(5);
    LCD_WriteReg(0x0032,LCD.Reg34);    LCD_Delay(5);//0x0707
    LCD_WriteReg(0x0033,LCD.Reg35);    LCD_Delay(5);
    LCD_WriteReg(0x0034,LCD.Reg36);    LCD_Delay(5);
    LCD_WriteReg(0x0035,LCD.Reg37);    LCD_Delay(5);//0x0707
    LCD_WriteReg(0x0036,LCD.Reg38);    LCD_Delay(5);
    LCD_WriteReg(0x0037,LCD.Reg39);    LCD_Delay(5);
    LCD_WriteReg(0x003A,LCD.Reg3A);    LCD_Delay(5);
    LCD_WriteReg(0x003B,LCD.Reg3B);    LCD_Delay(5);
}
*/

/******************************************************************************
* Function Name  : LCD_SetWindows
* Description    : Sets Windows Area.
* Input          : - StartX: Row Start Coordinate 
*                  - StartY: Line Start Coordinate  
*				   - xLong:  
*				   - yLong: 
* Output         : None
* Return         : None
* Attention		 : None
******************************************************************************
void LCD_SetWindows(uint16_t xStart,uint16_t yStart,uint16_t xLong,uint16_t yLong)
{
  LCD_SetCursor(xStart,yStart); 
  LCD_WriteReg(0x0050,xStart);         
  LCD_WriteReg(0x0051,xStart+xLong-1); 
  LCD_WriteReg(0x0052,yStart);         
  LCD_WriteReg(0x0053,yStart+yLong-1); 
}
*/
//------------------------------
void LCD_Clear(uint16_t Color)
{
  uint32_t index=0;
  LCD_SetCursor(0,0); 
  LCD_WriteRAM_Prepare(); /* Prepare to write GRAM */
  for(index=0;index<76800;index++)
  {
     LCD_RAM=Color;
  }
}

/******************************************************************************
* Function Name  : LCD_GetPoint
* Description    : ¬¸Láã¬¦è+•-ú¦-T-ãëã¦
* Input          : - Xpos: Row Coordinate
*                  - Xpos: Line Coordinate 
* Output         : None
* Return         : Screen Color
* Attention		 : None
*******************************************************************************/
uint16_t LCD_GetPoint(uint16_t Xpos,uint16_t Ypos)
{
  LCD_SetCursor(Xpos,Ypos);
  if( DeviceCode==0x7783 || DeviceCode==0x4531 || DeviceCode==0x8989 )
    return ( LCD_ReadRAM() );
  else
    return ( LCD_BGR2RGB(LCD_ReadRAM()) );
}

/******************************************************************************
* Function Name  : LCD_SetPoint
* Description    : L-ã¬¦è+•-ú¬í¦ó
* Input          : - Xpos: Row Coordinate
*                  - Ypos: Line Coordinate 
* Output         : None
* Return         : None
* Attention		 : None
*******************************************************************************/
void LCD_SetPoint(uint16_t Xpos,uint16_t Ypos,uint16_t point)
{
  if ( ( Xpos >= LCD_XSIZE ) ||( Ypos>=LCD_YSIZE ) ) return;
  LCD_SetCursor(Xpos,Ypos);
  LCD_WriteRAM_Prepare();
  LCD_WriteRAM(point);
}

/******************************************************************************
* Function Name  : LCD_DrawPicture
* Description    : L-ã¬¦è+°-ú¬¦+ç¦L¦-T¬¬-=-¦ì
* Input          : - StartX: Row Start Coordinate 
*                  - StartY: Line Start Coordinate  
*				   - EndX: Row End Coordinate 
*				   - EndY: Line End Coordinate   
* Output         : None
* Return         : None
* Attention		 : =-¦ìLá-ã¬¸¦-+êTî¦-ãè+øãì16+¬T-ãë-ã¦-
******************************************************************************/
void LCD_DrawPicture(uint16_t StartX,uint16_t StartY,uint16_t EndX,uint16_t EndY,uint16_t *pic)
{

  uint16_t  i;
  LCD_SetCursor(StartX,StartY);  
  LCD_WriteRAM_Prepare();
  for (i=0;i<(EndX*EndY);i++)
  {
      LCD_WriteRAM(*pic++);
  }

  /////////////////////////////////////////////////////////////////////////////////
 /*     
    for(int i=0; i<width; i++)
    {                
        LCD_SetCursor(i+x,y+height);  
      
        LCD_WriteRAM_Prepare();
        //for (int j=0; j<height; j++)
        for (int j=height-1; j>=0; j--)
        {              
          unsigned char pixel;
          unsigned char bpl = width/2 + (width-2*(width>>1));
            
          if ((i+j*width)%2)
            pixel = ptr[2+i/2+j*bpl]&0x0F;
          else
            pixel = (ptr[2+i/2+j*bpl]&0xF0)>>4;
          
          unsigned short color = LCD_RGB2BRG(colors[pixel]);
          if(color==0xFFFF)
          {
            LCD_WriteRAM(background);
            //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  background);
            LCD_SetPoint(i+x,j+y,background);
          }
          else
          {
            //LCD_WriteRAM(color);
            //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color);            
          }
        }
    }
*/
  /////////////////////////////////////////////////////////////////////////////////  
}

void LCD_DrawPictureLCDImageConverter(uint16_t StartX,uint16_t StartY,tImage pic,uint16_t color)
{
  
uint16_t i,j,k;

//image_data=pic[0];
//Lines=pic.height;
//Columns=pic[2];

LCD_SetCursor(StartX,StartY);  
LCD_WriteRAM_Prepare();
k=0;
for (j=0;j<pic.height;j++) 
  {
  for (i=0;i<pic.width;i++)
    {       
    if (color!=pic.data[k])
      {
      LCD_SetPoint(StartX+i,StartY+j,pic.data[k]);
      }
    k=k+1;
    };
  }; 

}

//------------------------------------------------
void LCD_DrawVertLine(int x1, int y1, int height, uint16_t color)  
{
    LCD_SetCursor(LCD_LINE_CNT-(y1+height)-1,x1);
    LCD_WriteRAM_Prepare();
    for(int i=LCD_LINE_CNT-(y1+height)-1; i<LCD_LINE_CNT-y1-1; i++) 
    {
      //LCD_SetCursor(i,x1);
      //LCD_WriteRAM_Prepare();
      LCD_WriteRAM(color);      
    }  
}

void LCD_DrawHorzLine(int x1, int y1, int width, uint16_t color)  
{
    for(int i = x1; i<x1+width; i++)
    {
      LCD_SetCursor(LCD_LINE_CNT-y1-1,i);
      LCD_WriteRAM_Prepare();
      LCD_WriteRAM(color);
    }
}

void LCD_Line(int x1, int y1, int x2, int y2, unsigned short color)
{
  LCD_DrawLine(LCD_LINE_CNT - y1, x1, LCD_LINE_CNT - y2, x2, color);    
}

void LCD_DrawLine(int x1, int y1, int x2, int y2,uint16_t bkColor)  
{ 
  int x,y,dx,dy,Dx,Dy,e,i; 
  Dx=x2-x1; 
  Dy=y2-y1; 

  dx=(int)fabs(x2-x1); 
  dy=(int)fabs(y2-y1); 
  x=x1; 
  y=y1; 
  if(dy>dx) 
  { 
    e=-dy; 
    for(i=0;i<dy;i++) 
    { 
      LCD_SetPoint(x,y,bkColor); 
      if(Dy>=0) y++;   
      else y--;    
      e+=2*dx; 
      if(e>=0) 
      { 
        if(Dx>=0) x++; 
        else x--;  
        e-=2*dy; 
      } 
    } 
  } 
  else 
  { 
    e=-dx; 
    for(i=0;i<dx;i++) 
    { 
      LCD_SetPoint(x,y,bkColor); 
      if(Dx>=0) x++; 
      else x--; 
      e+=2*dy; 
      if(e>=0) 
      { 
        if(Dy>=0) y++; 
        else y--;
        e-=2*dx;
      } 
    } 
  } 
} 

uint16_t LCD_RGB2BRG(uint32_t color)
{
  uint16_t  r, g, b, bgr;

  b = (( color>>0 )>>3)  & 0x1f;
  g = (( color>>8 )>>2)  & 0x3f;
  r = (( color>>16 )>>3) & 0x1f;
 
  bgr =  (b<<11) + (g<<5) + (r<<0);

  return( bgr );  
}

/******************************************************************************
* Function Name  : LCD_BGR2RGB
* Description    : RRRRRGGGGGGBBBBB -> BBBBBGGGGGGRRRRR 
* Input          : - color: BRG 
* Output         : None
* Return         : RGB 
* Attention		 : 
*******************************************************************************/
uint16_t LCD_BGR2RGB(uint16_t color)
{
  uint16_t  r, g, b, rgb;

  b = ( color>>0 )  & 0x1f;
  g = ( color>>5 )  & 0x3f;
  r = ( color>>11 ) & 0x1f;
 
  rgb =  (b<<11) + (g<<5) + (r<<0);

  return( rgb );
}

void DisplaySymbol(int x, int y, int width, int height,  char* ptr, int color)
{
    if ( x >= LCD_COLUMN_CNT)  
        return; 
    if ( x < 0)
    {        
        x=0;    width = width+x;         
        if (width <= 0)
            return;
    }
    
    if ( (y>>3) >= LCD_LINE_CNT)  
        return; 
    
    if ( y < 0)
    {
        y=0;      
        height = height+y;         
        if (height <= 0)
            return;
    }
    
    int ptr_size_x = (width>>3);
    if (width%8)
      ptr_size_x++;
    
    for (int j=0; j<height; j++)
    {
        int idx_ptr = j*ptr_size_x;
        unsigned char ptr_mask = 0x80;
        
        for(int i=0; i<width; i++)
        {              
            if (ptr[idx_ptr] & ptr_mask)
            {
                //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color);
            LCD_SetPoint(i+x,j+y,color);
            }

            if (ptr_mask == 0x01)
            {
                ptr_mask = 0x80;
                idx_ptr++;
            }
            else
                ptr_mask = ptr_mask>>1;            
        }
    }
}

void DisplaySymbolRect(int x, int y, int width, int height,  char* ptr, int background,int color)
{
  if (x>= LCD_COLUMN_CNT)  
    return; 
  if (x<0)
    {x=0;width = width+x;         
        if (width <= 0)
            return;
    }
    
    if ( (y>>3) >= LCD_LINE_CNT)  
        return; 
    
    if ( y < 0)
    {
        y=0;      
        height = height+y;         
        if (height <= 0)
            return;
    }
    
    int ptr_size_x = (width>>3);
    if (width%8)
      ptr_size_x++;
    
    for (int j=0; j<height; j++)
    {
        int idx_ptr = j*ptr_size_x;
        unsigned char ptr_mask = 0x80;
        
        for(int i=0; i<width; i++)
        {              
            if (ptr[idx_ptr] & ptr_mask)
                //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color);
              LCD_SetPoint(i+x,j+y,color);
            else
                //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  background);
                LCD_SetPoint(i+x,j+y,background);

            if (ptr_mask == 0x01)
            {
                ptr_mask = 0x80;
                idx_ptr++;
            }
            else
                ptr_mask = ptr_mask>>1;            
        }
    }
}

void DisplaySymbol_16(int x, int y,  int width, int height, const unsigned char* ptr, const unsigned int* colors, unsigned short background)
{
        
    for(int i=0; i<width; i++)
    {                
        //LCD_SetCursor(LCD_LINE_CNT - (y+height), (i+x));  
        LCD_SetCursor(i+x,height+y);
        
        LCD_WriteRAM_Prepare();
        //for (int j=0; j<height; j++)
        for (int j=height-1; j>=0; j--)
        {              
          unsigned char pixel;
          unsigned char bpl = width/2 + (width-2*(width>>1));
            
          if ((i+j*width)%2)
            pixel = ptr[2+i/2+j*bpl]&0x0F;
          else
            pixel = (ptr[2+i/2+j*bpl]&0xF0)>>4;
          
          unsigned short color = LCD_RGB2BRG(colors[pixel]);
          if(color==0xFFFF)
          {
            LCD_WriteRAM(background);
            //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  background);
            //LCD_SetPoint(i+x,j+y,background);
          }
          else
          {
            LCD_WriteRAM(color);
            //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color); 
            LCD_SetPoint(i+x,j+y,color);
          }
        }
    }
}

/*
int DisplayDigit(int x, int y, char align, char* str, char pallete)
{
    int len =0;
    for(int i=0; i<strlen(str); i++)
    {
        switch(str[i])
        {
          case '0':len+=Middle0[0];break;
          case '1':len+=Middle1[0];break;
          case '2':len+=Middle2[0];break;
          case '3':len+=Middle3[0];break;
          case '4':len+=Middle4[0];break;
          case '5':len+=Middle5[0];break;
          case '6':len+=Middle6[0];break;
          case '7':len+=Middle7[0];break;
          case '8':len+=Middle8[0];break;
          case '9':len+=Middle9[0];break;
          case 'C':len+=MiddleC[0];break;
          case '+':len+=MiddlePlus[0];break;
          case '-':len+=MiddleMinus[0];break;
          case '.':len+=MiddlePt[0];break;
          case ',':len+=MiddleComma[0];break;
          case ':':len+=MiddleDpt[0];break;
          case '%':len+=MiddlePercent[0];break;          
          case 'I':len+=Ieng[0];break;
          case 'L':len+=Irus[0];break;
//          case '+':len+=Erus[0];break;
          case 'E':len+=Eeng[0];break;
//          case 'V':len+=Veng[0];break;          
//          case 'T':len+=Vrus[0];break;          
          case ' ':len+=16; break;
          
        }      
    }
    
    switch(align)
    {
        case ALIGN_LEFT: break;
        case ALIGN_CENTER: x = x-len/2;break;
        case ALIGN_RIGHT:  x = x-len; break;
    }    
    
    while(*str!=0)
    {
        unsigned char* ptr;
        switch(*str)
        {
          case '0':ptr = (unsigned char*)Middle0;break;
          case '1':ptr = (unsigned char*)Middle1;break;
          case '2':ptr = (unsigned char*)Middle2;break;
          case '3':ptr = (unsigned char*)Middle3;break;
          case '4':ptr = (unsigned char*)Middle4;break;
          case '5':ptr = (unsigned char*)Middle5;break;
          case '6':ptr = (unsigned char*)Middle6;break;
          case '7':ptr = (unsigned char*)Middle7;break;
          case '8':ptr = (unsigned char*)Middle8;break;
          case '9':ptr = (unsigned char*)Middle9;break;
          case 'C':ptr = (unsigned char*)MiddleC;break;
          case '+':ptr = (unsigned char*)MiddlePlus;break;
          case '-':ptr = (unsigned char*)MiddleMinus;break;
          case '.':ptr = (unsigned char*)MiddlePt;break;
          case ',':ptr = (unsigned char*)MiddleComma;break;
          case ':':ptr = (unsigned char*)MiddleDpt;break;
          case '%':ptr = (unsigned char*)MiddlePercent;break;          
          case 'I':ptr = (unsigned char*)Ieng;break;
          case 'L':ptr = (unsigned char*)Irus;break;
//          case '+':ptr = (unsigned char*)Erus;break;
          case 'E':ptr = (unsigned char*)Eeng;break;
//          case 'V':ptr = (unsigned char*)Veng;break;
//          case 'T':ptr = (unsigned char*)Vrus;break;
          case ' ':ptr=0; LCD_ClearRect(x,y,16,26, BACKGROUND); x= x+16; break; 
          default:      ptr=0; break;
        }
        
        
        if(ptr)
        {
            short width = ptr[0];
            short height = ptr[1];
            
            for(int i=0; i<width; i++)
            {
                LCD_SetCursor(LCD_LINE_CNT - (y+height), (i+x));
                LCD_WriteRAM_Prepare();
                //for (int j=0; j<height; j++)
                for (int j=height-1; j>=0; j--)
                {              
                    unsigned char pixel;          
                    unsigned char bpl = width/2 + (width-2*(width>>1));
                    if ((i+j*width)%2)
                      pixel = ptr[2+i/2+j*bpl]&0x0F;
                    else
                      pixel = (ptr[2+i/2+j*bpl]&0xF0)>>4;
          
                    unsigned short color;
                    
                    switch(pallete)
                    {
                      case 0:  color = LCD_RGB2BRG(ColorsMiddleBlue[pixel]); break;
                      case 1:  color = LCD_RGB2BRG(ColorsMiddleGreen[pixel]); break;
                      case 2:  color = LCD_RGB2BRG(ColorsMiddleRed[pixel]); break;
                      default: color = LCD_RGB2BRG(0); break;
                    }
                      
                    LCD_WriteRAM(color);
                    //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color);
                }
            }
            x= x+width;
        }
        
        str++;
    }    
    return len;
}
*/

/*
int DisplayBigDigit(int x, int y, int z, char align, char* str, char pallete)
{
    int len=0, dz=0;
    for(int i=0; i<strlen(str); i++)
    {
        switch(str[i])
        {
          case '0':len+=Big0[0];break;
          case '1':len+=Big1[0];break;
          case '2':len+=Big2[0];break;
          case '3':len+=Big3[0];break;
          case '4':len+=Big4[0];break;
          case '5':len+=Big5[0];break;
          case '6':len+=Big6[0];break;
          case '7':len+=Big7[0];break;
          case '8':len+=Big8[0];break;
          case '9':len+=Big9[0];break;
          case 'C':len+=BigC[0];break;
          //case '+':len+=BigPlus[0];break;
          case '-':len+=BigMinus[0];break;
          case '.':len+=BigPt[0];break;
          default: len+=Big0[0];break;
          
        }      
    }
    dz = z - len;
    if(dz<2)dz=0;
    switch(align)
    {
      case ALIGN_LEFT: 
        if(dz)
        {
          LCD_ClearRect((x+len),y,(dz),Big0[1], BACKGROUND);
        }
        break;
      case ALIGN_CENTER: 
        if(dz)
        {
          LCD_ClearRect((x-z/2),y,(dz/2),Big0[1], BACKGROUND);
          LCD_ClearRect((x+len/2),y,(dz/2),Big0[1], BACKGROUND);
        }
        x = x-len/2;
        break;
      case ALIGN_RIGHT:  
        if(dz)
        {
          LCD_ClearRect((x-z),y,dz,Big0[1], BACKGROUND);
        }
        x = x-len; 
        break;
    }
    
    while(*str!=0)
    {
        unsigned char* ptr;
        switch(*str)
        {
          case '0':ptr = (unsigned char*)Big0;break;
          case '1':ptr = (unsigned char*)Big1;break;
          case '2':ptr = (unsigned char*)Big2;break;
          case '3':ptr = (unsigned char*)Big3;break;
          case '4':ptr = (unsigned char*)Big4;break;
          case '5':ptr = (unsigned char*)Big5;break;
          case '6':ptr = (unsigned char*)Big6;break;
          case '7':ptr = (unsigned char*)Big7;break;
          case '8':ptr = (unsigned char*)Big8;break;
          case '9':ptr = (unsigned char*)Big9;break;
          case 'C':ptr = (unsigned char*)BigC;break;
          //case '+':ptr = (unsigned char*)BigPlus;break;
          case '-':ptr = (unsigned char*)BigMinus;break;
          case '.':ptr = (unsigned char*)BigPt;break;
          default:
            LCD_ClearRect(x,y,Big0[0],Big0[1], BACKGROUND);
            x =x+Big0[0];
            ptr=0;
         break;   
        }
        if(ptr)
        {
            short width = ptr[0];
            short height = ptr[1];
             for(int i=0; i<width; i++)
             {
                LCD_SetCursor(LCD_LINE_CNT - (y+height), (i+x));
                LCD_WriteRAM_Prepare();
                //for (int j=0; j<height; j++)
                for (int j=height-1; j>=0; j--)
                {              
                    unsigned char pixel;          
                    unsigned char bpl = width/2 + (width-2*(width>>1));
                    if ((i+j*width)%2)
                      pixel = ptr[2+i/2+j*bpl]&0x0F;
                    else
                      pixel = (ptr[2+i/2+j*bpl]&0xF0)>>4;
          
                    unsigned short color;
                    
                    switch(pallete)
                    {
                      case 1:  color = LCD_RGB2BRG(ColorsMiddleGreen[pixel]); break;
                      case 2:  color = LCD_RGB2BRG(ColorsMiddleRed[pixel]); break;
                      default: color = LCD_RGB2BRG(ColorsMiddleBlue[pixel]); break;
                    }
                      
                    LCD_WriteRAM(color);
                    //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color);            

                }
            }
            x= x+width;
        }
        
        str++;
    }    
    return len;
}

int DisplaySmallDigit(int x, int y, char align, char* str, char pallete)
{
    int len =0;
    for(int i=0; i<strlen(str); i++)
    {
        switch(str[i])
        {
          case '0':len+=small0[0];break;
          case '1':len+=small1[0];break;
          case '2':len+=small2[0];break;
          case '3':len+=small3[0];break;
          case '4':len+=small4[0];break;
          case '5':len+=small5[0];break;
          case '6':len+=small6[0];break;
          case '7':len+=small7[0];break;
          case '8':len+=small8[0];break;
          case '9':len+=small9[0];break;
          case 'C':len+=smallC[0];break;
          case '.':len+=smallPt[0];break;
          case ':':len+=smallCollon[0];break;
          case '%':len+=smallPercent[0];break;
          case '+':len+=smallPlus[0];break;
          case '-':len+=smallMinus[0];break;
          
          default: len+=small0[0];break;
          
          
        }      
    }
    switch(align)
    {
    case ALIGN_LEFT: break;
    case ALIGN_CENTER: x = x-len/2;break;
    case ALIGN_RIGHT:  x = x-len; break;
    }
    
    while(*str!=0)
    {
        unsigned char* ptr;
        switch(*str)
        {
          case '0':ptr = (unsigned char*)small0;break;
          case '1':ptr = (unsigned char*)small1;break;
          case '2':ptr = (unsigned char*)small2;break;
          case '3':ptr = (unsigned char*)small3;break;
          case '4':ptr = (unsigned char*)small4;break;
          case '5':ptr = (unsigned char*)small5;break;
          case '6':ptr = (unsigned char*)small6;break;
          case '7':ptr = (unsigned char*)small7;break;
          case '8':ptr = (unsigned char*)small8;break;
          case '9':ptr = (unsigned char*)small9;break;
          case 'C':ptr = (unsigned char*)smallC;break;
          case '.':ptr = (unsigned char*)smallPt;break;
          case ':':ptr = (unsigned char*)smallCollon;break;
          case '%':ptr = (unsigned char*)smallPercent;break;
          case '+':ptr = (unsigned char*)smallPlus;break;
          case '-':ptr = (unsigned char*)smallMinus;break;
          default:
            LCD_ClearRect(x,y,small0[0],small0[1], BACKGROUND);
            x =x+small0[0];
            ptr=0;
         break;   
        }
        if(ptr)
        {
            short width = ptr[0];
            short height = ptr[1];
            for(int i=0; i<width; i++)
            {
                LCD_SetCursor(LCD_LINE_CNT - (y+height), (i+x));
                LCD_WriteRAM_Prepare();
                //for (int j=0; j<height; j++)
                for (int j=height-1; j>=0; j--)
               {              
                    unsigned char pixel;          
                    unsigned char bpl = width/2 + (width-2*(width>>1));
                    if ((i+j*width)%2)
                      pixel = ptr[2+i/2+j*bpl]&0x0F;
                    else
                      pixel = (ptr[2+i/2+j*bpl]&0xF0)>>4;
          
                    unsigned short color;
                    
                    if(!pallete)
                      color = LCD_RGB2BRG(ColorsSmallBlue[pixel]);
                    else
                      color = LCD_RGB2BRG(ColorsSmallGreen[pixel]);
                      
                    LCD_WriteRAM(color);
                    //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color);            

                }
            }
            x= x+width;
        }
        
        str++;
    }    
    return len;
}
*/

/* ====================  Íå èñïîëüçóåòñÿ =======================================
int GetTextExtent(char* str, char* font)
{
        int s, i, xf, yf;
        int len;
        int symbol_offset;

        xf = (int)font[0];
        yf = (int)font[1];

        symbol_offset = xf>>3;
        if (font[0]%8)
          symbol_offset++;

        symbol_offset = symbol_offset*yf+1;

        len = 0;
        i=0;
        do
        {
            s = (int)(str[i]-0x20);
            s = 2+s*symbol_offset;
            len = len + font[s];
            i++;
        }while((str[i]!=0)&&(i<MAX_STR_LEN-1));

        return len;
}
*/

int DisplayFontStr(int x, int y, char align, char* str,  char* font, int color)
{
        int s, i, xf, yf;
        int len;
        int symbol_offset;
       
        xf = (int)font[0];      //13
        yf = (int)font[1];      //21

        symbol_offset = xf>>3;      //1
        if (xf%8)                   // true
          symbol_offset++;          //2

        symbol_offset=symbol_offset*yf+1;     //2*()

            len = 0;
            i=0;
            do
            {
                if (str[i] >= 0x20)
                    s = (int)(str[i]-0x20);
                else
                    s = '?'-0x20;
                  
                s = 2+s*symbol_offset;
                len = len + font[s];                
                i++;                  
                  
            }while((str[i]!=0)&&(i<MAX_STR_LEN-1));

        switch(align)
        {
                case ALIGN_RIGHT:       x = x - len;         break;
                case ALIGN_CENTER:      x = x - len/2;        break;
        }

        for(i=0; i<strlen((char*)str); i++)
        {
                if (str[i] >= 0x20)
                    s = (int)(str[i]-0x20);
                else
                    s = '?'-0x20;

                s = 2+s*symbol_offset;
                DisplaySymbol(x, y, xf, yf, &font[s+1], color);
                x = x + font[s];
        }
        return len;  
}

int DisplayFontRect(int x, int y, char align, char* str,  char* font, int background, int color)
{
  int s, i, xf, yf;
  int len;
  int symbol_offset;
       
  xf = (int)font[0];      //13
  yf = (int)font[1];      //21

  symbol_offset = xf>>3;      //1
  if (xf%8)                   // true
    symbol_offset++;          //2
  symbol_offset=symbol_offset*yf+1;     //2*()
  len = 0;
  i=0;
        do
  {
            if (str[i] >= 0x20)
                s = (int)(str[i]-0x20);
            else
                s = '?'-0x20;
              
            s = 2+s*symbol_offset;
            len = len + font[s];                
            i++;                  
              
        }while((str[i]!=0)&&(i<MAX_STR_LEN-1));

        switch(align)
        {
                case ALIGN_RIGHT:       x = x - len;         break;
                case ALIGN_CENTER:      x = x - len/2;        break;
        }

        for(i=0; i<strlen((char*)str); i++)
        {
                if (str[i] >= 0x20)
                    s = (int)(str[i]-0x20);
                else
                    s = '?'-0x20;

                s = 2+s*symbol_offset;
                DisplaySymbolRect(x, y, font[0], yf, &font[s+1], background, color);
                x = x + font[s];
        }
        return len;  
}

/* ====================  Íå èñïîëüçóåòñÿ =======================================
void LCD_DrawInvertFrame(int x, int y, int width, int height, int color)
{
 for(int i=0; i<width; i++)
     for(int j=0; j<height; j++)
     {
       unsigned int point = LCD_GetPoint(LCD_LINE_CNT - (j+y), (i+x));
       if(point == BACKGROUND)
          point = color;
       else
         point = point^color;

       LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  point);       
     }
}
*/

void LCD_ClearRect(int x, int y, int width, int height, int color)
{
  for(int i=0; i<width; i++)
  {
    LCD_SetCursor(LCD_LINE_CNT - (y+height), (i+x));
    LCD_WriteRAM_Prepare();
    for(int j=0; j<height; j++)
    {
       LCD_WriteRAM(color);
       //LCD_SetPoint(LCD_LINE_CNT - (j+y), (i+x),  color);
    }
  }
}

/* ====================  Íå èñïîëüçóåòñÿ =======================================
void VFD_DrawWindow(int x, int y, int width, int height, char* header)
{
    if ( x+width > LCD_COLUMN_CNT)
        return;

    if ( y+height > LCD_LINE_CNT)
        return;


        VFD_DrawLine(x, y, 1, height);
        VFD_DrawLine(x+2, y+2, 1, height-4);
        VFD_DrawLine(x+width-1-2, y+2, 1, height-4);
        VFD_DrawLine(x+width-1, y, 1, height);
        VFD_DrawLine(x+1, y, width-2, 1);
        VFD_DrawLine(x+3, y+height-1-2, width-6, 1);
        VFD_DrawLine(x+1, y+height-1, width-2, 1);

        if (strlen(header))
        {
          VFD_DrawLine(x+3, y+2, width-6,13);
          
          DisplayFontStr(x+width/2,  y+3, ALIGN_CENTER, header, (char*)Myriad_Pro_16, Cyan);//Small_Fonts_7_Bold);
        }
        else
          VFD_DrawLine(x+3, y+2, width-6,1);
  
}
void VFD_DrawTopWindow(int x, int y, int width, int height)
{
}
*/

void LCD_DrawSingleFrame(int x, int y, int width, int height, unsigned short color)
{
    if ( x+width > LCD_COLUMN_CNT)
        return;

    if ( y+height > LCD_LINE_CNT)
        return;
//  LCD_LINE_CNT - (j+y), (i+x)

    unsigned short r,g,b;
  
    r = ( color>>0 )  & 0x1f;
    g = ( color>>5 )  & 0x3f;
    b = ( color>>11 ) & 0x1f;
  
    unsigned short color_back1 =  (((b*8/5)&0x1f)<<11) + (((g*8/6)&0x3f)<<5) + (((r*8/5)&0x1f)<<0);
    unsigned short color_back2 =  (((b*8/6)&0x1f)<<11) + (((g*8/6)&0x3f)<<5) + (((r*8/6)&0x1f)<<0);
    unsigned short color_back3 =  (((b*8/7)&0x1f)<<11) + (((g*8/7)&0x3f)<<5) + (((r*8/7)&0x1f)<<0);


    unsigned short color_back4 =  (((b*8/9)&0x1f)<<11) + (((g*8/9)&0x3f)<<5) + (((r*8/9)&0x1f)<<0);
    unsigned short color_back5 =  (((b*8/10)&0x1f)<<11) + (((g*8/10)&0x3f)<<5) + (((r*8/10)&0x1f)<<0);
    unsigned short color_back6 =  (((b*8/11)&0x1f)<<11) + (((g*8/11)&0x3f)<<5) + (((r*8/11)&0x1f)<<0);
    //    unsigned short color_back =  (b<<11) + (g<<5) + (r<<0);

    

    //for(int j=y+3; j<y+height-2; j++)
    //  LCD_DrawHorzLine(x+3, j, width-6, color);
    LCD_ClearRect(x, y, width, height, color);
    
    
    LCD_DrawHorzLine(x+2, y+2, width-4, color_back3);
    LCD_DrawVertLine(x+2, y+2, height-4, color_back3);
    
    LCD_DrawHorzLine(x+1, y+1, width-2, color_back2);
    LCD_DrawVertLine(x+1, y+1, height-2, color_back2);

    LCD_DrawHorzLine(x, y, width, color_back1);
    LCD_DrawVertLine(x, y, height, color_back1);

    
    LCD_DrawHorzLine(x, y+height, width, color_back4);
    LCD_DrawVertLine(x+width-1, y, height, color_back4);

    LCD_DrawHorzLine(x+1, y+height-1, width-2, color_back5);
    LCD_DrawVertLine(x+width-2, y+1, height-2, color_back5);

    LCD_DrawHorzLine(x+2, y+height-2, width-4, color_back6);    
    LCD_DrawVertLine(x+width-3, y+2, height-4, color_back6);
}

/* ====================  Íå èñïîëüçóåòñÿ =======================================
void VFD_Line(int x1, int y1, int x2, int y2)
{
  LCD_DrawLine(x1, y1, x2, y2, Cyan);  
}
void VFD_MoveTo(int x, int y)
{
}
void VFD_LineTo(int x, int y)
{
}
*/


/*********************************************************************************************************
      END FILE
*********************************************************************************************************/

