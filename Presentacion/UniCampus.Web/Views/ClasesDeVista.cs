using System;

namespace UniCampus.Web.Views
{
    /// <summary>Evento con un identificador (actividad o inscripción).</summary>
    public class IdEventArgs : EventArgs
    {
        public int Id { get; private set; }

        public IdEventArgs(int id)
        {
            Id = id;
        }
    }

    /// <summary>Un botón del paginador (« ‹ 1 2 3 › »).</summary>
    public class ItemPaginacion
    {
        public string Texto { get; set; }
        public int Pagina { get; set; }
        public bool Activa { get; set; }
        public bool Deshabilitada { get; set; }
    }
}
