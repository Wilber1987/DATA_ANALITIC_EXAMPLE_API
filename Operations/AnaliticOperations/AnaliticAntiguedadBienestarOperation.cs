using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APPCORE;
using Operations.AnaliticOperations.Model;

namespace Operations.AnaliticOperations
{
    public class AnaliticAntiguedadBienestarOperation
    {
        public static  List<V_Analisis_Antiguedad_Bienestar> GetByPeriodo(DateTime desde, DateTime hasta)
        {
            return new V_Analisis_Antiguedad_Bienestar().Where<V_Analisis_Antiguedad_Bienestar>(
                FilterData.GreaterEqual("Fecha", desde),
                 FilterData.LessEqual("Fecha", hasta)
            );
        }
    }
}