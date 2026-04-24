using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APPCORE;
using Operations.AnaliticOperations.Model;
using Operations.Utility;

namespace Operations.AnaliticOperations
{
    public class AnaliticAntiguedadBienestarOperation
    {
        static readonly Dictionary<string, ModelProperty> ModelObject = new Dictionary<string, ModelProperty>
        {
            ["Estado_Inicial_Valor"] = new ModelProperty { Type = "NUMBER" },
            ["Estado_Final_Valor"] = new ModelProperty { Type = "NUMBER" },
            ["Estado_Final_Color"] = new ModelProperty { Type = "TEXT" },
            ["Estado_Final_Etiqueta"] = new ModelProperty { Type = "TEXT" }
        };
        public static List<V_Analisis_Antiguedad_Bienestar> GetByPeriodo(DateTime desde, DateTime hasta)
        {
            return new V_Analisis_Antiguedad_Bienestar().Where<V_Analisis_Antiguedad_Bienestar>(
                FilterData.GreaterEqual("Fecha", desde),
                 FilterData.LessEqual("Fecha", hasta)
            );
        }
        public static object? GetByPeriodo(DataAnaliticRequest request)
        {
            // Consulta a la vista/entidad
            var bdData = new V_Analisis_Antiguedad_Bienestar().Where<V_Analisis_Antiguedad_Bienestar>(
                FilterData.GreaterEqual("Fecha", request.Desde),
                FilterData.LessEqual("Fecha", request.Hasta)
            ); // 👈 Importante: materializar la consulta

            // Ejecución del helper genérico
            var result = DataGroupingHelper.GroupData(
                data: bdData,
                groupParams: request.GroupParams,
                evalParams: request.EvalParams,
                modelObject: ModelObject,
                title: "Test",
                isFinalGroupedData: true
            );

            return result;
        }
    }
}