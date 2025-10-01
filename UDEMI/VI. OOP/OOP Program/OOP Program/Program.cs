using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Program
{
    internal class Bai01
    {
        static void Main(string[] args)
        {
            Cat cat = new Cat();

            cat.Rate = new int[5];
            cat.Rate[0] = 1;
            cat.Rate[1] = 2;
            cat.Rate[2] = 3;
            cat.Rate[3] = 4;
            cat.Rate[4] = 5;

            cat.City = "Cat";
            var name = cat.City;
            Console.WriteLine(name);
        }

        class Cat
        {
            private string city;

            int[] rate;
            string[] comment;

            public Cat()
            {
            }

            public string City
            {
                get 
                {
                    return city;
                }
                set
                {
                    city = value;
                }
            }

            public int[] Rate { get => rate; set => rate = value; }
            public string[] Comment { get => comment; set => comment = value; }

            //Indexer: Áp dụng cho thành phần dữ liệu mảng, truy cập vào các phần tử mảng thông qua obj
            public int this[int index] 
            { 
              get => Rate[index]; 
              set => Rate[index] = value; 
            }
            public string this[float index]
            {
                get => Comment[(int)index];
                set => Comment[(int)index] = value;
            }
        }
    }
}
