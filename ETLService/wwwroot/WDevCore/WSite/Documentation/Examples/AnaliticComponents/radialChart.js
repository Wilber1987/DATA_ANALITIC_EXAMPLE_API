import { RadialChart } from "../../../../../WDevCore/WComponents/WChartJSComponents.js";
import { data } from "../data.js";

document.querySelector("main").append(
    new RadialChart({
        Title: "CHART 1",
        Dataset: data,
        AttNameEval: "homeTown"
    })
)
document.querySelector("main").append(
    new RadialChart({
        Title: "CHART 2",
        Dataset: data,
        percentCalc: true,        
        AttNameEval: "homeTown"
    })
) 
document.querySelector("main").append(
    new RadialChart({
        Title: "CHART 3",
        Dataset: data,
        percentCalc: true,        
        AttNameEval: "homeTown",
        EvalValue: "value"
    })
)