
namespace CoffeeRoasto_GUI
{
    partial class Form_CoffeeRoasto
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_CoffeeRoasto));
            this.groupBox_PanelConnection = new System.Windows.Forms.GroupBox();
            this.progressBar_PanelConnection = new System.Windows.Forms.ProgressBar();
            this.button_Disconnect = new System.Windows.Forms.Button();
            this.button_Connect = new System.Windows.Forms.Button();
            this.comboBox_ComPort = new System.Windows.Forms.ComboBox();
            this.label_ComPort = new System.Windows.Forms.Label();
            this.button_SaveRecordData = new System.Windows.Forms.Button();
            this.button_StopRecordData = new System.Windows.Forms.Button();
            this.button_StartRecordData = new System.Windows.Forms.Button();
            this.button_StopHeatingMode = new System.Windows.Forms.Button();
            this.label_RecordData = new System.Windows.Forms.Label();
            this.button_StartHeatingMode = new System.Windows.Forms.Button();
            this.label_HeatingMode = new System.Windows.Forms.Label();
            this.textBox_InputTemp = new System.Windows.Forms.TextBox();
            this.button_SENDInputTemperature = new System.Windows.Forms.Button();
            this.label_DerajatCellcius = new System.Windows.Forms.Label();
            this.label_InputTemperature = new System.Windows.Forms.Label();
            this.groupBox_AutomaticControl = new System.Windows.Forms.GroupBox();
            this.button_StopPID = new System.Windows.Forms.Button();
            this.label_PID = new System.Windows.Forms.Label();
            this.button_StartPID = new System.Windows.Forms.Button();
            this.pilotLamp_PIDMode = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.button_bean = new System.Windows.Forms.Button();
            this.button_BrowseFile = new System.Windows.Forms.Button();
            this.comboBox_Sheet = new System.Windows.Forms.ComboBox();
            this.label_Sheet = new System.Windows.Forms.Label();
            this.button_StopAutoMode = new System.Windows.Forms.Button();
            this.label_FileName = new System.Windows.Forms.Label();
            this.label_AutoMode = new System.Windows.Forms.Label();
            this.button_StartAutoMode = new System.Windows.Forms.Button();
            this.pilotLamp_AutoMode = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.pilotLamp_HeatingMode = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.chart_Temperature = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label_BeanTemperature = new System.Windows.Forms.Label();
            this.label_DrumTemperature = new System.Windows.Forms.Label();
            this.label_DataBeanTemperature = new System.Windows.Forms.Label();
            this.label_DataDrumTemperature = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label_DataReferenceTemperature = new System.Windows.Forms.Label();
            this.label_ReferenceTemperature = new System.Windows.Forms.Label();
            this.label_Burner = new System.Windows.Forms.Label();
            this.button_ONBurner = new System.Windows.Forms.Button();
            this.button_OFFBurner = new System.Windows.Forms.Button();
            this.label_MotorDrum = new System.Windows.Forms.Label();
            this.button_ONMotorDrum = new System.Windows.Forms.Button();
            this.button_OFFMotorDrum = new System.Windows.Forms.Button();
            this.label_BlowerDrum = new System.Windows.Forms.Label();
            this.button_ONBlowerDrum = new System.Windows.Forms.Button();
            this.button_OFFBlowerDrum = new System.Windows.Forms.Button();
            this.label_Cooler = new System.Windows.Forms.Label();
            this.button_ONCooler = new System.Windows.Forms.Button();
            this.button_OFFCooler = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_OFFMotorValve = new System.Windows.Forms.Button();
            this.button_ONMotorValve = new System.Windows.Forms.Button();
            this.pilotLampMotorValve = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.pilotLamp_Burner = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.pilotLamp_RecordData = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.pilotLamp_Cooler = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.pilotLamp_BlowerDrum = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.pilotLamp_MotorDrum = new CoffeeRoasto_GUI.UI.PilotLamp();
            this.groupBox_PanelConnection.SuspendLayout();
            this.groupBox_AutomaticControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Temperature)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_PanelConnection
            // 
            this.groupBox_PanelConnection.Controls.Add(this.progressBar_PanelConnection);
            this.groupBox_PanelConnection.Controls.Add(this.button_Disconnect);
            this.groupBox_PanelConnection.Controls.Add(this.button_Connect);
            this.groupBox_PanelConnection.Controls.Add(this.comboBox_ComPort);
            this.groupBox_PanelConnection.Controls.Add(this.label_ComPort);
            this.groupBox_PanelConnection.ForeColor = System.Drawing.Color.RoyalBlue;
            this.groupBox_PanelConnection.Location = new System.Drawing.Point(18, 387);
            this.groupBox_PanelConnection.Name = "groupBox_PanelConnection";
            this.groupBox_PanelConnection.Size = new System.Drawing.Size(225, 142);
            this.groupBox_PanelConnection.TabIndex = 0;
            this.groupBox_PanelConnection.TabStop = false;
            this.groupBox_PanelConnection.Text = "CONNECTION";
            // 
            // progressBar_PanelConnection
            // 
            this.progressBar_PanelConnection.Location = new System.Drawing.Point(9, 89);
            this.progressBar_PanelConnection.Name = "progressBar_PanelConnection";
            this.progressBar_PanelConnection.Size = new System.Drawing.Size(205, 23);
            this.progressBar_PanelConnection.TabIndex = 6;
            // 
            // button_Disconnect
            // 
            this.button_Disconnect.Location = new System.Drawing.Point(113, 58);
            this.button_Disconnect.Name = "button_Disconnect";
            this.button_Disconnect.Size = new System.Drawing.Size(101, 23);
            this.button_Disconnect.TabIndex = 5;
            this.button_Disconnect.Text = "DISCONNECT";
            this.button_Disconnect.UseVisualStyleBackColor = true;
            this.button_Disconnect.Click += new System.EventHandler(this.button_Disconnect_Click);
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(9, 58);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(92, 23);
            this.button_Connect.TabIndex = 4;
            this.button_Connect.Text = "CONNECT";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // comboBox_ComPort
            // 
            this.comboBox_ComPort.BackColor = System.Drawing.SystemColors.Highlight;
            this.comboBox_ComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ComPort.FormattingEnabled = true;
            this.comboBox_ComPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.comboBox_ComPort.Location = new System.Drawing.Point(93, 26);
            this.comboBox_ComPort.Name = "comboBox_ComPort";
            this.comboBox_ComPort.Size = new System.Drawing.Size(121, 25);
            this.comboBox_ComPort.TabIndex = 1;
            this.comboBox_ComPort.DropDown += new System.EventHandler(this.comboBox_ComPort_DropDown);
            // 
            // label_ComPort
            // 
            this.label_ComPort.AutoSize = true;
            this.label_ComPort.Location = new System.Drawing.Point(6, 30);
            this.label_ComPort.Name = "label_ComPort";
            this.label_ComPort.Size = new System.Drawing.Size(81, 17);
            this.label_ComPort.TabIndex = 0;
            this.label_ComPort.Text = "COM PORT :";
            // 
            // button_SaveRecordData
            // 
            this.button_SaveRecordData.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_SaveRecordData.Location = new System.Drawing.Point(433, 599);
            this.button_SaveRecordData.Name = "button_SaveRecordData";
            this.button_SaveRecordData.Size = new System.Drawing.Size(68, 23);
            this.button_SaveRecordData.TabIndex = 24;
            this.button_SaveRecordData.Text = "EXPORT";
            this.button_SaveRecordData.UseVisualStyleBackColor = true;
            this.button_SaveRecordData.Click += new System.EventHandler(this.button_ExportRecordData_Click);
            // 
            // button_StopRecordData
            // 
            this.button_StopRecordData.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_StopRecordData.Location = new System.Drawing.Point(362, 599);
            this.button_StopRecordData.Name = "button_StopRecordData";
            this.button_StopRecordData.Size = new System.Drawing.Size(68, 23);
            this.button_StopRecordData.TabIndex = 23;
            this.button_StopRecordData.Text = "STOP";
            this.button_StopRecordData.UseVisualStyleBackColor = true;
            this.button_StopRecordData.Click += new System.EventHandler(this.button_StopRecordData_Click);
            // 
            // button_StartRecordData
            // 
            this.button_StartRecordData.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_StartRecordData.Location = new System.Drawing.Point(291, 599);
            this.button_StartRecordData.Name = "button_StartRecordData";
            this.button_StartRecordData.Size = new System.Drawing.Size(68, 23);
            this.button_StartRecordData.TabIndex = 22;
            this.button_StartRecordData.Text = "START";
            this.button_StartRecordData.UseVisualStyleBackColor = true;
            this.button_StartRecordData.Click += new System.EventHandler(this.button_StartRecordData_Click);
            // 
            // button_StopHeatingMode
            // 
            this.button_StopHeatingMode.Location = new System.Drawing.Point(201, 24);
            this.button_StopHeatingMode.Name = "button_StopHeatingMode";
            this.button_StopHeatingMode.Size = new System.Drawing.Size(51, 23);
            this.button_StopHeatingMode.TabIndex = 24;
            this.button_StopHeatingMode.Text = "OFF";
            this.button_StopHeatingMode.UseVisualStyleBackColor = true;
            this.button_StopHeatingMode.Click += new System.EventHandler(this.button_StopHeatingMode_Click);
            // 
            // label_RecordData
            // 
            this.label_RecordData.AutoSize = true;
            this.label_RecordData.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_RecordData.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_RecordData.Location = new System.Drawing.Point(283, 569);
            this.label_RecordData.Name = "label_RecordData";
            this.label_RecordData.Size = new System.Drawing.Size(103, 17);
            this.label_RecordData.TabIndex = 20;
            this.label_RecordData.Text = "RECORD DATA ";
            // 
            // button_StartHeatingMode
            // 
            this.button_StartHeatingMode.Location = new System.Drawing.Point(144, 24);
            this.button_StartHeatingMode.Name = "button_StartHeatingMode";
            this.button_StartHeatingMode.Size = new System.Drawing.Size(54, 23);
            this.button_StartHeatingMode.TabIndex = 23;
            this.button_StartHeatingMode.Text = "ON";
            this.button_StartHeatingMode.UseVisualStyleBackColor = true;
            this.button_StartHeatingMode.Click += new System.EventHandler(this.button_StartHeatingMode_Click);
            // 
            // label_HeatingMode
            // 
            this.label_HeatingMode.AutoSize = true;
            this.label_HeatingMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_HeatingMode.Location = new System.Drawing.Point(35, 26);
            this.label_HeatingMode.Name = "label_HeatingMode";
            this.label_HeatingMode.Size = new System.Drawing.Size(108, 17);
            this.label_HeatingMode.TabIndex = 22;
            this.label_HeatingMode.Text = "HEATING MODE";
            // 
            // textBox_InputTemp
            // 
            this.textBox_InputTemp.Location = new System.Drawing.Point(160, 534);
            this.textBox_InputTemp.Name = "textBox_InputTemp";
            this.textBox_InputTemp.Size = new System.Drawing.Size(55, 23);
            this.textBox_InputTemp.TabIndex = 34;
            // 
            // button_SENDInputTemperature
            // 
            this.button_SENDInputTemperature.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_SENDInputTemperature.Location = new System.Drawing.Point(160, 567);
            this.button_SENDInputTemperature.Name = "button_SENDInputTemperature";
            this.button_SENDInputTemperature.Size = new System.Drawing.Size(83, 23);
            this.button_SENDInputTemperature.TabIndex = 19;
            this.button_SENDInputTemperature.Text = "SEND";
            this.button_SENDInputTemperature.UseVisualStyleBackColor = true;
            this.button_SENDInputTemperature.Click += new System.EventHandler(this.button_SENDInputTemperature_Click);
            // 
            // label_DerajatCellcius
            // 
            this.label_DerajatCellcius.AutoSize = true;
            this.label_DerajatCellcius.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_DerajatCellcius.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_DerajatCellcius.Location = new System.Drawing.Point(217, 538);
            this.label_DerajatCellcius.Name = "label_DerajatCellcius";
            this.label_DerajatCellcius.Size = new System.Drawing.Size(26, 17);
            this.label_DerajatCellcius.TabIndex = 18;
            this.label_DerajatCellcius.Text = "ºC ";
            // 
            // label_InputTemperature
            // 
            this.label_InputTemperature.AutoSize = true;
            this.label_InputTemperature.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_InputTemperature.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_InputTemperature.Location = new System.Drawing.Point(14, 537);
            this.label_InputTemperature.Name = "label_InputTemperature";
            this.label_InputTemperature.Size = new System.Drawing.Size(149, 17);
            this.label_InputTemperature.TabIndex = 16;
            this.label_InputTemperature.Text = "INPUT TEMPERATURE :";
            // 
            // groupBox_AutomaticControl
            // 
            this.groupBox_AutomaticControl.Controls.Add(this.button_StopPID);
            this.groupBox_AutomaticControl.Controls.Add(this.label_PID);
            this.groupBox_AutomaticControl.Controls.Add(this.button_StartPID);
            this.groupBox_AutomaticControl.Controls.Add(this.pilotLamp_PIDMode);
            this.groupBox_AutomaticControl.Controls.Add(this.textBox_FileName);
            this.groupBox_AutomaticControl.Controls.Add(this.button_bean);
            this.groupBox_AutomaticControl.Controls.Add(this.button_BrowseFile);
            this.groupBox_AutomaticControl.Controls.Add(this.comboBox_Sheet);
            this.groupBox_AutomaticControl.Controls.Add(this.label_Sheet);
            this.groupBox_AutomaticControl.Controls.Add(this.button_StopAutoMode);
            this.groupBox_AutomaticControl.Controls.Add(this.label_FileName);
            this.groupBox_AutomaticControl.Controls.Add(this.label_AutoMode);
            this.groupBox_AutomaticControl.Controls.Add(this.button_StartAutoMode);
            this.groupBox_AutomaticControl.Controls.Add(this.pilotLamp_AutoMode);
            this.groupBox_AutomaticControl.Controls.Add(this.button_StopHeatingMode);
            this.groupBox_AutomaticControl.Controls.Add(this.label_HeatingMode);
            this.groupBox_AutomaticControl.Controls.Add(this.button_StartHeatingMode);
            this.groupBox_AutomaticControl.Controls.Add(this.pilotLamp_HeatingMode);
            this.groupBox_AutomaticControl.ForeColor = System.Drawing.Color.RoyalBlue;
            this.groupBox_AutomaticControl.Location = new System.Drawing.Point(507, 387);
            this.groupBox_AutomaticControl.Name = "groupBox_AutomaticControl";
            this.groupBox_AutomaticControl.Size = new System.Drawing.Size(265, 290);
            this.groupBox_AutomaticControl.TabIndex = 2;
            this.groupBox_AutomaticControl.TabStop = false;
            this.groupBox_AutomaticControl.Text = "AUTOMATIC CONTROL";
            // 
            // button_StopPID
            // 
            this.button_StopPID.Location = new System.Drawing.Point(202, 229);
            this.button_StopPID.Name = "button_StopPID";
            this.button_StopPID.Size = new System.Drawing.Size(54, 23);
            this.button_StopPID.TabIndex = 39;
            this.button_StopPID.Text = "OFF";
            this.button_StopPID.UseVisualStyleBackColor = true;
            this.button_StopPID.Click += new System.EventHandler(this.button_StopPID_Click);
            // 
            // label_PID
            // 
            this.label_PID.AutoSize = true;
            this.label_PID.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_PID.Location = new System.Drawing.Point(37, 231);
            this.label_PID.Name = "label_PID";
            this.label_PID.Size = new System.Drawing.Size(73, 17);
            this.label_PID.TabIndex = 37;
            this.label_PID.Text = "PID MODE";
            // 
            // button_StartPID
            // 
            this.button_StartPID.Location = new System.Drawing.Point(145, 228);
            this.button_StartPID.Name = "button_StartPID";
            this.button_StartPID.Size = new System.Drawing.Size(54, 23);
            this.button_StartPID.TabIndex = 38;
            this.button_StartPID.Text = "ON";
            this.button_StartPID.UseVisualStyleBackColor = true;
            this.button_StartPID.Click += new System.EventHandler(this.button_StartPID_Click);
            // 
            // pilotLamp_PIDMode
            // 
            this.pilotLamp_PIDMode.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_PIDMode.Location = new System.Drawing.Point(9, 228);
            this.pilotLamp_PIDMode.Name = "pilotLamp_PIDMode";
            this.pilotLamp_PIDMode.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_PIDMode.TabIndex = 36;
            this.pilotLamp_PIDMode.Text = "pilotLamp2";
            this.pilotLamp_PIDMode.UseVisualStyleBackColor = false;
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.Location = new System.Drawing.Point(94, 88);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.ReadOnly = true;
            this.textBox_FileName.Size = new System.Drawing.Size(165, 23);
            this.textBox_FileName.TabIndex = 35;
            // 
            // button_bean
            // 
            this.button_bean.Location = new System.Drawing.Point(184, 180);
            this.button_bean.Name = "button_bean";
            this.button_bean.Size = new System.Drawing.Size(72, 23);
            this.button_bean.TabIndex = 32;
            this.button_bean.Text = "START";
            this.button_bean.UseVisualStyleBackColor = true;
            this.button_bean.Click += new System.EventHandler(this.button_bean_Click);
            // 
            // button_BrowseFile
            // 
            this.button_BrowseFile.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_BrowseFile.Location = new System.Drawing.Point(184, 116);
            this.button_BrowseFile.Name = "button_BrowseFile";
            this.button_BrowseFile.Size = new System.Drawing.Size(75, 23);
            this.button_BrowseFile.TabIndex = 31;
            this.button_BrowseFile.Text = "BROWSE";
            this.button_BrowseFile.UseVisualStyleBackColor = true;
            this.button_BrowseFile.Click += new System.EventHandler(this.button_BrowseFile_Click);
            // 
            // comboBox_Sheet
            // 
            this.comboBox_Sheet.FormattingEnabled = true;
            this.comboBox_Sheet.Location = new System.Drawing.Point(94, 148);
            this.comboBox_Sheet.Name = "comboBox_Sheet";
            this.comboBox_Sheet.Size = new System.Drawing.Size(162, 25);
            this.comboBox_Sheet.TabIndex = 30;
            this.comboBox_Sheet.SelectedIndexChanged += new System.EventHandler(this.comboBox_Sheet_SelectedIndexChanged);
            // 
            // label_Sheet
            // 
            this.label_Sheet.AutoSize = true;
            this.label_Sheet.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Sheet.Location = new System.Drawing.Point(6, 152);
            this.label_Sheet.Name = "label_Sheet";
            this.label_Sheet.Size = new System.Drawing.Size(54, 17);
            this.label_Sheet.TabIndex = 29;
            this.label_Sheet.Text = "SHEET :";
            // 
            // button_StopAutoMode
            // 
            this.button_StopAutoMode.Location = new System.Drawing.Point(200, 56);
            this.button_StopAutoMode.Name = "button_StopAutoMode";
            this.button_StopAutoMode.Size = new System.Drawing.Size(54, 23);
            this.button_StopAutoMode.TabIndex = 28;
            this.button_StopAutoMode.Text = "OFF";
            this.button_StopAutoMode.UseVisualStyleBackColor = true;
            this.button_StopAutoMode.Click += new System.EventHandler(this.button_StopAutoMode_Click);
            // 
            // label_FileName
            // 
            this.label_FileName.AutoSize = true;
            this.label_FileName.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_FileName.Location = new System.Drawing.Point(6, 91);
            this.label_FileName.Name = "label_FileName";
            this.label_FileName.Size = new System.Drawing.Size(82, 17);
            this.label_FileName.TabIndex = 17;
            this.label_FileName.Text = "FILE NAME :";
            // 
            // label_AutoMode
            // 
            this.label_AutoMode.AutoSize = true;
            this.label_AutoMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_AutoMode.Location = new System.Drawing.Point(35, 58);
            this.label_AutoMode.Name = "label_AutoMode";
            this.label_AutoMode.Size = new System.Drawing.Size(87, 17);
            this.label_AutoMode.TabIndex = 26;
            this.label_AutoMode.Text = "AUTO MODE";
            // 
            // button_StartAutoMode
            // 
            this.button_StartAutoMode.Location = new System.Drawing.Point(143, 55);
            this.button_StartAutoMode.Name = "button_StartAutoMode";
            this.button_StartAutoMode.Size = new System.Drawing.Size(54, 23);
            this.button_StartAutoMode.TabIndex = 27;
            this.button_StartAutoMode.Text = "ON";
            this.button_StartAutoMode.UseVisualStyleBackColor = true;
            this.button_StartAutoMode.Click += new System.EventHandler(this.button_StartAutoMode_Click);
            // 
            // pilotLamp_AutoMode
            // 
            this.pilotLamp_AutoMode.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_AutoMode.Location = new System.Drawing.Point(7, 55);
            this.pilotLamp_AutoMode.Name = "pilotLamp_AutoMode";
            this.pilotLamp_AutoMode.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_AutoMode.TabIndex = 25;
            this.pilotLamp_AutoMode.Text = "pilotLamp2";
            this.pilotLamp_AutoMode.UseVisualStyleBackColor = false;
            // 
            // pilotLamp_HeatingMode
            // 
            this.pilotLamp_HeatingMode.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_HeatingMode.Location = new System.Drawing.Point(7, 23);
            this.pilotLamp_HeatingMode.Name = "pilotLamp_HeatingMode";
            this.pilotLamp_HeatingMode.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_HeatingMode.TabIndex = 21;
            this.pilotLamp_HeatingMode.Text = "pilotLamp2";
            this.pilotLamp_HeatingMode.UseVisualStyleBackColor = false;
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // chart_Temperature
            // 
            this.chart_Temperature.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.chart_Temperature.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Temperature.BackImageTransparentColor = System.Drawing.SystemColors.Control;
            chartArea1.AxisX.Title = "Time (mm:ss)";
            chartArea1.AxisY.Title = "Temperature (°C)";
            chartArea1.Name = "ChartArea1";
            this.chart_Temperature.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart_Temperature.Legends.Add(legend1);
            this.chart_Temperature.Location = new System.Drawing.Point(18, 12);
            this.chart_Temperature.Name = "chart_Temperature";
            this.chart_Temperature.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.chart_Temperature.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.Crimson,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Lime};
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Bean Temperature";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.Name = "Drum Temperature";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Legend = "Legend1";
            series3.Name = "Reference Temperature";
            this.chart_Temperature.Series.Add(series1);
            this.chart_Temperature.Series.Add(series2);
            this.chart_Temperature.Series.Add(series3);
            this.chart_Temperature.Size = new System.Drawing.Size(754, 367);
            this.chart_Temperature.TabIndex = 3;
            this.chart_Temperature.Text = "chart_Temperature";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.Name = "Temperature";
            title1.Text = "Temperature";
            this.chart_Temperature.Titles.Add(title1);
            // 
            // label_BeanTemperature
            // 
            this.label_BeanTemperature.AutoSize = true;
            this.label_BeanTemperature.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label_BeanTemperature.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_BeanTemperature.ForeColor = System.Drawing.Color.Red;
            this.label_BeanTemperature.Location = new System.Drawing.Point(593, 111);
            this.label_BeanTemperature.Name = "label_BeanTemperature";
            this.label_BeanTemperature.Size = new System.Drawing.Size(133, 19);
            this.label_BeanTemperature.TabIndex = 4;
            this.label_BeanTemperature.Text = "Bean Temperature";
            // 
            // label_DrumTemperature
            // 
            this.label_DrumTemperature.AutoSize = true;
            this.label_DrumTemperature.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label_DrumTemperature.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_DrumTemperature.ForeColor = System.Drawing.Color.Blue;
            this.label_DrumTemperature.Location = new System.Drawing.Point(589, 176);
            this.label_DrumTemperature.Name = "label_DrumTemperature";
            this.label_DrumTemperature.Size = new System.Drawing.Size(137, 19);
            this.label_DrumTemperature.TabIndex = 5;
            this.label_DrumTemperature.Text = "Drum Temperature";
            // 
            // label_DataBeanTemperature
            // 
            this.label_DataBeanTemperature.AutoSize = true;
            this.label_DataBeanTemperature.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label_DataBeanTemperature.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_DataBeanTemperature.ForeColor = System.Drawing.Color.Red;
            this.label_DataBeanTemperature.Location = new System.Drawing.Point(617, 136);
            this.label_DataBeanTemperature.Name = "label_DataBeanTemperature";
            this.label_DataBeanTemperature.Size = new System.Drawing.Size(77, 31);
            this.label_DataBeanTemperature.TabIndex = 6;
            this.label_DataBeanTemperature.Text = "....ºC ";
            // 
            // label_DataDrumTemperature
            // 
            this.label_DataDrumTemperature.AutoSize = true;
            this.label_DataDrumTemperature.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label_DataDrumTemperature.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_DataDrumTemperature.ForeColor = System.Drawing.Color.Blue;
            this.label_DataDrumTemperature.Location = new System.Drawing.Point(617, 201);
            this.label_DataDrumTemperature.Name = "label_DataDrumTemperature";
            this.label_DataDrumTemperature.Size = new System.Drawing.Size(77, 31);
            this.label_DataDrumTemperature.TabIndex = 7;
            this.label_DataDrumTemperature.Text = "....ºC ";
            // 
            // dataGridView
            // 
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGridView.Location = new System.Drawing.Point(779, 50);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 62;
            this.dataGridView.Size = new System.Drawing.Size(294, 627);
            this.dataGridView.TabIndex = 8;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(779, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(294, 40);
            this.panel1.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(92, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 20);
            this.label3.TabIndex = 28;
            this.label3.Text = "Data Table";
            // 
            // label_DataReferenceTemperature
            // 
            this.label_DataReferenceTemperature.AutoSize = true;
            this.label_DataReferenceTemperature.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label_DataReferenceTemperature.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_DataReferenceTemperature.ForeColor = System.Drawing.Color.Lime;
            this.label_DataReferenceTemperature.Location = new System.Drawing.Point(617, 268);
            this.label_DataReferenceTemperature.Name = "label_DataReferenceTemperature";
            this.label_DataReferenceTemperature.Size = new System.Drawing.Size(77, 31);
            this.label_DataReferenceTemperature.TabIndex = 11;
            this.label_DataReferenceTemperature.Text = "....ºC ";
            // 
            // label_ReferenceTemperature
            // 
            this.label_ReferenceTemperature.AutoSize = true;
            this.label_ReferenceTemperature.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label_ReferenceTemperature.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ReferenceTemperature.ForeColor = System.Drawing.Color.Lime;
            this.label_ReferenceTemperature.Location = new System.Drawing.Point(581, 244);
            this.label_ReferenceTemperature.Name = "label_ReferenceTemperature";
            this.label_ReferenceTemperature.Size = new System.Drawing.Size(167, 19);
            this.label_ReferenceTemperature.TabIndex = 10;
            this.label_ReferenceTemperature.Text = "Reference Temperature";
            // 
            // label_Burner
            // 
            this.label_Burner.AutoSize = true;
            this.label_Burner.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Burner.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_Burner.Location = new System.Drawing.Point(293, 401);
            this.label_Burner.Name = "label_Burner";
            this.label_Burner.Size = new System.Drawing.Size(65, 17);
            this.label_Burner.TabIndex = 1;
            this.label_Burner.Text = "BURNER :";
            // 
            // button_ONBurner
            // 
            this.button_ONBurner.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_ONBurner.Location = new System.Drawing.Point(417, 398);
            this.button_ONBurner.Name = "button_ONBurner";
            this.button_ONBurner.Size = new System.Drawing.Size(40, 23);
            this.button_ONBurner.TabIndex = 2;
            this.button_ONBurner.Text = "ON";
            this.button_ONBurner.UseVisualStyleBackColor = true;
            this.button_ONBurner.Click += new System.EventHandler(this.button_ONBurner_Click);
            // 
            // button_OFFBurner
            // 
            this.button_OFFBurner.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_OFFBurner.Location = new System.Drawing.Point(461, 398);
            this.button_OFFBurner.Name = "button_OFFBurner";
            this.button_OFFBurner.Size = new System.Drawing.Size(40, 23);
            this.button_OFFBurner.TabIndex = 3;
            this.button_OFFBurner.Text = "OFF";
            this.button_OFFBurner.UseVisualStyleBackColor = true;
            this.button_OFFBurner.Click += new System.EventHandler(this.button_OFFBurner_Click);
            // 
            // label_MotorDrum
            // 
            this.label_MotorDrum.AutoSize = true;
            this.label_MotorDrum.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_MotorDrum.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_MotorDrum.Location = new System.Drawing.Point(293, 432);
            this.label_MotorDrum.Name = "label_MotorDrum";
            this.label_MotorDrum.Size = new System.Drawing.Size(106, 17);
            this.label_MotorDrum.TabIndex = 5;
            this.label_MotorDrum.Text = "MOTOR DRUM :";
            // 
            // button_ONMotorDrum
            // 
            this.button_ONMotorDrum.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_ONMotorDrum.Location = new System.Drawing.Point(417, 429);
            this.button_ONMotorDrum.Name = "button_ONMotorDrum";
            this.button_ONMotorDrum.Size = new System.Drawing.Size(40, 23);
            this.button_ONMotorDrum.TabIndex = 6;
            this.button_ONMotorDrum.Text = "ON";
            this.button_ONMotorDrum.UseVisualStyleBackColor = true;
            this.button_ONMotorDrum.Click += new System.EventHandler(this.button_ONMotorDrum_Click);
            // 
            // button_OFFMotorDrum
            // 
            this.button_OFFMotorDrum.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_OFFMotorDrum.Location = new System.Drawing.Point(461, 429);
            this.button_OFFMotorDrum.Name = "button_OFFMotorDrum";
            this.button_OFFMotorDrum.Size = new System.Drawing.Size(40, 23);
            this.button_OFFMotorDrum.TabIndex = 7;
            this.button_OFFMotorDrum.Text = "OFF";
            this.button_OFFMotorDrum.UseVisualStyleBackColor = true;
            this.button_OFFMotorDrum.Click += new System.EventHandler(this.button_OFFMotorDrum_Click);
            // 
            // label_BlowerDrum
            // 
            this.label_BlowerDrum.AutoSize = true;
            this.label_BlowerDrum.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_BlowerDrum.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_BlowerDrum.Location = new System.Drawing.Point(293, 463);
            this.label_BlowerDrum.Name = "label_BlowerDrum";
            this.label_BlowerDrum.Size = new System.Drawing.Size(111, 17);
            this.label_BlowerDrum.TabIndex = 9;
            this.label_BlowerDrum.Text = "BLOWER DRUM :";
            // 
            // button_ONBlowerDrum
            // 
            this.button_ONBlowerDrum.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_ONBlowerDrum.Location = new System.Drawing.Point(417, 460);
            this.button_ONBlowerDrum.Name = "button_ONBlowerDrum";
            this.button_ONBlowerDrum.Size = new System.Drawing.Size(40, 23);
            this.button_ONBlowerDrum.TabIndex = 10;
            this.button_ONBlowerDrum.Text = "ON";
            this.button_ONBlowerDrum.UseVisualStyleBackColor = true;
            this.button_ONBlowerDrum.Click += new System.EventHandler(this.button_ONBlowerDrum_Click);
            // 
            // button_OFFBlowerDrum
            // 
            this.button_OFFBlowerDrum.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_OFFBlowerDrum.Location = new System.Drawing.Point(461, 460);
            this.button_OFFBlowerDrum.Name = "button_OFFBlowerDrum";
            this.button_OFFBlowerDrum.Size = new System.Drawing.Size(40, 23);
            this.button_OFFBlowerDrum.TabIndex = 11;
            this.button_OFFBlowerDrum.Text = "OFF";
            this.button_OFFBlowerDrum.UseVisualStyleBackColor = true;
            this.button_OFFBlowerDrum.Click += new System.EventHandler(this.button_OFFBlowerDrum_Click);
            // 
            // label_Cooler
            // 
            this.label_Cooler.AutoSize = true;
            this.label_Cooler.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Cooler.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_Cooler.Location = new System.Drawing.Point(293, 494);
            this.label_Cooler.Name = "label_Cooler";
            this.label_Cooler.Size = new System.Drawing.Size(65, 17);
            this.label_Cooler.TabIndex = 13;
            this.label_Cooler.Text = "COOLER :";
            // 
            // button_ONCooler
            // 
            this.button_ONCooler.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_ONCooler.Location = new System.Drawing.Point(417, 491);
            this.button_ONCooler.Name = "button_ONCooler";
            this.button_ONCooler.Size = new System.Drawing.Size(40, 23);
            this.button_ONCooler.TabIndex = 14;
            this.button_ONCooler.Text = "ON";
            this.button_ONCooler.UseVisualStyleBackColor = true;
            this.button_ONCooler.Click += new System.EventHandler(this.button_ONCooler_Click);
            // 
            // button_OFFCooler
            // 
            this.button_OFFCooler.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_OFFCooler.Location = new System.Drawing.Point(461, 491);
            this.button_OFFCooler.Name = "button_OFFCooler";
            this.button_OFFCooler.Size = new System.Drawing.Size(40, 23);
            this.button_OFFCooler.TabIndex = 15;
            this.button_OFFCooler.Text = "OFF";
            this.button_OFFCooler.UseVisualStyleBackColor = true;
            this.button_OFFCooler.Click += new System.EventHandler(this.button_OFFCooler_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(292, 526);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 17);
            this.label1.TabIndex = 36;
            this.label1.Text = "MOTOR VALVE :";
            // 
            // button_OFFMotorValve
            // 
            this.button_OFFMotorValve.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_OFFMotorValve.Location = new System.Drawing.Point(417, 551);
            this.button_OFFMotorValve.Name = "button_OFFMotorValve";
            this.button_OFFMotorValve.Size = new System.Drawing.Size(84, 23);
            this.button_OFFMotorValve.TabIndex = 38;
            this.button_OFFMotorValve.Text = "CLOSE";
            this.button_OFFMotorValve.UseVisualStyleBackColor = true;
            this.button_OFFMotorValve.Click += new System.EventHandler(this.button_OFFMotorValve_Click);
            // 
            // button_ONMotorValve
            // 
            this.button_ONMotorValve.ForeColor = System.Drawing.Color.RoyalBlue;
            this.button_ONMotorValve.Location = new System.Drawing.Point(417, 522);
            this.button_ONMotorValve.Name = "button_ONMotorValve";
            this.button_ONMotorValve.Size = new System.Drawing.Size(84, 23);
            this.button_ONMotorValve.TabIndex = 37;
            this.button_ONMotorValve.Text = "OPEN";
            this.button_ONMotorValve.UseVisualStyleBackColor = true;
            this.button_ONMotorValve.Click += new System.EventHandler(this.button_ONMotorValve_Click);
            // 
            // pilotLampMotorValve
            // 
            this.pilotLampMotorValve.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLampMotorValve.Location = new System.Drawing.Point(255, 522);
            this.pilotLampMotorValve.Name = "pilotLampMotorValve";
            this.pilotLampMotorValve.Size = new System.Drawing.Size(23, 23);
            this.pilotLampMotorValve.TabIndex = 35;
            this.pilotLampMotorValve.Text = "pilotLampMotorValve";
            this.pilotLampMotorValve.UseVisualStyleBackColor = false;
            // 
            // pilotLamp_Burner
            // 
            this.pilotLamp_Burner.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_Burner.Location = new System.Drawing.Point(255, 397);
            this.pilotLamp_Burner.Name = "pilotLamp_Burner";
            this.pilotLamp_Burner.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_Burner.TabIndex = 25;
            this.pilotLamp_Burner.Text = "pilotLamp_Burner";
            this.pilotLamp_Burner.UseVisualStyleBackColor = false;
            // 
            // pilotLamp_RecordData
            // 
            this.pilotLamp_RecordData.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_RecordData.Location = new System.Drawing.Point(255, 567);
            this.pilotLamp_RecordData.Name = "pilotLamp_RecordData";
            this.pilotLamp_RecordData.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_RecordData.TabIndex = 26;
            this.pilotLamp_RecordData.Text = "pilotLamp_RecordData";
            this.pilotLamp_RecordData.UseVisualStyleBackColor = false;
            // 
            // pilotLamp_Cooler
            // 
            this.pilotLamp_Cooler.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_Cooler.Location = new System.Drawing.Point(255, 491);
            this.pilotLamp_Cooler.Name = "pilotLamp_Cooler";
            this.pilotLamp_Cooler.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_Cooler.TabIndex = 12;
            this.pilotLamp_Cooler.Text = "pilotLamp_Cooler";
            this.pilotLamp_Cooler.UseVisualStyleBackColor = false;
            // 
            // pilotLamp_BlowerDrum
            // 
            this.pilotLamp_BlowerDrum.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_BlowerDrum.Location = new System.Drawing.Point(255, 460);
            this.pilotLamp_BlowerDrum.Name = "pilotLamp_BlowerDrum";
            this.pilotLamp_BlowerDrum.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_BlowerDrum.TabIndex = 8;
            this.pilotLamp_BlowerDrum.Text = "pilotLamp_BlowerDrum";
            this.pilotLamp_BlowerDrum.UseVisualStyleBackColor = false;
            // 
            // pilotLamp_MotorDrum
            // 
            this.pilotLamp_MotorDrum.BackColor = System.Drawing.Color.DarkGreen;
            this.pilotLamp_MotorDrum.Location = new System.Drawing.Point(255, 429);
            this.pilotLamp_MotorDrum.Name = "pilotLamp_MotorDrum";
            this.pilotLamp_MotorDrum.Size = new System.Drawing.Size(23, 23);
            this.pilotLamp_MotorDrum.TabIndex = 4;
            this.pilotLamp_MotorDrum.Text = "pilotLamp_MotorDrum";
            this.pilotLamp_MotorDrum.UseVisualStyleBackColor = false;
            // 
            // Form_CoffeeRoasto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1085, 702);
            this.Controls.Add(this.button_OFFMotorValve);
            this.Controls.Add(this.button_ONMotorValve);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pilotLampMotorValve);
            this.Controls.Add(this.pilotLamp_Burner);
            this.Controls.Add(this.textBox_InputTemp);
            this.Controls.Add(this.button_OFFCooler);
            this.Controls.Add(this.label_DataReferenceTemperature);
            this.Controls.Add(this.button_ONCooler);
            this.Controls.Add(this.pilotLamp_RecordData);
            this.Controls.Add(this.label_Cooler);
            this.Controls.Add(this.button_SENDInputTemperature);
            this.Controls.Add(this.pilotLamp_Cooler);
            this.Controls.Add(this.label_ReferenceTemperature);
            this.Controls.Add(this.button_OFFBlowerDrum);
            this.Controls.Add(this.label_DerajatCellcius);
            this.Controls.Add(this.button_ONBlowerDrum);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label_BlowerDrum);
            this.Controls.Add(this.pilotLamp_BlowerDrum);
            this.Controls.Add(this.label_InputTemperature);
            this.Controls.Add(this.button_OFFMotorDrum);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.button_ONMotorDrum);
            this.Controls.Add(this.button_SaveRecordData);
            this.Controls.Add(this.label_MotorDrum);
            this.Controls.Add(this.label_DataDrumTemperature);
            this.Controls.Add(this.pilotLamp_MotorDrum);
            this.Controls.Add(this.label_DataBeanTemperature);
            this.Controls.Add(this.button_OFFBurner);
            this.Controls.Add(this.button_StopRecordData);
            this.Controls.Add(this.button_ONBurner);
            this.Controls.Add(this.label_DrumTemperature);
            this.Controls.Add(this.label_Burner);
            this.Controls.Add(this.label_BeanTemperature);
            this.Controls.Add(this.button_StartRecordData);
            this.Controls.Add(this.chart_Temperature);
            this.Controls.Add(this.groupBox_AutomaticControl);
            this.Controls.Add(this.label_RecordData);
            this.Controls.Add(this.groupBox_PanelConnection);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_CoffeeRoasto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Coffee Roasto";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_CoffeeRoasto_FormClosing);
            this.Load += new System.EventHandler(this.Form_CoffeeRoasto_Load);
            this.groupBox_PanelConnection.ResumeLayout(false);
            this.groupBox_PanelConnection.PerformLayout();
            this.groupBox_AutomaticControl.ResumeLayout(false);
            this.groupBox_AutomaticControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Temperature)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_PanelConnection;
        private System.Windows.Forms.ProgressBar progressBar_PanelConnection;
        private System.Windows.Forms.Button button_Disconnect;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.ComboBox comboBox_ComPort;
        private System.Windows.Forms.Label label_ComPort;
        private System.Windows.Forms.Button button_SaveRecordData;
        private System.Windows.Forms.Button button_StopRecordData;
        private System.Windows.Forms.Button button_StartRecordData;
        private System.Windows.Forms.Label label_RecordData;
        private System.Windows.Forms.Button button_SENDInputTemperature;
        private System.Windows.Forms.Label label_DerajatCellcius;
        private System.Windows.Forms.Label label_InputTemperature;
        private System.Windows.Forms.GroupBox groupBox_AutomaticControl;
        private System.Windows.Forms.Button button_StopAutoMode;
        private System.Windows.Forms.Button button_StartAutoMode;
        private System.Windows.Forms.Label label_AutoMode;
        private UI.PilotLamp pilotLamp_AutoMode;
        private System.Windows.Forms.Button button_StopHeatingMode;
        private System.Windows.Forms.Button button_StartHeatingMode;
        private System.Windows.Forms.Label label_HeatingMode;
        private UI.PilotLamp pilotLamp_HeatingMode;
        private System.Windows.Forms.Label label_FileName;
        private UI.PilotLamp pilotLamp_RecordData;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Temperature;
        private System.Windows.Forms.Label label_BeanTemperature;
        private System.Windows.Forms.Label label_DrumTemperature;
        private System.Windows.Forms.Label label_DataBeanTemperature;
        private System.Windows.Forms.Label label_DataDrumTemperature;
        private System.Windows.Forms.ComboBox comboBox_Sheet;
        private System.Windows.Forms.Label label_Sheet;
        private System.Windows.Forms.Button button_BrowseFile;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button button_bean;
        private System.Windows.Forms.TextBox textBox_InputTemp;
        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_DataReferenceTemperature;
        private System.Windows.Forms.Label label_ReferenceTemperature;
        private System.Windows.Forms.Label label_Burner;
        private System.Windows.Forms.Button button_ONBurner;
        private System.Windows.Forms.Button button_OFFBurner;
        private UI.PilotLamp pilotLamp_MotorDrum;
        private System.Windows.Forms.Label label_MotorDrum;
        private System.Windows.Forms.Button button_ONMotorDrum;
        private System.Windows.Forms.Button button_OFFMotorDrum;
        private UI.PilotLamp pilotLamp_BlowerDrum;
        private System.Windows.Forms.Label label_BlowerDrum;
        private System.Windows.Forms.Button button_ONBlowerDrum;
        private System.Windows.Forms.Button button_OFFBlowerDrum;
        private UI.PilotLamp pilotLamp_Cooler;
        private System.Windows.Forms.Label label_Cooler;
        private System.Windows.Forms.Button button_ONCooler;
        private System.Windows.Forms.Button button_OFFCooler;
        private UI.PilotLamp pilotLamp_Burner;
        private System.Windows.Forms.DataGridView dataGridView;
        private UI.PilotLamp pilotLampMotorValve;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_OFFMotorValve;
        private System.Windows.Forms.Button button_ONMotorValve;
        private System.Windows.Forms.Button button_StopPID;
        private System.Windows.Forms.Label label_PID;
        private System.Windows.Forms.Button button_StartPID;
        private UI.PilotLamp pilotLamp_PIDMode;
    }
}

