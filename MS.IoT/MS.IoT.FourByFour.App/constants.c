#include <stdlib.h>
#include <time.h>
#include <applibs/uart.h>

const uint8_t HEADER_TAG[] = { 0x46, 0x42, 0x46, 0x00 }; //Header tag (4 bytes) "FBF ". Used to detected the beginning of a packet.
const struct timespec SLEEP_TIME = { 0, 1000000 }; // 1 millisecond

const uint8_t *CMD_POKE_ALIVE = "POKE";
const uint8_t *CMD_GET_CONFIG = "CFGG";
const uint8_t *CMD_SET_CONFIG = "CFGS";
const uint8_t *CMD_POKE_IOT_HUB = "CIOT";
const uint8_t *CMD_SEND_TEMPLATE = "TPLT";
const uint8_t *CONFIG_CONNECTION_KEY = "SimulatorConnectionString";