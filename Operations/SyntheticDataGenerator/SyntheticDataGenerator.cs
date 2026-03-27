using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Operations.DataGenerator.Entities.Dimensions;
using Operations.DataGenerator.Entities.Facts;
using APPCORE;

namespace Operations.DataGenerator
{
    // ========================================================================
    // CONFIGURACIÓN
    // ========================================================================
    public class GeneratorConfig
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalEmpleados { get; set; } = 500;
        public int MinServiciosPorMes { get; set; } = 2;
        public int MaxServiciosPorMes { get; set; } = 4;
        public double PorcentajeSolicitudesPsicologo { get; set; } = 0.15;
        public double PorcentajeAbsentismo { get; set; } = 0.08;
        public int Seed { get; set; } = 42;
    }

    // ========================================================================
    // MODELO INTERNO DE EMPLEADO (para lógica de generación)
    // ========================================================================
    public class EmpleadoModel
    {
        public int Id { get; set; }
        public string IdUsuarioOrigen { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string EdadEtiqueta { get; set; } = string.Empty;
        public string Contrato { get; set; } = string.Empty;
        public decimal AntiguedadYears { get; set; }
        public string DepartamentoArea { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        // Tracking de estado para evolución mensual
        public int UltimoEstadoId { get; set; }
        public string UltimoEstadoColor { get; set; } = "Verde";
        public DateTime? UltimaFechaSeguimiento { get; set; }
    }

    // ========================================================================
    // GENERADOR PRINCIPAL
    // ========================================================================
    public class SyntheticDataGenerator
    {
        private readonly GeneratorConfig _config;
        private readonly Random _random;

        // Caché de dimensiones (cargadas una vez)
        private List<Dim_Estado_Psicoemocional>? _estados;
        private List<Dim_Area_Psicoemocional>? _areas;
        private List<Dim_Servicio>? _servicios;
        private List<Dim_Tipo_Evolucion>? _evoluciones;
        private Dictionary<string, int>? _colorToEstadoId;

        // Empleados generados en memoria
        private List<EmpleadoModel> _empleados = new();

        public SyntheticDataGenerator(GeneratorConfig config)
        {
            _config = config;           
            _random = new Random(config.Seed);
        }

        public static async Task Start()
        {
            var config = new GeneratorConfig
            {
                FechaInicio = new DateTime(2025, 1, 1),
                FechaFin = new DateTime(2026, 12, 31),
                TotalEmpleados = 500,
                MinServiciosPorMes = 2,
                MaxServiciosPorMes = 4,
                PorcentajeSolicitudesPsicologo = 0.15,
                PorcentajeAbsentismo = 0.08,
                Seed = 42
            };

            var generator = new SyntheticDataGenerator(config);
            await generator.EjecutarGeneracionAsync();
        }

        public async Task EjecutarGeneracionAsync()
        {
            Log("=== INICIANDO GENERACIÓN CON ENTITYCLASS ===");

            // 1. Cargar dimensiones desde BD usando EntityClass.Get<T>()
            await CargarDimensionesAsync();
            Log($"Dimensiones cargadas: {_estados?.Count ?? 0} estados, {_areas?.Count ?? 0} áreas");

            // 2. Generar y guardar empleados usando EntityClass.Save()
            await GenerarYGuardarEmpleadosAsync();
            Log($"Empleados generados y guardados: {_empleados.Count}");

            // 3. Generar hechos por mes (con transacción implícita en Save)
            await GenerarHechosPorMesAsync();

            // 4. Validar consistencia
            await ValidarConsistenciaAsync();

            Log("=== GENERACIÓN COMPLETADA ===");
        }

        // ====================================================================
        // CARGA DE DIMENSIONES USANDO ENTITYCLASS.GET<T>()
        // ====================================================================
        private async Task CargarDimensionesAsync()
        {
            // Usamos SimpleGet para lectura sin transacción explícita
            _estados = new Dim_Estado_Psicoemocional().SimpleGet<Dim_Estado_Psicoemocional>();
            _colorToEstadoId = _estados?.ToDictionary(e => e.Codigo_Color!, e => e.Id_Estado!.Value)
                ?? new Dictionary<string, int>();

            _areas = new Dim_Area_Psicoemocional().Where<Dim_Area_Psicoemocional>();

            _servicios = new Dim_Servicio().Where<Dim_Servicio>();

            _evoluciones = new Dim_Tipo_Evolucion().SimpleGet<Dim_Tipo_Evolucion>();
        }

        // ====================================================================
        // GENERACIÓN Y GUARDADO DE EMPLEADOS USANDO ENTITYCLASS.SAVE()
        // ====================================================================
        private async Task GenerarYGuardarEmpleadosAsync()
        {
            var generos = new[] { (1, "Masculino"), (2, "Femenino"), (3, "Otro") };
            var contratos = new[] { "Indefinido", "Temporal", "Por Proyecto", "Medio Tiempo" };
            var turnos = new[] { "Matutino", "Vespertino", "Nocturno" };
            var departamentos = new[] { "Tecnología", "Recursos Humanos", "Finanzas", "Operaciones", "Ventas", "Marketing", "Atención al Cliente", "Logística" };
            var edadesEtiqueta = new[] { "18-25", "26-35", "36-45", "46-55", "56+" };

            for (int i = 1; i <= _config.TotalEmpleados; i++)
            {
                var edad = _random.Next(18, 65);
                var antiguedadYears = (decimal)(_random.NextDouble() * 20);

                // Generar modelo interno para tracking
                var empleadoModel = new EmpleadoModel
                {
                    Id = i,
                    IdUsuarioOrigen = $"EMP-{i:D5}",
                    Edad = edad,
                    EdadEtiqueta = CalcularEtiquetaEdad(edad, edadesEtiqueta),
                    Contrato = contratos[_random.Next(contratos.Length)],
                    AntiguedadYears = antiguedadYears,
                    DepartamentoArea = departamentos[_random.Next(departamentos.Length)],
                    UltimoEstadoColor = "Verde",
                    UltimoEstadoId = _colorToEstadoId?.GetValueOrDefault("Verde", 1) ?? 1
                };
                _empleados.Add(empleadoModel);

                // Crear entidad Dim_Usuario y guardar con EntityClass.Save()
                var usuarioEntity = new Dim_Usuario
                {
                    Id_Usuario_Origen = empleadoModel.IdUsuarioOrigen,
                    Edad = empleadoModel.Edad,
                    Edad_Etiqueta = empleadoModel.EdadEtiqueta,
                    Id_Genero = generos[_random.Next(generos.Length)].Item1,
                    Genero = generos[_random.Next(generos.Length)].Item2,
                    Cargo = $"Cargo {i}",
                    Contrato = empleadoModel.Contrato,
                    Antiguedad = antiguedadYears < 1 ? "0-1 años" : $"{(int)antiguedadYears}-{(int)antiguedadYears + 2} años",
                    Antiguedad_Years = antiguedadYears,
                    Turno = turnos[_random.Next(turnos.Length)],
                    Id_Empresa = 1,
                    Nombre_Empresa = "Empresa Principal",
                    Id_Sector = 1,
                    Sector = "Tecnología",
                    Id_Departamento = _random.Next(1, departamentos.Length + 1),
                    Departamento_Area = empleadoModel.DepartamentoArea,
                    Centro = $"Centro {_random.Next(1, 6)}",
                    Activo = true,
                    Fecha_Carga = DateTime.Now
                };

                // ✅ GUARDADO USANDO ENTITYCLASS.SAVE() - Transacción implícita
                if (usuarioEntity.Find<Dim_Usuario>() != null)
                {
                    var result = usuarioEntity.Save();
                    // Actualizar ID generado (si es identity)
                    if (result is int generatedId && generatedId > 0)
                    {
                        usuarioEntity.Id_Usuario = generatedId;
                        empleadoModel.Id = generatedId; // Sincronizar para hechos
                    }
                }

            }
        }

        private string CalcularEtiquetaEdad(int edad, string[] etiquetas)
        {
            int indice = (edad / 10) - 1;
            return (indice >= 0 && indice < etiquetas.Length) ? etiquetas[indice] : "26-35";
        }

        // ====================================================================
        // GENERACIÓN DE HECHOS POR MES
        // ====================================================================
        private async Task GenerarHechosPorMesAsync()
        {
            var fechaActual = _config.FechaInicio;
            var mesCount = 0;

            while (fechaActual <= _config.FechaFin)
            {
                mesCount++;
                Log($"Procesando mes: {fechaActual:yyyy-MM}");

                // ✅ DataMapper maneja transacciones implícitas en Save()
                await GenerarSeguimientoAsync(fechaActual);
                await GenerarInteraccionesAsync(fechaActual);
                await GenerarSolicitudesPsicologoAsync(fechaActual);
                await GenerarAbsentismoAsync(fechaActual);

                fechaActual = fechaActual.AddMonths(1);
            }

            Log($"Total meses procesados: {mesCount}");
        }

        // ====================================================================
        // GENERAR SEGUIMIENTO USANDO ENTITYCLASS
        // ====================================================================
        private async Task GenerarSeguimientoAsync(DateTime fechaMes)
        {
            // Obtener o crear registro de fecha usando EntityClass
            var idFecha = await ObtenerOCrearFechaAsync(fechaMes);

            foreach (var empleado in _empleados)
            {
                if (!empleado.Activo) continue;

                // Calcular evolución de estado
                var estadoAnterior = empleado.UltimoEstadoColor;
                var estadoNuevo = CalcularNuevoEstado(estadoAnterior, empleado);
                var evolucion = DeterminarEvolucion(estadoAnterior, estadoNuevo);
                var areaPrincipal = _areas?.FirstOrDefault(a => a.Codigo_Area == "BG") ?? _areas?.First();

                // ✅ Crear y guardar Fact_Seguimiento_Usuario
                var seguimiento = new Fact_Seguimiento_Usuario
                {
                    Id_Usuario = empleado.Id,
                    Id_Fecha_Seguimiento = idFecha,
                    Id_Estado_Inicial = _colorToEstadoId?[estadoAnterior] ?? 1,
                    Id_Estado_Final = _colorToEstadoId?[estadoNuevo] ?? 1,
                    Id_Area_Principal = areaPrincipal?.Id_Area ?? 1,
                    Id_Tipo_Evolucion = evolucion?.Id_Evolucion ?? 2,
                    Estado_Inicial_Valor = GetEstadoValor(estadoAnterior),
                    Estado_Final_Valor = GetEstadoValor(estadoNuevo),
                    Delta_Bienestar = GetEstadoValor(estadoNuevo) - GetEstadoValor(estadoAnterior),
                    Es_Primera_Evaluacion = empleado.UltimaFechaSeguimiento == null,
                    Es_Recuperacion = estadoAnterior == "Fresa" && estadoNuevo == "Verde",
                    Flag_Alerta = estadoNuevo == "Fresa",
                    Fecha_Carga = DateTime.Now
                };

                // ✅ GUARDADO CON TRANSACCIÓN IMPLÍCITA
                var saveResult = seguimiento.Save();
                var idSeguimiento = saveResult is long sid ? sid : (saveResult is int si ? (long)si : 0);

                // Generar detalle por cada área psicoemocional
                if (_areas != null && idSeguimiento > 0)
                {
                    foreach (var area in _areas)
                    {
                        var estadoAreaNuevo = CalcularNuevoEstadoPorArea(estadoAnterior, estadoNuevo);
                        var evolucionArea = DeterminarEvolucion(estadoAnterior, estadoAreaNuevo);

                        var detalle = new Fact_Detalle_Estado_Dimension
                        {
                            Id_Seguimiento = idSeguimiento,
                            Id_Usuario = empleado.Id,
                            Id_Fecha = idFecha,
                            Id_Area = area.Id_Area,
                            Id_Estado_Inicial = _colorToEstadoId?[estadoAnterior] ?? 1,
                            Id_Estado_Final = _colorToEstadoId?[estadoAreaNuevo] ?? 1,
                            Id_Tipo_Evolucion = evolucionArea?.Id_Evolucion ?? 2,
                            Puntaje_Inicial = GetEstadoValor(estadoAnterior) * 33.33m,
                            Puntaje_Final = GetEstadoValor(estadoAreaNuevo) * 33.33m,
                            Variacion_Puntaje = (GetEstadoValor(estadoAreaNuevo) - GetEstadoValor(estadoAnterior)) * 33.33m,
                            Es_Dimension_Critica = area.Es_Dimension_Secundaria == true && estadoAreaNuevo == "Fresa",
                            Requiere_Atencion = estadoAreaNuevo == "Fresa",
                            Fecha_Carga = DateTime.Now
                        };

                        detalle.Save();
                    }
                }

                // Actualizar tracking del empleado
                empleado.UltimoEstadoColor = estadoNuevo;
                empleado.UltimoEstadoId = _colorToEstadoId?[estadoNuevo] ?? 1;
                empleado.UltimaFechaSeguimiento = fechaMes;
            }
        }

        // ====================================================================
        // MÉTODOS AUXILIARES CON ENTITYCLASS
        // ====================================================================
        private async Task<int> ObtenerOCrearFechaAsync(DateTime fecha)
        {
            // Intentar obtener fecha existente usando EntityClass.Find()
            var fechaExistente = new Dim_Tiempo().Find<Dim_Tiempo>(
                FilterData.Equal("Fecha", fecha.Date)
            );

            if (fechaExistente?.Id_Tiempo.HasValue == true)
            {
                return fechaExistente.Id_Tiempo.Value;
            }

            // Crear nuevo registro de fecha
            var nuevaFecha = new Dim_Tiempo
            {
                Fecha = fecha.Date,
                Dia_Mes = fecha.Day,
                Dia_Semana = (int)fecha.DayOfWeek,
                Nombre_Dia = fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES")),
                Mes = fecha.Month,
                Nombre_Mes = fecha.ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                Trimestre = (fecha.Month - 1) / 3 + 1,
                Semestre = fecha.Month <= 6 ? 1 : 2,
                Anio = fecha.Year,
                Semana_Anio = GetWeekNumber(fecha),
                Es_Fin_Semana = fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday,
                Es_Festivo = false,
                Es_Inicio_Mes = fecha.Day == 1,
                Es_Fin_Mes = fecha.Day == DateTime.DaysInMonth(fecha.Year, fecha.Month),
                Fecha_Carga = DateTime.Now
            };

            var result = nuevaFecha.Save();
            return (result as Dim_Tiempo)?.Id_Tiempo ?? -1;
        }

        private int GetWeekNumber(DateTime date) =>
            System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        private string CalcularNuevoEstado(string estadoAnterior, EmpleadoModel empleado)
        {
            var probabilidades = new Dictionary<string, Dictionary<string, double>>
            {
                ["Verde"] = new() { ["Verde"] = 0.70, ["Naranja"] = 0.25, ["Fresa"] = 0.05 },
                ["Naranja"] = new() { ["Verde"] = 0.30, ["Naranja"] = 0.50, ["Fresa"] = 0.20 },
                ["Fresa"] = new() { ["Verde"] = 0.15, ["Naranja"] = 0.50, ["Fresa"] = 0.35 }
            };

            var probs = probabilidades[estadoAnterior].ToDictionary(k => k.Key, v => v.Value);

            // Aplicar hipótesis H1 y H4
            if (empleado.AntiguedadYears > 5 && estadoAnterior == "Verde")
            {
                probs["Verde"] += 0.10;
                probs["Fresa"] -= 0.05;
            }
            if (empleado.Contrato == "Indefinido" && estadoAnterior == "Verde")
            {
                probs["Verde"] += 0.05;
                probs["Naranja"] -= 0.03;
            }

            // Normalizar y seleccionar
            var total = probs.Values.Sum();
            probs = probs.ToDictionary(k => k.Key, v => v.Value / total);

            var r = _random.NextDouble();
            var acum = 0.0;
            foreach (var kvp in probs)
            {
                acum += kvp.Value;
                if (r <= acum) return kvp.Key;
            }
            return estadoAnterior;
        }

        private string CalcularNuevoEstadoPorArea(string anterior, string nuevo) =>
            _random.NextDouble() < 0.85 ? nuevo : CalcularNuevoEstado(anterior, new EmpleadoModel { AntiguedadYears = 5, Contrato = "Indefinido" });

        private Dim_Tipo_Evolucion? DeterminarEvolucion(string inicial, string final)
        {
            if (inicial == final)
                return _evoluciones?.FirstOrDefault(e => e.Tipo_Evolucion == "Neutra");

            var mejora = new[] { ("Fresa", "Naranja"), ("Fresa", "Verde"), ("Naranja", "Verde") };
            var empeora = new[] { ("Verde", "Naranja"), ("Verde", "Fresa"), ("Naranja", "Fresa") };

            if (mejora.Any(m => m.Item1 == inicial && m.Item2 == final))
                return _evoluciones?.FirstOrDefault(e => e.Tipo_Evolucion == "Positiva");

            if (empeora.Any(m => m.Item1 == inicial && m.Item2 == final))
                return _evoluciones?.FirstOrDefault(e => e.Tipo_Evolucion == "Negativa");

            return _evoluciones?.FirstOrDefault(e => e.Tipo_Evolucion == "Neutra");
        }

        private int GetEstadoValor(string color) => color switch
        {
            "Verde" => 3,
            "Naranja" => 2,
            "Fresa" => 1,
            _ => 2
        };

        // ====================================================================
        // GENERAR INTERACCIONES, SOLICITUDES Y ABSENTISMO (Patrón similar)
        // ====================================================================
        private async Task GenerarInteraccionesAsync(DateTime fechaMes)
        {
            var idFecha = await ObtenerOCrearFechaAsync(fechaMes);
            var dispositivos = new[] { "Desktop", "Móvil", "Tablet" };
            var canales = new[] { "Web", "App", "Portal" };

            foreach (var empleado in _empleados.Where(e => e.Activo))
            {
                var numServicios = _random.Next(_config.MinServiciosPorMes, _config.MaxServiciosPorMes + 1);
                var serviciosSel = _servicios?.OrderBy(x => _random.Next()).Take(numServicios) ?? Enumerable.Empty<Dim_Servicio>();

                foreach (var servicio in serviciosSel)
                {
                    var interaccion = new Fact_Interaccion_Servicio
                    {
                        Id_Usuario = empleado.Id,
                        Id_Fecha = idFecha,
                        Id_Servicio = servicio.Id_Servicio,
                        Duracion_Real_Min = servicio.Duracion_Estimada_Min ?? _random.Next(15, 120),
                        Frecuencia_Acceso = _random.Next(1, 5),
                        Calificacion_Usuario = Math.Round((decimal)_random.NextDouble(), 2),
                        Completitud = Math.Round((decimal)_random.NextDouble(), 2), // 0.00 a 1.00
                        Dispositivo = dispositivos[_random.Next(dispositivos.Length)],
                        Canal_Acceso = canales[_random.Next(canales.Length)],
                        Es_Recomendado = _random.NextDouble() < 0.3,
                        Fecha_Carga = DateTime.Now
                    };
                    interaccion.Save();
                }
            }
        }

        private async Task GenerarSolicitudesPsicologoAsync(DateTime fechaMes)
        {
            var idFecha = await ObtenerOCrearFechaAsync(fechaMes);
            var elegibles = _empleados.Where(e => e.Activo && (e.UltimoEstadoColor == "Fresa" || e.UltimoEstadoColor == "Naranja")).ToList();
            var numSolicitudes = (int)(elegibles.Count * _config.PorcentajeSolicitudesPsicologo);
            var seleccionados = elegibles.OrderBy(x => _random.Next()).Take(numSolicitudes);

            foreach (var empleado in seleccionados)
            {
                var asiste = _random.NextDouble() < 0.8;

                var fechaPrevistaReal = fechaMes.AddDays(_random.Next(1, 15));
                var idFechaPrevista = await ObtenerOCrearFechaAsync(fechaPrevistaReal);

                var solicitud = new Fact_Solicitud_Psicologo
                {
                    Id_Usuario = empleado.Id,
                    Id_Fecha_Solicitud = idFecha,  // ID del mes actual (ya obtenido)
                    Id_Fecha_Prevista = idFechaPrevista,  // ✅ ID válido de fecha futura
                    Id_Fecha_Atencion = asiste ? idFechaPrevista : null,  // Si asiste, misma fecha prevista
                    Solicita = "SI",
                    Tiene_Psicologo_Asignado = asiste ? "SI" : "NO",
                    Usuario_Asiste = asiste ? "SI" : "NO",
                    Solicitud_Empresa = _random.NextDouble() < 0.2 ? "SI" : "NO",
                    N_Solicitudes_Acumuladas = _random.Next(1, 4),
                    Sesiones_Consumidas = asiste ? _random.Next(1, 6) : 0,
                    Sesiones_Pendientes = asiste ? 0 : _random.Next(1, 5),  // Si no asiste, hay pendientes
                    Tiempo_Espera_Dias = asiste ? _random.Next(0, 10) : null,
                    Tipo_Usuario = empleado.UltimoEstadoColor == "Fresa" ? "Tratamiento" : "Seguimiento",
                    Prioridad = empleado.UltimoEstadoColor == "Fresa" ? "Alta" : "Media",
                    Estado_Tratamiento = asiste ? "Activo" : "Pendiente",
                    Fecha_Carga = DateTime.Now
                };

                // ✅ Guardar usando EntityClass.Save() - transacción implícita
                solicitud.Save();
            }
        }

        private async Task GenerarAbsentismoAsync(DateTime fechaMes)
        {
            var idFecha = await ObtenerOCrearFechaAsync(fechaMes);
            var elegibles = _empleados.Where(e => e.Activo).ToList();
            var numAbsentismos = (int)(elegibles.Count * _config.PorcentajeAbsentismo);
            var seleccionados = elegibles.OrderBy(x => _random.Next()).Take(numAbsentismos);

            foreach (var empleado in seleccionados)
            {
                var dias = _random.Next(1, 10);
                var relacionado = empleado.UltimoEstadoColor == "Fresa" || _random.NextDouble() < 0.4;

                var absentismo = new Fact_Absentismo
                {
                    Id_Usuario = empleado.Id,
                    Id_Fecha_Inicio = idFecha,
                    Id_Fecha_Final = idFecha + dias,
                    Dias_Ausente = dias,
                    Justificado = _random.NextDouble() < 0.7,
                    Relacionado_Salud_Mental = relacionado,
                    Tipo_Absentismo = relacionado ? "Salud Mental" : "General",
                    Gravedad = dias > 5 ? "Moderado" : "Leve",
                    Comentario = relacionado ? "Relacionado con estrés/ansiedad" : "Motivos personales",
                    Fecha_Carga = DateTime.Now
                };
                absentismo.Save();
            }
        }

        // ====================================================================
        // VALIDACIÓN Y LOGGING
        // ====================================================================
        private async Task ValidarConsistenciaAsync()
        {
            Log("Validando consistencia...");

            var totalSeguimientos = new Fact_Seguimiento_Usuario().Count();
            var totalDetalles = new Fact_Detalle_Estado_Dimension().Count();
            var totalInteracciones = new Fact_Interaccion_Servicio().Count();

            Log($"✓ Seguimientos: {totalSeguimientos}");
            Log($"✓ Detalles: {totalDetalles}");
            Log($"✓ Interacciones: {totalInteracciones}");
        }

        private void Log(string message) => Console.Write(message);
        private void LogError(Exception ex, string message) => Console.Write(message);
    }
}