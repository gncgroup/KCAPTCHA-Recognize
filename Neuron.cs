class Neuron
{
    int pos;          
    int layer;        
    int size;         
    public float[] weight;  
    int number;      
    bool active=true;

    public Neuron(int size, int pos, int layer, int number, System.Random rnd)
    {
        this.pos = pos;
        this.layer = layer;
        this.size = size;
        this.number = number; 
        weight = new float[size+1]; 
        read_weight(rnd);
    }

    public bool read_weight(System.Random rnd)
    { 
        if (System.IO.File.Exists(@"weights\" + layer + "_" + number + "_" + pos + ".txt"))
        {
            using (var str = new System.IO.StreamReader(@"weights\" + layer + "_" + number + "_" + pos + ".txt")){
            string line;
            for (int i = 0; i < size; i++)
                if((line=str.ReadLine())!=null)
                    weight[i] = System.Single.Parse(line);
                else
                    weight[i] = (float)((rnd.NextDouble() / 1.65 - 0.3)) / 10; 
            }
        }
        else
        {
            for (int i = 0; i < size; i++)
                weight[i] = (float)((rnd.NextDouble() / 1.65 - 0.3))/10;
            weight[size] = (float)((1 + 0.5 / 10000 - (rnd.NextDouble() / 10000))); //Initialize BIAS
            return false;
        }
        return true;
    }


    public float ask(float[] vector)
    {
        if (!active)
            return 0;
        float sum = 0.0f;
         for (int i = 0; i < size; i++)
             sum += vector[i] * weight[i];

        sum += weight[size]; 
        return (float)(1 / (1 + System.Math.Exp((double)-(sum)))); 
    } 
}