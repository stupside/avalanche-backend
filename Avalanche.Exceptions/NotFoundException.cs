using System.Reflection;

namespace Avalanche.Exceptions;

public class NotFoundException : ArgumentException
{
    public NotFoundException(MemberInfo type, string? id = null) : base(id is null
        ? $"{type.Name} not found"
        : $"{type.Name} with id {id} not found"
    )
    {
    }

    public NotFoundException(Type type, Guid id) : this(type, id.ToString())
    {
    }
}