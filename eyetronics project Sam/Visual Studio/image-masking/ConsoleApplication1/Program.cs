using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //first image
            int width = 6;
            int height = 5;
            int[] array = new int[width * height];

            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = i;
            }
            //second image
            int width2 = 12;
            int height2 = 7;
            int[] array2 = new int[width2 * height2];

            for (int i = 0; i < height2; i++)
            {
                for (int j = 0; j < width2; j++)
                {
                    if (j > 5)
                        array2[j + (i * width2)] = 1;
                    else
                        array2[j + (i * width2)] = 0;
                }
            }

            //show image1
            string test = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    test = test + (j + (i * width));
                    if (j + (i * width) < 10)
                        test = test + "  ";
                    else
                        test = test + " ";
                }
                test = test + "\n";
            }
            Console.WriteLine(test);

            //show image2
            test = "";
            for (int i = 0; i < height2; i++)
            {
                for (int j = 0; j < width2; j++)
                {
                    test = test + array2[(j + (i * width2))] + "  ";
                }
                test = test + "\n";
            }
            Console.WriteLine(test);

            //show image2 cropped by image1 width and height
            test = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    test = test + array2[(j + 3 + (i * width2))] + "  ";
                }
                test = test + "\n";
            }
            Console.WriteLine(test);

            //show images combined
            while (true)
            {
                test = "";
                int offset = -width2 + System.Convert.ToInt32(Console.ReadLine());
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int t = j + (i * width);
                        int t2 = j - offset + (i * width2);
                        test = test + (array[t] * array2[t2]);
                        if (array[t] * array2[t2] < 10)
                            test = test + "  ";
                        else
                            test = test + " ";
                    }
                    test = test + "\n";
                }
                Console.WriteLine(test);
            }
        }
    }
}
