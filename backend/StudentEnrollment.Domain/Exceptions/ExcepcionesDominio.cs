namespace StudentEnrollment.Domain.Exceptions;

public class ExcepcionDominio : Exception
{
    public ExcepcionDominio(string mensaje) : base(mensaje) { }
}

public class ExcepcionLimiteInscripciones : ExcepcionDominio
{
    public ExcepcionLimiteInscripciones() : base("No puedes inscribirte en más de 3 materias.") { }
}

public class ExcepcionProfesorDuplicado : ExcepcionDominio
{
    public ExcepcionProfesorDuplicado() : base("Ya tienes una clase con este profesor.") { }
}

public class ExcepcionAsignaturaLlena : ExcepcionDominio
{
    public ExcepcionAsignaturaLlena() : base("Esta materia ya alcanzó su cupo máximo.") { }
}

public class ExcepcionYaInscrito : ExcepcionDominio
{
    public ExcepcionYaInscrito() : base("Ya estás inscrito en esta materia.") { }
}
