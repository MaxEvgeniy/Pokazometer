#include "GlobalDefault.h"
#include "main.h"
#include "wizchip_conf.h"
#include "dhcp.h"
#include "dns.h"
#include "MQTTClient.h"
#include <stdbool.h>
#include <cmsis_os2.h>

#ifdef ThermoSensors
#include "DS18B20.h"
#endif // ThermoSensors

extern SPI_HandleTypeDef hspi2;
uint8_t rx_tx_buff_sizes[] = { 2, 2, 2, 2, 2, 2, 2, 2 };
wiz_NetInfo net_info = { .mac = { 0xEA, 0x11, 0x22, 0x33, 0x44, 0xEA }, .dhcp = NETINFO_DHCP };
uint8_t dhcp_buffer[1024];
uint8_t dns_buffer[1024];
volatile bool ip_assigned = false; // Флажок о назначении сетевого адреса плате
uint8_t dns[4];
uint8_t W5500InitFlag = 0b01111111; // Флажок прохождения операций инициализации узла связи. При прохождении операций инициализации поэтапно обнуляется. 0- инициализировано
bool InitializationDone = false;
uint8_t PubMessagePos = 0; // 
MQTTMessage *PubMessageIndex[20]; // Массив указателей на сообщения

#ifdef ThermoSensors
//DS18B20 temperatureSensor[THERMOSENSORS_COUNT_MAX];
extern UART_HandleTypeDef huart1;
#endif // ThermoSensors

void W5500_Select(void)
{
	HAL_GPIO_WritePin(SPI2_CS_GPIO_Port, SPI2_CS_Pin, GPIO_PIN_RESET);
}
void W5500_Unselect(void)
{
	HAL_GPIO_WritePin(SPI2_CS_GPIO_Port, SPI2_CS_Pin, GPIO_PIN_SET);
}
void W5500_ReadBuff(uint8_t* buff, uint16_t len)
{
	HAL_SPI_Receive(&hspi2, buff, len, HAL_MAX_DELAY);
	//HAL_SPI_Receive_DMA(&hspi2, buff, len);
}
void W5500_WriteBuff(uint8_t* buff, uint16_t len)
{
	HAL_SPI_Transmit(&hspi2, buff, len, HAL_MAX_DELAY);
	//HAL_SPI_Transmit_DMA(&hspi2, buff, len);
}
uint8_t W5500_ReadByte(void)
{
	uint8_t byte;
	W5500_ReadBuff(&byte, sizeof(byte));
	return byte;
}
void W5500_WriteByte(uint8_t byte)
{
	W5500_WriteBuff(&byte, sizeof(byte));
}
void Callback_IPAssigned(void)
{
	//UART_Printf("Callback: IP assigned! Leased time: %d sec\r\n", getDHCPLeasetime());
	ip_assigned = true;
}
void Callback_IPConflict(void)
{
	//UART_Printf("Callback: IP conflict!\r\n");
}
void W5500Init(void)
{
	reg_wizchip_cs_cbfunc(W5500_Select, W5500_Unselect); // Registering W5500 callbacks...
	W5500InitFlag = W5500InitFlag & 0b11111110; // Стираем бит (флажок) ошибки выполнения операции
	reg_wizchip_spi_cbfunc(W5500_ReadByte, W5500_WriteByte);
	W5500InitFlag = W5500InitFlag & 0b11111101; // Стираем бит (флажок) ошибки выполнения операции
	reg_wizchip_spiburst_cbfunc(W5500_ReadBuff, W5500_WriteBuff);
	W5500InitFlag = W5500InitFlag & 0b11111011; // Стираем бит (флажок) ошибки выполнения операции
	wizchip_init(rx_tx_buff_sizes, rx_tx_buff_sizes); // Calling wizchip_init()
	W5500InitFlag = W5500InitFlag & 0b11110111; // Стираем бит (флажок) ошибки выполнения операции
	setSHAR(net_info.mac); // Установка MAC адреса перед использованием DHCP
	W5500InitFlag = W5500InitFlag & 0b11101111; // Стираем бит (флажок) ошибки выполнения операции
	DHCP_init(DHCP_SOCKET, dhcp_buffer);
	W5500InitFlag = W5500InitFlag & 0b11011111; // Стираем бит (флажок) ошибки выполнения операции
	reg_dhcp_cbfunc(Callback_IPAssigned, Callback_IPAssigned, Callback_IPConflict); // Registering DHCP callbacks...
	W5500InitFlag = W5500InitFlag & 0b10111111; // Стираем бит (флажок) ошибки выполнения операции
}
void ErrorsReserch(void)
{
	osDelay(10);
}
void Initialization(void)
{
	//W5500Init();
	
#ifdef ThermoSensors
	//DS18B20Init();
	get_ROMid();
#endif // ThermoSensors

	osDelay(100);
}

