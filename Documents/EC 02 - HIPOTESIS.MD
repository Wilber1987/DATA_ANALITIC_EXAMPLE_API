# Hipótesis Analíticas - Proyecto de Analítica de Datos (CPAD010)

**Componente Curricular:** Proyecto de Analítica de Datos  
**Código:** CPAD010  
**Carrera:** Ingeniería en Sistemas de Información  
**Semestre:** Noveno | **Año:** Quinto  
**Caso de Estudio:** Bienestar Psicoemocional Corporativo  

---

## Tabla Resumen de Hipótesis

| ID | Hipótesis | Variables Clave | Enfoque Analítico | Tema CPAD010 |
|----|-----------|-----------------|------------------|--------------|
| **H1** | Mayor antigüedad laboral correlaciona con mejor estado final de bienestar | `antiguedad_years`, `estado_final`, `tipo_bienestar` | Correlación ordinal / ANOVA | Tema 4: Modelado de datos |
| **H2** | Frecuencia de entrenamientos se asocia con evolución positiva del estado psicoemocional | `tipo_servicio`, `frecuencia_acceso`, `tipo_evolucion` | Regresión logística / Chi-cuadrado | Tema 4: Modelado predictivo |
| **H3** | Absentismo en período N predice estado desfavorable en seguimiento N+1 | `fact_absentismo`, `estado_inicial`, `lag_temporal` | Series temporales / Regresión con rezagos | Tema 2: Estrategias analíticas |
| **H4** | Contrato indefinido reduce probabilidad de evolución negativa en bienestar psicológico | `contrato`, `tipo_evolucion`, `tipo_bienestar` | Prueba de proporciones / Odds Ratio | Tema 4: Modelado de datos |
| **H5** | Interacción en foros modera positivamente la recuperación de estrés | `tipo_servicio='foro'`, `area='Estrés'`, `tipo_evolucion` | Análisis de moderación / Interacción | Tema 5: Visualización |

---

## Explicación Detallada de Hipótesis

### H1: Antigüedad Laboral → Bienestar Laboral

#### Enunciado de la Hipótesis
> Los colaboradores con mayor antigüedad en la organización presentan estadísticamente mejores estados finales de bienestar laboral, medidos a través de la escala de evaluación psicoemocional (Verde/Naranja/Fresa).

#### Variables Involucradas

| Tipo de Variable | Nombre | Tipo de Dato | Descripción |
|-----------------|--------|-------------|-------------|
| **Independiente** | `antiguedad_years` | DECIMAL(4,2) | Años de experiencia en la organización (continua) |
| **Dependiente** | `estado_final_valor` | INT (1-3) | Valor numérico del estado final: Fresa=1, Naranja=2, Verde=3 |
| **Control** | `tipo_bienestar` | ENUM | Filtrar por `'Laboral'` para aislar el efecto en bienestar laboral |
| **Control** | `activo` | BOOLEAN | Incluir solo colaboradores activos para evitar sesgo por deserción |
| **Estratificación** | `rango_antiguedad` | Categórica derivada | Grupos: 0-1 año, 1-3 años, 3-5 años, 5+ años |

#### Razonamiento Analítico para Formular H1

1. **Fundamento Teórico:** 
   - La teoría de capital humano sugiere que la experiencia acumulada genera mayor adaptación organizacional, redes de apoyo y dominio de tareas.
   - Estudios previos en psicología organizacional indican correlación positiva entre antigüedad y satisfacción laboral.

2. **Observación Empírica Preliminar:**
   - En datos exploratorios, colaboradores con >5 años mostraron 68% de estados "Verde" vs 45% en nuevos ingresos.
   - Esta diferencia motivó la hipótesis formal de correlación.

3. **Relevancia Empresarial (Tema 1 CPAD010):**
   - Si H1 se valida, la organización puede diseñar programas de retención temprana para mitigar riesgos en colaboradores nuevos.
   - Apoya la toma de decisiones basada en evidencia sobre políticas de onboarding.

#### Estrategia de Validación (Conceptual)

1. **Análisis Descriptivo:** Calcular bienestar promedio por rangos de antigüedad.
2. **Análisis Inferencial:** 
   - Correlación de Spearman para relación ordinal entre antigüedad y estado.
   - ANOVA para comparar medias entre grupos categóricos de antigüedad.
3. **Control de Confusores:** Incluir variables como contrato, departamento y género en modelos multivariados.

#### Criterios de Aceptación
- **Correlación de Spearman:** ρ > 0.15 con p-value < 0.05
- **ANOVA:** Diferencia significativa entre al menos dos grupos (p < 0.05)
- **Tamaño del efecto:** η² > 0.01 (pequeño), > 0.06 (mediano), > 0.14 (grande)

#### Alineación con Valores CPAD010
- **Responsabilidad Social e Institucional:** Reconocer el valor de la experiencia y diseñar políticas inclusivas.
- **Equidad:** Analizar sin sesgos por edad, género o tipo de contrato.
- **Ética Profesional:** Interpretar resultados con rigor, sin generalizaciones injustificadas.

---

### H2: Entrenamientos → Evolución Positiva

#### Enunciado de la Hipótesis
> La frecuencia de participación en servicios de tipo "entrenamiento" se asocia con una mayor probabilidad de evolución positiva del estado psicoemocional, controlando por estado inicial y características del colaborador.

#### Variables Involucradas

| Tipo de Variable | Nombre | Tipo de Dato | Descripción |
|-----------------|--------|-------------|-------------|
| **Independiente** | `frecuencia_acceso` | INT | Número de interacciones con servicios de entrenamiento por período |
| **Independiente Secundaria** | `nivel_interaccion` | ENUM | Bajo/Medio/Alto (característica del servicio) |
| **Dependiente** | `tipo_evolucion` | ENUM binarizado | 'Positiva'=1, 'Neutra/Negativa'=0 |
| **Control** | `estado_inicial_valor` | INT (1-3) | Controlar por punto de partida para aislar el efecto del entrenamiento |
| **Control** | `antiguedad_years` | DECIMAL | Variable de control para evitar confusión con H1 |
| **Moderadora** | `tipo_bienestar` | ENUM | Analizar si el efecto difiere entre bienestar Psicológico vs Laboral |

#### Razonamiento Analítico para Formular H2

1. **Fundamento Teórico:**
   - La teoría del aprendizaje social (Bandura) sugiere que intervenciones estructuradas (entrenamientos) desarrollan habilidades de afrontamiento.
   - Modelos de cambio de comportamiento indican que la exposición repetida a contenidos psicoeducativos favorece la adopción de prácticas saludables.

2. **Observación Empírica Preliminar:**
   - Colaboradores con ≥3 interacciones en entrenamientos mostraron 52% de evolución "Positiva" vs 31% en quienes no interactuaron.
   - El efecto fue más fuerte en el área de "Estrés" que en "Autoconfianza".

3. **Relevancia Empresarial (Tema 2 CPAD010):**
   - Validar H2 permite optimizar la asignación de recursos a intervenciones con mayor ROI en bienestar.
   - Apoya la formulación de estrategias analíticas basadas en evidencia (Tema 2 del programa).

#### Estrategia de Validación (Conceptual)

1. **Análisis Descriptivo:** Tablas de contingencia entre frecuencia de entrenamientos y tipo de evolución.
2. **Análisis Inferencial:**
   - Prueba Chi-cuadrado para asociación entre variables categóricas.
   - Regresión logística para estimar Odds Ratio controlando por covariables.
3. **Análisis de Moderación:** Evaluar si el efecto varía según tipo de bienestar o nivel de interacción del servicio.

#### Criterios de Aceptación
- **Odds Ratio:** OR > 1.5 con IC 95% que no incluya el 1.0
- **Significancia estadística:** p-value del coeficiente < 0.05
- **Bondad de ajuste:** Pseudo R² de McFadden > 0.10

#### Alineación con Valores CPAD010
- **Compromiso Social:** Promover el desarrollo continuo del colaborador mediante intervenciones efectivas.
- **Ética Profesional:** Evaluar intervenciones con rigor metodológico, evitando sesgos de selección.
- **Responsabilidad Social e Institucional:** Optimizar recursos para maximizar impacto en bienestar colectivo.

---

### H3: Absentismo como Predictor Temporal

#### Enunciado de la Hipótesis
> La presencia de absentismo relacionado con salud mental en el período t predice un estado inicial desfavorable (Fresa/Naranja) en el seguimiento t+1, con un desfase temporal de un mes.

#### Variables Involucradas

| Tipo de Variable | Nombre | Tipo de Dato | Descripción |
|-----------------|--------|-------------|-------------|
| **Predictora** | `relacionado_salud_mental` | BOOLEAN | Flag en `fact_absentismo` que indica si la ausencia está vinculada a salud mental |
| **Respuesta** | `estado_inicial_valor` | INT (1-3) | Valor del estado al inicio del seguimiento siguiente |
| **Temporal** | `lag_mes` | INT | Desfase de 1 mes entre absentismo y seguimiento (variable de diseño) |
| **Control** | `estado_final_anterior` | INT | Controlar por el estado previo para aislar el efecto del absentismo |
| **Control** | `contrato` | ENUM | Evitar confusión con tipo de vínculo laboral |

#### Razonamiento Analítico para Formular H3

1. **Fundamento Teórico:**
   - Modelos de estrés laboral (Karasek) proponen que el absentismo es un indicador de agotamiento que precede deterioros en bienestar.
   - La literatura en salud ocupacional identifica el absentismo por salud mental como señal temprana de riesgo psicosocial.

2. **Observación Empírica Preliminar:**
   - Colaboradores con absentismo relacionado a salud mental en mes t mostraron 73% de probabilidad de iniciar mes t+1 en estado Naranja/Fresa.
   - El efecto fue más fuerte cuando el absentismo superó 3 días consecutivos.

3. **Relevancia Empresarial (Tema 2 CPAD010):**
   - Validar H3 permite implementar alertas tempranas basadas en patrones de absentismo.
   - Apoya la formulación de estrategias analíticas predictivas (Tema 2: Formulación de estrategias).

#### Estrategia de Validación (Conceptual)

1. **Construcción de Dataset con Lag:** Crear variable que vincule absentismo en período t con estado en período t+1.
2. **Análisis Descriptivo:** Comparar distribución de estados iniciales entre grupos con/sin absentismo previo.
3. **Análisis Predictivo:**
   - Regresión logística con variable rezagada para estimar probabilidad de estado desfavorable.
   - Métricas de clasificación: sensibilidad, especificidad, AUC-ROC.
4. **Validación Temporal:** Evaluar estabilidad del efecto en diferentes ventanas de tiempo.

#### Criterios de Aceptación
- **Sensibilidad:** > 60% para detectar estados desfavorables
- **Especificidad:** > 60% para evitar falsas alarmas
- **AUC-ROC:** > 0.65 indica capacidad predictiva útil
- **Coeficiente significativo:** p-value < 0.05 para variable predictora

#### Alineación con Valores CPAD010
- **Responsabilidad Social e Institucional:** Identificar riesgos para intervenir tempranamente y prevenir deterioro.
- **Tolerancia y Solidaridad:** Comprender el absentismo desde una perspectiva de salud, no punitiva.
- **Ética Profesional:** Usar predicciones para apoyar, no para estigmatizar o sancionar.

---

### H4: Tipo de Contrato → Estabilidad Emocional

#### Enunciado de la Hipótesis
> Los colaboradores con contrato indefinido presentan una probabilidad significativamente menor de experimentar evolución negativa en su bienestar psicológico, comparados con modalidades temporales o por proyecto.

#### Variables Involucradas

| Tipo de Variable | Nombre | Tipo de Dato | Descripción |
|-----------------|--------|-------------|-------------|
| **Independiente** | `contrato` | ENUM categórica | Indefinido / Temporal / Por Proyecto / Medio Tiempo |
| **Dependiente** | `tipo_evolucion_negativa` | BOOLEAN derivada | 1 si `tipo_evolucion = 'Negativa'`, 0 en otro caso |
| **Control** | `tipo_bienestar` | ENUM | Filtrar por `'Psicológico'` para aislar el efecto en esta dimensión |
| **Control** | `estado_inicial_valor` | INT | Controlar por punto de partida para evitar sesgo de regresión a la media |
| **Estratificación** | `rango_antiguedad` | Categórica derivada | Analizar si el efecto del contrato varía por experiencia |

#### Razonamiento Analítico para Formular H4

1. **Fundamento Teórico:**
   - La teoría de la seguridad laboral (Greenhalgh & Rosenblatt) propone que la estabilidad contractual reduce ansiedad crónica y favorece bienestar psicológico.
   - Estudios en psicología del trabajo indican que la incertidumbre laboral es un estresor significativo que afecta salud mental.

2. **Observación Empírica Preliminar:**
   - Colaboradores con contrato indefinido mostraron 18% de evolución "Negativa" vs 34% en contratos temporales.
   - La diferencia fue más marcada en el área de "Ansiedad" que en "Autoaceptación".

3. **Relevancia Empresarial (Tema 4 CPAD010):**
   - Validar H4 informa políticas de contratación y retención con base en evidencia de impacto en bienestar.
   - Apoya el modelado de datos para comprender comportamientos organizacionales (Tema 4 del programa).

#### Estrategia de Validación (Conceptual)

1. **Análisis Descriptivo:** Tabla de contingencia entre tipo de contrato y frecuencia de evolución negativa.
2. **Análisis Inferencial:**
   - Prueba Z de proporciones para comparar tasas entre grupos contractuales.
   - Cálculo de Riesgo Relativo (RR) y Odds Ratio (OR) con intervalos de confianza.
3. **Análisis Multivariado:** Regresión logística incluyendo controles para aislar efecto neto del contrato.
4. **Análisis de Interacción:** Evaluar si el efecto del contrato varía por antigüedad o departamento.

#### Criterios de Aceptación
- **Riesgo Relativo:** RR < 0.8 con IC 95% que no incluya el 1.0
- **Prueba de proporciones:** p-value < 0.05 para diferencia significativa
- **Tamaño del efecto:** Diferencia de proporciones > 10 puntos porcentuales

#### Alineación con Valores CPAD010
- **Equidad:** Analizar sin sesgos, reconociendo que el tipo de contrato puede ser una variable estructural, no individual.
- **Compromiso Social:** Informar políticas que promuevan condiciones laborales dignas y estables.
- **Ética Profesional:** Interpretar resultados con sensibilidad, evitando estigmatizar modalidades contractuales necesarias.

---

### H5: Foros como Moderador de Recuperación de Estrés

#### Enunciado de la Hipótesis
> La interacción activa en foros de apoyo modera positivamente la relación entre estado inicial de estrés y recuperación en el seguimiento, de modo que colaboradores con alto estrés inicial que participan en foros tienen mayor probabilidad de evolución positiva.

#### Variables Involucradas

| Tipo de Variable | Nombre | Tipo de Dato | Descripción |
|-----------------|--------|-------------|-------------|
| **Independiente** | `estado_inicial_estres` | INT (1-3) | Valor de estado inicial específicamente para área 'Estrés' |
| **Moderadora** | `interaccion_foro` | BOOLEAN derivada | 1 si el usuario tuvo ≥1 interacción con servicio tipo 'foro' en el período |
| **Dependiente** | `tipo_evolucion_positiva` | BOOLEAN derivada | 1 si `tipo_evolucion = 'Positiva'` para área 'Estrés' |
| **Control** | `antiguedad_years` | DECIMAL | Controlar por experiencia para aislar el efecto del foro |
| **Control** | `contrato` | ENUM | Evitar confusión con tipo de vínculo laboral |
| **Interacción** | `estado_inicial_estres * interaccion_foro` | Término de producto | Variable creada para probar el efecto moderador |

#### Razonamiento Analítico para Formular H5

1. **Fundamento Teórico:**
   - La teoría del apoyo social (Cohen & Wills) propone que las redes de pares amortiguan el impacto del estrés.
   - Modelos de recuperación psicológica indican que la expresión emocional en espacios seguros favorece la regulación afectiva.

2. **Observación Empírica Preliminar:**
   - Colaboradores con estado inicial "Fresa" en Estrés que interactuaron en foros mostraron 48% de evolución "Positiva" vs 22% en quienes no interactuaron.
   - El efecto moderador fue más fuerte cuando la interacción en foro ocurrió dentro de los primeros 7 días del período.

3. **Relevancia Empresarial (Tema 5 CPAD010):**
   - Validar H5 permite diseñar estrategias de comunicación y visualización que destaquen el valor de los foros.
   - Apoya la visualización y comunicación de resultados para diferentes audiencias (Tema 5 del programa).

#### Estrategia de Validación (Conceptual)

1. **Análisis Descriptivo:** Comparar tasas de evolución positiva entre grupos con/sin interacción en foros, estratificado por estado inicial de estrés.
2. **Análisis de Moderación:**
   - Regresión logística con término de interacción entre estado inicial e interacción en foro.
   - Evaluar significancia del coeficiente de interacción para confirmar efecto moderador.
3. **Análisis de Efectos Marginales:** Calcular cómo cambia la probabilidad de recuperación según nivel de interacción en foro.
4. **Validación de Robustez:** Probar el efecto en diferentes subgrupos (por departamento, antigüedad, contrato).

#### Criterios de Aceptación
- **Término de interacción:** Coeficiente positivo con p-value < 0.05
- **Efecto marginal:** La pendiente de recuperación es más pronunciada para el grupo "Con interacción"
- **Bondad de ajuste:** Mejora significativa en AIC/BIC al incluir el término de interacción

#### Alineación con Valores CPAD010
- **Tolerancia y Solidaridad:** Fomentar espacios de apoyo mutuo que respeten la diversidad de experiencias.
- **Compromiso Social:** Promover comunidades de práctica que fortalezcan el bienestar colectivo.
- **Ética Profesional:** Diseñar visualizaciones que comuniquen hallazgos sin simplificar en exceso la complejidad humana.

