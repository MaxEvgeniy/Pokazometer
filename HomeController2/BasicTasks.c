#include "BasicTasks.h"
#include "cmsis_os2.h"
#include "Devices.h"
//#include "w5500_driver.h"
//#include "usbd_customhid.h"
//#include "BME280.h"

#ifdef MajorMainBoard
#include "MajorMain.h" // Для тестовой платы
char BoardAdress[] = "/MajorMain"; // Адрес платы
char SubString[50] = "/MajorMain/IncCommand";
extern Button Buttons[1]; // Массив кнопок
extern RegularEvents RegularEvent[1]; // Регулярно выполняемые события
extern GPIOSwitch GPIOSwitchs[4]; // Массив простых переключателей
extern BME280Sensor BME280Sensors[1]; // Массив датчиков давления8
extern Ds18b20Sensor Ds18b20Sensors[];
#endif
#ifdef BathLighter2Board
#include "BathLighter2.h" // Для платы управления ванной
char BoardAdress[] = "/BathLighter2"; // Адрес платы
char SubString[50] = "/BathLighter2/IncCommand"; // Адрес входящих команд
char MainLightMess[] = "/MainLight"; // Адрес MQTT сообщений о включенном и выключенном основном освещении
char VentMess[] = "/Vent"; // Адрес MQTT сообщений о включенной и выключенной вентиляции
extern Button Buttons[1]; // Массив кнопок
extern RegularEvents RegularEvent[3]; // Регулярно выполняемые события
extern GPIOSwitch GPIOSwitchs[1]; // Массив простых переключателей
extern BME280Sensor BME280Sensors[1]; // Массив датчиков давления
#endif
#ifdef VentorBoard
#include "Ventor.h" // Для платы плавного управления вентилятором
char BoardAdress[] = "/Ventor"; // Адрес платы
char SubString[50] = "/Ventor/IncCommand"; // Адрес входящих команд
extern Button Buttons[1]; // Массив кнопок
extern RegularEvents RegularEvent[1]; // Регулярно выполняемые события
extern GPIOSwitch GPIOSwitchs[4]; // Массив простых переключателей
extern BME280Sensor BME280Sensors[1]; // Массив датчиков давления8
extern Ds18b20Sensor Ds18b20Sensors[];
#endif
#ifdef EmptyBoard
extern Button Buttons[1]; // Массив кнопок
extern GPIOSwitch GPIOSwitchs[1]; // Массив простых переключателей
extern RegularEvents RegularEvent[1]; // Регулярно выполняемые события
#endif

//-------------------------Переменные опроса клавиатуры---------------------------------------------
uint16_t KeyBoardTimerMainCount = 0; // Основной таймер для опроса кнопок
uint16_t KeyBoardTimerMainCountOLD = 0; // Старое значение основного таймера
uint16_t KeyBoardTimerMainMax = 100; //200; // Максимальное значение основного таймера
//uint8_t KeyBoardTimerCount=0; // Таймер для опроса кнопок
//uint8_t KeyBoardTimerMax=100; // Максимальное значение таймера опроса кнопок
uint8_t TimeFirstHotPoint = 1; //30; // Количество тиков таймера для достижения первой контрольной точки относительно стартовой.
uint8_t TimeSecondHotPoint = 3; //30; // Количество тиков таймера для достижения второй контрольной точки относительно первой.
uint8_t TimeFinalHotPoint = 5; // Количество тиков таймера для достижения последней контрольной точки относительно второй.

//------------------------------------------Таймер-----------------------------------------------------------------
uint16_t Timer2Ticks = 0;
uint16_t Timer2TicksMax = 10; // 10= 1 сек
uint32_t MaxWorkTimeParam; // Для расчета секунд глобального времени
uint8_t LifeTimer = 0; // Таймер для счетчика времени работы
uint8_t LifeTimerMax = 10; // Максимальное значение таймера счетчика времени работы 10=1 сек
uint8_t RegularWorkTimer = 0; // Таймер для проверки регулярных действий
uint8_t RegularWorkTimerMax = 10; // Максимальное значение таймера 10= 1 сек

//------------------------------------Переменные для работы с флеш памятью-------------------------------------------------------------
static FLASH_EraseInitTypeDef EraseInitStruct; // Структура для очистки флеша
uint32_t page_error = 0; // Переменная, в которую запишется адрес страницы при неудачном стирании
uint32_t Address = 0x0801FC00; //127; // Адрес страницы с хранящимися переменными
uint32_t CellAddress; // Адрес текущей ячейки с хранящимися переменными
uint8_t CountData = 5; // Количество элементов в массиве для чтения и записи в память
//uint8_t SaveData[2] = {0,0}; // Массив данных для записи в память
//uint8_t ReadData[2] = {0,0}; // Массив данных для чтения из памяти
uint8_t ReadDataBuffer; // Буфер для чтения переменных из памяти
bool SaveParametersBuffer[8] = { 0, 0, 0, 0, 0, 0, 0, 0 }; // Массив булевских переменных, сохраняемых в энергонезависимой памяти

bool MemParametrBool1 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)
bool MemParametrBool2 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)
bool MemParametrBool3 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)
bool MemParametrBool4 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)
bool MemParametrBool5 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)
bool MemParametrBool6 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)
bool MemParametrBool7 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)
bool MemParametrBool8 = false; // Булевская переменная, сохраняемая в энергонезависимой памяти (Можно воткнуть что нужно)

extern uint8_t MyName; // Имя платы из списка
extern uint8_t RegularEventsCount; // Количество регулярных событий
extern uint8_t ButtonsCount; // Количество подключенных кнопок
extern uint8_t GPIOSwitchCount; // Количество используемых переключателей
extern uint8_t BME280sCount; // Количество подключенных датчиков давления

char AdrIncCommand[] = "/IncCommand"; // Адрес входящих команд для платы по MQTT
char AdrGPIOSwitch[] = "/GPIOSwitch"; // Адрес группы GPIO переключателей
Board BoardDrive; // Плата

char strAdr[5]; // Для перевода числового адреса в строку
char AddressOfPost[128]; // Для формирования адреса отправки сообщений
char BoardWarnings[] = "/Warnings"; // Адрес MQTT сообщений для предупреждений от платы

//BaseType_t ReturnButTask;
//BaseType_t ReturnMQTTTask;
////char Parametr2[50];

//void ButtonsListen(void)
//{
//	uint16_t StartDist = 0; // Для расчета границы первого участка
//	uint16_t SecondDist = 0; // Для расчета границы второго участка
//	uint16_t ThreedDist = 0; // Для расчета границы третьего участка
//	uint16_t ResultPos = 0; // Текущая позиция
//	if (KeyBoardTimerMainCount == 0)
//	{
//		for (int i = 0; i < ButtonsCount; i++) // Перебор массива кнопок
//		{ButtonsReset(i); } // Сброс состояния всех кнопок
//		KeyBoardTimerMainCount = KeyBoardTimerMainMax; // Если основной счетчик обнулился- выставляем максимум
//	} // Сброс состояния всех кнопок
//	if (KeyBoardTimerMainCount != KeyBoardTimerMainCountOLD)
//	{
//		KeyBoardTimerMainCountOLD = KeyBoardTimerMainCount;
//
//		for (int i = 0; i < ButtonsCount; i++)
//		{
//			//if(Buttons[i].Result==ButtonUp | Buttons[i].Result==ButtonHold) // Если кнопка отпущена или зажата
//			if (Buttons[i].Result == ButtonUp)
//			{
//				if (ReadButton(Buttons[i]) == ButtonIsPress)
//				{
//					if (Buttons[i].HotPointTime == 0)
//					{
//						Buttons[i].HotPointTime = KeyBoardTimerMainCount; // Cохраняем текущее значение таймера
//					} // Если кнопка нажата первый раз,
//					else
//					{
//						StartDist = Buttons[i].HotPointTime - TimeFirstHotPoint; // Точка границы первого участка
//						SecondDist = Buttons[i].HotPointTime - TimeSecondHotPoint; // Точка границы второго участка
//						ThreedDist = Buttons[i].HotPointTime - TimeFinalHotPoint; // Точка границы третьего участка
//						//ResultPos=Buttons[i].HotPointTime-KeyBoardTimerMainCount; // Текущая позиция
//						ResultPos = KeyBoardTimerMainCount;
//						if (ResultPos<StartDist && ResultPos>SecondDist)
//						{
//							Buttons[i].FirstPointState = true;
//							break;
//						} // Первая точка пройдена
//						if (ResultPos<SecondDist && ResultPos>ThreedDist)
//						{
//							Buttons[i].SecondPointState = true;
//							break;
//						} // Вторая точка пройдена
//						if (ResultPos < ThreedDist)
//						{
//							if (Buttons[i].SecondPointState == 1)
//							{Buttons[i].Result = ButtonHold; } // Если активность в первой, второй и третьей точках- удержаниие
//							else
//							{Buttons[i].Result = ButtonDoubleClick; } // Если активность в первой и третьей точке- двойное нажатие
//						} // Третья точка пройдена
//					} // Если таймер не нулевой (Кнопка была нажата)
//				} // Если кнопка нажата
//				else
//				{
//					if (Buttons[i].HotPointTime != 0)
//					{
//						StartDist = Buttons[i].HotPointTime - TimeFirstHotPoint; // Точка границы первого участка
//						SecondDist = Buttons[i].HotPointTime - TimeSecondHotPoint; // Точка границы второго участка
//						ThreedDist = Buttons[i].HotPointTime - TimeFinalHotPoint; // Точка границы третьего участка
//						//ResultPos=Buttons[i].HotPointTime-KeyBoardTimerMainCount; // Текущая позиция
//						ResultPos = KeyBoardTimerMainCount;
//
//						if (ResultPos > StartDist)
//						{
//							ButtonsReset(i); // Сбросить состояние кнопки
//							break;
//						} // Первая точка не пройдена
//						if (ResultPos<SecondDist & ResultPos>ThreedDist)
//						{
//							Buttons[i].SecondPointState = false;
//							break;
//						} // Вторая точка пройдена
//						if (ResultPos < ThreedDist)
//						{
//							if (Buttons[i].SecondPointState == true)
//							{
//								Buttons[i].Result = ButtonLongPress;
//							} // Если активность в первой и второй точках- длинное нажатие
//							else
//							{
//								Buttons[i].Result = ButtonShortClick;
//							} // Если активность в первой точке- короткое нажатие
//							break;
//						} // Третья точка пройдена
//					} // Если кнопка нажата не в первый раз
//				} // Если кнопка не нажата
//			}
//		} // Перебор массива кнопок
//	} // Отслеживаем изменение состояния кнопки
//}
void Initialisation(void)
{
	////EraseInitStruct.TypeErase = FLASH_TYPEERASE_PAGES; // Постраничная очистка, FLASH_TYPEERASE_MASSERASE - очистка всего флеша
	//EraseInitStruct.PageAddress = Address; // адрес нужной страницы
	//EraseInitStruct.NbPages = 1; // кол-во страниц для стирания
	//EraseInitStruct.Banks = FLASH_BANK_1; // FLASH_BANK_2 - банк №2, FLASH_BANK_BOTH - оба банка
	////MemorySave();
	////MemoryRead();

	///////////////////////////////////////////////////////////////////////////
	//InitW5500();
	//////////////////////////////////////////////////////////////////////////////
#ifdef MajorMainBoard

	//Ds18b20_Init(osPriorityRealtime);

	//InitW5500();
	//osDelay(10);
	BME280_Init();
	//osDelay(10);
	//Ds18b20_Init(osPriorityAboveNormal);
	osDelay(1);
	MajorMainInit();
	//vStartMQTTTasks(400,osPriorityAboveNormal); // Запуск задачи сети MQTT
	//osDelay(10);
#endif

#ifdef BathLighter2Board
	//InitW5500(); // Инициализация модуля Ethernet
	osDelay(1);
	//BME280_Init(); // Инициализация датчика BME280
	osDelay(1);
	BathLighter2Init();
	osDelay(1);
	//vStartMQTTTasks(400,osPriorityAboveNormal); // Запуск задачи сети MQTT
	osDelay(10);
#endif

#ifdef VentorBoard // Модуль плавной регулировки вентилятора
	//InitW5500(); // Инициализация модуля Ethernet
	//osDelay(1);
	BME280_Init();
	osDelay(1);
	VentorInit(); // Инициализация платы
#endif

	//InitializationDone=true;
}
void ProcessingIncomigMessage(void)
{

}
void MemorySave(void)
{
	////////////////////////////////////////////////////////////////////////////////
	//BoardDrive.LifeSeconds=49; // Время работы. Секунды
	//BoardDrive.LifeMinutes=30; // Время работы. Минуты
	//BoardDrive.LifeHours=14; // Время работы. Часы
	//BoardDrive.LifeDays=200; // Время работы. Дни
	//BoardDrive.LifeYears=5; // Время работы. Годы
	////////////////////////////////////////////////////////////////////////////////

	uint8_t SaveBigData[5] = { BoardDrive.LifeSeconds, BoardDrive.LifeMinutes, BoardDrive.LifeHours, BoardDrive.LifeDays, BoardDrive.LifeYears };
	/*
	SaveParametersBuffer[0]=MemParametrBool1; // Заполняем массив переменных на основании значений текущих параметров
	SaveParametersBuffer[1]=MemParametrBool2;
	SaveParametersBuffer[2]=MemParametrBool3;
	SaveParametersBuffer[3]=MemParametrBool4;
	SaveParametersBuffer[4]=MemParametrBool5;
	SaveParametersBuffer[5]=MemParametrBool6;
	SaveParametersBuffer[6]=MemParametrBool7;
	SaveParametersBuffer[7]=MemParametrBool8;

	SaveData[0]=0x00; // Чистим массив
	SaveData[0]=SaveData[0]|SaveParametersBuffer[0]; // Записываем первое значение
	for(uint8_t i = 1; i<8; i++)
	{
	SaveData[0]=SaveData[0]<<1; // Сдвиг значения на 1 бит
	SaveData[0]=SaveData[0]|SaveParametersBuffer[i]; // Запись бита
	};
	*/

	CellAddress = Address;

	HAL_FLASH_Unlock(); // Разблокировать память
	if (HAL_FLASHEx_Erase(&EraseInitStruct, &page_error) != HAL_OK) // Чистка памяти
	{
		uint32_t er = HAL_FLASH_GetError();
	}

	//HAL_FLASH_Lock(); // Заблокировать память
	//HAL_FLASH_Unlock(); // Разблокировать память

	for (uint8_t i = 0; i < CountData; i++)
	{
		//if(HAL_FLASH_Program(FLASH_TYPEPROGRAM_HALFWORD, CellAddress, SaveData[i]) != HAL_OK)// Запись в память
		if (HAL_FLASH_Program(FLASH_TYPEPROGRAM_HALFWORD, CellAddress, SaveBigData[i]) != HAL_OK)// Запись в память
		{
			uint32_t er = HAL_FLASH_GetError();
		}
		CellAddress = CellAddress + 2;
	}
	HAL_FLASH_Lock(); // Заблокировать память
}
void MemoryRead(void)
{
	uint8_t ReadBigData[5];

	CellAddress = Address;
	for (uint8_t i = 0; i < CountData; i++)
	{
		//ReadData[i] = *(uint32_t*)CellAddress; // Читаем число по адресу
		ReadBigData[i] = *(uint32_t*)CellAddress; // Читаем число по адресу
		CellAddress = CellAddress + 2;
	}
	/*
	ReadDataBuffer=ReadData[0];
	ReadDataBuffer>>7;
	SaveParametersBuffer[0]=ReadDataBuffer;

	for(uint8_t i = 1; i<8; i++)
	{
	ReadDataBuffer=ReadData[0];
	ReadDataBuffer=ReadDataBuffer>>7-i;
	ReadDataBuffer=ReadDataBuffer&0x01;
	SaveParametersBuffer[i]=ReadDataBuffer;
	};
	*/
	/*
	MemParametrBool1=SaveParametersBuffer[0]; // Обновляем параметр на основе прочитанных данных из памяти
	MemParametrBool2=SaveParametersBuffer[1]; // Обновляем параметр на основе прочитанных данных из памяти
	MemParametrBool3=SaveParametersBuffer[2]; // Обновляем параметр на основе прочитанных данных из памяти
	MemParametrBool4=SaveParametersBuffer[3]; // Обновляем параметр на основе прочитанных данных из памяти
	MemParametrBool5=SaveParametersBuffer[4]; // Обновляем параметр на основе прочитанных данных из памяти
	MemParametrBool6=SaveParametersBuffer[5]; // Обновляем параметр на основе прочитанных данных из памяти
	MemParametrBool7=SaveParametersBuffer[6]; // Обновляем параметр на основе прочитанных данных из памяти
	MemParametrBool8=SaveParametersBuffer[7]; // Обновляем параметр на основе прочитанных данных из памяти
	*/
	BoardDrive.LifeSeconds = ReadBigData[0]; // Время работы. Секунды
	BoardDrive.LifeMinutes = ReadBigData[1]; // Время работы. Минуты
	BoardDrive.LifeHours = ReadBigData[2]; // Время работы. Часы
	BoardDrive.LifeDays = ReadBigData[3]; // Время работы. Дни
	BoardDrive.LifeYears = ReadBigData[4]; // Время работы. Годы
}
void Drive(void)
{
	//if(DataForReceiveReady){ReadCommandUSB();}; // Если получены данные по USB- обработать
	//if(IncomingMessageReady){ReadCommandMQTT();}; // Если получена команда по MQTT- обработать
//	LifeCycle();
//	RegularWork();
//	BufferMQTTSending();

#ifdef MajorMainBoard
	MajorMainDrive();
#endif

#ifdef BathLighter2Board // Модуль управления светом в ванной
	BathLighter2Drive();
#endif

#ifdef VentorBoard // Модуль плавной регулировки вентилятора
	VentorDrive(); // Инициализация модуля Ethernet
#endif
}
void InitSensors(void)
{
	////////////////////////////////////////DS18B20Init(); ????
}
void ReadSensors(void)
{

}
void USBSendData(void)
{
	////if(DataForTransmitReady && USBLogsSend) // Если готовы данные для передачи по USB
	//if(DataForTransmitReady) // Если готовы данные для передачи по USB
	// {
	// if (USBD_CUSTOM_HID_SendReport(&hUsbDeviceFS, USB_Transmit_Buffer, sizeof(USB_Transmit_Buffer)+1)==USBD_OK)// Передать данные по USB
	// {DataForTransmitReady=false;}; // Сбрасываем флажок готовности данных к передаче
	// };
}
void USBReceiveData(void)
{
	//if(DataUSBCome && dataToReceiveRep1[0]!=0) // Если пришли данные по USB? и не нулевые
	// {
	// for (uint8_t i=0; i<sizeof(USB_Receive_Buffer); i++)
	// {
	// USB_Receive_Buffer[i]=dataToReceiveRep1[i]; // Забираем их из буфера
	// };
	// DataUSBCome=false; // Сброс флажка о приеме данных
	// DataForReceiveReady=true; // Установка флажка о готовности принятых данных к обработке
	// }
}
void ReadCommandUSB(void)
{
	//switch (USB_Receive_Buffer[1]) // Проверяем вопрос
	// {
	// case USBGroup: // Если команда управления USB
	// {
	// USBTweak(); // Настройка USB
	// break;
	// };
	// case BoardGroup: // Если команда управления USB
	// {
	// BoardTweak(); // Настройка USB
	// break;
	// };
	// default: // Если команда не входит ни в одну из групп
	// {
	//
	// };
	// };
	//
	//for (uint8_t i=0; i<sizeof(USB_Receive_Buffer); i++){USB_Receive_Buffer[i]=0;}; // Зачистка массива полученных данных по USB
	//DataForReceiveReady=false; // Сброс флажка о полученных данных по USB
}
void USBTweak(void)
{
	//switch (USB_Receive_Buffer[2]) // Проверяем вопрос
	// {
	// case USBLogsSendCommand: // Если команда управления USB
	// {
	// if (USB_Receive_Buffer[3]==1)
	// {USBLogsSend=true;}
	// else
	// {USBLogsSend=false;};
	// break;
	// };
	// default: // Если команда не входит ни в одну из групп
	// {
	//
	// };
	// };
}
void BoardTweak(void)
{
	// switch (USB_Receive_Buffer[2])
	// {
	// case AutonomousWorkCommand:
	// {
	// if (USB_Receive_Buffer[3] == 1)
	// {AutonomousWork = true; }
	// else
	// {AutonomousWork = false; }
	// break;
	// } // Если команда управления USB
	// default:
	// {
	//
	// } // Если команда не входит ни в одну из групп
	// } // Проверяем вопрос
}
void MessageUSBCook(uint8_t Mess0,
	uint8_t Mess1,
	uint8_t Mess2,
	uint8_t Mess3,
	uint8_t Mess4,
	uint8_t Mess5,
	uint8_t Mess6,
	uint8_t Mess7,
	uint8_t Mess8,
	uint8_t Mess9,
	uint8_t Mess10,
	uint8_t Mess11,
	uint8_t Mess12,
	uint8_t Mess13,
	uint8_t Mess14,
	uint8_t Mess15)
{
	//if (USBLogsSend)
	// {
	// USB_Transmit_Buffer[0]=Mess0;
	// USB_Transmit_Buffer[1]=Mess1;
	// USB_Transmit_Buffer[2]=Mess2;
	// USB_Transmit_Buffer[3]=Mess3;
	// USB_Transmit_Buffer[4]=Mess4;
	// USB_Transmit_Buffer[5]=Mess5;
	// USB_Transmit_Buffer[6]=Mess6;
	// USB_Transmit_Buffer[7]=Mess7;
	// USB_Transmit_Buffer[8]=Mess8;
	// USB_Transmit_Buffer[9]=Mess9;
	// USB_Transmit_Buffer[10]=Mess10;
	// USB_Transmit_Buffer[11]=Mess11;
	// USB_Transmit_Buffer[12]=Mess12;
	// USB_Transmit_Buffer[13]=Mess13;
	// USB_Transmit_Buffer[14]=Mess14;
	// USB_Transmit_Buffer[15]=Mess15;
	// DataForTransmitReady=true;
	// };
}
void MessageMQTTCook(char PostingString[128], char Format[10], float *Parametr)
{
//	if (SendingMessageReady == true) // Если сообщение готово, но не отправлено
//	{
//		BufferMQTTCount = BufferMQTTCount + 1; // Увеличим количество ожидающих записей
//	
//		if (BufferMQTTCount == BufferMQTTMax){BufferMQTTCount = 1; }
//		;
//	
//		for (int i = 0; i < 128; i++) // Сохраняем в буфер первое значение
//		{
//			MQTTBuffer[BufferMQTTCount - 1].PostingString[i] = PostingString[i];
//		}
//		;
//	
//		for (int i = 0; i < 10; i++) // Сохраняем в буфер второе значение
//		{
//			MQTTBuffer[BufferMQTTCount - 1].Format[i] = Format[i];
//		}
//		;
//		MQTTBuffer[BufferMQTTCount - 1].Parametr = Parametr; // Сохраняем в буфер третье значение
//	}
//	else
//	{
//		if (ErrorMQTT == false)
//		{
//			SendingMessage.qos = 1;
//			SendingMessage.retained = 0;
//			SendingMessage.payload = payload;
//			sprintf(payload, Format, *Parametr);
//			SendingMessage.payloadlen = strlen(payload);
//			for (int i = 0; i < 128; i++)
//			{
//				PostString[i] = '\0';
//			}
//			;
//			for (int i = 0; i < 128; i++)
//			{
//				PostString[i] = PostingString[i];
//			}
//			;
//			SendingMessageReady = true;
//		}
//		;
//	}
//	;
//	/*
//	uint8_t BufferCount=0; // Количество ождидающих записей в буфере
//	uint8_t BufferMax=5; // Максимально возможное количество записей в буфере
//
//	TupeMQTTBuffer MQTTBuffer[5]; // Записи буфера MQTT
//	*/
}
void ButtonsReset(uint8_t NumderOfButton)
{
	//Buttons[NumderOfButton].Result=ButtonUp; // Кнопка в положении "отпущена"
	//Buttons[NumderOfButton].HotPoint=0; // Стартовая точка не пройдена
	//Buttons[NumderOfButton].HotPointTime=0; // Время до точки нулевое
	//Buttons[NumderOfButton].FirstPointState=false; // Первая точка- нет нажатия
	//Buttons[NumderOfButton].SecondPointState=false; // Вторая точка- нет нажатия
	//Buttons[NumderOfButton].FinalPointState=false; // Последняя точка- нет нажатия
}
void SetTime(uint8_t Hours, uint8_t Minutes, uint8_t Seconds)
{
	//BoardTime.Hours = Hours;
	//BoardTime.Minutes = Minutes;
	//BoardTime.Seconds = Seconds;
	//
	//if (HAL_RTC_SetTime(&hrtc, &BoardTime, RTC_FORMAT_BCD) != HAL_OK)
	// {
	// Error_Handler();
	// };
}
void SetDate(uint8_t WeekDay, uint8_t Month, uint8_t Date, uint8_t Year)
{
	//BoardDate.WeekDay = WeekDay;
	//BoardDate.Month = Month;
	//BoardDate.Date = Date;
	//BoardDate.Year = Year;
	//
	//if (HAL_RTC_SetDate(&hrtc, &BoardDate, RTC_FORMAT_BCD) != HAL_OK)
	// {
	// Error_Handler();
	// };
}
void BME280Drive(void)
{
	//if (BME280Sensors[0].Update==false)
	// {
	// BME280_ReadTemperature();
	// BME280Sensors[0].TemperFloat=TemperFloat;
	// osDelay(1);
	// BME280_ReadPressure();
	// BME280Sensors[0].PressureFloat=PressureFloat;
	// osDelay(1);
	// BME280_ReadHumidity();
	// BME280Sensors[0].HumidityFloat=HumidityFloat;
	// osDelay(1);
	// BME280Sensors[0].Update=true;
	// };
}
void ReadCommandMQTT(void)
{
	//bool ErrorOfMessage=false;
	//uint8_t k=0;
	//uint8_t n=0;
	//uint8_t TransTupe=0;
	//
	//uint8_t Hours=0;
	//uint8_t Minutes=0;
	//uint8_t Seconds=0;
	//
	//uint8_t WeekDay=0;
	//uint8_t Month=0;
	//uint8_t Date=0;
	//uint8_t Year=0;
	//
	//for (uint8_t i=0; i<CommandCount; i++)
	// {
	// for (uint8_t j=0; j<SymbolCount; j++)
	// {
	// DecodingCommands[i][j]='\0';
	// }
	// };
	//
	//for (uint8_t m=0; m<ui32_len_buffer_control; m++)
	// {
	// if ((m-k*CommandCount)>SymbolCount-1){ErrorOfMessage=true; break;};
	// if (k>CommandCount-1){ErrorOfMessage=true; break;};
	// if (buffer_control[m]=='\0'){break;};
	//
	// if (buffer_control[m]=='_')
	// {k++;
	// n=0;
	// }
	// else
	// {
	// DecodingCommands[k][n]=buffer_control[m];
	// n++;
	// };
	// };
	//////////////////////////////Перезапуск системы//////////////////////////////////
	//if (strcmp(DecodingCommands[0],CommRestartSTM)==0)
	// {
	// NVIC_SystemReset(); // Перезапуск системы
	// goto exit;
	// };
	//////////////////////////////////////////////////////////////////////////////////
	//
	/////////////////////////////Переключатели////////////////////////////////////////
	//if(strcmp(DecodingCommands[0],CommSwitchOn)==0) // Команда на включение переключателя
	// {
	// TransTupe=atoi(DecodingCommands[1]); // Переводим адрес из текста в цифры
	// GPIOSwitchOn (&GPIOSwitchs[TransTupe]); // Включаем переключатель
	// goto exit; // Выход
	// };
	//
	//if(strcmp(DecodingCommands[0],CommSwitchOff)==0) // Команда на включение переключателя
	// {
	// TransTupe=atoi(DecodingCommands[1]); // Переводим адрес из текста в цифры
	// GPIOSwitchOff (&GPIOSwitchs[TransTupe]); // Включаем переключатель
	// goto exit; // Выход
	// };
	//
	//if(strcmp(DecodingCommands[0],CommSwitchToggle)==0) // Команда на переключение переключателя
	// {
	// TransTupe=atoi(DecodingCommands[1]); // Переводим адрес из текста в цифры
	// GPIOSwitchToggle (&GPIOSwitchs[TransTupe]); // Переключаем переключатель
	// goto exit; // Выход
	// };
	////////////////////////////////Время и дата//////////////////////////////////////
	//if (strcmp(DecodingCommands[0],CommSetTime)==0)
	// {
	// Hours=atoi(DecodingCommands[1]); // Переводим часы из текста в цифры
	// Minutes=atoi(DecodingCommands[2]); // Переводим минуты из текста в цифры
	// Seconds=atoi(DecodingCommands[3]); // Переводим секунды из текста в цифры
	// SetTime(Hours,Minutes,Seconds);
	// goto exit;
	// };
	//
	//if (strcmp(DecodingCommands[0],CommSetDate)==0)
	// {
	// WeekDay=atoi(DecodingCommands[1]); // Переводим день недели из текста в цифры
	// Month=atoi(DecodingCommands[2]); // Переводим месяц из текста в цифры
	// Date=atoi(DecodingCommands[3]); // Переводим дату из текста в цифры
	// Year=atoi(DecodingCommands[4]); // Переводим год из текста в цифры
	// SetDate(WeekDay,Month,Date,Year);
	// goto exit;
	// };
	//////////////////////////////////////////////////////////////////////////////////
	//
	//#ifdef BathLighter2Board
	/////////////////////////////////////Освещение////////////////////////////////////
	//if (strcmp(DecodingCommands[0],CommMainLight)==0)
	// {
	// TransTupe=atoi(DecodingCommands[1]); // Считываем цифру. 1- вкл, 0- выкл
	// if(TransTupe==1){MainLightOn();};
	// if(TransTupe==0){MainLightOff();};
	// goto exit;
	// };
	//////////////////////////////////////////////////////////////////////////////////
	//#endif
	//
	//#ifdef BathLighter2Board
	/////////////////////////////////////Вентиляция///////////////////////////////////
	//if (strcmp(DecodingCommands[0],CommVent)==0)
	// {
	// TransTupe=atoi(DecodingCommands[1]); // Считываем цифру. 1- вкл, 0- выкл
	// if(TransTupe==1){VentOn();};
	// if(TransTupe==0){VentOff();};
	// goto exit;
	// };
	//////////////////////////////////////////////////////////////////////////////////
	//#endif
	//
	//#ifdef BathLighter2Board
	/////////////////////////////Перезапуск датчика давления//////////////////////////
	//if (strcmp(DecodingCommands[0],CommRestartBME280)==0)
	// {
	// BME280_Init(); // Инициализация датчика BME280
	// goto exit;
	// };
	//////////////////////////////////////////////////////////////////////////////////
	//#endif
	//
	//exit:
	// IncomingMessageReady=false; // Команда выполнена
}
//void LifeCycle(void)
//{
//	if (LifeTimer == 0)
//	{
//		BoardDrive.LifeSeconds++; // Увеличиваем количество секунд полного рабочего времени платы
//		if (BoardDrive.LifeSeconds > 59)
//		{
//			BoardDrive.LifeSeconds = 0;
//			BoardDrive.LifeMinutes++;
//			if (BoardDrive.LifeMinutes > 59)
//			{
//				BoardDrive.LifeMinutes = 0;
//				BoardDrive.LifeHours++;
//				if (BoardDrive.LifeHours > 23)
//				{
//					BoardDrive.LifeHours = 0;
//					BoardDrive.LifeDays++;
//					if (BoardDrive.LifeDays > 364)
//					{
//						BoardDrive.LifeDays = 0;
//						BoardDrive.LifeYears++;
//					} // Если количество дней- больше года
//				} // Если количество часов- больше суток
//			} // Если количество минут- больше часа
//		} // Если количество секунд- больше минуты
//		/////////////////Время работы переключателей////////////////////////////////////
//		for (int j = 0; j < GPIOSwitchCount; j++)
//		{
//			if (GPIOSwitchs[j].DelayOnCount > 0)
//			{
//				//{GPIOSwitchs[j].DelayOnCount=GPIOSwitchs[j].DelayOnCount-1;};
//				GPIOSwitchs[j].DelayOnCount--;
//			} // Если пауза после выключения еще не выдержана- уменьшаем таймер
//			if (GPIOSwitchs[j].GPIOSwitchState == GPIOSwitchIsOn)
//			{
//				GPIOSwitchs[j].LifeSeconds++; // Увеличиваем количество секунд полного рабочего времени
//				GPIOSwitchs[j].WorkTimeSeconds++; // Увеличиваем количество секунд непрерывной работы
//				if (GPIOSwitchs[j].LifeSeconds > 59)
//				{
//					GPIOSwitchs[j].LifeSeconds = 0;
//					GPIOSwitchs[j].LifeMinutes++;
//					if (GPIOSwitchs[j].LifeMinutes > 59)
//					{
//						GPIOSwitchs[j].LifeMinutes = 0;
//						GPIOSwitchs[j].LifeHours++;
//						if (GPIOSwitchs[j].LifeHours > 23)
//						{
//							GPIOSwitchs[j].LifeHours = 0;
//							GPIOSwitchs[j].LifeDays++;
//							if (GPIOSwitchs[j].LifeDays > 364)
//							{
//								GPIOSwitchs[j].LifeDays = 0;
//								GPIOSwitchs[j].LifeYears++;
//							} // Если количество дней- больше года
//						} // Если количество часов- больше суток
//					} // Если количество минут- больше часа
//				} // Если количество секунд- больше минуты
//				if (GPIOSwitchs[j].WorkTimeSeconds > 59)
//				{
//					GPIOSwitchs[j].WorkTimeSeconds = 0;
//					GPIOSwitchs[j].WorkTimeMinutes++;
//					if (GPIOSwitchs[j].WorkTimeMinutes > 59)
//					{
//						GPIOSwitchs[j].WorkTimeMinutes = 0;
//						GPIOSwitchs[j].WorkTimeHours++;
//						if (GPIOSwitchs[j].WorkTimeHours > 23)
//						{
//							GPIOSwitchs[j].WorkTimeHours = 0;
//							GPIOSwitchs[j].WorkTimeDays++;
//							if (GPIOSwitchs[j].WorkTimeDays > 364)
//							{
//								GPIOSwitchs[j].WorkTimeDays = 0;
//								GPIOSwitchs[j].WorkTimeYears++;
//							} // Если количество дней- больше года
//						} // Если количество часов больше дня
//					} // Если количество минут больше часа
//				} // Если количество секунд больше минуты
//				MaxWorkTimeParam = GPIOSwitchs[j].WorkTimeSeconds +
//				GPIOSwitchs[j].WorkTimeMinutes * 60 +
//				GPIOSwitchs[j].WorkTimeHours * 3600 +
//				GPIOSwitchs[j].LifeDays * 86400 +
//				GPIOSwitchs[j].WorkTimeYears * 31536000;
//				if (MaxWorkTimeParam > GPIOSwitchs[j].MaxWorkTime)
//				{
//					GPIOSwitchOff(&GPIOSwitchs[j]); // Выключаем переключатель
//				} // Если время работы превысило максимум
//			} // Если статус выключателя "Включен"
//		} // Перебор массива переключателей
//		LifeTimer = LifeTimerMax; // Обнуляем счетчик
//	} // Если счетчик остановился
//}
//void RegularWork(void)
//{
//	if (RegularWorkTimer == 0)
//	{
//		for (int i = 0; i < RegularEventsCount; i++)
//		{
//			if (RegularEvent[i].Active == true)
//			{
//				MaxWorkTimeParam = BoardDrive.LifeSeconds + // Расчет отработанных секунд платы
//				BoardDrive.LifeMinutes * 60 +
//				BoardDrive.LifeHours * 3600 +
//				BoardDrive.LifeDays * 86400 +
//				BoardDrive.LifeYears * 31536000;
//				if (RegularEvent[i].LastTime + RegularEvent[i].TimePeriod <= MaxWorkTimeParam)
//				{
//					RegularEvent[i].LastTime = MaxWorkTimeParam; // Обновляем последнее время запуска
//					RegularEvent[i].Event(); // Запуск события
//				} // Если подошло время запуска
//			} // Если событие активно
//		} // Перебор массива событий
//		RegularWorkTimer = RegularWorkTimerMax;
//	} // Если таймер обычной работы не нулевой
//}
//void BufferMQTTSending(void)
//{
//	// if (SendingMessageReady == false)
//	// {
//	// if (BufferMQTTCount > 0)
//	// {
//	// MessageMQTTCook(MQTTBuffer[BufferMQTTCount - 1].PostingString, MQTTBuffer[BufferMQTTCount - 1].Format, MQTTBuffer[BufferMQTTCount - 1].Parametr); // Отправляем сообщение
//	// BufferMQTTCount = BufferMQTTCount - 1;
//	// } // Если есть сообщения
//	// }
//}
