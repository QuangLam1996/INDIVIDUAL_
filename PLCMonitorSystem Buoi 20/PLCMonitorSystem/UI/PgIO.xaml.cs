using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PLCMonitorSystem.UI
{
    /// <summary>
    /// Interaction logic for PgIO.xaml
    /// </summary>
    public partial class PgIO : Page
    {
        MyLogger logger = new MyLogger("Pg.IO");
        Sock_SLMP sMLP = new Sock_SLMP();

        private String TITLE_INPUT = "MODULE TYPE: INPUT 32";
        private String TITLE_OUTPUT = "MODULE TYPE: OUTPUT 32";

        private Brush BT_ACTIVE_BACKGROUND = Brushes.SkyBlue;
        private Brush BT_INACTIVE_BACKGROUND = new SolidColorBrush(Color.FromArgb(255, (byte)0xd5, (byte)0xd5, (byte)0xd5));
        private Brush RECT_ACTIVE_FILL = Brushes.LimeGreen;
        private Brush RECT_INACTIVE_FILL = Brushes.DarkGray;

        private const int ROW_CNT = 17;

        private IOPort[] inputPortList = {
            new IOPort(0x00, "Nut bam Start"),
            new IOPort(0x01, "Nut bam Stop"),
            new IOPort(0x02, "X Axis Upper limit"),
            new IOPort(0x03, "X Axis Alarm"),
            new IOPort(0x04, "X Axis Ready"),
            new IOPort(0x05, "Spare"),
            new IOPort(0x06, "Door sensor 1"),
            new IOPort(0x07, "Door sensor 2"),

            new IOPort(0x08, "Cylinder 1 FW"),
            new IOPort(0x09, "Cylinder 1 BW"),
            new IOPort(0x0A, "Cylinder 2 FW"),
            new IOPort(0x0B, "Cylinder 2 BW"),
            new IOPort(0x0C, "Y Axis Ready"),
            new IOPort(0x0D, "Spare"),
            new IOPort(0x0E, "Door sensor 3"),
            new IOPort(0x0F, "Door sensor 4"),

            new IOPort(0x10, "Input stacker up jig sensor"),
            new IOPort(0x11, "Input jig detect sensor"),
            new IOPort(0x12, "Middle jig detect sensor"),
            new IOPort(0x13, "Out jig detect sensor"),
            new IOPort(0x14, "Input jig detect sensor <Return jig>"),
            new IOPort(0x15, "Stopper jig detect sensor <Return jig>"),
            new IOPort(0x16, "Spare"),
            new IOPort(0x17, "Spare"),

            new IOPort(0x18, "Emergency 1"),
            new IOPort(0x19, "Emergency 2"),
            new IOPort(0x1A, "Spare"),
            new IOPort(0x1B, "Spare"),
            new IOPort(0x1C, "LS Lower Cylinder Up/Dowm Jig"),
            new IOPort(0x1D, "LS Upper Cylinder Up/Dowm Jig"),
            new IOPort(0x1E, "LS Lower Cylinder midder stop"),
            new IOPort(0x1F, "LS Upper Cylinder midder stop"),
        };

        private IOPort[] outputPortList = {
            new IOPort(0x20, "X Axis Pulse"),
            new IOPort(0x21, "Y Axis Pulse"),
            new IOPort(0x22, "X Axis Dir"),
            new IOPort(0x23, "Y Axis Dir"),
            new IOPort(0x24, "X Axis Reset"),
            new IOPort(0x25, "Y Axis Reset"),
            new IOPort(0x26, "X Axis On"),
            new IOPort(0x27, "Y Axis On"),

            new IOPort(0x28, "Tower Green lamp"),
            new IOPort(0x29, "Tower Yellow lamp"),
            new IOPort(0x2A, "Tower Red lamp"),
            new IOPort(0x2B, "Handle Conveyor"),
            new IOPort(0x2C, "Handle Conveyor Return"),
            new IOPort(0x2D, "Solenoi Cylinder LOad stack Jig"),
            new IOPort(0x2E, "Solenoi Cylinder middle stop"),
            new IOPort(0x2F, "Buzzer"),

            new IOPort(0x30, "Spare"),
            new IOPort(0x31, "Spare"),
            new IOPort(0x32, "Spare"),
            new IOPort(0x33, "Spare"),
            new IOPort(0x34, "Spare"),
            new IOPort(0x35, "Spare"),
            new IOPort(0x36, "Spare"),
            new IOPort(0x37, "Spare"),

            new IOPort(0x38, "Spare"),
            new IOPort(0x39, "Spare"),
            new IOPort(0x3a, "Spare"),
            new IOPort(0x3b, "Spare"),
            new IOPort(0x3c, "Spare"),
            new IOPort(0x3d, "Spare"),
            new IOPort(0x3e, "Spare"),
            new IOPort(0x3f, "Spare"),
        };

        private bool isInputActive = true;

        private System.Timers.Timer timer = new System.Timers.Timer(100);


        public PgIO()
        {
            InitializeComponent();

            this.Loaded += this.PgIO_Loaded;
            this.Unloaded += this.PgIO_Unloaded;

            this.btnInputIO.Click += this.BtInput_Click;
            this.btnOutputIO.Click += this.BtOutput_Click;

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
                for (int i = 0; i < 2; i++)
                {
                    if (sMLP.Connect() == 0)
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
            logger.CreateLog("Stop timer!");
            this.timer.Stop();
            logger.CreateLog("Stop PlcComm!");
            sMLP.Disconnect();
        }

        private void updateUI()
        {
            if (isInputActive)
            {
                this.lblTitle.Content = TITLE_INPUT;
                this.btnInputIO.Background = BT_ACTIVE_BACKGROUND;
                this.rtgInputIO.Fill = RECT_ACTIVE_FILL;
                this.btnOutputIO.Background = BT_INACTIVE_BACKGROUND;
                this.rtgOutputIO.Fill = RECT_INACTIVE_FILL;
            }
            else
            {
                this.lblTitle.Content = TITLE_OUTPUT;
                this.btnInputIO.Background = BT_INACTIVE_BACKGROUND;
                this.rtgInputIO.Fill = RECT_INACTIVE_FILL;
                this.btnOutputIO.Background = BT_ACTIVE_BACKGROUND;
                this.rtgOutputIO.Fill = RECT_ACTIVE_FILL;
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
            // Trả về một số nguyên 32 bit
            // Đọc giá trị của 1 thanh ghi bao gồm 32 bit từ bit X00 - X1F:
            bits = sMLP.ReadDWord(Device.D, 100);

            for (int i = 0; i < 32; i++)
            {
                int a = bits >> i;
                int b = a & 1;
                var port = portList[i];
                if (b == 0)
                {
                    port.State = "OFF";
                }
                else
                {
                    port.State = "ON";
                }
                port.UpdateUI();
            }
        }

        private void generateCells(IOPort[] portList)
        {
            this.gridIN1.Children.Clear();
            this.gridIN1.RowDefinitions.Clear();
            for (int r = 0; r < ROW_CNT; r++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                gridIN1.RowDefinitions.Add(rowDef);
            }
            addHeader(gridIN1);

            this.gridIN2.Children.Clear();
            this.gridIN2.RowDefinitions.Clear();
            for (int r = 0; r < ROW_CNT; r++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                gridIN2.RowDefinitions.Add(rowDef);
            }
            addHeader(gridIN2);

            for (int i = 0; i < portList.Length; i++)
            {
                if (i < 16)
                {
                    addPort(gridIN1, i + 1, portList[i]);
                }
                else if (i < 32)
                {
                    addPort(gridIN2, i + 1 - 16, portList[i]);
                }
            }
        }

        private void addHeader(Grid grid)
        {
            var cell = new Label();
            cell.Content = "Device";
            cell.Background = Brushes.DarkBlue;
            cell.Foreground = Brushes.White;
            grid.Children.Add(cell);
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, 0);

            cell = new Label();
            cell.Content = "Name";
            cell.Background = Brushes.DarkBlue;
            cell.Foreground = Brushes.White;
            grid.Children.Add(cell);
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, 1);

            cell = new Label();
            cell.Content = "State";
            cell.Background = Brushes.DarkBlue;
            cell.Foreground = Brushes.White;
            grid.Children.Add(cell);
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, 2);
        }

        private void addPort(Grid grid, int rowIndex, IOPort port)
        {
            var cell = new Label();
            cell.Content = port.PortID;
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

        private void bindCell(IOPort port, Label cell)
        {
            var b1 = new Binding("State");
            b1.Source = port;
            b1.Mode = BindingMode.OneWay;
            cell.SetBinding(Label.ContentProperty, b1);

            var b2 = new Binding("StatusColor");
            b2.Source = port;
            b2.Mode = BindingMode.OneWay;
            cell.SetBinding(Label.BackgroundProperty, b2);
        }
    }
    public class IOPort : INotifyPropertyChanged
    {
        #region Property:
        private Brush STATE_ON = Brushes.LightGreen;
        private Brush STATE_OFF = Brushes.OrangeRed;
        private string portID; //
        private UInt16 portAddr; // Địa chỉ In/Out [X/Y]
        private string name; // Tên địa chỉ
        private string state; // Trạng thái ON/OFF

        public string PortID { get => portID; set => portID = value; }
        public ushort PortAddr { get => portAddr; set => portAddr = value; }
        public string Name { get => name; set => name = value; }
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                if (state != null & state.Equals("ON"))
                {
                    this.StatusColor = STATE_ON;
                }
                else
                {
                    this.StatusColor = STATE_OFF;
                }
            }
        }

        #endregion

        #region Method

        public Brush StatusColor { get; private set; }

        public IOPort() { }

        public IOPort(UInt16 portAddr, String name)
        {
            this.PortAddr = portAddr;
            if (portAddr < 0x20)
            {
                this.PortID = String.Format("X{0}", portAddr.ToString("X"));
            }
            else
            {
                this.PortID = String.Format("Y{0}", portAddr.ToString("X"));
            }
            this.Name = name;
            this.State = "OFF";
            this.StatusColor = Brushes.Brown;
        }

        public void UpdateUI()
        {
            OnPropertyChanged("PortID");
            OnPropertyChanged("Name");
            OnPropertyChanged("State");
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
        #endregion
    }

}
