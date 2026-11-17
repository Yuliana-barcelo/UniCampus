using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uniCampus.BLL.Services;
using UniCampus.Entities;
using UniCampus.Entities.Contracts;

namespace UniCampus.Test
{
    public class RepoInscripcionFalso : IInscripcionRepository
    {
        public ResultadoInscripcionDatos Respuesta = ResultadoInscripcionDatos.Exito;
        public bool Lanzar;
        public int Llamadas;

        public IList<EstudianteInscritoDto> ListarPorActividad(int actividadId) { return new List<EstudianteInscritoDto>(); }
        public int ContarInscritos(int actividadId) { return 0; }
        public IList<MiInscripcionDto> ListarPorUsuario(int usuarioId) { return new List<MiInscripcionDto>(); }
        public bool Cancelar(int inscripcionId, int usuarioSolicitanteId, bool esAdministrador) { return true; }
        public ResultadoInscripcionDatos Inscribir(int actividadId, int usuarioId)
        {
            Llamadas++;
            if (Lanzar) throw new InvalidOperationException("fallo simulado");
            return Respuesta;
        }
    }

    public class RepoResenaFalso : IResenaRepository
    {
        public bool Existe;
        public int Guardadas;

        public int Guardar(ResenaDto resena) { Guardadas++; return 1; }
        public bool ExisteResena(int actividadId, int usuarioId) { return Existe; }
        public IList<ResenaDto> ListarPorActividad(int actividadId) { return new List<ResenaDto>(); }
    }

    [TestClass]
    public class InscripcionServiceTests
    {
        [TestMethod]
        public void Inscribir_Exito()
        {
            var s = new InscripcionService(new RepoInscripcionFalso());
            Assert.IsTrue(s.Inscribir(1, 10).Exito);
        }

        [TestMethod]
        public void Inscribir_Duplicada_SeRechaza()
        {
            var s = new InscripcionService(new RepoInscripcionFalso { Respuesta = ResultadoInscripcionDatos.Duplicada });
            var r = s.Inscribir(1, 10);
            Assert.IsFalse(r.Exito);
            StringAssert.Contains(r.Mensaje, "inscrito");
        }

        [TestMethod]
        public void Inscribir_SinCupo_SeRechaza()
        {
            var s = new InscripcionService(new RepoInscripcionFalso { Respuesta = ResultadoInscripcionDatos.SinCupo });
            Assert.IsFalse(s.Inscribir(1, 10).Exito);
        }

        [TestMethod]
        public void Inscribir_ActividadInexistente_SeRechaza()
        {
            var s = new InscripcionService(new RepoInscripcionFalso { Respuesta = ResultadoInscripcionDatos.ActividadNoExiste });
            Assert.IsFalse(s.Inscribir(99, 10).Exito);
        }

        [TestMethod]
        public void Inscribir_ActividadNoDisponible_SeRechaza()
        {
            var s = new InscripcionService(new RepoInscripcionFalso { Respuesta = ResultadoInscripcionDatos.ActividadNoDisponible });
            Assert.IsFalse(s.Inscribir(1, 10).Exito);
        }

        [TestMethod]
        public void InscribirEstudiante_ComoEstudiante_SeRechazaSinLlamarAlRepositorio()
        {
            var repo = new RepoInscripcionFalso();
            var s = new InscripcionService(repo);
            Assert.IsFalse(s.InscribirEstudiante(1, 10, "Estudiante").Exito);
            Assert.AreEqual(0, repo.Llamadas);
        }

        [TestMethod]
        public void InscribirEstudiante_ComoAdministrador_Funciona()
        {
            var s = new InscripcionService(new RepoInscripcionFalso());
            Assert.IsTrue(s.InscribirEstudiante(1, 10, "Administrador").Exito);
        }

        [TestMethod]
        public void Inscribir_IdsInvalidos_NoLlamaAlRepositorio()
        {
            var repo = new RepoInscripcionFalso();
            var s = new InscripcionService(repo);
            Assert.IsFalse(s.Inscribir(0, 10).Exito);
            Assert.IsFalse(s.Inscribir(1, -5).Exito);
            Assert.AreEqual(0, repo.Llamadas);
        }

        [TestMethod]
        public void Inscribir_ErrorDeDatos_DevuelveMensajeGenerico()
        {
            var s = new InscripcionService(new RepoInscripcionFalso { Lanzar = true });
            var r = s.Inscribir(1, 10);
            Assert.IsFalse(r.Exito);
            Assert.IsFalse(r.Mensaje.Contains("fallo simulado"));
        }
    }

    [TestClass]
    public class ResenaServiceTests
    {
        private static ResenaDto Resena(int calificacion)
        {
            return new ResenaDto { ActividadId = 1, UsuarioId = 10, Calificacion = calificacion, Comentario = "Muy buena" };
        }

        [TestMethod]
        public void Guardar_CalificacionEnRango_Funciona()
        {
            var s = new ResenaService(new RepoResenaFalso());
            for (int c = 1; c <= 5; c++)
                Assert.IsTrue(s.Guardar(Resena(c)).Exito, "Calificación " + c);
        }

        [TestMethod]
        public void Guardar_CalificacionFueraDeRango_SeRechaza()
        {
            var repo = new RepoResenaFalso();
            var s = new ResenaService(repo);
            Assert.IsFalse(s.Guardar(Resena(0)).Exito);
            Assert.IsFalse(s.Guardar(Resena(6)).Exito);
            Assert.AreEqual(0, repo.Guardadas);
        }

        [TestMethod]
        public void Guardar_Duplicada_SeRechaza()
        {
            var s = new ResenaService(new RepoResenaFalso { Existe = true });
            Assert.IsFalse(s.Guardar(Resena(5)).Exito);
        }

        [TestMethod]
        public void Guardar_ComentarioMuyLargo_SeRechaza()
        {
            var s = new ResenaService(new RepoResenaFalso());
            var r = Resena(4);
            r.Comentario = new string('a', 501);
            Assert.IsFalse(s.Guardar(r).Exito);
        }
    }
}