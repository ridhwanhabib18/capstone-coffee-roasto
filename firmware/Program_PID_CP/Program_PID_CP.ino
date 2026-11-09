#include <max6675.h>
#include <Servo.h>

unsigned long interval1 = 1000; //untuk interval1=500,buat kelipatan 500 (memengaruhi periode sampling)(maksimal 2000/2 detik)
int banyak_sampling = 4;       //banyak sampling suhu yang dilakukan(maksimal 10).karena diset 10 
unsigned long waktu_ref=600; //waktu referensi untuk pemanasan mesin(dalam detik)di awal roasting [60detik x 10(menit)]
unsigned long previousMillis=0,start_millis=0;

int kompor =9,motorDC=12,blower=13,cooler=10;          //pin aktuator

float sensor1[11],sensor2[11],sum_sensor1=0,sum_sensor2=0; //array maks 10 cuplikan suhu
float ave_sensor1_saatini,ave_sensor2_saatini,ave_sensor1_sebelum,ave_sensor2_sebelum,delta_sensor1,delta_sensor2;
String input_pengguna2_string,input_pengguna_string;    //string yang diterima dari komputer

// MAX6675 Pins
int thermoDO1 = 3,thermoCS1 = 4,thermoCLK1 = 5,thermoDO2 = 6,thermoCS2 = 7,thermoCLK2 = 8; //pin sensor suhu(13-11)(drum), (10-8)(biji)
MAX6675 thermocouple1(thermoCLK1, thermoCS1, thermoDO1); //drum
MAX6675 thermocouple2(thermoCLK2, thermoCS2, thermoDO2); //biji

int i=0;
String Array_suhu[15]; //array suhu referensi yang diterima dari komputer
float Array_suhu2[15]; //array suhu referensi sebagai array buffer di arduino
int index_suhu=9;      // banyak jumlah data yang dikirimkan (10 data)
float suhu_ref_saatini;

bool heating=true,temperature_done=false,kompor_hidup=false; //Inisiasi State 

//Konfigurasi PID Nilai Kp, Ki, Kd dari Simulasi MATLAB
float kp = 21.095;
float ki = 0.20000 ;
float kd = 30;
float proporsional,integral,derivatif,pid;
float error,errorx,sumerr;

Servo myservo;

void setup() {
  // put your setup code here, to run once:
  myservo.attach(11);
  pinMode(kompor, OUTPUT);
  pinMode(motorDC,OUTPUT);
  pinMode(blower,OUTPUT);
  pinMode(cooler,OUTPUT);
  //kondisi semua dalam keadaan mati
  digitalWrite(kompor,HIGH);
  digitalWrite(motorDC,HIGH);
  digitalWrite(blower,HIGH);
  digitalWrite(cooler,HIGH);
  myservo.write(0); // Valve Full Buka
  kompor_hidup=false;
  Serial.begin(9600);
  delay(500);

}

void loop() {
  // put your main code here, to run repeatedly:
  //inisiasi kondisi awal
  heating=true;
  index_suhu=9;

  if (Serial.available()){
    delay(2);
    input_pengguna_string=Serial.readStringUntil('\n');
    //membaca input dari user, apakah akan melakukan roasting otomatis, heating, memberikan input aktuator manual, input suhu manual
    }

  read_temperature(); //fungsi memmbaca suhu thermokopel yang ada d bawah program untuk detail programnya

            if(input_pengguna_string=="k"){           //kompor mati (k)
                digitalWrite(kompor,HIGH);
                suhu_ref_saatini=0;
                kompor_hidup=false;
                }
                
            else if(input_pengguna_string=="n"){       //kompor hidup (n)
                digitalWrite(kompor,LOW);
                suhu_ref_saatini=0;
                kompor_hidup=true;
                }
      
            else if(input_pengguna_string=="d"){       //drum mati (d)
                digitalWrite(motorDC,HIGH);}
                
            else if(input_pengguna_string=="t"){       //drum hidup (t)
                digitalWrite(motorDC,LOW);}
                
            else if(input_pengguna_string=="b"){       //blower mati (b)
                digitalWrite(blower,HIGH);}
                
            else if(input_pengguna_string=="y"){       //blower hidup (y)
                digitalWrite(blower,LOW);}
                
            else if(input_pengguna_string=="c"){       //Cooler mati (c)
                digitalWrite(cooler,HIGH);}
                
            else if(input_pengguna_string=="p"){       //cooler hidup (p)
                digitalWrite(cooler,LOW);}

            else if(input_pengguna_string=="x"){       //valve Full Tutup (x)
                  myservo.write(40);}
                
            else if(input_pengguna_string=="v"){       //valve Full Buka (v)
                  myservo.write(0);}                
                
            else if (input_pengguna_string!=""){              //saat ada input, namun bukan karakter yang telah didefiniskan maka akan dianggap berupa input suhu
              
                suhu_ref_saatini=input_pengguna_string.toFloat();   //melakukan konversi dari tipe data string ke tipe data float

                }
              
                if (temperature_done){                            //saat pembacaan suhu selesai maka boolean temperature done akan TRUE sehingga akan masuk ke perintah if
                  if (suhu_ref_saatini!=0 ){                      //saat terdapat input suhu referensi dari user maka akan melakukan on off kompor secara otomatis
                    // PID FORMULA
                    error = suhu_ref_saatini - ave_sensor1_saatini; // Set Point - Suhu Aktual dari Sensor
                    proporsional = error * kp;       
                    sumerr = error + errorx;
                    integral = ki * sumerr;
                    derivatif = error - errorx;
                    pid = proporsional + integral + derivatif;
         
                    if(pid < 0){
                        pid = 0;
                       }
                      
                    if(pid > 40){
                       pid = 40;
                       } 
                   
                    if ((not kompor_hidup&&(pid >= 40))){
                        digitalWrite(kompor,LOW);
                        myservo.write(40-pid);
                        kompor_hidup=true;
                        Serial.println("Y");
                        }    //KOMPOR NYALA
                    
                    else if (kompor_hidup&&(pid <= 0)){
                        digitalWrite (kompor, HIGH);
                        myservo.write(40-pid);
                        kompor_hidup=false;
                        Serial.println("Z");
                        } //KOMPOR MATI
                  }
                }

        //PID Heat Auto
        while (input_pengguna_string=="q"){
        //akan masuk ke while saat mendapatkan input "q" yang mengindikasikan user ingin melakukan roasting otomatis metode PID
        
             read_temperature();
    
             // kondisi yakni motor dan blower hidup, sedangkan cooler akan mati
             digitalWrite(motorDC,LOW);
             digitalWrite(blower,LOW);
             digitalWrite(cooler,HIGH);
          
             // PID FORMULA
             error = suhu_ref_saatini - ave_sensor1_saatini; // Set Point - Suhu Aktual dari Sensor
             proporsional = error * kp;       
             sumerr = error + errorx;
             integral = ki * sumerr;
             derivatif = error - errorx;
             pid = proporsional + integral + derivatif;
 
             if(pid < 0){
                pid = 0;
                }
              
             if(pid > 40){
                pid = 40;
                }
             myservo.write(40-pid); // Valve Full Buka (BYPASS)
             
             if (Serial.available()){                        
             //fungsi bagian ini yakni untuk membaca input lanjutan dari user, mulai dari stop roasting atau suhu referensi
                  delay(2);
                  input_pengguna2_string=Serial.readStringUntil('\n');}
             
             if((input_pengguna2_string=="s")||(ave_sensor1_saatini >= 250)){              
             //melakukan stop manual roasting ketika diberi perintah stop dari aplikasi atau sistem akan mati ketika suhu mencapai 250 C
                 input_pengguna2_string="";
                 suhu_ref_saatini=0;
                 //kompor mati, blower mati, motor hidup, cooler mati
                 Serial.println("X");
                 digitalWrite(kompor,HIGH);
                 kompor_hidup=false;
                 Serial.println("Z");
                 digitalWrite(blower,HIGH);
                 digitalWrite(motorDC,LOW);
                 digitalWrite(cooler,HIGH);
                 myservo.write(0); // Valve Full Buka (BYPASS)
                 break;}

              //saat terdapat kiriman data suhu referensi 
             else if (input_pengguna2_string!="") {
              //suhu dikirimkan dalam kumpulan 10 data sekaligus, sehingga harus dipisahkan 
                  for (byte index=0; index<=9;index++){
                      Array_suhu[index]=pisahkansuhuref(input_pengguna2_string, ',', int(index));
                  }
              //saat sudah ada suhu referensi maka proses heating(menjaga suhu tetap di 150 derajat)
                  heating=false;
             }
             // suhu ref saat belum ada kiriman suhu dari komputer didefinisikan di 150 derajat celcius
             if (heating){
                  suhu_ref_saatini=150;
             }
          
             if (temperature_done){  
                  //saat sudah ada suhu referensi             
                  if(not heating){
                        //saat array suhu refrensi sudah habis
                        if (index_suhu==9){
                            // array suhu dalam bentuk string yang sebagai array buffer akan memindahkan datanya ke array lain dengan di ubah menjadi tipe data float
                            //array buffer digunakan untuk mengurangi kemungkinan terjadinya tabrakan data(saat data yang terkirim belum habis digunakan namun seudah menerima data baru dari komputer
                            for (byte index=0;index<=9;index++){
                                Array_suhu2[index]=Array_suhu[index].toFloat();
                            }
                            
                            index_suhu=-1;
                        }
                        //increment nilai address
                        index_suhu++;
                        
                        suhu_ref_saatini=Array_suhu2[index_suhu];
                        
                  }
              
             }
             //on off kompor sesuai dengan kondisi yang didefinisikan dengan PID
             if ((not kompor_hidup&&(pid == 40))){
                 digitalWrite(kompor,LOW);
                 kompor_hidup=true;
                 myservo.write(40-pid);
                 Serial.println("Y");}    //KOMPOR NYALA
            
             else if (kompor_hidup&&(pid == 0)){
                 digitalWrite (kompor, HIGH);
                 kompor_hidup=false;
                 myservo.write(40-pid);
                 Serial.println("Z");
                 } //KOMPOR MATI
        //saat suhu referensi=0 maka artinya suhu yang dikirimkan oleh komputer telah habis(roasting sudah selesai
             if (suhu_ref_saatini==0){                                           //roasting telah selesai
                 // tanda ke komputer bahwa roasting telah selesai
                 //kompor akan mati, blower mati, drum tetap menyala, dan cooler hidup
                 Serial.println("X");
                 digitalWrite(kompor,HIGH);
                 myservo.write(0); // Valve Full Buka
                 kompor_hidup=false;
                 delay (100);
                 digitalWrite(blower,HIGH);
                 Serial.println("Z");
                 digitalWrite(motorDC,LOW);
                 digitalWrite(cooler,LOW);
                 //input_pengguna_string="";
                 input_pengguna2_string="";
                 index_suhu=0;
                 break;
             }
            //melakukan pengosongan input user agar tidak terjadi timpaan data.
             if (input_pengguna2_string.length()>0){
    
                 input_pengguna2_string="";
             }
        }

      //HEATING
      while (input_pengguna_string=="h"){//heating akan berlangsung saat terdapat input huruf h dari komputer
              //melakukan pencatatan waktu saat ini
              unsigned long currentMillis=millis();
              digitalWrite(motorDC,LOW);
              digitalWrite(blower,LOW);
              digitalWrite(cooler,HIGH);
              
              // PID FORMULA
              error = suhu_ref_saatini - ave_sensor1_saatini; // Set Point - Suhu Aktual dari Sensor
              proporsional = error * kp;       
              sumerr = error + errorx;
              integral = ki * sumerr;
              derivatif = error - errorx;
              pid = proporsional + integral + derivatif;
 
              if(pid < 0){
                 pid = 0;
                 }
              
              if(pid > 40){
                 pid = 40;
                 }
              myservo.write(0); // Valve Full Buka (BYPASS)
              
              //melakukan pencatatan waktu saat proses heating dimulai dan inisiasi nilai suhu yang diinginkan yakni 150 derajat celcius
          if (heating==true){
              start_millis=millis();
              heating=false;
              suhu_ref_saatini=150;
          }
     
          read_temperature();

          if (Serial.available()){
              delay(2);
              input_pengguna2_string=Serial.readStringUntil('\n');
          }
              
          if(input_pengguna2_string=="s"){
              //input_pengguna_string="";
              input_pengguna2_string="";
              digitalWrite(kompor,HIGH);
              kompor_hidup=false;
              digitalWrite(motorDC,LOW);
              digitalWrite(blower,HIGH);
              digitalWrite(cooler,HIGH);
              suhu_ref_saatini=0;
              myservo.write(0); // Valve Full Buka (BYPASS)
              break;
          }

          // on off kompor sesuai kondisi
          if ((not kompor_hidup&&(pid >= 40))&&((currentMillis-start_millis)<=(1000*waktu_ref))){
              digitalWrite(kompor,LOW);
              myservo.write(0); // Valve Full Buka (BYPASS)
              kompor_hidup=true;
              Serial.println("Y");}    //KOMPOR NYALA
          
          else if (kompor_hidup&&(pid <= 0)){
              digitalWrite (kompor, HIGH);
              myservo.write(0); // Valve Full Buka (BYPASS)
              kompor_hidup=false;
              Serial.println("Z");
              } //KOMPOR MATI

          //heating akan selesai saat lama waktu heating yakni 10 menit
          if ((currentMillis-start_millis)>=waktu_ref*1000){
              Serial.println("V");
              digitalWrite(kompor,HIGH);
              delay (100);
              kompor_hidup=false;
              digitalWrite(motorDC,LOW);
              digitalWrite(blower,HIGH);
              digitalWrite(cooler,HIGH);
              myservo.write(0); // Valve Full Buka (BYPASS)
              Serial.println("Z");
              break;}
              
          if (input_pengguna2_string.length()>0){
               //suhu_ref_saatini=0;
               input_pengguna2_string="";}
              
      }
      input_pengguna_string="";      
}
