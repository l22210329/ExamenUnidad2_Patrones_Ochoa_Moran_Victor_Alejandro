using System;

public sealed class Singleton
{
    // ESTA LÍNEA ES MAGIA: Garantiza que solo exista UN medidor
    // revisar por que se pone "=>" y poner en comentarios para que sirve y por que lo puse que mo se te olvide !!!!!
    private static readonly Lazy<Singleton> _instance = new(() => new Singleton());
    public static Singleton Instance => _instance.Value;

    // energía disponible en la central
    private double _energiaDisponible;
    // Candado para que no se mescle 
    private readonly object _candado = new();

    // Constructor PRIVADO nadie más puede crear medidores
    private Singleton() => _energiaDisponible = 0;

    // Método para AGREGAR energía
    public void AddEnergy(double amount)
    {
        if (amount <= 0) return;
        lock (_candado)
        {
            _energiaDisponible += amount;
        }
    }

    // Intenta consumir energía y devuelve true si la operación tuvo éxito.
    public bool TryConsume(double amount)
    {
        if (amount <= 0) return true;
        lock (_candado)
        {
            if (_energiaDisponible >= amount)
            {
                _energiaDisponible -= amount;
                return true;
            }
            return false;
        }
    }

    // lectura segura del estado de energía.
    public double GetAvailableEnergy()
    {
        lock (_candado)
        {
            return _energiaDisponible;
        }
    }
}