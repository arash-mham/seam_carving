using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HaarImage
{
    class ImageSeam
    {
        Bitmap originalImage;
        public ImageSeam(Bitmap b)
        {
            originalImage = b;
        }
        public Color GradientX(Bitmap b, int indexX,int indexY)
        {
            Color c=Color.White;
            if (indexX > 0 && indexY > -1 && indexX < b.Width-1)
            {
                c = Color.FromArgb(Math.Abs(b.GetPixel(indexX + 1, indexY).R + b.GetPixel(indexX - 1, indexY).R-2*b.GetPixel(indexX,indexY).R)/2,
                                    Math.Abs(b.GetPixel(indexX + 1, indexY).G + b.GetPixel(indexX - 1, indexY).G-2*b.GetPixel(indexX,indexY).G)/2,
                                    Math.Abs(b.GetPixel(indexX + 1, indexY).B + b.GetPixel(indexX - 1, indexY).B-2*b.GetPixel(indexX,indexY).B)/2);
            }
            
            return c;
 
        }
        public Color GradientY(Bitmap b, int indexX, int indexY)
        {
            Color c=Color.White;
            if (indexX > -1 && indexY > 0 && indexY < b.Height-1)
            {
                c = Color.FromArgb(Math.Abs(b.GetPixel(indexX , indexY+1).R + b.GetPixel(indexX , indexY-1).R - 2 * b.GetPixel(indexX, indexY).R) / 2,
                                   Math.Abs(b.GetPixel(indexX, indexY+1).G + b.GetPixel(indexX , indexY-1).G - 2 * b.GetPixel(indexX, indexY).G) / 2,
                                   Math.Abs(b.GetPixel(indexX, indexY+1).B + b.GetPixel(indexX , indexY-1).B - 2 * b.GetPixel(indexX, indexY).B) / 2);
            }
            return c;
        }
        public double EnergyPixel(Color gradientX,Color gradientY) 
        {
            return (gradientX.R + gradientX.G + gradientX.B + gradientY.R + gradientY.G + gradientY.B) / 6.0;
        }
        public List<List<double>> EnergyImage(Bitmap b)
        {
            
            List<List<double>> results = new List<List<double>>();
            if (b != null)
            {
                #region array_making
                for (int i = 0; i < b.Width; i++)
                {
                    List<double> temp = new List<double>();
                    for (int j = 0; j < b.Height; j++)
                    {
                        temp.Add(EnergyPixel(GradientX(b, i, j), GradientY(b, i, j)));
                    }
                    results.Add(temp);
                }
                #endregion
            }
                return results;
        }
        public List<List<double>> DynamicProgramming_VerticalSeam(List<List<double>> energy, Bitmap b) 
        {
            
            List<List<double>> results = new List<List<double>>();
            #region array_making
            for (int i = 0; i < b.Width; i++)
            {
                List<double> temp = new List<double>();
                for (int j = 0; j < b.Height; j++)
                {
                    temp.Add(energy[i][j]);
                }
                results.Add(temp);
            }
            #endregion
            #region dynamic filling
            for (int j = 1; j < b.Height; j++)
            {
                for (int i = 0; i < b.Width; i++)
                {
                    double EnergyTemp0 = 1000000;
                    double EnergyTemp1 = 1000000;
                    double EnergyTemp2 = 1000000;
                    double EnergyTotal = 1000000;
                    int indexI = i - 1;
                    int indexJ = j - 1;
                    if (indexI > -1 && indexJ > -1 && indexI < b.Width && indexJ < b.Height)
                        EnergyTemp0 = results[indexI][indexJ];
                    indexI = i ;
                    indexJ = j-1;
                    if (indexI > -1 && indexJ > -1 && indexI < b.Width && indexJ < b.Height)
                        EnergyTemp1 = results[indexI][indexJ];
                    indexI = i + 1;
                    indexJ = j - 1;
                    if (indexI > -1 && indexJ > -1&& indexI < b.Width && indexJ < b.Height)
                        EnergyTemp2 = results[indexI][indexJ];
                    EnergyTotal = Math.Min(Math.Min(EnergyTemp0, EnergyTemp1), Math.Min(EnergyTemp1, EnergyTemp2));
                    results[i][j] = results[i][j] + EnergyTotal;
                }
            }
            #endregion
            return results;


        }
        public List<int> FindMinimumColumn(List<List<double>> dynamicList)
        {
            

            List<int> result = new List<int>();
            if(dynamicList.Count>0)
            {
                int minIndexX = 0;
                int minIndexY = dynamicList[0].Count - 1;
                double min = 10000000;
               
#region find minimum at th end row
                for (int i = 0; i < dynamicList.Count; i++)
                {
                    if (dynamicList[i][dynamicList[i].Count-1] < min)
                    {
                        minIndexX = i;
                        min = dynamicList[i][dynamicList[i].Count - 1];
                    }
                }
                result.Add(minIndexX);
#endregion
#region Find the minimum of three possible indices
                int indexX=minIndexX;
                while (minIndexY>0)
                {
                    minIndexY--;
                    int indexX0=indexX-1;
                    int indexX1=indexX;
                    int indexX2=indexX+1;
                    if (indexX0 > -1 && indexX2 < dynamicList.Count)
                    {
                        if (dynamicList[indexX0][minIndexY] <= dynamicList[indexX1][minIndexY] && dynamicList[indexX0][minIndexY] <= dynamicList[indexX2][minIndexY])
                        {                            
                            indexX = indexX0;
                            result.Add(indexX);
                        }
                        else if (dynamicList[indexX1][minIndexY] <= dynamicList[indexX0][minIndexY] && dynamicList[indexX1][minIndexY] <= dynamicList[indexX2][minIndexY])
                        {                            
                            indexX = indexX1;
                            result.Add(indexX);
                        }
                        else if (dynamicList[indexX2][minIndexY] <= dynamicList[indexX0][minIndexY] && dynamicList[indexX2][minIndexY] <= dynamicList[indexX1][minIndexY])
                        {                            
                            indexX = indexX2;
                            result.Add(indexX);
                        }
                    }
                    else if(indexX0 > -1 && indexX2 >= dynamicList.Count)
                    {
                        if (dynamicList[indexX0][minIndexY] < dynamicList[indexX1][minIndexY])
                        {                            
                            indexX = indexX0;
                            result.Add(indexX);
                        }
                        else{indexX=indexX1;
                        result.Add(indexX);}
                    }
                    else
                    {
                         if (dynamicList[indexX2][minIndexY] < dynamicList[indexX1][minIndexY])
                        {                            
                            indexX = indexX2;
                            result.Add(indexX);
                        }
                        else{indexX=indexX1;
                        result.Add(indexX);}
                    }

                }
#endregion
                

            }
            return result;
        }
        public Bitmap MarkSeam_vertical( Bitmap b)
        {
            List<int> seam = new List<int>();
            List<List<double>> energy = new List<List<double>>();
            energy = EnergyImage(b);//finds the energy of each image
            seam = FindMinimumColumn(DynamicProgramming_VerticalSeam(energy, b));
            if (seam.Count != b.Height)
                MessageBox.Show("image and vertical seam has not the same length in markSeam_Vertical function!");
            for (int i = 0; i < seam.Count;i++)
            {
                
                b.SetPixel(seam[i], b.Height - i-1, Color.Pink);
            }
            return b;

        }
        public Bitmap DeleteSeam_Vertical(Bitmap b)
        {
            Bitmap result = new Bitmap(b.Width-1, b.Height);
             List<int> seam = new List<int>();
             List<List<double>> energy = new List<List<double>>();
             energy = EnergyImage(b);//finds the energy of each image
             seam = FindMinimumColumn(DynamicProgramming_VerticalSeam(energy, b));
            int indexI = 0;
            for (int j = b.Height-1; j > -1; j--)
            {
                indexI = -1; ;
                for (int i = 0; i < b.Width; i++)
                {
                    
                    if (i != seam[b.Height-j-1])
                    {
                        indexI++;
                        result.SetPixel(indexI,j,b.GetPixel(i,j));
                    }
                }
            }
            return result;
        }
        public Bitmap sample_generator()
        {
            Bitmap b = new Bitmap(8,8);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    b.SetPixel(i, j, Color.White);
                }
            }
                b.SetPixel(0, 0, Color.Red);
            b.SetPixel(0, 1, Color.Green);
            b.SetPixel(0, 2, Color.Blue);
            b.SetPixel(0, 3, Color.Pink);
            b.SetPixel(0, 4, Color.Red);
            b.SetPixel(0, 5, Color.Green);
            b.SetPixel(0, 6, Color.Blue);
            b.SetPixel(0, 7, Color.Pink);
            b.SetPixel(7, 0, Color.Red);
            b.SetPixel(7, 1, Color.Green);
            b.SetPixel(7, 2, Color.Blue);
            b.SetPixel(7, 3, Color.Pink);
            b.SetPixel(7, 4, Color.Red);
            b.SetPixel(7, 5, Color.Green);
            b.SetPixel(7, 6, Color.Blue);
            b.SetPixel(7, 7, Color.Pink);

            b.SetPixel(1, 0, Color.Gray);
            b.SetPixel(1, 1, Color.Pink);
            b.SetPixel(1, 2, Color.Red);
            b.SetPixel(1, 3, Color.Green);
            b.SetPixel(1, 4, Color.Red);
            b.SetPixel(1, 5, Color.Blue);
            b.SetPixel(1, 6, Color.Red);
            b.SetPixel(1, 7, Color.Green);
            b.SetPixel(6, 0, Color.Gray);
            b.SetPixel(6, 1, Color.Pink);
            b.SetPixel(6, 2, Color.Red);
            b.SetPixel(6, 3, Color.Green);
            b.SetPixel(6, 4, Color.Red);
            b.SetPixel(6, 5, Color.Blue);
            b.SetPixel(6, 6, Color.Red);
            b.SetPixel(6, 7, Color.Green);

            b.SetPixel(1, 0, Color.Green);
            b.SetPixel(1, 1, Color.Red);
            b.SetPixel(1, 2, Color.Pink);
            b.SetPixel(1, 3, Color.Gray);
            b.SetPixel(1, 4, Color.Green);
            b.SetPixel(1, 5, Color.Red);
            b.SetPixel(1, 6, Color.Pink);
            b.SetPixel(1, 7, Color.Blue);

            return b;
        }
        public List<List<double>> DynamicProgramming_HorizontalSeam(List<List<double>> energy,Bitmap b)
        {
            List<List<double>> results = new List<List<double>>();
            #region array_making
            for (int i = 0; i < b.Width; i++)
            {
                List<double> temp = new List<double>();
                for (int j = 0; j < b.Height; j++)
                {
                    temp.Add(energy[i][j]);
                }
                results.Add(temp);
            }
            #endregion
            #region dynamic filling
            for (int i = 1; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    double EnergyTemp0 = 1000000;
                    double EnergyTemp1 = 1000000;
                    double EnergyTemp2 = 1000000;
                    double EnergyTotal = 1000000;
                    int indexI = i - 1;
                    int indexJ = j - 1;
                    if (indexI > -1 && indexJ > -1 && indexI < b.Width && indexJ < b.Height)
                        EnergyTemp0 = results[indexI][indexJ];
                    indexI = i-1;
                    indexJ = j ;
                    if (indexI > -1 && indexJ > -1 && indexI < b.Width && indexJ < b.Height)
                        EnergyTemp1 = results[indexI][indexJ];
                    indexI = i -1;
                    indexJ = j +1;
                    if (indexI > -1 && indexJ > -1 && indexI < b.Width && indexJ < b.Height)
                        EnergyTemp2 = results[indexI][indexJ];
                    EnergyTotal = Math.Min(Math.Min(EnergyTemp0, EnergyTemp1), Math.Min(EnergyTemp1, EnergyTemp2));
                    results[i][j] = results[i][j] + EnergyTotal;
                }
            }
            #endregion
            return results;


        }
        public List<int> FindMinimumRow(List<List<double>> dynamicList)
        {


            List<int> result = new List<int>();
            if (dynamicList.Count > 0)
            {
                int minIndexY = 0;
                int minIndexX = dynamicList.Count - 1;
                double min = 10000000;

                #region find minimum at th end row
                for (int i = 0; i < dynamicList[0].Count; i++)
                {
                    if (dynamicList[dynamicList.Count - 1][i] < min)
                    {
                        minIndexY = i;
                        min = dynamicList[dynamicList.Count - 1][i];
                    }
                }
                result.Add(minIndexY);
                #endregion
                #region Find the minimum of three possible indices
                int indexY = minIndexY;
                while (minIndexX > 0)
                {
                    minIndexX--;
                    int indexY0 = indexY - 1;
                    int indexY1 = indexY;
                    int indexY2 = indexY + 1;
                    if (indexY0 > -1 && indexY2 < dynamicList[0].Count)
                    {
                        if (dynamicList[minIndexX][indexY0] <= dynamicList[minIndexX][indexY1] && dynamicList[minIndexX][indexY0] <= dynamicList[minIndexX][indexY2])
                        {
                            indexY = indexY0;
                            result.Add(indexY);
                        }
                        else if (dynamicList[minIndexX][indexY1] <= dynamicList[minIndexX][indexY0] && dynamicList[minIndexX][indexY1] <= dynamicList[minIndexX][indexY2])
                        {
                            indexY = indexY1;
                            result.Add(indexY);
                        }
                        else if (dynamicList[minIndexX][indexY2] <= dynamicList[minIndexX][indexY0] && dynamicList[minIndexX][indexY2] <= dynamicList[minIndexX][indexY1])
                        {
                            indexY = indexY2;
                            result.Add(indexY);
                        }
                    }
                    else if (indexY0 > -1 && indexY2 >= dynamicList[0].Count)
                    {
                        if (dynamicList[minIndexX][indexY0] < dynamicList[minIndexX][indexY1])
                        {
                            indexY = indexY0;
                            result.Add(indexY);
                        }
                        else
                        {
                            indexY = indexY1;
                            result.Add(indexY);
                        }
                    }
                    else
                    {
                        if (dynamicList[minIndexX][indexY2] < dynamicList[minIndexX][indexY1])
                        {
                            indexY = indexY2;
                            result.Add(indexY);
                        }
                        else
                        {
                            indexY = indexY1;
                            result.Add(indexY);
                        }
                    }

                }
                #endregion


            }
            return result;
        }
        public Bitmap MarkSeam_Horizontal(Bitmap b)
        {
            List<int> seam = new List<int>();
            seam = FindMinimumRow(DynamicProgramming_HorizontalSeam(EnergyImage(b),b));
            if (seam.Count != b.Width)
                MessageBox.Show("image and vertical seam has not the same length in markSeam_Vertical function!");
            for (int i = 0; i < seam.Count; i++)
            {

                b.SetPixel( b.Width - i - 1,seam[i], Color.Pink);
            }
            return b;

        }
        public Bitmap DeleteSeam_Horizontal(Bitmap b)
        {
            Bitmap result = new Bitmap(b.Width, b.Height-1);
            List<int> seam = new List<int>();
            seam = FindMinimumRow(DynamicProgramming_HorizontalSeam(EnergyImage(b),b));
            int indexJ = 0;
            for (int j = b.Width - 1; j > -1; j--)
            {
                indexJ = -1; ;
                for (int i = 0; i < b.Height; i++)
                {

                    if (i != seam[b.Width - j - 1])
                    {
                        indexJ++;
                        result.SetPixel(j,indexJ, b.GetPixel(j, i));
                    }
                }
            }
            return result;
        }
        public Bitmap DeletSeam_Vertical_List(Bitmap b, int numberColumns)
        {
            Bitmap result = new Bitmap(b.Width - numberColumns, b.Height);
            List<List<int>> seam = new List<List<int>>();
            List<List<double>> energy = new List<List<double>>();
            energy = EnergyImage(b);//finds the energy of each image
            List<List<double>> dynamicList = DynamicProgramming_VerticalSeam(energy,b);
            for (int number = 0; number < numberColumns; number++)
            {
                List<int>temp=new List<int> ();
                temp = FindMinimumColumn(dynamicList);
                seam.Add(temp);
                dynamicList=Update_DynamicList(energy, seam);
            }
            

                int indexI = 0;
                for (int j = b.Height - 1; j > -1; j--)
                {
                    indexI = -1; ;
                    for (int i = 0; i < b.Width; i++)
                    {
                        List<double> temp=new List<double> ();
                        for(int k=0;k<numberColumns;k++)
                        {    
                            
                            temp.Add(seam[k][b.Height - j - 1]);
                        }
                            if (temp.Contains(i)!=true)
                            {
                                indexI++;
                                result.SetPixel(indexI, j, b.GetPixel(i, j));
                            }
                        }

                }
            
            return result;
        }
        public List<List<double>> Update_DynamicList(List<List<double>>energy,List<List<int>> seam)
        {
            List<List<double>> results = new List<List<double>>();
           
            for (int i = 0; i < seam[0].Count; i++)
            {
                for (int k = 0; k < seam.Count;k++)
                    energy[seam[k][i]][energy[0].Count - i - 1] = 100000;
            }
            #region array_making
            for (int i = 0; i < energy.Count; i++)
            {
                List<double> temp = new List<double>();
                for (int j = 0; j < energy[0].Count; j++)
                {
                    temp.Add(energy[i][j]);
                }
                results.Add(temp);
            }
            #endregion
            #region dynamic filling
            for (int j = 1; j < energy[0].Count; j++)
            {
                for (int i = 0; i < energy.Count; i++)
                {
                    double EnergyTemp0 = 100000;
                    double EnergyTemp1 = 100000;
                    double EnergyTemp2 = 100000;
                    double EnergyTotal = 100000;
                    int indexI = i - 1;
                    int indexJ = j - 1;
                    if (indexI > -1 && indexJ > -1 && indexI < energy.Count && indexJ < energy[0].Count)
                        EnergyTemp0 = results[indexI][indexJ];
                    indexI = i;
                    indexJ = j - 1;
                    if (indexI > -1 && indexJ > -1 && indexI < energy.Count && indexJ < energy[0].Count)
                        EnergyTemp1 = results[indexI][indexJ];
                    indexI = i + 1;
                    indexJ = j - 1;
                    if (indexI > -1 && indexJ > -1 && indexI < energy.Count && indexJ < energy[0].Count)
                        EnergyTemp2 = results[indexI][indexJ];
                    EnergyTotal = Math.Min(Math.Min(EnergyTemp0, EnergyTemp1), Math.Min(EnergyTemp1, EnergyTemp2));
                    results[i][j] = results[i][j] + EnergyTotal;
                }
            }
            #endregion
            return results;

        }

        public Bitmap DeletSeam_Horizontal_List(Bitmap b, int numberRows)
        {
            Bitmap result = new Bitmap(b.Width, b.Height - numberRows);
            List<List<int>> seam = new List<List<int>>();
            List<List<double>> energy = new List<List<double>>();
            energy = EnergyImage(b);//finds the energy of each image
            List<List<double>> dynamicList = DynamicProgramming_VerticalSeam(energy, b);
            for (int number = 0; number < numberRows; number++)
            {
                List<int> temp = new List<int>();
                temp = FindMinimumRow(dynamicList);
                seam.Add(temp);
                dynamicList = Update_DynamicList_Horizontal(energy, seam);
            }


            int indexJ = 0;
            for (int j = b.Width - 1; j > -1; j--)
            {
                indexJ = -1; ;
                for (int i = 0; i < b.Height; i++)
                {
                    List<double> temp = new List<double>();
                    for (int k = 0; k < numberRows; k++)
                    {

                        temp.Add(seam[k][b.Width - j - 1]);
                    }
                    if (temp.Contains(i) != true)
                    {
                        indexJ++;
                        result.SetPixel(j,indexJ, b.GetPixel(j,i));
                    }
                }

            }

            return result;
        }
        public List<List<double>> Update_DynamicList_Horizontal(List<List<double>> energy, List<List<int>> seam)
        {
            List<List<double>> results = new List<List<double>>();

            for (int i = 0; i < seam[0].Count; i++)
            {
                for (int k = 0; k < seam.Count; k++)
                    energy[energy.Count - i - 1][seam[k][i]] = 100000;
            }
            #region array_making
            for (int i = 0; i < energy.Count; i++)
            {
                List<double> temp = new List<double>();
                for (int j = 0; j < energy[0].Count; j++)
                {
                    temp.Add(energy[i][j]);
                }
                results.Add(temp);
            }
            #endregion
            #region dynamic filling
            for (int i = 1; i < energy.Count; i++)
            {
                for (int j = 0; j < energy[0].Count; j++)
                {
                    double EnergyTemp0 = 1000000;
                    double EnergyTemp1 = 1000000;
                    double EnergyTemp2 = 1000000;
                    double EnergyTotal = 1000000;
                    int indexI = i - 1;
                    int indexJ = j - 1;
                    if (indexI > -1 && indexJ > -1 && indexI < energy.Count && indexJ < energy[0].Count)
                        EnergyTemp0 = results[indexI][indexJ];
                    indexI = i - 1;
                    indexJ = j;
                    if (indexI > -1 && indexJ > -1 && indexI < energy.Count && indexJ < energy[0].Count)
                        EnergyTemp1 = results[indexI][indexJ];
                    indexI = i - 1;
                    indexJ = j + 1;
                    if (indexI > -1 && indexJ > -1 && indexI < energy.Count && indexJ < energy[0].Count)
                        EnergyTemp2 = results[indexI][indexJ];
                    EnergyTotal = Math.Min(Math.Min(EnergyTemp0, EnergyTemp1), Math.Min(EnergyTemp1, EnergyTemp2));
                    results[i][j] = results[i][j] + EnergyTotal;
                }
            }
            #endregion
            return results;

        }
    }
}
