using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APPCORE;
using Operations.AnaliticOperations.Model;

namespace Operations.AnaliticOperations
{
    public class AnaliticAbsentismoOperation
    {
        public static List<V_Analisis_Absentismo_Predictor> GetByPeriodo(DateTime desde, DateTime hasta)
        {
            return new V_Analisis_Absentismo_Predictor().Where<V_Analisis_Absentismo_Predictor>(
                FilterData.GreaterEqual("Fecha", desde),
                 FilterData.LessEqual("Fecha", hasta)
            );
        }
    }
}