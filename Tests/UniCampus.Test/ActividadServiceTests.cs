using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;
using uniCampus.BLL.Services;
using uniCampus.BLL.Validations;

namespace UniCampus.Test
{
    [TestClass]
    public class ActividadServiceTests
    {
        [TestMethod]
        public void Guardar_DtoNulo_DevuelveErrorSinPersistir()
        {
            AssertInvalida(null, "actividad");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" \t ")]
        public void Guardar_TituloVacio_NoPersiste(string titulo)
        {
            var actividad = ActividadValida();
            actividad.Titulo = titulo;
            AssertInvalida(actividad, "título");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" \t ")]
        public void Guardar_DescripcionVacia_NoPersiste(string descripcion)
        {
            var actividad = ActividadValida();
            actividad.Descripcion = descripcion;
            AssertInvalida(actividad, "descripción");
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Guardar_TipoNoPositivo_NoPersiste(int tipo)
        {
            var actividad = ActividadValida();
            actividad.TipoId = tipo;
            AssertInvalida(actividad, "tipo");
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Guardar_CuposNoPositivos_NoPersiste(int cupos)
        {
            var actividad = ActividadValida();
            actividad.Cupos = cupos;
            AssertInvalida(actividad, "cupos");
        }

        [TestMethod]
        public void Guardar_FechaInicioSinInicializar_ExigeFechaSinPersistir()
        {
            var actividad = ActividadValida();
            actividad.FechaInicio = default(DateTime);
            AssertFechasObligatorias(actividad, "fecha de inicio");
        }

        [TestMethod]
        public void Guardar_FechaFinSinInicializar_ExigeFechaSinPersistir()
        {
            var actividad = ActividadValida();
            actividad.FechaFin = default(DateTime);
            AssertFechasObligatorias(actividad, "fecha de fin");
        }

        [TestMethod]
        public void Guardar_AmbasFechasSinInicializar_ExigeAmbasSinPersistir()
        {
            var actividad = ActividadValida();
            actividad.FechaInicio = default(DateTime);
            actividad.FechaFin = default(DateTime);
            AssertFechasObligatorias(actividad, "fecha de inicio", "fecha de fin");
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Guardar_FechaNoProporcionadaConHoraInterna_NoEludeValidacion(bool esFin)
        {
            var actividad = ActividadValida();
            if (esFin)
                actividad.FechaFin = DateTime.MinValue.AddHours(12);
            else
                actividad.FechaInicio = DateTime.MinValue.AddHours(12);
            AssertFechasObligatorias(actividad, esFin ? "fecha de fin" : "fecha de inicio");
        }
        [TestMethod]
        public void Guardar_FechaFinalAnterior_NoPersiste()
        {
            var actividad = ActividadValida();
            actividad.FechaFin = actividad.FechaInicio.AddDays(-1);
            AssertInvalida(actividad, "fecha");
        }

        [DataTestMethod]
        [DataRow(8)]
        [DataRow(7)]
        public void Guardar_FinalIgualOAnteriorAlInicioMismoDia_NoPersiste(int horaFin)
        {
            var actividad = ActividadValida();
            actividad.HoraFin = TimeSpan.FromHours(horaFin);
            AssertInvalida(actividad, "intervalo");
        }

        [DataTestMethod]
        [DataRow(-1L, false)]
        [DataRow(864000000000L, false)]
        [DataRow(long.MaxValue, false)]
        [DataRow(-1L, true)]
        [DataRow(864000000000L, true)]
        [DataRow(long.MinValue, true)]
        public void Guardar_HoraFueraDelDia_NoPersiste(long ticks, bool esFin)
        {
            var actividad = ActividadValida();
            if (esFin)
                actividad.HoraFin = TimeSpan.FromTicks(ticks);
            else
                actividad.HoraInicio = TimeSpan.FromTicks(ticks);
            AssertInvalida(actividad, "hora");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("Desconocida")]
        public void Guardar_ModalidadNoPermitida_NoPersiste(string modalidad)
        {
            var actividad = ActividadValida();
            actividad.Modalidad = modalidad;
            AssertInvalida(actividad, "modalidad");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Guardar_PresencialSinLugar_NoPersiste(string lugar)
        {
            var actividad = ActividadValida();
            actividad.Lugar = lugar;
            AssertInvalida(actividad, "lugar");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("Desconocido")]
        public void Guardar_EstadoNoPermitido_NoPersiste(string estado)
        {
            var actividad = ActividadValida();
            actividad.Estado = estado;
            AssertInvalida(actividad, "estado");
        }

        [TestMethod]
        public void Guardar_Valida_DevuelveIdYPersisteUnaSolaVezConLosDatosRecibidos()
        {
            var repository = new RepositoryFalso { Id = 72 };
            var actividad = ActividadValida();
            var resultado = new ActividadService(repository, Reglas()).Guardar(actividad);

            Assert.IsTrue(resultado.Exitoso);
            Assert.AreEqual(72, resultado.ActividadId);
            Assert.AreEqual(0, resultado.Errores.Count);
            Assert.AreEqual(1, repository.Llamadas);
            Assert.AreSame(actividad, repository.Recibida);
            Assert.AreEqual(0, actividad.ActividadId, "El servicio no asigna un id al DTO de entrada.");
        }

        [TestMethod]
        public void Guardar_VariosErrores_LosAcumulaSinPersistir()
        {
            var repository = new RepositoryFalso();
            var actividad = ActividadValida();
            actividad.Titulo = null;
            actividad.Descripcion = null;
            actividad.TipoId = 0;
            actividad.Cupos = 0;
            var resultado = new ActividadService(repository, Reglas()).Guardar(actividad);

            Assert.IsFalse(resultado.Exitoso);
            Assert.IsNull(resultado.ActividadId);
            Assert.AreEqual(4, resultado.Errores.Count);
            Assert.AreEqual(0, repository.Llamadas);
        }

        [TestMethod]
        public void Guardar_VirtualSinLugar_PermiteGuardar()
        {
            var actividad = ActividadValida();
            actividad.Modalidad = "Virtual";
            actividad.Lugar = null;
            AssertValida(actividad);
        }

        [TestMethod]
        public void Guardar_ActividadDeVariosDias_AdmiteHoraFinalMenor()
        {
            var actividad = ActividadValida();
            actividad.FechaFin = actividad.FechaInicio.AddDays(2);
            actividad.HoraInicio = TimeSpan.FromHours(23);
            actividad.HoraFin = TimeSpan.FromHours(1);
            AssertValida(actividad);
        }

        [TestMethod]
        public void Guardar_CruceDeMedianoche_AdmiteFinalDelDiaSiguiente()
        {
            var actividad = ActividadValida();
            actividad.FechaFin = actividad.FechaInicio.AddDays(1);
            actividad.HoraInicio = TimeSpan.FromDays(1) - TimeSpan.FromTicks(1);
            actividad.HoraFin = TimeSpan.Zero;
            AssertValida(actividad);
        }

        [TestMethod]
        public void Guardar_PrimeraFechaProporcionableYMaxima_HorasLimite_NoDesborda()
        {
            var actividad = ActividadValida();
            actividad.FechaInicio = new DateTime(1, 1, 2);
            actividad.FechaFin = DateTime.MaxValue;
            actividad.HoraInicio = TimeSpan.Zero;
            actividad.HoraFin = TimeSpan.FromDays(1) - TimeSpan.FromTicks(1);
            AssertValida(actividad);
        }

        [TestMethod]
        public void Guardar_UltimoDiaRepresentable_AdmiteUltimoTick()
        {
            var actividad = ActividadValida();
            actividad.FechaInicio = DateTime.MaxValue.Date;
            actividad.FechaFin = DateTime.MaxValue.Date;
            actividad.HoraInicio = TimeSpan.Zero;
            actividad.HoraFin = TimeSpan.FromDays(1) - TimeSpan.FromTicks(1);
            AssertValida(actividad);
        }

        [TestMethod]
        public void Guardar_FechaConHora_UsaSoloFechaJuntoConHoraExplicita()
        {
            var actividad = ActividadValida();
            actividad.FechaInicio = actividad.FechaInicio.AddHours(22);
            actividad.FechaFin = actividad.FechaFin.AddHours(1);
            AssertValida(actividad);
        }

        [TestMethod]
        public void Guardar_CuposYTipoMinimosPositivos_AdmiteGuardar()
        {
            var actividad = ActividadValida();
            actividad.Cupos = 1;
            actividad.TipoId = 1;
            AssertValida(actividad);
        }

        [TestMethod]
        public void Guardar_ErrorDePersistencia_NoFiltraDetallesNiReintenta()
        {
            var repository = new RepositoryFalso { Error = new InvalidOperationException("servidor=privado;clave=secreta") };
            var resultado = new ActividadService(repository, Reglas()).Guardar(ActividadValida());

            Assert.IsFalse(resultado.Exitoso);
            Assert.IsNull(resultado.ActividadId);
            Assert.AreEqual(1, resultado.Errores.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(resultado.Errores[0]));
            Assert.IsFalse(resultado.Errores[0].Contains("privado"));
            Assert.IsFalse(resultado.Errores[0].Contains("secreta"));
            Assert.AreEqual(1, repository.Llamadas);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Guardar_RepositoryDevuelveIdNoPositivo_NoInformaExito(int id)
        {
            var repository = new RepositoryFalso { Id = id };
            var resultado = new ActividadService(repository, Reglas()).Guardar(ActividadValida());
            Assert.IsFalse(resultado.Exitoso);
            Assert.IsNull(resultado.ActividadId);
            Assert.IsTrue(resultado.Errores.Count > 0);
            Assert.AreEqual(1, repository.Llamadas);
        }

        [TestMethod]
        public void Constructor_RepositoryNulo_FallaDeFormaExplicita()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ActividadService(null, Reglas()));
        }

        [TestMethod]
        public void Constructor_ReglasNulas_FallaDeFormaExplicita()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ActividadService(new RepositoryFalso(), null));
        }

        [TestMethod]
        public void Reglas_CatalogosNulos_FallanDeFormaExplicita()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ReglasActividad(null, new string[0], new[] { "Activa" }));
            Assert.ThrowsException<ArgumentNullException>(() => new ReglasActividad(new[] { "Virtual" }, null, new[] { "Activa" }));
            Assert.ThrowsException<ArgumentNullException>(() => new ReglasActividad(new[] { "Virtual" }, new string[0], null));
        }

        [TestMethod]
        public void Reglas_CatalogosVaciosONombresEnBlanco_NoPermitenConfiguracionInvalida()
        {
            Assert.ThrowsException<ArgumentException>(() => new ReglasActividad(new string[0], new string[0], new[] { "Activa" }));
            Assert.ThrowsException<ArgumentException>(() => new ReglasActividad(new[] { "Virtual" }, new string[0], new string[0]));
            Assert.ThrowsException<ArgumentException>(() => new ReglasActividad(new[] { " " }, new string[0], new[] { "Activa" }));
            Assert.ThrowsException<ArgumentException>(() => new ReglasActividad(new[] { "Virtual" }, new string[0], new string[] { null }));
            Assert.ThrowsException<ArgumentException>(() => new ReglasActividad(new[] { "Virtual" }, new[] { "" }, new[] { "Activa" }));
        }

        [TestMethod]
        public void Reglas_ModalidadConLugarFueraDelCatalogo_NoPermiteConfiguracionIncoherente()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new ReglasActividad(new[] { "Virtual" }, new[] { "Presencial" }, new[] { "Activa" }));
        }

        [TestMethod]
        public void Guardar_ReglasConfiguradas_AdmiteCatalogosDelEquipoSinEstadosFijos()
        {
            var reglas = new ReglasActividad(new[] { "Remota" }, new string[0], new[] { "Borrador" });
            var actividad = ActividadValida();
            actividad.Modalidad = "Remota";
            actividad.Estado = "Borrador";
            actividad.Lugar = null;
            var repository = new RepositoryFalso();
            var resultado = new ActividadService(repository, reglas).Guardar(actividad);
            Assert.IsTrue(resultado.Exitoso);
            Assert.AreEqual(1, repository.Llamadas);
        }

        [TestMethod]
        public void Guardar_ReglasCopianCatalogos_NoCambianAlMutarLaConfiguracionOriginal()
        {
            var modalidades = new[] { "Presencial" };
            var lugares = new[] { "Presencial" };
            var estados = new[] { "Activa" };
            var reglas = new ReglasActividad(modalidades, lugares, estados);
            modalidades[0] = "Otra";
            lugares[0] = "Otra";
            estados[0] = "Otro";
            var repository = new RepositoryFalso();
            var servicio = new ActividadService(repository, reglas);
            Assert.IsTrue(servicio.Guardar(ActividadValida()).Exitoso);

            var sinLugar = ActividadValida();
            sinLugar.Lugar = null;
            Assert.IsFalse(servicio.Guardar(sinLugar).Exitoso);
            Assert.AreEqual(1, repository.Llamadas);
        }

        [TestMethod]
        public void Guardar_ModalidadConfiguradaConLugar_ExigeLugarSinDependerDelNombre()
        {
            var reglas = new ReglasActividad(new[] { "Mixta" }, new[] { "Mixta" }, new[] { "Abierta" });
            var actividad = ActividadValida();
            actividad.Modalidad = "Mixta";
            actividad.Estado = "Abierta";
            actividad.Lugar = " ";
            var repository = new RepositoryFalso();
            var resultado = new ActividadService(repository, reglas).Guardar(actividad);
            Assert.IsFalse(resultado.Exitoso);
            Assert.IsTrue(resultado.Errores.Any(error => error.Contains("lugar")));
            Assert.AreEqual(0, repository.Llamadas);
        }

        private static void AssertFechasObligatorias(ActividadDto actividad, params string[] campos)
        {
            var repository = new RepositoryFalso();
            var resultado = new ActividadService(repository, Reglas()).Guardar(actividad);
            Assert.IsFalse(resultado.Exitoso);
            Assert.IsNull(resultado.ActividadId);
            foreach (var campo in campos)
            {
                Assert.IsTrue(resultado.Errores.Any(error =>
                    error.IndexOf(campo, StringComparison.OrdinalIgnoreCase) >= 0 &&
                    error.Contains("obligatoria")), "Debe exigir el campo: " + campo);
            }
            Assert.AreEqual(campos.Length, resultado.Errores.Count,
                            "No debe comparar el intervalo cuando falta alguna fecha.");
            Assert.AreEqual(0, repository.Llamadas);
        }
        private static void AssertInvalida(ActividadDto actividad, string campo)
        {
            var repository = new RepositoryFalso();
            var resultado = new ActividadService(repository, Reglas()).Guardar(actividad);
            Assert.IsFalse(resultado.Exitoso);
            Assert.IsNull(resultado.ActividadId);
            Assert.IsTrue(resultado.Errores.Any(error => error.IndexOf(campo, StringComparison.OrdinalIgnoreCase) >= 0),
                          "Debe informar el campo inválido: " + campo);
            Assert.AreEqual(0, repository.Llamadas);
        }

        private static void AssertValida(ActividadDto actividad)
        {
            var repository = new RepositoryFalso();
            var resultado = new ActividadService(repository, Reglas()).Guardar(actividad);
            Assert.IsTrue(resultado.Exitoso, string.Join("; ", resultado.Errores));
            Assert.AreEqual(72, resultado.ActividadId);
            Assert.AreEqual(0, resultado.Errores.Count);
            Assert.AreEqual(1, repository.Llamadas);
        }

        private static ReglasActividad Reglas()
        {
            // Datos de prueba; los catálogos de producción los debe acordar el equipo.
            return new ReglasActividad(new[] { "Presencial", "Virtual" }, new[] { "Presencial" }, new[] { "Activa" });
        }

        private static ActividadDto ActividadValida()
        {
            return new ActividadDto
            {
                ActividadId = 0,
                Titulo = "Taller de programación",
                Descripcion = "Práctica de arquitectura en capas",
                TipoId = 1,
                FechaInicio = new DateTime(2026, 11, 9),
                FechaFin = new DateTime(2026, 11, 9),
                HoraInicio = TimeSpan.FromHours(8),
                HoraFin = TimeSpan.FromHours(10),
                Modalidad = "Presencial",
                Lugar = "Laboratorio 3",
                Cupos = 30,
                Estado = "Activa"
            };
        }

        private sealed class RepositoryFalso : IActividadRepository
        {
            public int Llamadas { get; private set; }
            public ActividadDto Recibida { get; private set; }
            public int Id { get; set; } = 72;
            public Exception Error { get; set; }

            public int Guardar(ActividadDto actividad)
            {
                Llamadas++;
                Recibida = actividad;
                if (Error != null)
                    throw Error;
                return Id;
            }
        }
    }
}