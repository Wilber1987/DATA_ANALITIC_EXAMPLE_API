//@ts-check
//import { WAjaxTools } from "../WDevCore/WModules/WAjaxTools";

import { WTableDynamicComp } from "../WDevCore/WComponents/WTableDynamic.js";
// @ts-ignore
import { ModelProperty } from "../WDevCore/WModules/CommonModel.js";

window.onload = async () => {
    //ASUMIMOS QUE ESTA PETICION FETCH SERA A UNA API QUE POSTERIORMENTE DARA LOS DATOS PROCESADOS QUE SE MOSTRARAN EN EL DASHBOARD
    const dataPromise = await fetch("../Resource/data.json");
    const data = await dataPromise.json();

    //const data2  = await WAjaxTools.GetRequest("../Resource/data.json");
    console.log(data);

    class ModelObject {
    /**@type {ModelProperty}*/ product = { type: 'draw' };
    /**@type {ModelProperty}*/ category = {
            type: 'wselect',
            ModelObject:
                { category: { type: "text", primary: true }, desc: { type: "text" } },
            Dataset: data.map(d => ({ category: d.category, desc: d.category }))
        };
    /**@type {ModelProperty}*/ year = { type: 'select' };
    /**@type {ModelProperty}*/ mes = { type: 'select' };
    /**@type {ModelProperty}*/ quarter = { type: 'text' };
    /**@type {ModelProperty}*/ units_sold = { type: 'number', hiddenInTable: true };
    /**@type {ModelProperty}*/ unit_price = { type: 'money', hiddenInTable: true };
    /**@type {ModelProperty}*/ total_sale = { type: 'money', hiddenInTable: true };
    }


    const TableConfigG = {
        Dataset: data,
        EvalValue: "total_sale",
        AttNameEval: "category",
        groupParams: ["year"],
        AddChart: true,
        ModelObject: new ModelObject()
    };
    const WTableReport = new WTableDynamicComp(TableConfigG);
    //APP ES EL DIV CON ID="app" dentro de la pagina de razor index.cshtml la cual es solo para un dashboard de prueba
    app.append(WTableReport)
}


