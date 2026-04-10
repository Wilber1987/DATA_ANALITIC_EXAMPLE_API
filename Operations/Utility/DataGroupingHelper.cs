using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Operations.Utility
{


    public class GroupingResult
    {
        public object GroupedData { get; set; }
        public Dictionary<string, Dictionary<string, SummaryMetric>> MetricLevels { get; set; }
    }

    public class SummaryMetric
    {
        public decimal? Sum { get; set; }
        public int Count { get; set; }
        public decimal Avg { get; set; }
    }

    public class ModelProperty
    {
        public string Type { get; set; } // "MONEY", "NUMBER", "TEXT", etc.
    }

    public static class DataGroupingHelper
    {
        public static object GroupData<T>(
            IEnumerable<T> data,
            List<string> groupParams,
            List<string> evalParams,
            Dictionary<string, ModelProperty> modelObject,
            string title,
            bool isFinalGroupedData = false)
        {
            var metricLevels = new Dictionary<string, object>();

            if (groupParams == null || groupParams.Count == 0)
            {
                metricLevels[title ?? "Reporte"] = CalculateSummary(data.Cast<object>().ToList(), null, evalParams, modelObject, isFinalGroupedData);

                return new Dictionary<string, object>
                {
                    [title ?? "Reporte"] = data
                };
            }

            var groupedData = new Dictionary<string, object>();

            foreach (var item in data)
            {
                object currentLevel = groupedData;
                List<string> path = new List<string>();
                string metricKey = groupParams.Last();

                for (int i = 0; i < groupParams.Count; i++)
                {
                    var param = groupParams[i];

                    var value = GetPropertyValue(item, param)?.ToString() ?? "Undefined";

                    var dict = currentLevel as Dictionary<string, object>;

                    if (!dict.ContainsKey(value))
                    {
                        dict[value] = (i == groupParams.Count - 1)
                            ? new List<object>()
                            : new Dictionary<string, object>();
                    }

                    currentLevel = dict[value];
                }


                var list = currentLevel as List<object>;

                if (isFinalGroupedData && list != null)
                {
                    var existing = list.FirstOrDefault(it =>
                        GetPropertyValue(it, metricKey)?.ToString() ==
                        GetPropertyValue(item, metricKey)?.ToString()
                    );

                    if (existing == null)
                    {
                        var newItem = ToDictionary(item);
                        newItem["count"] = 1;
                        list.Add(newItem);
                    }
                    else
                    {
                        var dictExisting = existing as Dictionary<string, object>;
                        dictExisting["count"] = Convert.ToInt32(dictExisting["count"]) + 1;

                        foreach (var param in evalParams)
                        {
                            var val = Convert.ToDouble(dictExisting[param]);
                            var newVal = Convert.ToDouble(GetPropertyValue(item, param) ?? 0);
                            dictExisting[param] = val + newVal;
                        }
                    }
                }
                else
                {
                    list?.Add(item);
                }
            }

            // Procesar métricas
            List<object> ProcessGroup(object group, List<string> path)
            {
                var allItems = new List<object>();

                var dict = group as Dictionary<string, object>;

                foreach (var key in dict.Keys)
                {
                    var currentPath = new List<string>(path) { key };

                    if (dict[key] is List<object> list)
                    {
                        metricLevels[string.Join(" > ", currentPath)] =
                            CalculateSummary(list, data.Cast<object>().ToList(), evalParams, modelObject, isFinalGroupedData);

                        allItems.AddRange(list);
                    }
                    else
                    {
                        var subItems = ProcessGroup(dict[key], currentPath);
                        allItems.AddRange(subItems);
                    }
                }

                if (allItems.Count > 0)
                {
                    metricLevels[string.Join(" > ", path)] =
                        CalculateSummary(allItems, data.Cast<object>().ToList(), evalParams, modelObject, isFinalGroupedData);
                }

                return allItems;
            }

            ProcessGroup(groupedData, new List<string>());

            metricLevels["General Summary"] =
                CalculateSummary(data.Cast<object>().ToList(), null, evalParams, modelObject, isFinalGroupedData);

            return new
            {
                groupedData,
                metricLevels
            };
        }

        // =========================
        // SUMMARY
        // =========================
        private static Dictionary<string, object> CalculateSummary(
            List<object> data,
            List<object> parentData,
            List<string> evalParams,
            Dictionary<string, ModelProperty> modelObject,
            bool isFinalGroupedData)
        {
            var summary = new Dictionary<string, object>();

            foreach (var param in evalParams)
            {
                bool isWithModel = modelObject != null && modelObject.ContainsKey(param);
                bool isMoney = isWithModel && modelObject[param].Type?.ToUpper() == "MONEY";
                bool isNumber = isWithModel && modelObject[param].Type?.ToUpper() == "NUMBER";

                double? totalSum = null;

                if (isMoney || isNumber)
                {
                    totalSum = data.Sum(item =>
                    {
                        var val = GetPropertyValue(item, param);
                        return val != null ? Convert.ToDouble(val) : 0;
                    });
                }

                int totalElements = parentData?.Count ?? data.Count;

                int validCount = isFinalGroupedData
                    ? Convert.ToInt32(GetPropertyValue(data[0], "count") ?? 0)
                    : data.Count(item => GetPropertyValue(item, param) != null);

                double avg = totalElements > 0
                    ? (double)validCount / totalElements * 100
                    : 0;

                var metric = new Dictionary<string, object>
                {
                    ["count"] = validCount,
                    ["avg"] = avg
                };

                if (totalSum.HasValue)
                    metric["sum"] = totalSum.Value;

                summary[param] = metric;
            }

            return summary;
        }

        // =========================
        // HELPERS
        // =========================
        private static object GetPropertyValue(object obj, string prop)
        {
            if (string.IsNullOrEmpty(prop))
                return null;

            if (obj is Dictionary<string, object> dict)
                return dict.ContainsKey(prop) ? dict[prop] : null;

            var property = obj.GetType().GetProperty(prop);
            return property?.GetValue(obj);
        }

        private static Dictionary<string, object> ToDictionary(object obj)
        {
            return obj.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(obj)!);
        }
    }

}