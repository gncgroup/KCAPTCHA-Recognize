using System; 
using System.Linq; 

class Test
{
    ImageProcess img_process;
    NeuroNet letters_network;
    int tests_length;
    int length;
    string[] tests;
    string alphabet;

    public Test(int width, int height, string alphabet, int length, int tests_length)
    {
        this.alphabet = alphabet;
        this.tests_length = tests_length;
        this.length = length;
        tests = new string[tests_length];
        img_process = new ImageProcess(width, height);
        System.Random rnd = new System.Random((int)System.DateTime.Now.Ticks);

        System.Console.WriteLine("Initialize network...");
        letters_network = new NeuroNet(width * height, new int[] { 0, 2000, alphabet.Length * length }, new int[] { 0, 0 }, 1);
        read_filenames();
    }
    void read_filenames()
    {
        var str = new System.IO.StreamReader(@"tests.txt");
        for (int i = 0; i < tests_length; i++)
            tests[i] = str.ReadLine();
        str.Close();
    }
    public string single_recognize(string path)
    {
        string text="";
        var res = letters_network.ask_network(img_process.img2array(path))[0];
        var result = new float[length][];
        for (int t = 0; t < length; t++)
            result[t] = new float[alphabet.Length];
        for (int t = 0; t < res.Length; t++)
            result[(int)t / alphabet.Length][(t % alphabet.Length)] = res[t]; 
        for (int t = 0; t < result.Length; t++) 
            text+=alphabet[Array.IndexOf(result[t], result[t].Max())];  
        return text;

    }
    public float l_test()
    {
        System.Console.WriteLine("Testing...");
        int errors = 0, error_digits = 0, count = 0;

        for (int i = 0; i < tests.Length; i++)
        {
            var res = letters_network.ask_network(img_process.img2array(@"tests\" + tests[i] + ".gif"))[0];
            var result = new float[length][];
            for (int t = 0; t < length; t++)
                result[t] = new float[alphabet.Length];

            for (int t = 0; t < res.Length; t++)
                result[(int)t / alphabet.Length][(t % alphabet.Length)] = res[t];
            var cur_error = 0;
            for (int t = 0; t < result.Length; t++)
            {
                var index = Array.IndexOf(result[t], result[t].Max());

                if (index != alphabet.IndexOf(tests[i][t]))
                {
                    error_digits++;
                    cur_error++;
                }
            }
            if (cur_error > 0)
                errors++;

            Console.Write("\r{0}  of {1}",i,tests_length);

            count++;
        } 
        Console.WriteLine("\nTest set:\n TOTAL Count:" + count + ". Digits Errors:" + error_digits + ". Digits Error rate:" + (float)error_digits / (count * 6) + ". Errors:" + errors + ". Error rate: " + (float)errors / count + "\n");
        return (float)errors / count;
    }
}
