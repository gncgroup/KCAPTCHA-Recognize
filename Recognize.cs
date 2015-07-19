using System;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Test test = new Test(100, 50, "23456789abcdegikpqsvxyz",6,1000 /*10000*/);
         //   System.Console.WriteLine(test.single_recognize(@"tests\2a3qqc.gif"));
            test.l_test();
        }
        catch (System.IO.FileNotFoundException e)
        {
            Console.WriteLine("File does not exist " + e.Message + " " + e.FileName);
        }
        Console.Read();
    }
}