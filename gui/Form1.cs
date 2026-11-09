using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using ExcelDataReader;
using System.Diagnostics;

namespace CoffeeRoasto_GUI
{
    public partial class Form_CoffeeRoasto : Form
    {                                                                           // inisialisasi variabel
        Double beantemperature=0, drumtemperature=0, referencetemperature; 
        bool updateData = false;                                           
        private List<Coffee> coffeList = new List<Coffee>();            
        private bool isRecording = false;
        private Stopwatch stopwatch = new Stopwatch();
        private BackgroundWorker backgroundworker = new BackgroundWorker();
        
        public Form_CoffeeRoasto()
        {
            InitializeComponent();
            inibackgroundworker();
        }

        private void inibackgroundworker ()     
        {
            backgroundworker.WorkerSupportsCancellation = true;
            backgroundworker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundworker_RunWorkerCompleted);
        }

        private void backgroundworker_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e) //ngecek background worker running atau ga
        {
            if (e.Error != null)
            {
                Console.WriteLine("error sent to hardware " + e.Error.Message);
            }
            else if (e.Cancelled)
            {
                Console.WriteLine("cancelled");
            }
            else
            {
                Console.WriteLine("success sent to hardware");
            }
        }

        private void ButtonControl_Initialization(bool state)     // inisialisasi button control agar button disable sebelum koneksi berhasil
        {
            button_ONBurner.Enabled = state;
            button_OFFBurner.Enabled = state;

            button_ONMotorDrum.Enabled = state;
            button_OFFMotorDrum.Enabled = state;

            button_ONBlowerDrum.Enabled = state;
            button_OFFBlowerDrum.Enabled = state;

            button_ONCooler.Enabled = state;
            button_OFFCooler.Enabled = state;

            button_ONMotorValve.Enabled = state;
            button_OFFMotorValve.Enabled = state;

            textBox_InputTemp.Enabled = state;
            button_SENDInputTemperature.Enabled = state;

            button_StartRecordData.Enabled = state;
            button_StopRecordData.Enabled = state;
            button_SaveRecordData.Enabled = state;

            textBox_FileName.Enabled = state;
            button_BrowseFile.Enabled = state;
            comboBox_Sheet.Enabled = state;

            button_StartHeatingMode.Enabled = state;
            button_StopHeatingMode.Enabled = state;

            button_StartAutoMode.Enabled = state;
            button_StopAutoMode.Enabled = state;
            button_bean.Enabled = state;
        }

        private void PilotLamp_Initialization()   // inisialisasi lampu indikator
        {
            pilotLamp_Burner.BackColor = Color.DarkGreen;
            pilotLamp_MotorDrum.BackColor = Color.DarkGreen;
            pilotLamp_BlowerDrum.BackColor = Color.DarkGreen;
            pilotLamp_Cooler.BackColor = Color.DarkGreen;
            pilotLamp_RecordData.BackColor = Color.DarkGreen;
            pilotLamp_HeatingMode.BackColor = Color.DarkGreen;
            pilotLamp_AutoMode.BackColor = Color.DarkGreen;
            pilotLampMotorValve.BackColor = Color.DarkGreen;
            pilotLamp_PIDMode.BackColor = Color.DarkGreen;

        }
        private void Form_CoffeeRoasto_Load(object sender, EventArgs e) 
        {
            button_Connect.Enabled = true;
            button_Disconnect.Enabled = false;
            progressBar_PanelConnection.Value = 0;
            ButtonControl_Initialization(false); 
            PilotLamp_Initialization();

            chart_Temperature.Series["Bean Temperature"].Points.AddXY(1, 1); // garis chart saat form load
            chart_Temperature.Series["Drum Temperature"].Points.AddXY(1, 1);

        }

        private void comboBox_ComPort_DropDown(object sender, EventArgs e) //methord port koneksi
        {
            string[] portList = SerialPort.GetPortNames();                 
            comboBox_ComPort.Items.Clear();
            comboBox_ComPort.Items.AddRange(portList);
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                stopwatch.Reset();  // stopwatch tereset saat awal connect
                stopwatch.Start();  // kemudian kembali memulai fungsi stopwatch pada grafik
                
                serialPort1.PortName = comboBox_ComPort.Text; //melakukan koneksi ke hardware berdasarkan port yg dipilih
                serialPort1.BaudRate = 9600;                  // baudrate yg digunakan disesuaikan sama hardware
                serialPort1.Open();                           // fungsi open serial port
                serialPort1.Write("#");                       // buat hapus data arduino saat pertamakali konek arduino

                button_Connect.Enabled = false;                 // button connect disabled
                button_Disconnect.Enabled = true;               // sebaliknya button disconnect jadi disabled
                progressBar_PanelConnection.Value = 100;        // progress bar penanda berhasil melakukan koneksi ke hardware
                ButtonControl_Initialization(true);             // button pada method tersebut jadi enabled (bisa digunakan)

                chart_Temperature.Series["Bean Temperature"].Points.Clear();  // chart clear
                chart_Temperature.Series["Drum Temperature"].Points.Clear();


            }
            catch (Exception error) // fungsi agar ketika method gagal berfungsi, ada pesan error
            {
                MessageBox.Show(error.Message);
            }
        }

        private void button_Disconnect_Click(object sender, EventArgs e)  //method event button disconnect
        {
            if(serialPort1.IsOpen) 
            {
                try
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    serialPort1.Close(); // menutup komunikasi serial
                    
                    button_Connect.Enabled = true;
                    button_Disconnect.Enabled = false;
                    progressBar_PanelConnection.Value = 0;
                    ButtonControl_Initialization(false);
                    chart_Temperature.Series["Bean Temperature"].Points.Clear();
                    chart_Temperature.Series["Drum Temperature"].Points.Clear();
                    chart_Temperature.Series["Reference Temperature"].Points.Clear();

                    label_DataBeanTemperature.Text = "....ºC"; // label pada antarmuka jadi "....ºC"
                    label_DataDrumTemperature.Text = "....ºC";
                    label_DataReferenceTemperature.Text = "....ºC";
                    dataGridView.DataSource = null;            // data table antarmuka clear
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void Form_CoffeeRoasto_FormClosing(object sender, FormClosingEventArgs e) // method saat aplikasi di close
        {
            if(serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                }
                catch(Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_ONBurner_Click(object sender, EventArgs e) // button on burner
        {
                    serialPort1.Write("n");                     // mengirim string n secara serial ke arduino
                    pilotLamp_Burner.BackColor = Color.Lime;    // lampu indikator pilot lamp menu burner jadi warna lime
        }

        private void button_OFFBurner_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("k");
                    pilotLamp_Burner.BackColor = Color.DarkGreen;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_ONMotorDrum_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("t");
                    pilotLamp_MotorDrum.BackColor = Color.Lime;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_OFFMotorDrum_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("d");
                    pilotLamp_MotorDrum.BackColor = Color.DarkGreen;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_ONBlowerDrum_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("y");
                    pilotLamp_BlowerDrum.BackColor = Color.Lime;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_OFFBlowerDrum_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("b");
                    pilotLamp_BlowerDrum.BackColor = Color.DarkGreen;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_ONCooler_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("p");
                    pilotLamp_Cooler.BackColor = Color.Lime;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_OFFCooler_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("c");
                    pilotLamp_Cooler.BackColor = Color.DarkGreen;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_SENDInputTemperature_Click(object sender, EventArgs e) // method fitur input manual suhu referensi
        {
            serialPort1.Write(textBox_InputTemp.Text); // kirim data serial berdasarkan yg di input
            textBox_InputTemp.Text = " ";              // setelah itu di clear
        }

        private void button_StartRecordData_Click(object sender, EventArgs e) // button start fitur perekaman data
        {
            chart_Temperature.Series["Bean Temperature"].Points.Clear(); // chart clear
            chart_Temperature.Series["Drum Temperature"].Points.Clear();
            chart_Temperature.Series["Reference Temperature"].Points.Clear();
            isRecording = true;                                // pada metod dataparsing data yg masuk akan dijadikan list pada class coffelist
            pilotLamp_RecordData.BackColor = Color.Lime;    
            stopwatch.Reset();
            stopwatch.Start();
            if (coffeList.Count > 0)                          // clear isi pada class coffeelist
            { 
                coffeList.Clear();
                dataGridView.DataSource = null;
            }
        }

        private void button_StopRecordData_Click(object sender, EventArgs e)
        {
            isRecording = false;  // kondisi isrecording pada method data parsing false artinya data yg masuk tidak dijadikan list pada class coffelist
            
            dataGridView.DataSource = coffeList; //menampilkan data yg sudah direkam ke data table

            pilotLamp_RecordData.BackColor = Color.DarkGreen;
        }

        private void button_ExportRecordData_Click(object sender, EventArgs e) // fungsi export data dari gridview ke excel
        {

            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();  // creating Excel Application 
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);                 // creating new WorkBook within Excel application 
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;                 // creating new Excelsheet in workbook  
            app.Visible = true;   // see the excel sheet behind the program  
             
            worksheet = workbook.Sheets["Sheet1"]; // get the reference of first sheet. By default its name is Sheet1.
            worksheet = workbook.ActiveSheet;      // store its reference to worksheet
            worksheet.Name = "Exported from gridview";  // changing the name of active sheet 

            for (int i = 1; i < dataGridView.Columns.Count + 1; i++)  // storing header part in Excel 
            {
                worksheet.Cells[1, i] = dataGridView.Columns[i - 1].HeaderText;
            }
              
            for (int i = 0; i < dataGridView.Rows.Count - 1; i++)  // storing Each row and column value to excel sheet
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataGridView.Rows[i].Cells[j].Value.ToString();
                }
            } 
        }

        DataTableCollection tableCollection;


        private void comboBox_Sheet_SelectedIndexChanged(object sender, EventArgs e)   // sheet pada fitur automatis
        {
            DataTable dt = tableCollection[comboBox_Sheet.SelectedItem.ToString()]; // datatable sesuai dengan sheet yg dipilih
            dataGridView.DataSource = dt; // menampilkan pada datatable
        }

        private void button_StartHeatingMode_Click(object sender, EventArgs e) // button start pada heating mode
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("h");
                    pilotLamp_HeatingMode.BackColor = Color.Lime;
                    pilotLamp_MotorDrum.BackColor = Color.Lime;
                    pilotLamp_BlowerDrum.BackColor = Color.Lime;
                    pilotLamp_Cooler.BackColor = Color.DarkGreen;
                    pilotLampMotorValve.BackColor = Color.Lime;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        } // start heating process

        private void button_StopHeatingMode_Click(object sender, EventArgs e)  // button stop pada heating mode
        {
            MessageBox.Show("Roasting Selesai");
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("s");
                    pilotLamp_HeatingMode.BackColor = Color.DarkGreen;
                    pilotLamp_BlowerDrum.BackColor = Color.DarkGreen;
                    pilotLamp_Burner.BackColor = Color.DarkGreen;
                    pilotLamp_MotorDrum.BackColor = Color.Lime;
                    pilotLamp_Cooler.BackColor = Color.DarkGreen;
                    pilotLampMotorValve.BackColor = Color.Lime;

                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        } // stop heating process

        private void button_StartAutoMode_Click(object sender, EventArgs e)  // start automatic mode
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    chart_Temperature.Series["Bean Temperature"].Points.Clear();
                    chart_Temperature.Series["Drum Temperature"].Points.Clear();
                    chart_Temperature.Series["Reference Temperature"].Points.Clear();
                    serialPort1.Write("m");
                    pilotLamp_AutoMode.BackColor = Color.Lime;
                    pilotLamp_MotorDrum.BackColor = Color.Lime;
                    pilotLamp_BlowerDrum.BackColor = Color.Lime;
                    pilotLamp_Cooler.BackColor = Color.DarkGreen;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_StopAutoMode_Click(object sender, EventArgs e)  // stop automatic mode
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("s");
                    pilotLamp_AutoMode.BackColor = Color.DarkGreen;
                    pilotLamp_BlowerDrum.BackColor = Color.DarkGreen;
                    pilotLamp_Burner.BackColor = Color.DarkGreen;
                    pilotLamp_MotorDrum.BackColor = Color.Lime;
                    pilotLamp_Cooler.BackColor = Color.DarkGreen;
                    pilotLampMotorValve.BackColor = Color.DarkGreen;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) // method fungsi menerima data pada class serial port
        {
            string dataTemp = serialPort1.ReadTo("\n");   
            Data_Parsing(dataTemp);                            
        }
        
        private void Show_DataTempChart(object sender, EventArgs e)    // method menampilkan data pada chart 
        {
            label_DataBeanTemperature.Text = string.Format("{0}ºC ", beantemperature.ToString()); // nilai label text berdasarkan data realtime
            label_DataDrumTemperature.Text = string.Format("{0}ºC ", drumtemperature.ToString());
            
            var time = stopwatch.Elapsed.ToString("mm':'ss");    // fungsi stopwatch

            chart_Temperature.Series["Bean Temperature"].Points.AddXY(time, beantemperature);  // plot data pada chart
            chart_Temperature.Series["Drum Temperature"].Points.AddXY(time, drumtemperature);
            chart_Temperature.Series["Reference Temperature"].Points.AddXY(time, referencetemperature);

            if (referencetemperature > 0) // kondisi ketika data reference temperature > 0 maka label text nyesuain nilai data
            {
                label_DataReferenceTemperature.Text = string.Format("{0}ºC", referencetemperature.ToString());
            }
            else // tapi kalo tidak lebih dari 0 maka label text ke default yaitu tidak ada nilainya
            {
                label_DataReferenceTemperature.Text = "....ºC";
            }
        }
        private void button_BrowseFile_Click(object sender, EventArgs e) // button browse pada fitur automatic roasting
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Excel 97-2003 Workbook|*.xls|Excel Workbook|*.xlsx" }) // buka file dialog dan filter file excel yg ditampilkan
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK) // kondisi setelah memilih file dan click ok pada file dialog
                {
                    textBox_FileName.Text = openFileDialog.FileName;
                    using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read)) 
                    {
                        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream)) // read data file
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration() // data excel menajadi dataset
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()  // fileter data yg digunakan data pada column dengan header beantemperature
                                {
                                    UseHeaderRow = true,
                                    FilterColumn = (columnReader, columnIndex) =>
                                    {
                                        string header = columnReader.GetString(columnIndex);
                                        return (header == "beantemperature");
                                    }
                                }
                            });
                            tableCollection = result.Tables;
                            comboBox_Sheet.Items.Clear();
                            foreach (DataTable table in tableCollection)
                                comboBox_Sheet.Items.Add(table.TableName);
                        }
                    }
                }
            }
        }

        private void button_bean_Click(object sender, EventArgs e) // button start pada fitur otomatis roasting
        {
            try
            {
                DataTable dt = new DataTable();
                dt = ((DataTable)dataGridView.DataSource);
                backgroundworker.RunWorkerAsync(dt);        // pengiriman data dengan data pada dt > datagridview
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        
        private void backgroundWorker_DoWork (object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            DataTable arg = (DataTable)e.Argument;
            sendtohardware(arg, bw );
            if (bw.CancellationPending )
            {
                e.Cancel = true;
            }
            
        }

        private void pilotLamp_Burner_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("v");
                    pilotLampMotorValve.BackColor = Color.Lime;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }


        private void button_OFFMotorValve_Click_1(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("x");
                    pilotLampMotorValve.BackColor = Color.DarkGreen;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void pilotLamp_Cooler_Click(object sender, EventArgs e)
        {

        }

        private void pilotLampMotorValve_Click(object sender, EventArgs e)
        {

        }

        private void pilotLamp_AutoMode_Click(object sender, EventArgs e)
        {

        }

        private void button_StartPID_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    chart_Temperature.Series["Bean Temperature"].Points.Clear();
                    chart_Temperature.Series["Drum Temperature"].Points.Clear();
                    chart_Temperature.Series["Reference Temperature"].Points.Clear();
                    serialPort1.Write("q");
                    pilotLamp_PIDMode.BackColor = Color.Lime;
                    pilotLamp_MotorDrum.BackColor = Color.Lime;
                    pilotLamp_BlowerDrum.BackColor = Color.Lime;
                    pilotLamp_Cooler.BackColor = Color.DarkGreen;
                    pilotLampMotorValve.BackColor = Color.Lime;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_StopPID_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("s");
                    pilotLamp_PIDMode.BackColor = Color.DarkGreen;
                    pilotLamp_BlowerDrum.BackColor = Color.DarkGreen;
                    pilotLamp_Burner.BackColor = Color.DarkGreen;
                    pilotLamp_MotorDrum.BackColor = Color.Lime;
                    pilotLamp_Cooler.BackColor = Color.DarkGreen;
                    pilotLampMotorValve.BackColor = Color.Lime;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void sendtohardware(DataTable dt , BackgroundWorker bw ) // method pengiriman data
        {
             while(!bw.CancellationPending )
            {
                var payload = "";

                for ( var i=0; i<dt.Rows.Count; i++)
                {
                    if ((i + 1) % 10 == 0)                                     // pengiriman data kelipatan sepuluh
                    {
                        payload += dt.Rows[i]["beantemperature"].ToString();
                        serialPort1.WriteLine(payload);
                        payload = "";
                        System.Threading.Thread.Sleep(40000);                 // delay 40 detik antar pengiriman
                    }

                    else                                                      // pengiriman data sisa yg buka kelipatan 10
                    {
                        payload += dt.Rows[i]["beantemperature"].ToString() + ",";
                    }
                }
                if (payload != "")
                {
                    serialPort1.WriteLine(payload);
                }
                break;
            }
        }

        private void Data_Parsing(string dataTemp)                                 // method parsing data
        {
            sbyte indexOf_startDataCharacter = (sbyte)dataTemp.IndexOf("@");
            sbyte indexOfA = (sbyte)dataTemp.IndexOf("A");
            sbyte indexOfB = (sbyte)dataTemp.IndexOf("B");
            sbyte indexOfC = (sbyte)dataTemp.IndexOf("C");
            sbyte indexOfX = (sbyte)dataTemp.IndexOf("X"); // roasting Selesai
            sbyte indexOfV = (sbyte)dataTemp.IndexOf("V"); // heating selesai
            sbyte indexOfY = (sbyte)dataTemp.IndexOf("Y"); // lampu indikator burner nyala
            sbyte indexOfZ = (sbyte)dataTemp.IndexOf("Z"); // lampu indikator burner mati
            //@37.11A38.67B39.41C\n  @37.11A38.67B39.41C
            //if character  "A", "B", "C", "@" exist in the data package
            if (indexOfA != -1 && indexOfB != -1 && indexOfC != -1 && indexOf_startDataCharacter != -1)
            {
                try
                {
                    string str_beantemperature = dataTemp.Substring(indexOf_startDataCharacter + 1, (indexOfA -
                        indexOf_startDataCharacter) - 1);
                    string str_drumtemperature = dataTemp.Substring(indexOfA + 1, (indexOfB - indexOfA) - 1);
                    string str_referencetemperature = dataTemp.Substring(indexOfB + 1, (indexOfC - indexOfB) - 1);

                    beantemperature = Convert.ToDouble(str_beantemperature)/100;
                    drumtemperature = Convert.ToDouble(str_drumtemperature)/100;
                    referencetemperature = Convert.ToDouble(str_referencetemperature)/100;
                    
                    updateData = true;

                    if(isRecording)
                    {
                        coffeList.Add(new Coffee() { beantemperature = beantemperature, drumtemperature = drumtemperature});
                    }

                    this.BeginInvoke(new EventHandler(Show_DataTempChart));
                }
                catch (Exception)
                {

                }
            }
            else if (indexOfX != -1)
            {
                MessageBox.Show("Roasting Selesai");
                pilotLamp_AutoMode.BackColor = Color.DarkGreen;
                pilotLamp_BlowerDrum.BackColor = Color.DarkGreen;
                pilotLamp_Burner.BackColor = Color.DarkGreen;
                pilotLamp_MotorDrum.BackColor = Color.Lime;
                pilotLamp_Cooler.BackColor = Color.Lime;
            }

            else if (indexOfV != -1)
            {
                MessageBox.Show("Heating Selesai");
                pilotLamp_HeatingMode.BackColor = Color.DarkGreen;
                pilotLamp_BlowerDrum.BackColor = Color.DarkGreen;
                pilotLamp_Burner.BackColor = Color.DarkGreen;
                pilotLamp_MotorDrum.BackColor = Color.Lime;
                pilotLamp_Cooler.BackColor = Color.DarkGreen;
            }
            else if (indexOfY != -1)
            {
               pilotLamp_Burner.BackColor = Color.Lime;
            }
            else if (indexOfZ != -1)
            {
                pilotLamp_Burner.BackColor = Color.DarkGreen;
            }
            else
            {
                updateData = false;
            }
        }
    }
}
