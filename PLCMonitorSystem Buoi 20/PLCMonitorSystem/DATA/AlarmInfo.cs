using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem.DATA
{
    class AlarmInfo
    {
        public const int MODE_AUTO = 0;
        public const int MODE_MANUAL = 1;

        // Alarm Codes:
        public const int PLC_COMM_FAILED = 30001;
        public const int PLC_ERROR = 30002;

        public const int MES_CHECK_TIMEOUT = 30010;
        public const int MES_CHECK_NG = 30011;

        public const int SCANNER_RECYCLE = 30020;
        public const int SCANNER_CALIB = 30021;

        public const int JIG_REMAIN_INLOADING = 31143;
        public const int JIG_REMAIN_MIDDLE = 31144;
        public const int JIG_REMAIN_UNLOADING = 31145;

        public const int READ_QR_ERROR_ONE = 31132;
        public const int READ_QR_ERROR_ALL = 31169;

        public const int NEW_LOT_REQUIRE = 31157;

        public const int JIG_UNKNOWN = 31160;

        public const int LOT_FULL = 31168;

        public const int DOOR_OPEN = 31180;

        public const int JIG_EXISTING = 31182;

        public const int JIG_INVALID = 31189;

        public const int LOT_MIXING = 31190;

        public const int NEW_ALARM_ID = 31191;

        public static String getMessage(int alarmType)
        {
            var ret = "";
            switch (alarmType)
            {
                case PLC_COMM_FAILED:
                    ret = "Enter AUTO TEST failed.";
                    break;

                case PLC_ERROR:
                    ret = "PLC Error.";
                    break;

                case MES_CHECK_TIMEOUT:
                    ret = "MES checking timed out.";
                    break;

                case MES_CHECK_NG:
                    ret = "MES checking NG.";
                    break;

                case SCANNER_RECYCLE:
                    ret = "The 2D scanner has reached its life cycle.";
                    break;

                case SCANNER_CALIB:
                    ret = "The 2D scanner has reached its calibration.";
                    break;

                case DOOR_OPEN:
                    ret = "The door has been opened.";
                    break;

                case JIG_REMAIN_INLOADING:
                    ret = "Jig remains on inloading main conveyor.";
                    break;

                case JIG_REMAIN_MIDDLE:
                    ret = "Jig remains on middle main conveyor.";
                    break;

                case JIG_REMAIN_UNLOADING:
                    ret = "Jig remains on unloading conveyor.";
                    break;

                case READ_QR_ERROR_ONE:
                    ret = "Could not read PKG code.";
                    break;

                case READ_QR_ERROR_ALL:
                    ret = "The whole is bad.";
                    break;

                case NEW_LOT_REQUIRE:
                    ret = "The new Lot will be changed.";
                    break;

                case LOT_FULL:
                    ret = "Lot In Input count exceeded.\r\n\r\n" +
                          "1.Continue: Initialize the counter and continue execution.\r\n" +
                          "2.LotEnd: Fully initialized and restart.";
                    break;

                case JIG_UNKNOWN:
                    ret = "Unknown jig detected in 2D site.";
                    break;

                case JIG_INVALID:
                    ret = "Jig ID is different from equiqment ID.";
                    break;

                case JIG_EXISTING:
                    ret = "The jig information is the same as the previous jig.";
                    break;

                case LOT_MIXING:
                    ret = "Lot ID and QR Code information are different.";
                    break;
                case NEW_ALARM_ID:
                    ret = "Doc du lieu tu PLC gap van de";
                    break;

            }
            return ret;
        }

        public static String getSolution(int alarmType)
        {
            var ret = "";
            switch (alarmType)
            {
                case PLC_COMM_FAILED:
                    ret = String.Format("Check PLC RS232 cable.");
                    break;

                case PLC_ERROR:
                    ret = "Checking error in PLC.";
                    break;

                case MES_CHECK_TIMEOUT:
                    ret = "1. Check Ethernet Cable.\r\n" +
                          "2. Check MES settings.";
                    break;

                case MES_CHECK_NG:
                    ret = "1. Check LOT ID.\r\n" +
                          "2. Check jig for empty PKG.\r\n" +
                          "3. Check MES for Mixing Lot.";
                    break;

                case SCANNER_RECYCLE:
                    ret = "Replace the 2D scanner.";
                    break;

                case SCANNER_CALIB:
                    ret = "Re-calibrate the 2D scanner.";
                    break;

                case DOOR_OPEN:
                    ret = "The door is open or the sensor is malfunctioning. Please close the door and operate it.";
                    break;

                case JIG_REMAIN_INLOADING:
                    ret = "Remove jig from the inloading main conveyor.";
                    break;

                case JIG_REMAIN_MIDDLE:
                    ret = "Remove jig from the middle main conveyor.";
                    break;

                case JIG_REMAIN_UNLOADING:
                    ret = "Remove jig from the unloading main conveyor.";
                    break;

                case READ_QR_ERROR_ONE:
                    ret = String.Format("The material will be marked NG and the defective material should be removed from the Jig.");
                    break;

                case READ_QR_ERROR_ALL:
                    ret = String.Format("1.Check the jig direction or check whether the product is inserted, and then restart it.\r\n" +
                                        "2.Do not send information to the tester");
                    break;

                case NEW_LOT_REQUIRE:
                    ret = String.Format("Press the LOTIN button on the main screen to enter the new LOT information.");
                    break;

                case LOT_FULL:
                    break;

                case JIG_UNKNOWN:
                    ret = String.Format("Remove the jig from the 2D site and load it on the loading stoker.");
                    break;

                case JIG_INVALID:
                    ret = String.Format("1.Remove the jig from the machine.");
                    break;

                case JIG_EXISTING:
                    ret = String.Format("1.Remove the jig to check for duplicate testing.\r\n" +
                                        "2.Take the jig out of the facility.\r\n" +
                                        "3.Do not send information to the tester.");
                    break;

                case LOT_MIXING:
                    ret = String.Format("1.Remove the jig from the machine.\r\n" +
                                        "2.Please check product QR information.");
                    break;
                case NEW_ALARM_ID:
                    ret = String.Format("1.Kiem tra ten thiet bi\r\n" +
                                        "2.Kiem tra ket noi den PLC.");
                    break;

            }
            return ret;
        }

        public int id { get; set; }
        public int alarmCode { get; set; }
        public DateTime createdTime { get; set; }
        public int mode { get; set; }
        public String message { get; set; }
        public String solution { get; set; }

        public String getMode()
        {
            if (mode == MODE_AUTO)
            {
                return "AUTO";
            }
            return "MANUAL";
        }

        public AlarmInfo() { }

        public AlarmInfo(int mode, int code, String msg, String sol)
        {
            this.id = 0;
            this.mode = mode;
            this.alarmCode = code;
            this.createdTime = DateTime.Now;
            this.message = msg;
            this.solution = sol;
        }
    }
}
