#include "structs.h"

#pragma once
//GENERAL
#define MAX_CONNECTION_STRING_SIZE 256
#define FIRM_VERSION "1.1"
#define API_WIFI "https://192.168.35.2/wifi/config/networks"
#define API_IP_ADDRESS "%s/api/ip/check"
#define API_WIFI_JSON "{\"ssid\":%s,\"configState\":\"enabled\",\"securityState\":%s,\"psk\":%s}"
#define API_COFFEE_LOG_JSON "[{\"brewStrength\":\"%d\",\"date\":\"%d\",\"grind\":\"%d\"}]"
#define LOG_INFO_JSON "[{ \"type\": \"debug\", \"message\": \"%s\"}]"
#define LOG_ERROR_JSON "[{ \"type\": \"error\", \"code\": %d, \"message\": \"%s\"}]"
#define DEVICE_PERIODIC_TASKS_TIMER_SEC 2
#define DEVICE_PERIODIC_TASKS_TIMER_MIN 300
#define DEVICE_BUTTON_PRESS_TIME_NANO 500000000L
#define SERIAL_PORT_BAUDS 115200

//LIMITS
#define IPV6_STR_MAX_SIZE 42
#define UINT8_STR_MAX_SIZE 4
#define UINT16_STR_MAX_SIZE 6
#define UINT32_STR_MAX_SIZE 11
#define TIME_STR_MAX_SIZE 11
#define FORMAT_TIME_STR_MAX_SIZE 31

//UART
//[HEAD] [ID] [CMAD] [SIZE] [DATA...]
#define HEADER_TAG { 0x46, 0x42, 0x46, 0x00 } //Header tag (4 bytes) "FBF ". Used to detected the beginning of a packet.
#define HEADER_TAG_SIZE 0x04
#define HEADER_MESSAGE_ID_SIZE 0x02
#define HEADER_DATASIZE_SIZE 0x02
#define HEADER_COMMAND_SIZE 0x04
#define UART_BUFFER_SIZE 1024 //max number of bytes analyse per loop cycle
//Tag of UART Packets
#define CMD_PING "PING" //Send a request to get a heartbeat from another other device
#define CMD_PONG "PONG" //Send a confirmation to the ping request from the other device
#define CMD_GET_PROPERTY "CMDG" //Request a property value from the other device
#define CMD_SEND_PROPERTY "CMDS" //Send a property value to the other device confirmation
#define CMD_CONFIRM_PROPERTY "CMDC" //Send a confirmation that a send property command was received by the other device
#define CMD_SEND_ACTION "ACTS" // Send an action order to another device
#define CMD_CONFIRM_ACTION "ACTC" //Send a confirmation that an action order was received by the other device
#define CONFIG_CONNECTION_KEY "HostName=%s;DeviceId=%s;SharedAccessKey=%s" //Connection string for IoT Hub
#define CONFIG_PARAM_HOSTNAME_IOT "HostNameIoT" //HostName IoT
#define CONFIG_PARAM_DEVICE_ID "DeviceId" //DeviceID
#define CONFIG_PARAM_SHARED_ACCESS_KEY "SharedAccessKey" //SharedAccessKey
#define CONFIG_CONNECTION_IP_CHECK_WEBSITE "IPCheckWebsiteConnectionString" //Connection string for IP Check website, ex: msiotdevicemanagementportalweb20171004060823.azurewebsites.net

//FEATURES
#define FEATURE_NAME_BREW_STRENGTH "brewStrengthFeature"
#define FEATURE_DISPLAY_BREW_STRENGTH "\"Brew Strength\""
#define FEATURE_METHODS_BREW_STRENGTH "\"changeBrewStrength\""
#define FEATURE_VARNAME_BREW_STRENGTH "FeatureBrewStrengthActivationStatus"
#define FEATURE_NAME_BREW "brewFeature"
#define FEATURE_DISPLAY_BREW "\"Brew\""
#define FEATURE_METHODS_BREW "\"changeBrewStrength,launchBrew\""
#define FEATURE_VARNAME_BREW "FeatureBrewActivationStatus"
#define FEATURE_NAME_GRIND_AND_BREW "grindAndBrewFeature"
#define FEATURE_DISPLAY_GRIND_AND_BREW "\"Grind and Brew\""
#define FEATURE_METHODS_GRIND_AND_BREW "\"changeBrewStrength,launchBrew,launchGrindAndBrew\""
#define FEATURE_VARNAME_GRIND_BREW "FeatureGrindBrewActivationStatus"
#define FEATURE_NAME_WIFI "wifiFeature"
#define FEATURE_DISPLAY_WIFI "\"Wifi Button\""
#define FEATURE_METHODS_WIFI "\"setWifi\""
#define FEATURE_VARNAME_WIFI "FeatureWifiActivationStatus"
#define FEATURE_NAME_DEBUG "debugFeature"
#define FEATURE_DISPLAY_DEBUG "\"Debug\""
#define FEATURE_METHODS_DEBUG "\"\""
#define FEATURE_VARNAME_DEBUG "FeatureDebugActivationStatus"


//IOT HUB
//Common
#define DESIRED "desired" //Report available features
#define DESIRED_FEATURES "features" //Report available features
#define DESIRED_DEVICE_STATE "deviceState" //Report device specific values
#define REPORTED_STATUSCODE "statusCode" //Report Status Code in case of errors
#define REPORTED_FIRMWARE_VERSION "firmwareVersion" //Report version
#define REPORTED_IP_ADDRESS "ipAddress" //Report IP address
#define REPORTED_FEATURES_DEFINITIONS "featuresDefinitions" //Report available features
#define REPORTED_DEVICE_STATE "deviceState" //Report device specific values
#define REPORTED_FEATURES_DEFINITIONS_METHODS "methods" //Report available features
#define REPORTED_FEATURES_DEFINITIONS_DISPLAY_NAME "displayName" //Report available features displayname
#define REPORTED_FEATURES_DEFINITIONS_IS_ACTIVATED "isActivated" //Report if a feature is currently activated
#define REPORTED_FEATURES_DEFINITIONS_INTERNAL_USE "internalUseOnly" //Report if a feature is currently activated
#define REPORTED_HEARTBEAT "heartbeat" //Report heartbeat
#define REPORTED_ACTIVATION_DATE "activationDate" //Activation Date
#define STATUSCODE_INIT "0" //0 - Init code
//Device Exclusive
#define VALUE_STATE_BREW_STRENGTH "StateBrewStrength"
#define VALUE_STATE_GRIND "StateGrind"
#define VALUE_STATE_BREW "StateActionBrew"
#define VALUE_STATE_BREW_ETA "StateActionBrewETA"
#define VALUE_STATE_BREW_GRIND "StateActionGrindBrew"
#define VALUE_STATE_BREW_GRIND_ETA "StateActionGrindBrewETA"
#define VALUE_STAT_LAST_BREWED "StatLastBrewed"
#define VALUE_STAT_NBR_BREWED_TODAY "StatNbrBrewedToday"
#define VALUE_STAT_NBR_BREWED_WEEKLY "StatNbrBrewedWeekly"
#define VALUE_STAT_NBR_BREWED_TOTAL "StatNbrBrewedTotal"
#define VALUE_UI_CONNECTION_STATE "ConnectionState"


//METHODS
typedef struct method_definition {
	const char name[32];
} method_definition;
#define METHOD_RESPONSE_FAIL "{status: \"error\"}"
#define METHOD_ACTION_CHANGE_BREW_STRENGTH "changeBrewStrength"
#define METHOD_RESPONSE_CHANGE_BREW_STRENGTH "{status: \"%s\", newStrength: \"%s\"}"
#define METHOD_ACTION_BREW "launchBrew"
#define METHOD_RESPONSE_BREW "{status: \"%s\", timeTillCompletion:  %s}"
#define METHOD_ACTION_GRIND_AND_BREW "launchGrindAndBrew"
#define METHOD_RESPONSE_GRIND_AND_BREW "{status: \"%s\", timeTillCompletion: %s}"
#define METHOD_ACTION_SET_WIFI "setWifi"
#define METHOD_ACTION_BREW_TIME 300
#define METHOD_ACTION_BREW_GRIND_TIME 360


//ERROR CODES
#define ERROR_CODE_GPIO_ERROR 400
#define ERROR_CODE_CURL_ERROR 401
#define ERROR_CODE_APPCONFIG_READ 402
#define ERROR_CODE_APPCONFIG_WRITE 403
#define ERROR_CODE_APPCONFIG_EXISTS 404
#define ERROR_CODE_UART_CONNECTION 405
#define ERROR_CODE_IOT_MESSAGE 406
#define ERROR_CODE_IOT_METHOD 407