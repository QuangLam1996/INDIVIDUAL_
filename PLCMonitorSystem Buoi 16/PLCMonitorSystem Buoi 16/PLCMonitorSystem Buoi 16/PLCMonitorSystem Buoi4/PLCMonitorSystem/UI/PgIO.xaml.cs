using PLCMonitorSystem.LIB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ActUtlTypeLib;
using System.Timers;
using System.Threading;

namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgIO.xaml
    /// </summary>
    public partial class PgIO : Page
    {
        private static MyLogger logger = new MyLogger("PgIO");

        private String TITLE_INPUT = "MODULE TYPE: INPUT 32";
        private String TITLE_OUTPUT = "MODULE TYPE: OUTPUT 32";

        private Brush BT_ACTIVE_BACKGROUND = Brushes.SkyBlue;
        private Brush BT_INACTIVE_BACKGROUND = new SolidColorBrush(Color.FromArgb(255, (byte)0xd5, (byte)0xd5, (byte)0xd5));
        private Brush RECT_ACTIVE_FILL = Brushes.LimeGreen;
        private Brush RECT_INACTIVE_FILL = Brushes.DarkGray;

        private const int ROW_CNT = 17;

        private IoPort[] inputPortList = {
            new IoPort(0x00, "Nut bam Start"),
            new IoPort(0x01, "Nut bam Stop"),
            new IoPort(0x02, "X Axis Upper limit"),
            new IoPort(0x03, "X Axis Alarm"),
            new IoPort(0x04, "X Axis Ready"),
            new IoPort(0x05, "Spare"),
            new IoPort(0x06, "Door sensor 1"),
            new IoPort(0x07, "Door sensor 2"),

            new IoPort(0x08, "Cylinder 1 FW"),
            new IoPort(0x09, "Cylinder 1 BW"),
            new IoPort(0x0A, "Cylinder 2 FW"),
            new IoPort(0x0B, "Cylinder 2 BW"),
            new IoPort(0x0C, "Y Axis Ready"),
            new IoPort(0x0D, "Spare"),
            new IoPort(0x0E, "Door sensor 3"),
            new IoPort(0x0F, "Door sensor 4"),

            new IoPort(0x10, "Input stacker up jig sensor"),
            new IoPort(0x11, "Input jig detect sensor"),
            new IoPort(0x12, "Middle jig detect sensor"),
            new IoPort(0x13, "Out jig detect sensor"),
            new IoPort(0x14, "Input jig detect sensor <Return jig>"),
            new IoPort(0x15, "Stopper jig detect sensor <Return jig>"),
            new IoPort(0x16, "Spare"),
            new IoPort(0x17, "Spare"),

            new IoPort(0x18, "Emergency 1"),
            new IoPort(0x19, "Emergency 2"),
            new IoPort(0x1A, "Spare"),
            new IoPort(0x1B, "Spare"),
            new IoPort(0x1C, "LS Lower Cylinder Up/Dowm Jig"),
            new IoPort(0x1D, "LS Upper Cylinder Up/Dowm Jig"),
            new IoPort(0x1E, "LS Lower Cylinder midder stop"),
            new IoPort(0x1F, "LS Upper Cylinder midder stop"),
        };

        private IoPort[] outputPortList = {
            new IoPort(0x20, "X Axis Pulse"),
            new IoPort(0x21, "Y Axis Pulse"),
            new IoPort(0x22, "X Axis Dir"),
            new IoPort(0x23, "Y Axis Dir"),
            new IoPort(0x24, "X Axis Reset"),
            new IoPort(0x25, "Y Axis Reset"),
            new IoPort(0x26, "X Axis On"),
            new IoPort(0x27, "Y Axis On"),

            new IoPort(0x28, "Tower Green lamp"),
            new IoPort(0x29, "Tower Yellow lamp"),
            new IoPort(0x2A, "Tower Red lamp"),
            new IoPort(0x2B, "Handle Conveyor"),
            new IoPort(0x2C, "Handle Conveyor Return"),
            new IoPort(0x2D, "Solenoi Cylinder LOad stack Jig"),
            new IoPort(0x2E, "Solenoi Cylinder middle stop"),
            new IoPort(0x2F, "Buzzer"),

            new IoPort(0x30, "Spare"),
            new IoPort(0x31, "Spare"),
            new IoPort(0x32, "Spare"),
            new IoPort(0x33, "Spare"),
            new IoPort(0x34, "Spare"),
            new IoPort(0x35, "Spare"),
            new IoPort(0x36, "Spare"),
            new IoPort(0x37, "Spare"),

            new IoPort(0x38, "Spare"),
            new IoPort(0x39, "Spare"),
            new IoPort(0x3a, "Spare"),
            new IoPort(0x3b, "Spare"),
            new IoPort(0x3c, "Spare"),
            new IoPort(0x3d, "Spare"),
            new IoPort(0x3e, "Spare"),
            new IoPort(0x3f, "Spare"),
        };

        private bool isInputActive = true;

        private System.Timers.Timer timer = new System.Timers.Timer(500);

        private ActUtlType plcComm;

        public PgIO()
        {
            InitializeComponent();

            this.Loaded += this.PgIO_Loaded;
            this.Unloaded += this.PgIO_Unloaded;

            this.btInput.Click += this.BtInput_Click;
            this.btOutput.Click += this.BtOutput_Click;

            this.timer.AutoReset = true;
            this.timer.Elapsed += this.Timer_Elapsed;
        }

        private void BtInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isInputActive)//==false : Đang hiển thị out, nếu như đang hiển thị input thì thôi
                {
                    generateCells(inputPortList);
                    isInputActive = true;
                }
                updateUI();
                updateStatus();
            }
            catch (Exception ex)
            {
                logger.CreateLog("BtInput_Click error:" + ex.Message);
            }
        }

        private void BtOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isInputActive)
                {
                    generateCells(outputPortList);
                    isInputActive = false;
                }
                updateUI();
                updateStatus();
            }
            catch (Exception ex)
            {
                logger.CreateLog("BtOutput_Click error:" + ex.Message);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                updateStatus();
            }
            catch (Exception ex)
            {
                logger.CreateLog("Timer_Elapsed error:" + ex.Message);
            }
        }

        private void PgIO_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                plcComm = new ActUtlType();
                plcComm.ActLogicalStationNumber = 1;
                for (int i = 0; i < 2; i++)
                {
                    if (plcComm.Open()==0)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                }

                updateUI();
                generateCells(inputPortList);

                this.timer.Start();
            }
            catch (Exception ex)
            {
                logger.CreateLog("PgIO_Loaded error:" + ex.Message);
            }
        }

        private void PgIO_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.timer != null)
            {
                logger.CreateLog("Stop timer!");
                this.timer.Stop();
            }

            if (plcComm != null)
            {
                logger.CreateLog("Stop PlcComm!");
                plcComm.Close();
            }
        }

        private void updateUI()
        {
            if (isInputActive)
            {
                this.lblTitle.Content = TITLE_INPUT;
                this.btInput.Background = BT_ACTIVE_BACKGROUND;
                this.rectInput.Fill = RECT_ACTIVE_FILL;
                this.btOutput.Background = BT_INACTIVE_BACKGROUND;
                this.rectOutput.Fill = RECT_INACTIVE_FILL;
            }
            else
            {
                this.lblTitle.Content = TITLE_OUTPUT;
                this.btInput.Background = BT_INACTIVE_BACKGROUND;
                this.rectInput.Fill = RECT_INACTIVE_FILL;
                this.btOutput.Background = BT_ACTIVE_BACKGROUND;
                this.rectOutput.Fill = RECT_ACTIVE_FILL;
            }
        }

        private void updateStatus()// Đọc dữ liệu từ PLC
        {
            // Select address & result holder:
            var portList = inputPortList;
            string startAddress = "X00";
            if (!isInputActive)
            {
                portList = outputPortList;
                startAddress = "Y20";
            }
            // Read: Đọc dữ liệu từ PLC
            int bits = 0;
            plcComm.ReadDeviceBlock(startAddress, 1, out bits);// Trả về một số nguyên 32 bit
                                                               // Đọc giá trị của 1 thanh ghi bao gồm 32 bit từ bit X00 - X1F:
            for (int i = 0; i < 32; i++)
            {
                int a = bits >> i; 
                int b = a & 1;
                var port = portList[i];
                if (b==0)
                {
                    port.Status = "OFF";
                }
                else
                {
                    port.Status = "ON";
                }
                port.UpdateUI();
            }
        }

        private void generateCells(IoPort[] portList)
        {
            this.gridIO0015.Children.Clear();
            this.gridIO0015.RowDefinitions.Clear();
            for (int r = 0; r < ROW_CNT; r++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                gridIO0015.RowDefinitions.Add(rowDef);
            }
            addHeader(gridIO0015);

            this.gridIO1631.Children.Clear();
            this.gridIO1631.RowDefinitions.Clear();
            for (int r = 0; r < ROW_CNT; r++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                gridIO1631.RowDefinitions.Add(rowDef);
            }
            addHeader(gridIO1631);

            for (int i = 0; i < portList.Length; i++)
            {
                if (i < 16)
                {
                    addPort(gridIO0015, i + 1, portList[i]);
                }
                else if (i < 32)
                {
                    addPort(gridIO1631, i + 1 - 16, portList[i]);
                }
            }
        }

        private void addHeader(Grid grid)
        {
            var cell = new Label();
            cell.Content = "Port";
            cell.Background = Brushes.DarkBlue;
            cell.Foreground = Brushes.White;
            grid.Children.Add(cell);
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, 0);

            cell = new Label();
            cell.Content = "I/O Name";
            cell.Background = Brushes.DarkBlue;
            cell.Foreground = Brushes.White;
            grid.Children.Add(cell);
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, 1);

            cell = new Label();
            cell.Content = "Status";
            cell.Background = Brushes.DarkBlue;
            cell.Foreground = Brushes.White;
            grid.Children.Add(cell);
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, 2);
        }

        private void addPort(Grid grid, int rowIndex, IoPort port)
        {
            var cell = new Label();
            cell.Content = port.PortId;
            cell.Background = Brushes.LightBlue;
            grid.Children.Add(cell);
            Grid.SetRow(cell, rowIndex);
            Grid.SetColumn(cell, 0);

            cell = new Label();
            cell.HorizontalContentAlignment = HorizontalAlignment.Left;
            cell.Content = port.Name;
            grid.Children.Add(cell);
            Grid.SetRow(cell, rowIndex);
            Grid.SetColumn(cell, 1);

            cell = new Label();
            cell.Foreground = Brushes.White;
            grid.Children.Add(cell);
            Grid.SetRow(cell, rowIndex);
            Grid.SetColumn(cell, 2);
            bindCell(port, cell);
        }

        private void bindCell(IoPort port, Label cell)
        {
            var b1 = new Binding("Status");
            b1.Source = port;
            b1.Mode = BindingMode.OneWay;
            cell.SetBinding(Label.ContentProperty, b1);

            var b2 = new Binding("StatusColor");
            b2.Source = port;
            b2.Mode = BindingMode.OneWay;
            cell.SetBinding(Label.BackgroundProperty, b2);
        }
    }
    class IoPort : INotifyPropertyChanged
    {
        private Brush ON_COLOR = Brushes.GreenYellow;
        private Brush OFF_COLOR = Brushes.DarkGreen;

        public string PortId { get; set; }
        public UInt16 PortAddr { get; set; }
        public string Name { get; set; }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                if (_status != null & _status.Equals("ON"))
                {
                    this.StatusColor = ON_COLOR;
                }
                else
                {
                    this.StatusColor = OFF_COLOR;
                }
            }
        }

        public Brush StatusColor { get; private set; }

        public IoPort() { }

        public IoPort(UInt16 portAddr, String name)
        {
            this.PortAddr = portAddr;
            if (portAddr < 0x20)
            {
                this.PortId = String.Format("X{0}", portAddr.ToString("X"));
            }
            else
            {
                this.PortId = String.Format("Y{0}", portAddr.ToString("X"));
            }
            this.Name = name;
            this.Status = "OFF";
            this.StatusColor = Brushes.Brown;
        }

        public void UpdateUI()
        {
            OnPropertyChanged("PortId");
            OnPropertyChanged("Name");
            OnPropertyChanged("Status");
            OnPropertyChanged("StatusColor");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}
