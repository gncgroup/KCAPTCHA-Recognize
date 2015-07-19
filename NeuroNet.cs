class NeuroNet
{   
    int[] neurons_count; 
    int[] layers_params;
    int layers_count;   
    int number;         
    Neuron[][] neurons;  

    public NeuroNet(int size, int[] neurons_count, int[]  layers_params, int number)
    { 
        this.number = number;
        this.neurons_count = neurons_count;
        this.layers_params = layers_params;
        this.layers_count = neurons_count.Length;
        neurons_count[0] = size;
        layers_count = neurons_count.Length - 1;
        neurons = new Neuron[layers_count + 1][]; 
        for (int layer = 1; layer <= layers_count; layer++)
            neurons[layer] = new Neuron[neurons_count[layer]];
        create_neurons();
    }

    void create_neurons()
    {
        System.Random rnd = new System.Random((int)System.DateTime.Now.Ticks);
        for (int layer = 1; layer <= layers_count; layer++)
            for (int i = 0; i < neurons_count[layer]; i++)
                neurons[layer][i] = new Neuron(neurons_count[layer - 1], i, layer, number, rnd);

    }
     
    public float[][] ask_network(float[] vector, bool teach_mode = false)
    {
        float[][] res = new float[layers_count + 1][];
        for (int layer = 0; layer <= layers_count; layer++)
            res[layer] = new float[neurons_count[layer]];
        res[0] = vector;
        for (int layer = 1; layer <= layers_count; layer++)
        { 
            for (int i = 0; i < neurons_count[layer];i++ )
                res[layer][i] = neurons[layer][i].ask(res[layer - 1]);
        }
        return new float[][] { res[layers_count] };
    }
}