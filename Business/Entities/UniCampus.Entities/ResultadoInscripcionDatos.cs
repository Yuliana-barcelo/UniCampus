namespace UniCampus.Entities
{
    public enum ResultadoInscripcionDatos
    {
        Exito,
        SinCupo,
        Duplicada,
        ActividadNoExiste,
        ActividadNoDisponible,   // Finalizada, Inactiva o con fecha ya vencida
        UsuarioNoValido          // No existe o no está activo
    }
}
