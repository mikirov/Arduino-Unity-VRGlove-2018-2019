#include <Wire.h>

long accelX, accelY, accelZ;
float gForceX, gForceY, gForceZ;

long gyroX, gyroY, gyroZ;
float rotX, rotY, rotZ;
float offsetX = 0, offsetY = 0, offsetZ = 0;



void setup() {
  Serial.begin(9600);
  Wire.begin();
  setupMPU();
  calibrateGyroscope(1000);
}


void loop() {
  recordAccelRegisters();
  processAccelData();
  recordGyroRegisters();
  processGyroData();
  printData();
  delay(100);
}

void setupMPU(){
  Wire.beginTransmission(0b1101000); //This is the I2C address of the MPU (b1101000/b1101001 for AC0 low/high datasheet sec. 9.2)
  Wire.write(0x6B); //Accessing the register 6B - Power Management (Sec. 4.28)
  Wire.write(0b00000000); //Setting SLEEP register to 0. (Required; see Note on p. 9) TODO change when using multiple gyroscopes
  Wire.endTransmission();  
  
  Wire.beginTransmission(0b1101000); //I2C address of the MPU
  Wire.write(0x1B); //Accessing the register 1B - Gyroscope Configuration (Sec. 4.4) 
  Wire.write(0x00000000); //Setting the gyro to full scale +/- 250deg./s 
  Wire.endTransmission(); 
  
  Wire.beginTransmission(0b1101000); //I2C address of the MPU
  Wire.write(0x1C); //Accessing the register 1C - Acccelerometer Configuration (Sec. 4.5) 
  Wire.write(0b00000000); //Setting the accel to +/- 2g
  Wire.endTransmission(); 
}

void recordAccelRegisters() {
  Wire.beginTransmission(0b1101000); //I2C address of the MPU
  Wire.write(0x3B); //Starting register for Accel Readings
  Wire.endTransmission();
  Wire.requestFrom(0b1101000,6); //Request Accel Registers (3B - 40)
  while(Wire.available() < 6);
  accelX = Wire.read()<<8|Wire.read(); //Store first two bytes into accelX
  accelY = Wire.read()<<8|Wire.read(); //Store middle two bytes into accelY
  accelZ = Wire.read()<<8|Wire.read(); //Store last two bytes into accelZ
  //processAccelData();
}

void processAccelData(){
  gForceX = accelX / 16384.0;
  gForceY = accelY / 16384.0; 
  gForceZ = accelZ / 16384.0;
}

void recordGyroRegisters() {
  Wire.beginTransmission(0b1101000); //I2C address of the MPU
  Wire.write(0x43); //Starting register for Gyro Readings
  Wire.endTransmission();
  Wire.requestFrom(0b1101000,6); //Request Gyro Registers (43 - 48)
  while(Wire.available() < 6);
  gyroX = (Wire.read()<<8|Wire.read()) ; //Store first two bytes into accelX
  gyroY = (Wire.read()<<8|Wire.read()) ;//Store middle two bytes into accelY
  gyroZ = (Wire.read()<<8|Wire.read()) ; //Store last two bytes into accelZ
  //processGyroData();
}

void processGyroData() {
  rotX = gyroX / 131.0;
  rotY = gyroY / 131.0; 
  rotZ = gyroZ / 131.0;
  rotX -= offsetX;
  rotY -= offsetY;
  rotZ -= offsetZ;
}

void calibrateGyroscope(int times){
  for(int i = 0; i < times; i++){
    recordGyroRegisters();
    offsetX += rotX;
    offsetY += rotY;
    offsetZ += rotZ;
    Serial.print(offsetX);
    Serial.print(offsetY);
    Serial.println(offsetZ);
    delay(7); // delay for 144Hz for loop
    }
  offsetX /= times;
  offsetY /= times;
  offsetZ /= times;
  Serial.print(offsetX);
  Serial.print(offsetY);
  Serial.println(offsetZ);
}

void printData() {

  Serial.print(rotX);
  Serial.print(" ");
  Serial.print(rotY);
  Serial.print(" ");
  Serial.print(rotZ);
  Serial.print(" ");
  Serial.print(gForceX);
  Serial.print(" ");
  Serial.print(gForceY);
  Serial.print(" ");
  Serial.println(gForceZ);
}
