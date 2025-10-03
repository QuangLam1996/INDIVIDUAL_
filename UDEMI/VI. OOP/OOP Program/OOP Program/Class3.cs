using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Program
{
    internal class Class3
    {
        static void Main(string[] args)
        {
            IAnimal animal = new Class3_1();
            animal.Speak();
            animal.Move();
        }
    }

    class Class3_1 :  Dog1, IAnimal, ITrencan
    {
        public string Name { get; set; }
        public int[] Rate { get ; set ; }

        public void Speak()
        {
            Console.WriteLine("Go");
        }
    }

    interface IAnimal
    {
        string Name { get; set; }
        int[] Rate { get; set; }
        void Move()
        {
            Console.WriteLine("Move");
        }
        void Speak();
    }
    interface ITrencan
    {


    }

    class Dog1() { }

}
