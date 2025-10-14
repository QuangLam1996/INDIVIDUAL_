using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static OOP_Program.Class1.Dog;

namespace OOP_Program
{
    internal class Class1
    {
        static void Main(string[] args)
        {
            Dog myDog = new Dog("Brak");
            Animal myCat = new Cat("Meow");
            Animal animal = new Animal("");
            //myDog.Run();
            Console.WriteLine(myDog.Sound);  // Output: Bark
            Console.WriteLine(myCat.Sound);  // Output: Meow
            Console.WriteLine(animal.Sound);  // Output:
            //Console.WriteLine(animal.GetState());
            
            //animal.SetTimer();
            //while (true)
            //{
            //    if (!animal.GetDelayTimer())
            //    {
            //        Console.WriteLine(animal.GetDelayTimer());
            //        animal.ResetTimer();
            //        animal.SetTimer();
            //    }
            //}
        }


        public class Animal
        {
            public virtual string Sound { get; set; } = "Some generic animal sound";
            public Stopwatch _timer = new Stopwatch();
            public int _step;

            public enum eState
            {
                NONE,
                ALARM,
            }
            eState _state = eState.ALARM;
            eState _oldState = eState.NONE;

            public Animal(string _name)
            {


            }
            public void SetTimer()
            {
                _timer.Restart();
            }
            public void ResetTimer()
            {
                _timer.Reset();
            }

            public bool GetDelayTimer()
            {
                int elapsed = 100;
                return elapsed > _timer.ElapsedMilliseconds;
            }


            public eState GetState(bool old = false)
            {
                if (old)
                    return _oldState;
                else
                    return _state;
            }

            public void NextStep(int step)
            {
                SetTimer();

                if (-1 == step)
                    _step++;
                else
                    _step = step;
            }
        }

        public class Dog : Animal
        {
            public enum eStep
            {
                IDLE = -1,
                START,
                STOP,
                PAUSE,
            }

            public override string Sound { get; set; }
            public Dog(string _name) : base(_name)
            {
                this.Sound = _name;
            }

            public void NextStep(eStep step =eStep.IDLE)
            {
                base.NextStep((int)step);
            }

            public void NextStep_(int a=0)
            {
                //base.NextStep((int)step);
            }


            public void Run()
            {
                switch ((eStep)_step)
                {
                    case eStep.START:
                        NextStep();
                        break;
                    case eStep.STOP:
                        NextStep();
                        break;
                    case eStep.PAUSE:
                        NextStep();
                        break;
                }
            }
        }

        public class Cat : Animal
        {
            public override string Sound { get; set; }
            public Cat(string _name) : base(_name)
            {
                this.Sound = _name;
            }

        }

    }
}
