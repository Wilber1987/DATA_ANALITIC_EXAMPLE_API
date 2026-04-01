# Vistas Analíticas para Hipótesis CPAD010

**Componente:** Proyecto de Analítica de Datos (CPAD010)  
**Propósito:** Crear vistas SQL y clases ViewClass para soporte de análisis de hipótesis H1-H5  
**Patrón:** Propiedades con nombres exactos a columnas de BD (Capitalized_Snake_Case)

---

## 📋 Índice de Vistas

| Hipótesis | Vista SQL | Clase ViewClass | Propósito Analítico |
|-----------|-----------|-----------------|-------------------|
| **H1** | `v_Analisis_Antiguedad_Bienestar` | `V_Analisis_Antiguedad_Bienestar` | Correlación antigüedad vs estado final |
| **H2** | `v_Analisis_Entrenamientos_Evolucion` | `V_Analisis_Entrenamientos_Evolucion` | Asociación frecuencia entrenamiento vs evolución |
| **H3** | `v_Analisis_Absentismo_Predictor` | `V_Analisis_Absentismo_Predictor` | Predicción estado desfavorable por absentismo |
| **H4** | `v_Analisis_Contrato_Estabilidad` | `V_Analisis_Contrato_Estabilidad` | Probabilidad evolución negativa por tipo contrato |
| **H5** | `v_Analisis_Foros_Moderacion_Estres` | `V_Analisis_Foros_Moderacion_Estres` | Efecto moderador de foros en recuperación de estrés |

---

## 🔍 H1: Antigüedad → Bienestar Laboral

### SQL VIEW

```sql
-- ============================================================================
-- VISTA: v_Analisis_Antiguedad_Bienestar
-- Hipótesis H1: Mayor antigüedad correlaciona con mejor bienestar laboral
-- ============================================================================

CREATE VIEW v_Analisis_Antiguedad_Bienestar
AS
SELECT 
    -- Identificadores
    f.Id_Seguimiento,
    f.Id_Usuario,
    f.Id_Fecha_Seguimiento,
    
    -- Datos del usuario (desnormalizados para análisis)
    u.Id_Usuario_Origen,
    u.Edad,
    u.Edad_Etiqueta,
    u.Contrato,
    u.Antiguedad,
    u.Antiguedad_Years,
    u.Departamento_Area,
    u.Nombre_Empresa,
    
    -- Estado psicoemocional
    f.Estado_Inicial_Valor,
    f.Estado_Final_Valor,
    f.Delta_Bienestar,
    f.Flag_Alerta,
    
    -- Dimensiones descriptivas
    e_final.Codigo_Color AS Estado_Final_Color,
    e_final.Etiqueta AS Estado_Final_Etiqueta,
    a.Nombre_Area AS Area_Evaluada,
    a.Tipo_Bienestar,
    
    -- Tiempo
    t.Anio,
    t.Trimestre,
    t.Mes,
    t.Nombre_Mes,
    
    -- Variable derivada para análisis: rango de antigüedad categórico
    CASE 
        WHEN u.Antiguedad_Years < 1 THEN '0-1 año'
        WHEN u.Antiguedad_Years < 3 THEN '1-3 años'
        WHEN u.Antiguedad_Years < 5 THEN '3-5 años'
        ELSE '5+ años'
    END AS Rango_Antiguedad

FROM Fact_Seguimiento_Usuario f
INNER JOIN Dim_Usuario u ON f.Id_Usuario = u.Id_Usuario
INNER JOIN Dim_Estado_Psicoemocional e_final ON f.Id_Estado_Final = e_final.Id_Estado
INNER JOIN Dim_Area_Psicoemocional a ON f.Id_Area_Principal = a.Id_Area
INNER JOIN Dim_Tiempo t ON f.Id_Fecha_Seguimiento = t.Id_Tiempo

WHERE 
    u.Activo = 1 
    AND a.Tipo_Bienestar = 'Laboral'
    AND e_final.Codigo_Color IN ('Verde', 'Naranja', 'Fresa');
GO

-- Índices para optimizar consultas analíticas
CREATE NONCLUSTERED INDEX IX_v_Antiguedad_Bienestar_Usuario 
ON v_Analisis_Antiguedad_Bienestar(Id_Usuario, Anio, Trimestre);

CREATE NONCLUSTERED INDEX IX_v_Antiguedad_Bienestar_Rango 
ON v_Analisis_Antiguedad_Bienestar(Rango_Antiguedad, Estado_Final_Valor);
GO