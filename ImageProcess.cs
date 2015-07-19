using System.Drawing;
using System.Collections.Generic;
using System.Linq;
class ImageProcess
{
    int width;                           
    int height;                          
     
    public ImageProcess(int width, int height)
    {
        this.width = width;
        this.height = height; 
    }
     
    public float[] img2array(string location, int window = 1)
    {   
           float[] grayscale;
           Image image = Image.FromStream(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(location)));
           Bitmap img = new Bitmap(image, width, height); 

           grayscale = new float[img.Width * img.Height];    
           for (int x = 0; x < img.Width; x++)
           {
               for (int y = 0; y < img.Height; y++)
               {
                   Color oc = img.GetPixel(x, y); 
                   grayscale[x * img.Height + y] = (255f-(float)(((0.299 * oc.R) + (0.587 * oc.G) + (0.114 * oc.B))));   
               }
           }
           img.Dispose(); 
           image.Dispose();
           grayscale = normalize(grayscale, 0, 1); 
           return grayscale; 
    }

    public float[] normalize(float[] vector,float a,float b)
    { 
        float min=vector.Min();
        float max=vector.Max(); 
        for (int i = 0; i < vector.Length; i++)
            vector[i] = (((float)vector[i] - min) * (b - a)) / (max - min) + a; 
        return vector;
    } 
}