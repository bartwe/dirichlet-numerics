namespace Dirichlet.Numerics;

public interface INumber<T> {
    // static (dotnet 7)
    T Sign(T value);
}