#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include "Multiplexer.h"

#define INPUT_COUNT 4

Multiplexer multiplexer(INPUT_COUNT);
int results[INPUT_COUNT];

const char* ssid = "MadaMada"; //put your wifi network name here
const char* password = "12345678"; //put your wifi password here

IPAddress ip(192,168,43,69);    
IPAddress gateway(192,168,43,1);
IPAddress subnet(255,255,255,0);

WiFiServer server(80);
WiFiClient client;


void setup() {
  multiplexer.setup();
  Serial.begin(9600);
  WiFi.begin(ssid, password);
  // WiFi.config(ip, gateway, subnet);
  Serial.println("Connecting");

  while(WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.println(".");
  }

  Serial.print("Connected to ");
  Serial.println(ssid);

  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  //Start the TCP server
  server.begin();
  Serial.printf("Web server started, open %s in a web browser\n", WiFi.localIP().toString().c_str());
}

void loop() {
  client = server.available();
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  if(client)
  {
    Serial.println("Client Connected");
    while(client.connected())
    {
      multiplexer.readAllInputs(results);
      for(int i = 0; i < INPUT_COUNT -1; i++){
        Serial.printf("input: %d value: %d\n", i + 1, results[i]);
        client.print(results[i]);
        client.print(' ');
     
      }
       client.print(results[INPUT_COUNT -1]);
       

       client.print('\r');
      
    
      //Delay before the next reading
      delay(10);
    }
  }

}
