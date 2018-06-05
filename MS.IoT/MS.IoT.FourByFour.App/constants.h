#include <stdlib.h>
#include <time.h>
#include <applibs/uart.h>

#pragma once
#define MAX_CONNECTION_STRING_SIZE 256
#define HEADER_TAG_SIZE 0x04
#define HEADER_DATASIZE_SIZE 0x02
#define HEADER_COMMAND_SIZE 0x04
#define UART_BUFFER_SIZE 1024 //max number of bytes analyse per loop cycle

extern const uint8_t HEADER_TAG[]; //Header tag (4 bytes) "FBF ". Used to detected the beginning of a packet.
extern const struct timespec SLEEP_TIME; // 1 millisecond

extern const uint8_t *CMD_POKE_ALIVE;
extern const uint8_t *CMD_GET_CONFIG;
extern const uint8_t *CMD_SET_CONFIG;
extern const uint8_t *CMD_POKE_IOT_HUB;
extern const uint8_t *CMD_SEND_TEMPLATE;
extern const uint8_t *CONFIG_CONNECTION_KEY;