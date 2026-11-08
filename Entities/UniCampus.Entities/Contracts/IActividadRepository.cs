namespace UniCampus.Entities.Contracts
{
    public interface IActividadRepository
    {
        /// <summary>
        /// Persiste la actividad y devuelve un identificador positivo.
        /// La implementación concreta y sus excepciones corresponden a DAL.
        /// </summary>
        int Guardar(ActividadDto actividad);
    }
}