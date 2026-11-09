void read_temperature(){
    temperature_done=false;
    unsigned long currentMillis=millis();
    //melakukan perekaman suhu selama 500ms sekali
    if((currentMillis-previousMillis)>=interval1){
      //menyimpan hasil perekaman dalam array tertentu
        sensor1[i]=thermocouple1.readCelsius();
        sensor2[i]=thermocouple2.readCelsius(); 
        i++;
        previousMillis=millis();}
    //melakuakn rata-rata dari pembacaan sensor saat jumlah data sesuai dengan banyak sampling yang diingikan (dalam hal ini 4 kali sampling)    
    if(i==banyak_sampling){
        for(int j=0;j<banyak_sampling;j++){
            sum_sensor1=sum_sensor1+sensor1[j];
            sum_sensor2=sum_sensor2+sensor2[j];
            if(j==(banyak_sampling-1)){
              //melakukan faktor korenksi menggunakan faktor koreksi dari proses regresi linear
                ave_sensor1_saatini=((sum_sensor2/(j+1))- 3.4921)/0.9194; //bean
                ave_sensor2_saatini=((sum_sensor1/(j+1))- 6.8969)/0.8711;//drum
                //ave_sensor1_saatini=((sum_sensor2/(j+1))+ 0.9845)/0.9655; //bean
                //ave_sensor2_saatini=((sum_sensor1/(j+1))- 3.6176)/0.8963;//drum       

                //untuk mengurangi fluktuasi data maka dapat menggunakan mekanisme ini (Sesuai paper yang dibaca)
                //Perangkat cold junction compensator yang digunakan pada Coffee Roasto adalah MAX6675 
                //dengan keluaran 12 bit dengan ketelitian pembacaan temperatur sebesar 0,25 ℃
                
                if (abs(ave_sensor1_saatini-ave_sensor1_sebelum)<0.25){
                  ave_sensor1_saatini=ave_sensor1_sebelum;
                }
                if (abs(ave_sensor2_saatini-ave_sensor2_sebelum)<0.25){
                  ave_sensor2_saatini=ave_sensor2_sebelum;
                }
                //perhitungan besar kenaikan suhu
                delta_sensor1=(ave_sensor1_saatini-ave_sensor1_sebelum);
                delta_sensor2=(ave_sensor2_saatini-ave_sensor2_sebelum);

                sum_sensor1=0;
                sum_sensor2=0;
                //deret data yang dikirimkan ke komputer
                Serial.print("@");
                Serial.print(ave_sensor1_saatini);
                Serial.print("A");
                Serial.print(ave_sensor2_saatini);
                Serial.print("B");
                if (String(suhu_ref_saatini)==""){
                Serial.print("0");
                Serial.print("C");
                Serial.print("\n");  
                }
                else{
                Serial.print(suhu_ref_saatini);
                Serial.print("C");
                Serial.print("\n");}
                ave_sensor1_sebelum=ave_sensor1_saatini;
                ave_sensor2_sebelum=ave_sensor2_saatini;
                temperature_done=true;
              }
              
        }
        
      i=0;
    }
  
}
