using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Program
{


    public class Class5
    {
        static void Main(string[] args)
        {
            Animal animal = new Animal();
            Dog dog = animal as Dog;

            if (dog != null)
            {
                Console.WriteLine("animal as dog");
            }
            else
            {
                Console.WriteLine("aniaml as null");
            }
        }
    }

    public class Animal { }
    public class Dog : Animal
    {


    }

}
