using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gra
{
    public class Results
    {
        public int[] Result { get; set; }

        public Results()
        {
            Result = new int[4];
            for(int i=0;i<4;i++)
            {
                Result[i] = 0;
            }
        }

        public void addPoint(int index)
        {
            Result[index] += 1;
        }
    }
}
