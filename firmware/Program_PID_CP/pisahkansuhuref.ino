//melakukan pemisahan deret data suhu dari komputer, return value yakni nilai suhu ref
String pisahkansuhuref(String data, char pemisah, int index_2)
{
    int suhu_ke = 0;       
    String suhu_ref = "";      
   
    for(int k = 0; k<data.length(); k++) {    
      
      if(data[k]==pemisah) {
      
        suhu_ke++;
        
      }else if(suhu_ke==index_2) {
        //saat sudah sampai ke urutan suhu yang diinginkan
        suhu_ref+=data[k]; 
      }
    }
    return suhu_ref;
}
