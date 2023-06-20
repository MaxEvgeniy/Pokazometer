#ifndef BUZZER
#define BUZZER

#include "stdint.h"

#define SILENCE		0
#define DO			2093
#define DO_DIEZ		2217
#define RE			2349
#define RE_DIEZ		2489
#define MI			2637
#define FA			2794
#define FA_DIEZ		2960
#define SOL			3136
#define SOL_DIEZ	3322
#define LA			3520
#define LA_DIEZ		3729
#define SI			3951

const unsigned int AlarmSound[] = { 2*FA_DIEZ, SILENCE, 2*RE_DIEZ, SILENCE, 2*DO };

const unsigned int  *SoundArray;
uint32_t SoundArraySize = 0;
uint8_t SoundRepeat = 0;
uint8_t SoundPause = 0;
uint32_t SoundStep = 0;

void SoundSetPeriod(uint32_t period); // Установка параметров пьездинамика для воспроизведения ноты

#endif