int kompor =9,motorDC=12,blower=13,cooler=10; 

void setup() {
  // put your setup code here, to run once:
  pinMode(kompor, OUTPUT);
  pinMode(motorDC,OUTPUT);
  pinMode(blower,OUTPUT);
  pinMode(cooler,OUTPUT);

}

void loop() {
  // put your main code here, to run repeatedly:
  /*digitalWrite(cooler,LOW);
  delay(500); //Frekuensi Switch Relay elektromekanik 0,5 Hz Maka Periode Swicth untuk 1 gelombang 2 detik atau 2000 ms
               // Tetapi karena sistem membutuhkan 1000ms atau 1 detik maka akan ada delay pensaklaran
  digitalWrite(cooler,HIGH);
  delay(500);*/
  digitalWrite(kompor,LOW); 
  delay(500); //Frekuensi Switch SSR 5 Hz Maka Periode Swicth untuk 1 gelombang 0,2 detik atau 200 ms
  digitalWrite(kompor,HIGH);
  delay(500);
}
