namespace Dirichlet.Numerics;

// placeholder to be more dotnet7 compatible

public interface ISignedNumber<T> : INumber<T> { }

public interface IUnsignedNumber<T> : INumber<T> { }

public interface INumber<T> {
    // static (dotnet 7)
    T Sign(T value);
}
