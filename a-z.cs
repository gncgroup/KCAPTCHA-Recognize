using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace FractureRecognize
{
    class Program
    { 
        static int width = 20;               //Ширина окна
        static int height = 20;             //Высота окна
        static int win_width = 20;           //Ширина окна
        static int win_height = 20;          //Высота окна 
        static ImageProcess imgprocess;      //Предварительная обработка и кадрирование
        static NeuroNet neuronet_fractures;  //Сеть распознающая переломы 
        static string alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
        static int cur_epoch=0;
        static void Main(string[] args)
        {
            try
            {
                //Для дальнейшей обработки и кадрирования инициализируем ImageProcess
                imgprocess = new ImageProcess(width, height, win_width, win_height);
                //Случайные числа, для многопоточного обучения
                System.Random rnd = new System.Random((int)System.DateTime.Now.Ticks);

                //Инициализируем нейросеть 
                //Сеть распознающая переломы - 4 слоя
                neuronet_fractures = new NeuroNet(width * height, new int[] { 0,  300,36 }, 1);

                fast_test_fractures();
                //Инициализируем потоки обучения
                for (int i = 0; i < 3; i++)
                {
                    //Инициализируем поток обучения сети распознающей переломы
                    new Thread(() => teach_fracture_network(3000)).Start();
                    //Случайная задержка
                   System.Threading.Thread.Sleep(rnd.Next(100, 5000));
                }
                
                
                //Загружаем основной поток обучением переломов, дабы не простаивал
                teach_fracture_network(3000,1);
                //Случайная задержка
                System.Threading.Thread.Sleep(rnd.Next(100, 200));
                //Пауза 500 мс, что бы все потоки успели завержиться
                System.Threading.Thread.Sleep(500);
                //Сохраняем веса для сетей
                neuronet_fractures.write_weights();
                //Запускаем тестирование
                test_fractures();
                //Если файл не найден
                System.Threading.Thread.Sleep(10000000);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("File does not exist", e.Source);
            }
        } 

        //Обучает нейросеть распознавать переломы
        static void teach_fracture_network(int epoch,int write_weights=0)
        {
            //Подаем обучающую выборку переломов. Epoch - количество эпох
            for (int k = 0; k < epoch; k++)
            {
                double[] cur_resp = new double[36];
                for (int i = 0; i < cur_resp.Length; i++)
                {
                    cur_resp[i] = 0;
                }
                for (int i = 0; i < 10000; i++) 
                    for (int x = 0; x < 36; x++)
                    {
                        cur_resp[x] = 1;
                        if (x > 0)
                            cur_resp[x - 1] = 0;
                        else
                            cur_resp[cur_resp.Length-1] = 0;
 
                        neuronet_fractures.teach_network(imgprocess.img2array(@"C:\samples\samples\" + alphabet[x] + @"\" + i + ".png"), cur_resp);
                    }


                Interlocked.Increment(ref cur_epoch);
                System.Console.WriteLine("\n" + cur_epoch + "\n");
                if (write_weights == 1)
                    neuronet_fractures.write_weights();
                if (cur_epoch  % 20 == 0 && k > 0)
                { 
                        fast_test_fractures();
                        test_fractures();
                        mnist_fast_test_fractures();
                }
            }
        }

        //Тестирует на обучающей выборке распознавание переломов
        static void test_fractures()
        {
            int error = 0, count = 0; //Инициализация счетчиков ошибок и количества примеров
            for (int x = 0; x < 36; x++)
                for (int i = 0; i < 1000; i++)
                {
                    var res = neuronet_fractures.ask_network(imgprocess.img2array(@"C:\samples\tests\" + alphabet[x] + @"\" + i + ".png"))[0];
                    var index=Array.IndexOf(res, res.Max());
                    error += index == x ? 0 : 1;
                 //   System.Console.WriteLine(string.Join(",", neuronet_fractures.ask_network(imgprocess.img2array(@"C:\samples\tests\" + alphabet[x] + @"\" + i + ".png"))[0]));
                 //   System.Console.WriteLine("\n"+alphabet[x]+"\n\n");
                    count++;
                }
            Console.WriteLine("Tests:\n");
            Console.WriteLine("Count:" + count + ". Errors:" + error + ". Error rate: " + (double)error / count + "\n");
        }

        //Тестирует на обучающей выборке распознавание переломов
        static void fast_test_fractures()
        {
            int error = 0, count = 0; //Инициализация счетчиков ошибок и количества примеров
            for (int x = 0; x < 36; x++)
                for (int i = 0; i < 1000; i++)
                {
                    var res = neuronet_fractures.ask_network(imgprocess.img2array(@"C:\samples\samples\" + alphabet[x] + @"\" + i + ".png"))[0];
                    var index = Array.IndexOf(res, res.Max());
                    error += index == x ? 0 : 1;
                       //System.Console.WriteLine(string.Join(",", neuronet_fractures.ask_network(imgprocess.img2array(@"C:\samples\tests\" + alphabet[x] + @"\" + i + ".png"))[0]));
                       //System.Console.WriteLine("\n"+alphabet[x]+"\n\n");
                    count++;
                } 
            Console.WriteLine("Test:\n");
            Console.WriteLine("Count:" + count + ". Errors:" + error + ". Error rate: " + (double)error / count + "\n");
        }

        static void mnist_test_fractures()
        {
            int error = 0, count = 0; //Инициализация счетчиков ошибок и количества примеров
            for (int x = 0; x < 10; x++)
                for (int i = 0; i < 800; i++)
                {
                    var res = neuronet_fractures.ask_network(imgprocess.img2array(@"C:\mnist\samples\" + alphabet[x] + @"\" + i + ".png"))[0];
                    for (int t = 10; t < 36; t++)
                        res[t] = 0;
                    var index = Array.IndexOf(res, res.Max());

                    error += index == x ? 0 : 1;
                    //   System.Console.WriteLine(string.Join(",", neuronet_fractures.ask_network(imgprocess.img2array(@"C:\samples\tests\" + alphabet[x] + @"\" + i + ".png"))[0]));
                    //   System.Console.WriteLine("\n"+alphabet[x]+"\n\n");
                    count++;
                }
            Console.WriteLine("Tests:\n");
            Console.WriteLine("Count:" + count + ". Errors:" + error + ". Error rate: " + (double)error / count + "\n");
        }

        //Тестирует на обучающей выборке распознавание переломов
        static void mnist_fast_test_fractures()
        {
            int error = 0, count = 0; //Инициализация счетчиков ошибок и количества примеров
            for (int x = 0; x < 10; x++)
                for (int i = 0; i < 200; i++)
                {
                    var res = neuronet_fractures.ask_network(imgprocess.img2array(@"C:\mnist\tests\" + alphabet[x] + @"\" + i + ".png"))[0];
                    for (int t = 10; t < 36; t++)
                        res[t] = 0;
                    var index = Array.IndexOf(res, res.Max());
                    error += index == x ? 0 : 1;
                    //System.Console.WriteLine(string.Join(",", neuronet_fractures.ask_network(imgprocess.img2array(@"C:\samples\tests\" + alphabet[x] + @"\" + i + ".png"))[0]));
                    //System.Console.WriteLine("\n"+alphabet[x]+"\n\n");
                    count++;
                }
            Console.WriteLine("MNIST Test:\n");
            Console.WriteLine("Count:" + count + ". Errors:" + error + ". Error rate: " + (double)error / count + "\n");
        }
    }
}
