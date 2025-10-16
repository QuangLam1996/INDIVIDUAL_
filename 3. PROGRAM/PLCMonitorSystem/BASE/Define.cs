using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    public class Define
    {

    }

    public enum eTimer
    {
        ON,
        OFF,
    }

    public enum eWM
    {
        DEVICECHANGE = 0x0219,

        USER = 0x400,
        SAVE,
        ALARM,
    }

    public enum eDBT
    {
        DEVTUP_VOLUME = 0x02,
        DEVICEARRIVAL = 0x8000,
        DEVICEREMOVECOMPLETE = 0x8004,
    }

    public enum eState
    {
        NONE,
        INIT,       
        IDLE,       
        SETUP,	    
        READY,      
        MANUAL,	   
        AUTO,       
        ALARM,     
        PAUSE,     
        END, 
        ABORT,
    }

    public enum eIndicatorTime
    {
        BlinkTime,
        BuzzerTime,
    }

    public enum eTower
    {
        RED,
        YELLOW,
        GREEN,
    }

    public enum eButton
    {
        START,
        STOP,
        RESET,
        INITIAL,
    }

    public enum eLamp
    {
        OFF,
        ON,
        BLINK,
    }

    public enum eBuzzer
    {
        WARNING,
        HEAVY,
        AUTO,
    }

    public enum eConnect
    {
        MOTION,
        IO,
        MES,
        SCANNER,
    }

    public enum eLevel
    {
        OPERATOR,
        MAINT,
        MASTER,
        SUPER,
    }

    public enum eMode
    {
        AUTO,
        DRY,
    }

    public enum ePop
    {
        ALARM,
    }

    public enum eView
    {
        AUTO = 0,
        RECIPE,
        INIT,
        MANUAL,
        CONFIG,
        MOTOR,
        IO,
    }

    public enum eDirection
    {
        FRONT,
        REAR,
    }

    public enum eRecipeOption
    {
        ALIGN,
    }

    public enum eDevice
    {
        ROW,
        COL,
        GAP_X,
        GAP_Y,
        OFFSET_X,
        OFFSET_Y,
    }
    public enum eTarget
    {
        PATTERN = 0,
    }

    public enum eTeaching
    {
        BASE_X,
        BASE_Y,
        BASE_Z,

        MARK_X,
        MARK_Y,
        MARK_Z,
    }

    public enum eLegend
    {
        TARGET,
        MARKED,
        GOOD,
        BAD,
    }

    public enum eManual
    {
        CONVEYOR = 0,
        MARKING,
    }

    public enum eConfig
    {
        INDICATOR,
        OPTION,
        SETTING,
        COMM,
        SOCKET,
    }

    public enum eOption
    {
        ALTERNATELY,
        BYPASS,
        MES,
        MARK,
        QR,

        DOOR,
        VAC_ALARM,
        AOI_LOG,
        BUFFER_LOG,
        MARK_LOG,
    }

    public enum eFile
    {
        DELETE,
        LOG,
        RESULT,
    }

    public enum eMES
    {
        EQUIP,
        IP,
        PORT,
        DB,
        ID,
        PW,
        TABLE,
    }

    public enum eCount
    {
        RETRY,
        PICK,
    }

    public enum eTimeout
    {
        MOTOR,
        SOL,
        COMM,
        MARK,
        CONVEYOR,
    }

    public enum eDelay
    {
        VACUUM = 0,
        BLOW,
        MARK,
        RESET,
        CONVEYOR,
    }

    public enum eComm
    {
        BUFFER,
        MARK,
    }

    public enum eCommState
    {
        NONE,
        SENDING,
        ACK,
        NAK,
        ERROR,
    }

    public enum eMarkCmd
    {
        START,
        STOP,
        CLOSE,
        OPEN,
        ON,
        OFF,
        ECHO,
        LOAD,
        INFORMATION,
    }

    public enum eAir
    {
        VACUUM,
        BLOW,
        STOP,
    }

    public enum eAoiCmd
    {
        NG_COMP_LIST_REQ,
        NG_COMP_LIST_REP,
    }

    public enum eStateComm
    {
        NONE,
        SENDING,
        PCB,
        READY,
        QR,
        WRONG,
    }

    public enum eMarkNozzle
    {
        INVALID,
        OPENING,
        OPEN,
        CLOSING,
        CLOSE,
        INTER,
    }

    public enum eMarkState
    {
        STANDBY = 1,
        INITIALIZE, 
        INTERVAL,
        OPERATE,  
        READY,
        PRINTING,
    }

    public enum eMarkHead
    {
        CLOSED,
        OPEN,
    }

    public enum eMarkFlag
    {
        NO_CHANGES,
        CHANGES,
    }

    public enum eReverse
    {
        HOME,
        TURN,
    }

    public enum eStopper
    {
        UP,
        DOWN,
    }

    public enum eExist
    {
        LOAD,
        ALIGN,
        UNLOAD,
    }

    public enum eInterface
    {
        REQUEST,
        REPLY,
    }

    public enum ePosition
    {
        LOAD,
        ALIGN,
        ROBOT,
        UNLOAD,
    }

    public enum eProcess
    {
        LOAD,
        PICK_FRONT,
        PICK_REAR,
        MARK_FRONT,
        MARK_REAR,
        REVERSE_FRONT,
        REVERSE_REAR,
        PLACE,
        UNLOAD,
    }
    public enum eLotIn
    {
        LOT_ID,
        CONFIG,
        LOT_QTY,
    }

}
