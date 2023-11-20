namespace GameTrilha.API.Helpers;

/// <summary>
/// Helper class with extension methods for checking null objects.
/// </summary>
/// <summary xml:lang="pt-BR">
/// Classe auxiliar com métodos de extensão para verificar objetos nulos.
/// </summary>
public static class NullObjectHelper
{
    /// <summary>
    /// Returns true if the object is null.
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Retorna verdadeiro se o objeto for nulo.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is null.</returns>
    /// <remarks>
    /// This method is useful when you have a nullable type and you want to check if the value is null.
    /// </remarks>
    /// <example>
    /// <code>
    /// int? value = null;
    /// if (value.IsNull())
    /// {
    ///   // Do something
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="IsNotNull(object?)"/>
    /// <seealso cref="ThrowIfNull{T}(T?, string?)"/>
    /// <seealso cref="ThrowIfNull(object?, string?)"/>
    public static bool IsNull(this object? obj) => obj is null;

    /// <summary>
    /// Returns true if the object is not null.
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Retorna verdadeiro se o objeto não for nulo.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is not null.</returns>
    /// <remarks>
    /// This method is useful when you have a nullable type and you want to check if the value is not null.
    /// </remarks>
    /// <example>
    /// <code>
    /// int? value = null;
    /// if (value.IsNotNull())
    /// {
    ///  // Do something
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="IsNull(object?)"/>
    /// <seealso cref="ThrowIfNull{T}(T?, string?)"/>
    /// <seealso cref="ThrowIfNull(object?, string?)"/>
    /// <seealso cref="ThrowIfNull{T}(T?, string?)"/>
    public static bool IsNotNull(this object? obj) => !obj.IsNull();

    /// <summary>
    /// Returns true if the object is null.
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Retorna verdadeiro se o objeto for nulo.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is null.</returns>
    /// <remarks>
    /// This method is useful when you have a nullable type and you want to check if the value is null.
    /// </remarks>
    /// <example>
    /// <code>
    /// int? value = null;
    /// if (value.IsNull())
    /// {
    ///    // Do something
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="IsNotNull{T}(T?)"/>
    /// <seealso cref="ThrowIfNull{T}(T?, string?)"/>
    public static bool IsNull<T>(this T? obj) where T : class => obj is null;

    /// <summary>
    /// Returns true if the object is not null.
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Retorna verdadeiro se o objeto não for nulo.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is not null.</returns>
    /// <remarks>
    /// This method is useful when you have a nullable type and you want to check if the value is not null.
    /// </remarks>
    /// <example>
    /// <code>
    /// int? value = null;
    /// if (value.IsNotNull())
    /// {
    ///   // Do something
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="IsNull{T}(T?)"/>
    /// <seealso cref="ThrowIfNull{T}(T?, string?)"/>
    public static bool IsNotNull<T>(this T? obj) where T : class => !obj.IsNull();

    /// <summary>
    /// Throws a <see cref="NullReferenceException"/> if the object is null.
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Lança uma <see cref="NullReferenceException"/> se o objeto for nulo.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <param name="message">The message to include in the exception</param>
    /// <exception cref="NullReferenceException">Thrown if the object is null.</exception>
    public static void ThrowIfNull<T>(this T? obj, string? message = null) where T : class
    {
        if (obj.IsNull())
            throw new NullReferenceException(message);
    }
}