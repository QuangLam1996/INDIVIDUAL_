using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Program
{
    internal class Class1
    {
        static void Main(string[] args)
        {
            Animal myDog = new Dog();
            Animal myCat = new Cat();

            Console.WriteLine(myDog.Sound);  // Output: Bark
            Console.WriteLine(myCat.Sound);  // Output: Meow

        }
        public class Animal
        {
            public virtual string Sound { get; set; } = "Some generic animal sound";
        }

        public class Dog : Animal
        {
            public override string Sound { get; set; } = "Bark";
        }

        public class Cat : Animal
        {
            public override string Sound { get; set; } = "Meow";
        }

    }
}
