
char TxData[150];
char TxData1[150];
char TxData2[150];
char RxData[5];
const int TankFill = 51; //Digital Output for CTRL relay for Fill Valve
const int TankDrain = 49; //Digital Output for CTRL relay for Drain Valve
const int Lights = 23; //Digital Output for CTRL relay for Fill Valve
const int Fan = 25; //Digital Output for CTRL relay for Drain Valve
const int TankLevel = A3; //Analog reading for tank level



int num = 0;

void setup() {
  Serial.begin (115200);
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(TankFill, OUTPUT);
  pinMode(TankDrain, OUTPUT);
  pinMode(Lights, OUTPUT);
  pinMode(Fan, OUTPUT);
  pinMode(TankLevel, INPUT);

}

void loop() {
  if (Serial.available()>0)
  {
  //*****************Manual Light Control********************************************
  if (Serial.find("Light")) {
    Serial.readBytesUntil('>', RxData, 2);
    if (RxData[1] == '1')
    {
      digitalWrite(LED_BUILTIN, HIGH);// Testing purposes
      digitalWrite(Lights, HIGH); //Closes relay for lights
      sprintf(TxData, "ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights On\"}>ET");//Changes status label on control page
           Serial.write(TxData);
      sprintf(TxData1, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\":\  false\}>ET");//Changes status label on Home Page
           Serial.write(TxData1);
      sprintf(TxData2, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\":\ true\}>ET");//Changes status label on Home Page
           Serial.write(TxData2);
          
    }
    else if (RxData[1] == '2')
    {
      digitalWrite(LED_BUILTIN, LOW);// Testing Purposes
      digitalWrite(Lights, LOW);
      sprintf(TxData, "ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Light_Status\",\"text\":\"Lights Off\"}>ET");//Changes status label on control page
           Serial.write(TxData);
      sprintf(TxData1, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOff\",\"visible\":\  true\}>ET");//Changes status label on Home Page
           Serial.write(TxData1);
      sprintf(TxData2, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"LightsOn\",\"visible\":\ false\}>ET");//Changes status label on Home Page
           Serial.write(TxData2);    
         
    }
  }
//*****************End Manual Light Control********************************************


//***************** Manual Drain Control********************************************
  if (Serial.find("Drain")) {
    Serial.readBytesUntil('>', RxData, 2);
    if (RxData[1] == '1')
    {
      digitalWrite(TankDrain, HIGH);
      sprintf(TxData, "ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Open\"}>ET");
           Serial.write(TxData);
      sprintf(TxData1, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\":\  false\}>ET");//Changes status label on Home Page
           Serial.write(TxData1);
      sprintf(TxData2, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\":\ true\}>ET");//Changes status label on Home Page
           Serial.write(TxData2);
              

    }

    else if (RxData[1] == '2')
    {
      digitalWrite(LED_BUILTIN, LOW);
      digitalWrite(TankDrain, LOW);
      
      sprintf(TxData, "ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"Water_Drain_Status\",\"text\":\"Drain Closed\"}>ET");
          Serial.write(TxData);
      sprintf(TxData1, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainClosed\",\"visible\":\  true\}>ET");//Changes status label on Home Page
           Serial.write(TxData1);
      sprintf(TxData2, "ST<{\"cmd_code\":\"set_visible\",\"type\":\"widget\",\"widget\":\"DrainOpen\",\"visible\":\ false\}>ET");//Changes status label on Home Page
           Serial.write(TxData2);  
                      
    }
    
  }
//End Button

    //sprintf(TxData, "ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"label3\",\"text\":\"%d\"}>ET", num++);
    //sprintf(TxData, "ST<{\"cmd_code\":\"set_text\",\"type\":\"label\",\"widget\":\"label3\",\"text\":\"%TankLevel\"}>ET");
   // Serial.write(13);
   // Serial.write(TxData);
    
  } 
}
