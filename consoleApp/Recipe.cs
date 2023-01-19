using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecipe00
{
    public class Recipe
    {
        public string FieldName { get; set; }
        public int Product_e { get; set; }
        public float ProductCostPerKg { get; set; }
        public float ProductExtraCost { get; set; }
        public float ProductT1SP { get; set; }
        public int ProductTargetSpeed { get; set; }
        public int ProductTolerance { get; set; }
        public int ProductWeight { get; set; }
        public int CountThreshold { get; set; }
        public int AdjMax { get; set; }
        public int AdjMin { get; set; }
        public int AdjQtPercStart { get; set; }
        public bool AutoAdjustON { get; set; }
        public int AdjStartWeigh { get; set; }
        public int CwProgramNumber { get; set; }
        public bool CwBypass { get; set; }
        public int Tare { get; set; }

        public int WeightSP { get; set; }
        public float Moisture { get; set; }
        public int TolMin { get; set; }
    }
}
