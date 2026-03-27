import { ComponentsManager, WRender } from "../../../../../WDevCore/WModules/WComponentsTools.js";
import { WAppNavigator } from "../../../../../WDevCore/WComponents/WAppNavigator.js";
import { WTableComponent } from "../../../../../WDevCore/WComponents/WTableComponent.js";
import { data, ModelObject } from "../data.js";

const tab = WRender.Create({ className: "tab" })
const navigator = new WAppNavigator({
    Direction: "column",
    Inicialize: true,
    Elements: [{
        id: "Tab-dasboard", name: "Tabla basica", action: async (ev) => {
            DOMManager.NavigateFunction("Tab-dasboard", new WTableComponent({
                Dataset: data,
                ModelObject: new ModelObject(),
            }));
        }
    }, {
        id: "Tab-Generales", name: "Tabla con modelo",
        action: async (ev) => {
            DOMManager.NavigateFunction("Tab-Generales", new WTableComponent({
                Dataset: data,
                ModelObject: new ModelObject()
            }));
        }
    }, {
        id: "Tab-Options", name: "Tabla con modelo y opciones customizadas",
        action: async (ev) => {
            DOMManager.NavigateFunction("Tab-Options", new WTableComponent({
                Dataset: data,
                ModelObject: new ModelObject(),
                Options: {
                    Search: false,
                    Show: false,
                    Edit: false,
                    Filter: true,
                    MultiSelect: true,
                    Delete: true
                }
            }));
        }
    }, {
        id: "Tab-Options2", name: "Tabla con opciones y acciones customizadas",
        action: async (ev) => {
            DOMManager.NavigateFunction("Tab-Options2", new WTableComponent({
                Dataset: data,
                ModelObject: new ModelObject(),
                Options: {
                    Add: true, AddAction: (targetObject) => { alert("se agrego:" + JSON.stringify(targetObject)) },
                    Edit: true, EditAction: (targetObject) => { alert("se edito:" + JSON.stringify(targetObject)) },
                    MultiSelect: true, SelectAction: (targetObject) => { alert("se selecciono:" + JSON.stringify(targetObject)) },
                    Delete: true, DeleteAction: (targetObject) => { alert("se elimino:" + JSON.stringify(targetObject)) }
                }
            }));
        }
    }]
})
const DOMManager = new ComponentsManager({ MainContainer: tab, SPAManage: true, WNavigator: navigator });
document.querySelector("main").append(navigator)
document.querySelector("main").append(tab)


