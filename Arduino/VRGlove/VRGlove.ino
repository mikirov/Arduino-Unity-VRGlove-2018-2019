#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include "Multiplexer.h"

#include "I2Cdev.h"

#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"

#define INPUT_COUNT 9

#define DEBUG 1 // set to 0 to disable debug mode
#define INTERRUPT_PIN 15 // use pin 15 on ESP8266

Multiplexer multiplexer(INPUT_COUNT);
int results[INPUT_COUNT];

MPU6050 mpu;

bool dmpReady = false;  // set true if DMP init was successful
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

Quaternion q;           // [w, x, y, z]         quaternion container
VectorInt16 aa;         // [x, y, z]            accel sensor measurements
VectorInt16 aaReal;     // [x, y, z]            gravity-free accel sensor measurements
VectorInt16 aaWorld;    // [x, y, z]            world-frame accel sensor measurements
VectorFloat gravity;    // [x, y, z]            gravity vector



const char* ssid = "MadaMada"; //put your wifi network name here
const char* password = "12345678"; //put your wifi password here

IPAddress ip(192, 168, 43, 143);
IPAddress gateway(192, 168, 43, 1);
IPAddress subnet(255, 255, 255, 0);

WiFiServer server(80);
WiFiClient client;


volatile bool mpuInterrupt = false;     // indicates whether MPU interrupt pin has gone high
void dmpDataReady() {
    mpuInterrupt = true;
}


void mpu_setup()
{
  Wire.begin();
  Wire.setClock(400000); // 400kHz I2C clock. Comment this line if having compilation difficulties
  #ifdef DEBUG
  Serial.println(F("Initializing I2C devices..."));
  #endif
  mpu.initialize();
  pinMode(INTERRUPT_PIN, INPUT);
  #ifdef DEBUG
  Serial.println(F("Testing device connections..."));
  Serial.println(mpu.testConnection() ? F("MPU6050 connection successful") : F("MPU6050 connection failed"));
  
  Serial.println(F("Initializing DMP..."));
  #endif
  devStatus = mpu.dmpInitialize();

  // make sure it worked (returns 0 if so)
  if (devStatus == 0) {
    // turn on the DMP, now that it's ready
    #ifdef debug
    Serial.println(F("Enabling DMP..."));
    #endif
    mpu.setDMPEnabled(true);

    // enable Arduino interrupt detection
    #ifdef DEBUG
    Serial.println(F("Enabling interrupt detection (Arduino external interrupt 0)..."));
    #endif
    attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
    mpuIntStatus = mpu.getIntStatus();

    // set our DMP Ready flag so the main loop() function knows it's okay to use it
    #ifdef DEBUG
    Serial.println(F("DMP ready! Waiting for first interrupt..."));
    #endif
    dmpReady = true;

    // get expected DMP packet size for later comparison
    packetSize = mpu.dmpGetFIFOPacketSize();
  } else {
    // ERROR!
    // 1 = initial memory load failed
    // 2 = DMP configuration updates failed
    // (if it's going to break, usually the code will be 1)
    #ifdef DEBUG
    Serial.print(F("DMP Initialization failed (code "));
    Serial.print(devStatus);
    Serial.println(F(")"));
    #endif
  }
}



void setup() {
  multiplexer.setup();
  Serial.begin(115200);

  WiFi.begin(ssid, password);
  WiFi.config(ip, gateway, subnet);
  Serial.println("Connecting");

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    #ifdef DEBUG
    Serial.println(".");
    #endif
  }
  #ifdef DEBUG
  Serial.print("Connected to ");
  Serial.println(ssid);

  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
  #endif
  //Start the TCP server
  server.begin();
  #ifdef DEBUG
  Serial.printf("Web server started, open %s in a web browser\n", WiFi.localIP().toString().c_str());
  #endif
  mpu_setup();
}

void mpu_loop()
{
    // if programming failed, don't try to do anything
    if (!dmpReady) return;

    if (!mpuInterrupt && fifoCount < packetSize) return;
    
    mpuInterrupt = false;
    mpuIntStatus = mpu.getIntStatus();

    // get current FIFO count
    fifoCount = mpu.getFIFOCount();

    // check for overflow (this should never happen unless our code is too inefficient)
    if ((mpuIntStatus & _BV(MPU6050_INTERRUPT_FIFO_OFLOW_BIT)) || fifoCount >= 1024) {
        // reset so we can continue cleanly
        mpu.resetFIFO();
        fifoCount = mpu.getFIFOCount();
        #ifdef DEBUG
        Serial.println(F("FIFO overflow!"));
        #endif
    // otherwise, check for DMP data ready interrupt (this should happen frequently)
    } else if (mpuIntStatus & _BV(MPU6050_INTERRUPT_DMP_INT_BIT)) {
        // wait for correct available data length, should be a VERY short wait
        while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

        // read a packet from FIFO
        mpu.getFIFOBytes(fifoBuffer, packetSize);
        
        // track FIFO count here in case there is > 1 packet available
        // (this lets us immediately read more without waiting for an interrupt)
        fifoCount -= packetSize;

    // display quaternion values in easy matrix form: w x y z
    mpu.dmpGetQuaternion(&q, fifoBuffer);
    //get multiplexer readings
    multiplexer.readAllInputs(results);

    #ifdef DEBUG
    Serial.print("quat\t");
    Serial.print(q.w);
    Serial.print("\t");
    Serial.print(q.x);
    Serial.print("\t");
    Serial.print(q.y);
    Serial.print("\t");
    Serial.println(q.z);
    #endif
    
  }
}
void mux_loop(){
    
    multiplexer.readAllInputs(results);
    #ifdef DEBUG
    for(int i = 0; i < INPUT_COUNT; i++){
      Serial.printf("%d ", results[i]);
    }
    Serial.print("\n");
    #endif
}

void loop(){
  mpu_loop();
  mux_loop();
  client = server.available();
  if(client){
    while(client.connected()){
      for(int i = 0; i < INPUT_COUNT; i++){
        client.print(results[i]);
        client.print(' ');
      }
      client.print(q.w);
      client.print(' ');
      client.print(q.x);
      client.print(' ');
      client.print(q.y);
      client.print(' ');
      client.print(q.z);
      client.print('\r');
      mpu_loop();
      mux_loop();    
    }
  }
}
