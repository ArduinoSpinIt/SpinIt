
// Wiring http://301o583r8shhildde3s0vcnh.wpengine.netdna-cdn.com/wp-content/uploads/2014/11/conn.png

#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
// in "MPU6050_6Axis_MotionApps20.h"
// I'm now using the default value on line 305 to:  0x02,   0x16,   0x02,   0x00, 0x01                // D_0_22 inv_set_fifo_rate
// Correcting the PID code fixed my issues with the fifo buffer being too fast
#include "Wire.h"

MPU6050 mpu;

// These are my MPU6050 Offset numbers: for mpu.setXGyroOffset()
// supply your own gyro offsets here, scaled for min sensitivity use MPU6050_calibration.ino <<< download to calibrate your MPU6050 put the values the probram returns below
//                       XA      YA      ZA      XG      YG      ZG
int MPUOffsets[6] = {  -4232,  -706,   1729,    173,    -94,     37}; //MPU6050 on balanceing bot
//int MPUOffsets[6] = {-24640, 20392, 1784, -62.0, 19.0, 19.0};  //MPU9055 with pro mini

#include <SoftwareSerial.h>
//#include <AltSoftSerial.h>

#define LED_PIN 13 


// ================================================================
// ===                      GENERAL PURPOSE                     ===
// ================================================================

unsigned long calib_init_time;
//lets the sensor 30 seconds to get stabilized
const unsigned long calib_setup_time = 30000;
float yaw_init_val = 0, yaw_curr_val = 0, yaw_calib_val = 0;
enum State {INITIAL_CALIB_WAIT, CALIBRATION, DATA, WAIT_FOR_DATA};

const int white_button_pin = 4;
const int red_button_pin = 5;
const int green_button_pin = 6;
const int blue_button_pin = 7;

int white_button_state = 0;
int red_button_state = 0;
int green_button_state = 0;
int blue_button_state = 0;

int white_saved_state = 0;
int red_saved_state = 0;
int green_saved_state = 0;
int blue_saved_state = 0;

// ================================================================
// ===                  HALL EFFECT CONSTANTS                   ===
// ================================================================

const int magnet_pin = 3; 

const unsigned long milli_seconds_in_second = 1000;
const unsigned long milli_seconds_in_minute = 60000;
const unsigned long milli_seconds_in_hour = 3600000;
const unsigned long speed_zero_threshold = 3000;

volatile unsigned long current_time = millis(); 
volatile unsigned long revolution_time = 0;
volatile unsigned long last_revolution_start_time = millis();
volatile unsigned long last_checked_time = millis();

volatile float kph = 0.00;
const float bicycle_wheel_circumference = 2.1206; // Circumference of bicycle wheel expressed in meters


// ================================================================
// ===                    UNITY CONNECTION                      ===
// ================================================================

byte unity_input;
State unity_state = INITIAL_CALIB_WAIT;

const int delay_time = 25;

void sendUnityIntAsBytes(int x) {
  byte high, low;
  low = x & 255;
  high = (x >> 8) & 255;
  Serial.write(high);
  Serial.write(low);
}


// ================================================================
// ===                      APP CONNECTION                      ===
// ================================================================

State app_state = WAIT_FOR_DATA;
String app_input = ""; 
SoftwareSerial BT(10, 11); // RX, 
//AltSoftSerial BT;//RX=8 TX=9
volatile double distance_traveled = 0.0; 



// ================================================================
// ===               COMPUTE SPEED INTERRUPT ROUTINE            ===
// ================================================================

 void finishedCycle()//This function is called whenever a magnet/interrupt is detected by the arduino
 {
    current_time = millis();
    revolution_time = current_time - last_revolution_start_time;
    
    // Compute current speed in kilometers per hour based on time it took to complete last wheel revolution
    if(revolution_time > 0){
      kph = ( milli_seconds_in_hour/revolution_time)*(bicycle_wheel_circumference/1000);
    }
    last_revolution_start_time = current_time;
    last_checked_time = current_time;

    distance_traveled += bicycle_wheel_circumference;
 }


// ================================================================
// ===          MPU6050 INTERRUPT DETECTION ROUTINE             ===
// ================================================================
volatile bool mpuInterrupt = false;     // indicates whether MPU interrupt pin has gone high
void dmpDataReady() {
  mpuInterrupt = true;
}

// ================================================================
// ===                      MPU DMP SETUP                       ===
// ================================================================
int FifoAlive = 0; // tests if the interrupt is triggering
int IsAlive = -20;     // counts interrupt start at -20 to get 20+ good values before assuming connected
// MPU control/status vars
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container
VectorInt16 aa;         // [x, y, z]            accel sensor measurements
VectorInt16 aaReal;     // [x, y, z]            gravity-free accel sensor measurements
VectorInt16 aaWorld;    // [x, y, z]            world-frame accel sensor measurements
VectorFloat gravity;    // [x, y, z]            gravity vector
float euler[3];         // [psi, theta, phi]    Euler angle container
float ypr[3];           // [yaw, pitch, roll]   yaw/pitch/roll container and gravity vector
float Yaw, Pitch, Roll; // in degrees



void MPU6050Connect() {
  static int MPUInitCntr = 0;
  // initialize device
  mpu.initialize(); // same
  // load and configure the DMP
  devStatus = mpu.dmpInitialize();// same

  if (devStatus != 0) {
    // ERROR!
    // 1 = initial memory load failed
    // 2 = DMP configuration updates failed
    // (if it's going to break, usually the code will be 1)

    char * StatStr[5] { "No Error", "initial memory load failed", "DMP configuration updates failed", "3", "4"};

    MPUInitCntr++;

    Serial.print(F("MPU connection Try #"));
    Serial.println(MPUInitCntr);
    Serial.print(F("DMP Initialization failed (code "));
    Serial.print(StatStr[devStatus]);
    Serial.println(F(")"));

    if (MPUInitCntr >= 10) return; //only try 10 times
    delay(1000);
    MPU6050Connect(); // Lets try again
    return;
  }

  mpu.setXAccelOffset(MPUOffsets[0]);
  mpu.setYAccelOffset(MPUOffsets[1]);
  mpu.setZAccelOffset(MPUOffsets[2]);
  mpu.setXGyroOffset(MPUOffsets[3]);
  mpu.setYGyroOffset(MPUOffsets[4]);
  mpu.setZGyroOffset(MPUOffsets[5]);

  //Serial.println(F("Enabling DMP..."));
  mpu.setDMPEnabled(true);
  // enable Arduino interrupt detection

  //Serial.println(F("Enabling interrupt detection (Arduino external interrupt pin 2 on the Uno)..."));
  attachInterrupt(0, dmpDataReady, FALLING); //pin 2 on the Uno

  mpuIntStatus = mpu.getIntStatus(); // Same
  // get expected DMP packet size for later comparison
  packetSize = mpu.dmpGetFIFOPacketSize();
  delay(1000); // Let it Stabalize
  mpu.resetFIFO(); // Clear fifo buffer
  mpu.getIntStatus();
  mpuInterrupt = false; // wait for next interrupt
}

// ================================================================
// ===                      i2c SETUP Items                     ===
// ================================================================
void i2cSetup() {
  // join I2C bus (I2Cdev library doesn't do this automatically)
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
  Wire.begin();
  TWBR = 24; // 400kHz I2C clock (200kHz if CPU is 8MHz)
#elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
  Fastwire::setup(400, true);
#endif
}


// ================================================================
// ===              MPU GETTING DATA FUNCTIONS                  ===
// ================================================================

void GetDMP() { // Best version I have made so far
  // Serial.println(F("FIFO interrupt at:"));
  // Serial.println(micros());

  //Serial.println("here1");
  mpuInterrupt = false;
  FifoAlive = 1;
  fifoCount = mpu.getFIFOCount();
  //Serial.println("here2");
  /*
  fifoCount is a 16-bit unsigned value. Indicates the number of bytes stored in the FIFO buffer.
  This number is in turn the number of bytes that can be read from the FIFO buffer and it is
  directly proportional to the number of samples available given the set of sensor data bound
  to be stored in the FIFO
  */

  // PacketSize = 42; refference in MPU6050_6Axis_MotionApps20.h Line 527
  // FIFO Buffer Size = 1024;
  uint16_t MaxPackets = 20;// 20*42=840 leaving us with  2 Packets (out of a total of 24 packets) left before we overflow.
  // If we overflow the entire FIFO buffer will be corrupt and we must discard it!

  // At this point in the code FIFO Packets should be at 1 99% of the time if not we need to look to see where we are skipping samples.
  if ((fifoCount % packetSize) || (fifoCount > (packetSize * MaxPackets)) || (fifoCount < packetSize)) { // we have failed Reset and wait till next time!
    //Serial.println("here3");
    digitalWrite(LED_PIN, LOW); // lets turn off the blinking light so we can see we are failing.
    //Serial.println(F("Reset FIFO"));
    //if (fifoCount % packetSize) Serial.print(F("\t Packet corruption")); // fifoCount / packetSize returns a remainder... Not good! This should never happen if all is well.
    //Serial.print(F("\tfifoCount ")); Serial.print(fifoCount);
    //Serial.print(F("\tpacketSize ")); Serial.print(packetSize);

    mpuIntStatus = mpu.getIntStatus(); // reads MPU6050_RA_INT_STATUS       0x3A
    //Serial.println("here4");
    
    //Serial.print(F("\tMPU Int Status ")); Serial.print(mpuIntStatus , BIN);
    // MPU6050_RA_INT_STATUS       0x3A
    //
    // Bit7, Bit6, Bit5, Bit4          , Bit3       , Bit2, Bit1, Bit0
    // ----, ----, ----, FIFO_OFLOW_INT, I2C_MST_INT, ----, ----, DATA_RDY_INT

    /*
    Bit4 FIFO_OFLOW_INT: This bit automatically sets to 1 when a FIFO buffer overflow interrupt has been generated.
    Bit3 I2C_MST_INT: This bit automatically sets to 1 when an I2C Master interrupt has been generated. For a list of I2C Master interrupts, please refer to Register 54.
    Bit1 DATA_RDY_INT This bit automatically sets to 1 when a Data Ready interrupt is generated.
    */
    if (mpuIntStatus & B10000) { //FIFO_OFLOW_INT
      //Serial.print(F("\tFIFO buffer overflow interrupt "));
    }
    if (mpuIntStatus & B1000) { //I2C_MST_INT
      //Serial.print(F("\tSlave I2c Device Status Int "));
    }
    if (mpuIntStatus & B1) { //DATA_RDY_INT
      //Serial.print(F("\tData Ready interrupt "));
    }
    //Serial.println();
    //I2C_MST_STATUS
    //PASS_THROUGH, I2C_SLV4_DONE,I2C_LOST_ARB,I2C_SLV4_NACK,I2C_SLV3_NACK,I2C_SLV2_NACK,I2C_SLV1_NACK,I2C_SLV0_NACK,
    mpu.resetFIFO();// clear the buffer and start over
    //Serial.println("here5");
    mpu.getIntStatus(); // make sure status is cleared we will read it again.
    //Serial.println("here6");
  } else {
    //Serial.println("here7");
    while (fifoCount  >= packetSize) { // Get the packets until we have the latest!
      //Serial.println("here8");
      if (fifoCount < packetSize) break; // Something is left over and we don't want it!!!
      mpu.getFIFOBytes(fifoBuffer, packetSize); // lets do the magic and get the data
      //Serial.println("here9");
      fifoCount -= packetSize;
    }
    MPUMath(); // <<<<<<<<<<<<<<<<<<<<<<<<<<<< On success MPUMath() <<<<<<<<<<<<<<<<<<<
    digitalWrite(LED_PIN, !digitalRead(LED_PIN)); // Blink the Light
    //Serial.println("here10");
    if (fifoCount > 0) mpu.resetFIFO(); // clean up any leftovers Should never happen! but lets start fresh if we need to. this should never happen.
    //Serial.println("here11");
  }
}


void MPUMath() {
  mpu.dmpGetQuaternion(&q, fifoBuffer);
  mpu.dmpGetGravity(&gravity, &q);
  mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
  Yaw = (ypr[0] * 180 / M_PI);
  Pitch = (ypr[1] * 180 / M_PI);
  Roll = (ypr[2] * 180 / M_PI);
}





// ================================================================
// ===                      INITIAL SETUP                       ===
// ================================================================


void setup() {
  //Serial connection part (with unity):
  Serial.begin(9600); 
  while (!Serial);
  while (Serial.available() && Serial.read()); // empty buffer

  //MPU6050 part:
  i2cSetup();
  MPU6050Connect();
  pinMode(LED_PIN, OUTPUT); // LED Blinks when you are recieving FIFO packets from your MPU6050

  //hall effect sensor part:
  attachInterrupt(digitalPinToInterrupt(magnet_pin), finishedCycle, RISING);//Initialize the intterrupt pin (Arduino digital pin 3)
  pinMode(magnet_pin, INPUT); 

  //buttons part:  
  pinMode(white_button_pin, INPUT);
  pinMode(red_button_pin, INPUT);
  pinMode(green_button_pin, INPUT);
  pinMode(blue_button_pin, INPUT);

  //app part (bluetooth):
  BT.begin(9600); // HC-06 Usually default baud-rate
  while (BT.available() && BT.read()); // empty buffer
}


// ================================================================
// ===                    MAIN PROGRAM LOOP                     ===
// ================================================================

void loop() {
  if (mpuInterrupt ) { // wait for MPU interrupt or extra packet(s) available
    GetDMP(); // Gets the MPU Data and canculates angles
  }

    white_button_state = digitalRead(white_button_pin);
    red_button_state = digitalRead(red_button_pin);
    green_button_state = digitalRead(green_button_pin);
    blue_button_state = digitalRead(blue_button_pin);

    if(white_button_state == HIGH && white_saved_state == LOW){
      Serial.write(0x03);
    }
    if(red_button_state == HIGH && red_saved_state == LOW){
      Serial.write(0x04);
    }
    if(green_button_state == HIGH && green_saved_state == LOW){
      Serial.write(0x05);
    }
    if(blue_button_state == HIGH && blue_saved_state == LOW){
      Serial.write(0x06);
    }

    if(white_button_state == LOW && white_saved_state == HIGH){
      Serial.write(0xFC);
    }
    if(red_button_state == LOW && red_saved_state == HIGH){
      Serial.write(0xFB);
    }
    if(green_button_state == LOW && green_saved_state == HIGH){
      Serial.write(0xFA);
    }
    if(blue_button_state == LOW && blue_saved_state == HIGH){
      Serial.write(0xF9);
    }
    
    white_saved_state = white_button_state;
    red_saved_state = red_button_state;
    green_saved_state = green_button_state;
    blue_saved_state = blue_button_state;
    
    if(Serial.available()){
      //check if there are commands from unity
      unity_input = Serial.read();
      if(unity_input == 0x00){
        //if recieved a calibrating command
        
        if(unity_state == INITIAL_CALIB_WAIT){
          calib_init_time = millis();
          unity_state = CALIBRATION;
        }
        else{
          Serial.write(0x00);
          unity_state = WAIT_FOR_DATA;
        }
      }    
      if(unity_input == 0x01){
        //if recieved a 'send data' command
        unity_state = DATA;
      }
    }

    if(BT.available()){
      while(BT.available()){
        delay(10); // Delay added to make thing stable
        char c = BT.read();
        app_input += c; // Build the string.
      }
      if(app_input == "data"){
        app_state = DATA;
        distance_traveled = 0.0;
      }
      else if(app_input == "end"){
        app_state = WAIT_FOR_DATA;
      }
      app_input = "";
    }

    //hall effect part:
    current_time = millis();
    if((current_time - last_checked_time) > speed_zero_threshold){
      //if more than 3 seconds there has not been a cycle, the speed is probably 0.
      kph = 0;
    }

    if(unity_state == CALIBRATION && ((millis() - calib_init_time) > calib_setup_time) ){
            yaw_init_val = Yaw;
            Serial.write(0x00);           
            unity_state = WAIT_FOR_DATA;
    }
    
    if(unity_state == DATA){
            yaw_curr_val = Yaw;
            if(yaw_curr_val >= max((yaw_init_val - 180), -180) && yaw_curr_val <= min(180, (yaw_init_val + 180))){
              yaw_calib_val = yaw_curr_val - yaw_init_val;
            }
            if(yaw_curr_val >= -180 && yaw_curr_val <= max((yaw_init_val - 180), -180)){
              yaw_calib_val = 360 - yaw_init_val + yaw_curr_val;
            }
            if(yaw_curr_val >= min(180, (yaw_init_val + 180)) && yaw_curr_val <= 180){
              yaw_calib_val = yaw_curr_val - 360 - yaw_init_val;
            }
            
            Serial.write(0x02);
            sendUnityIntAsBytes((int)kph);
            Serial.write(0x01);
            sendUnityIntAsBytes((int)yaw_calib_val);
    }

    if(app_state == DATA){
      BT.print("D");
      BT.print((int)distance_traveled);   
      BT.print("S");
      BT.print((int)kph);     
    }
    
    delay(delay_time);
}
