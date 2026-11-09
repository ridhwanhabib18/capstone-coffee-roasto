void setup() {
  // put your setup code here, to run once:

}

void loop() {
  // put your main code here, to run repeatedly:
  moveServo();
}

void moveServo(int X){
  // vel satuannya derajat per 1 s
  // 10 ms per derajat 
  static unsigned long Now, Then;
  static int Deg;
  Now = millis();
  if(Deg<X){
    if(Now-Then>100)){ // 1 derajat membutuhkan waktu 100 ms
      myServo.write(Deg);
      Deg = Deg+1;
      Then = Now;
    }
  }
  if(Deg>X){
    if(Now-Then>100)){
      myServo.write(Deg);
      Deg = Deg-1;
      Then = Now;
    }
  }
  if(Deg == X){
    myServo.write(Deg);
  }
}
