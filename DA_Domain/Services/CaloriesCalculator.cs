using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA_Domain.Services
{
    public class CaloriesCalculator
    {
        public static decimal Calculator(decimal weight, decimal met, decimal duration)
        {
            return met * met * duration;
        }
    }
}
