using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Program
{
    internal class Class2
    {
        static void Main(string[] args)
        {
            Cat1 aniam1 = new Cat1();
            Dog1 aniam11 = new Dog1();
            aniam1.Move();
            aniam11.Move();
        }

        static class Class1 
        {
            private static string name;
            public static string Name { get => name; set => name = value; }
        }

        abstract class Aniam1
        {
            public abstract void Move();
        }

        class Cat1 : Aniam1
        {
            public override void Move()
            {
                Console.WriteLine("Cat1");
            }
        }
        class Dog1 : Aniam1  
        {
            public override void Move()
            {
                Console.WriteLine("Dog1");
            }

        }
    }
}
