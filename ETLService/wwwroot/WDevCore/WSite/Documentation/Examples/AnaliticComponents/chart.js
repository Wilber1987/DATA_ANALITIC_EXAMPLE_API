import { ColumChart } from "../../../../../WDevCore/WComponents/WChartJSComponents.js";
import { data } from "../data.js";
/**TRIPLE AGRUPACIóN*/
document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 1 - Triple agrupación",
        Dataset: data,
        AttNameEval: "homeTown",
        EvalValue: "value", // suma el valor del value para obtener la sumatoria completa del array
        groupParams: ["year", "trimestre", "mes"],
    })
)
document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 2 - Doble Agrupación",
        Dataset: data,
        AttNameEval: "homeTown",
        groupParams: ["year", "trimestre"],
    })
)
document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 3 - Mono Agrupación",
        Dataset: data,
        AttNameEval: "homeTown",
        groupParams: ["year"],
    })
)
document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 4 - Mono Agrupación",
        Dataset: data,
        AttNameEval: "secretBase",
        percentCalc: true,
        groupParams: ["year"],
    })
)
document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 5 - Stakef Agrupación",
        Dataset: data,
        AttNameEval: "secretBase",
        percentCalc: true,
        TypeChart: "STACKED",
        groupParams: ["year", "trimestre"],
    })
)

document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 1 - Mono Agrupación",
        TypeChart: "Line",
        Dataset: data,
        AttNameEval: "homeTown",
        groupParams: [ "year", "mes"],
    })
)
document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 1 - Mono Agrupación",
        TypeChart: "Line",
        Dataset: data,
        AttNameEval: "homeTown",
        groupParams: [ "year", "trimestre", "mes"],
    })
)
document.querySelector("main").append(
    new ColumChart({
        Title: "CHART 1 - Triple agrupación",
        Dataset: data,
        AttNameEval: "homeTown",
       // EvalValue: "value", // suma el valor del value para obtener la sumatoria completa del array
        groupParams: ["year", "trimestre", "mes"],
    })
)