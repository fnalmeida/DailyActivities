using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DA_GrpcService;
using Google.Protobuf.WellKnownTypes;

namespace DA_GrpcService.Utils
{
    

    public class DecimalConverter
    {
        public static DecimalValue ConvertToDecimalValue(decimal value)
        {
            var bits = decimal.GetBits(value);
            return new DecimalValue
            {
                Units = bits[0],
                Nanos = bits[1]
            };
        }

        public static decimal ConvertFromDecimalValue(DecimalValue decimalValue)
        {
            return new decimal(new int[] { (int)decimalValue.Units, 0, 0, decimalValue.Nanos });
        }
    }

}
