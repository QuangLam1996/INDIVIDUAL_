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
            Animal myDog = new Dog("Brak");
            Animal myCat = new Cat("Meow");

            Console.WriteLine(myDog.Sound);  // Output: Bark
            Console.WriteLine(myCat.Sound);  // Output: Meow

        }
        public class Animal
        {
            public virtual string Sound { get; set; } = "Some generic animal sound";

            public Animal(string _name)
            {
                
            }
        }


        public class Dog : Animal
        {
            public override string Sound { get; set; }
            public Dog(string _name) :base(_name)
            {
                this.Sound = _name;
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
