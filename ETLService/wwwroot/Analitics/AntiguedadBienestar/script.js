//@ts-check
import { WBarChart } from "../../WDevCore/WComponents/ChartsComponents/WBarChar.js";
import { WAjaxTools } from "../../WDevCore/WModules/WAjaxTools.js"


window.onload = async () => {
    const GroupParams = [
        "Anio", "Rango_Antiguedad", "Estado_Final_Color"
    ]
    const EvalParams = [
        "Estado_Final_Color"
    ];
    const request = {
        "Desde": "2025-04-05T04:15:41.242Z",
        "Hasta": "2027-04-05T04:15:41.242Z",
        "GroupParams": GroupParams,
        "EvalParams": EvalParams
    }

    const response = await WAjaxTools.PostRequest("/api/ApiAnalitic/AntiguedadBienestar", request);
    document.body.querySelector("#mainContent")?.append(new WBarChart({
        // @ts-ignore
        data: response,
        GroupParams: GroupParams,
        EvalParams: EvalParams,
        title: 'Bienestar X Antiguedad',
        Colors : ["#27e512", "#e56412", "#e51212"]
    }));
}