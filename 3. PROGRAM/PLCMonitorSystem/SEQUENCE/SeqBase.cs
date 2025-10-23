using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    [Flags]
    public enum eFlag : Int64
    {
        LOAD = 1 << 0,
        UNLOAD = 1 << 1,
        PLACE = 1 << 2,
        //EMPTY     = 1 << 3,

        PICK_FRONT = 1 << 4,
        PICK_REAR = 1 << 5,
        //EMPTY     = 1 << 6,
        //EMPTY     = 1 << 7,

        //EMPTY     = 1 << 8,
        //EMPTY     = 1 << 9,
        //EMPTY     = 1 << 10,
        //EMPTY     = 1 << 11,

        REVERSE_FRONT = 1 << 12,
        REVERSE_REAR = 1 << 13,
        //EMPTY     = 1 << 14,
        //EMPTY     = 1 << 15,

        MARK_FRONT = 1 << 16,
        MARK_REAR = 1 << 17,
        //EMPTY     = 1 << 18,
        //EMPTY     = 1 << 19,

        //EMPTY     = 1 << 20,
        //EMPTY     = 1 << 21,
        //EMPTY     = 1 << 22,
        //EMPTY     = 1 << 23,

        //EMPTY     = 1 << 24,
        //EMPTY     = 1 << 25,
        //EMPTY     = 1 << 26,
        //EMPTY     = 1 << 27,

        //EMPTY     = 1 << 28,
        //EMPTY     = 1 << 29,
        //EMPTY     = 1 << 30,
        //EMPTY     = 1 << 31,

        ////////////////////////

        //EMPTY     = 1 << 32,
        //EMPTY     = 1 << 33,
        //EMPTY     = 1 << 34,
        //EMPTY     = 1 << 35,

        //EMPTY     = 1 << 36,
        //EMPTY     = 1 << 37,
        //EMPTY     = 1 << 38,
        //EMPTY     = 1 << 39,

        //EMPTY     = 1 << 40,
        //EMPTY     = 1 << 41,
        //EMPTY     = 1 << 42,
        //EMPTY     = 1 << 43,

        //EMPTY     = 1 << 44,
        //EMPTY     = 1 << 45,
        //EMPTY     = 1 << 46,
        //EMPTY     = 1 << 47,

        //EMPTY     = 1 << 48,
        //EMPTY     = 1 << 49,
        //EMPTY     = 1 << 50,
        //EMPTY     = 1 << 51,

        //EMPTY     = 1 << 52,
        //EMPTY     = 1 << 53,
        //EMPTY     = 1 << 54,
        //EMPTY     = 1 << 55,

        //EMPTY     = 1 << 56,
        //EMPTY     = 1 << 57,
        //EMPTY     = 1 << 58,
        //EMPTY     = 1 << 59,

        //EMPTY     = 1 << 60,
        //EMPTY     = 1 << 61,
        //EMPTY     = 1 << 62,
        //EMPTY     = 1 << 63,

        ALL = Int64.MaxValue,
    }

    public abstract class SeqBase
    {
        private static Int64 _state = default;
        public eFlag _flag;

        private bool _workInit;
        private bool _workAuto;
        public int _stepInit;
        public int _stepAuto;

        public Stopwatch _timer = new Stopwatch();

        protected SeqBase(eFlag flag)
        {
            _workInit = false;
            _workAuto = false;
            _stepInit = 0;
            _stepAuto = 0;
            _flag = flag;
        }

        public abstract void InitStep(); // Logic chạy của mỗi Unit Seq
        public abstract void AutoStep(); // Logic chạy của mỗi Unit Seq

        public static void SetRun(eFlag state, bool action)
        {
            if (action)
                _state |= (Int64)state; // Thêm Flag vào _state
            else
                _state &= (~(Int64)state); // Gỡ Flag ra _state
        }

        public eFlag GetFlag() { return _flag; }

        public void Start()
        {
            _workAuto = true;
            _stepAuto = 0;
            SetRun((eFlag)_flag, true);

            //string log = string.Format("{0} : START SEQ", Enum.GetName(typeof(eFlag), GetFlag()));
            //theLog.Add(eLog.SEQUENCE, log);
        }

        public void Stop()
        {
            _workAuto = false;
            _workInit = false;
            _stepInit = -1;
            _stepAuto = -1;
            SetRun((eFlag)_flag, false);

            //string log = string.Format("{0} : STOP SEQ", Enum.GetName(typeof(eFlag), GetFlag()));
            //theLog.Add(eLog.SEQUENCE, log);
        }

        public void Reset()
        {

            //string log = string.Format("{0} : STOP SEQ", Enum.GetName(typeof(eFlag), GetFlag()));
            //theLog.Add(eLog.SEQUENCE, log);

        }

        public void Initial()
        {
            _workInit = true;
            _stepInit = -1;
            SetRun((eFlag)_flag, false);

            //string log = string.Format("{0} : STOP SEQ", Enum.GetName(typeof(eFlag), GetFlag()));
            //theLog.Add(eLog.SEQUENCE, log);
        }

        public void InitStep(int step)
        {
            SetTimer();

            if (-1 == step)
                _stepInit++;
            else
                _stepInit = step;
        }
        public void AutoStep(int step)
        {
            SetTimer();

            if (-1 == step)
                _stepAuto++;
            else
                _stepAuto = step;
        }


        public void SetTimer()
        {
            _timer.Restart();
        }

    }
}
