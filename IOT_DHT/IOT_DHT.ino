// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. 

// using
#include <WiFi.h>
#include <DHT.h>
#include "AzureIotHub.h"
#include "Esp32MQTTClient.h"


#define INTERVAL 5000
#define DEVICE_ID ""
#define MESSAGE_MAX_LEN 256
#define DHT_PIN 21
#define DHT_TYPE DHT11

DHT dht(DHT_PIN, DHT_TYPE);

// Please input the SSID and password of WiFi
const char* ssid     = "";
const char* password = "";
static const char* connectionString = "";

const char *messageData = "{\"deviceId\":\"%s\", \"messageId\":%d, \"temperature\":%f, \"humidity\":%d}";

int messageCount = 1;
static bool hasWifi = false;
static bool messageSending = true;
static uint64_t send_interval_ms;

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Utilities
static void InitWifi()
{
  Serial.println("Connecting...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  hasWifi = true;
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}


static void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result)
{
  if (result == IOTHUB_CLIENT_CONFIRMATION_OK)
  {
    Serial.println("Send Confirmation Callback finished.");
  }
}

static void MessageCallback(const char* payLoad, int size)
{
  Serial.println("Message callback:");
  Serial.println(payLoad);
}

static void DeviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size)
{
  char *temp = (char *)malloc(size + 1);
  if (temp == NULL)
  {
    return;
  }
  memcpy(temp, payLoad, size);
  temp[size] = '\0';
  // Display Twin message.
  Serial.println(temp);
  free(temp);
}

static int  DeviceMethodCallback(const char *methodName, const unsigned char *payload, int size, unsigned char **response, int *response_size)
{
  LogInfo("Try to invoke method %s", methodName);
  const char *responseMessage = "\"Successfully invoke device method\"";
  int result = 200;

  if (strcmp(methodName, "start") == 0)
  {
    LogInfo("Start sending temperature and humidity data");
    messageSending = true;
  }
  else if (strcmp(methodName, "stop") == 0)
  {
    LogInfo("Stop sending temperature and humidity data");
    messageSending = false;
  }
  else
  {
    LogInfo("No method %s found", methodName);
    responseMessage = "\"No method found\"";
    result = 404;
  }

  *response_size = strlen(responseMessage) + 1;
  *response = (unsigned char *)strdup(responseMessage);

  return result;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch
void setup()
{
  Serial.begin(115200);
  Serial.println("ESP32 Device");
  Serial.println("Initializing...");

  dht.begin();

  // Initialize the WiFi module
  Serial.println(" > WiFi");
  hasWifi = false;
  InitWifi();
  if (!hasWifi)
  {
    return;
  }
  randomSeed(analogRead(0));

  Serial.println(" > IoT Hub");
  Esp32MQTTClient_SetOption(OPTION_MINI_SOLUTION_NAME, "GetStarted");
  Esp32MQTTClient_Init((const uint8_t*)connectionString, true);

  Esp32MQTTClient_SetSendConfirmationCallback(SendConfirmationCallback);
  Esp32MQTTClient_SetMessageCallback(MessageCallback);
  Esp32MQTTClient_SetDeviceTwinCallback(DeviceTwinCallback);
  Esp32MQTTClient_SetDeviceMethodCallback(DeviceMethodCallback);

  send_interval_ms = millis();
}

void loop()
{
  if (hasWifi)
  {

    float temperature = (float)dht.readTemperature();
    int humidity = dht.readHumidity();
    
    if (messageSending && (int)(millis() - send_interval_ms) >= INTERVAL)
    {
      // Send teperature data
      char messagePayload[MESSAGE_MAX_LEN];
      
      snprintf(messagePayload,MESSAGE_MAX_LEN, messageData, DEVICE_ID, messageCount++, temperature,humidity);
      EVENT_INSTANCE* message = Esp32MQTTClient_Event_Generate(messagePayload, MESSAGE);
      Serial.println(messagePayload);      

      Esp32MQTTClient_Event_AddProp(message, "sensorType", "MCU-FEATHER-DHT11");
      Esp32MQTTClient_Event_AddProp(message, "placement", "Office");
        
      if(temperature > 28) {
        Esp32MQTTClient_Event_AddProp(message, "alertNotification", "true");
      } else {
        Esp32MQTTClient_Event_AddProp(message, "alertNotification", "false");
      }
      
         
      Esp32MQTTClient_SendEventInstance(message);  
      send_interval_ms = millis();
      
    }
    else
    {
      Esp32MQTTClient_Check();
    }
  }
  delay(10);
}
