using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class ObjectPool : IDisposable
{
    private readonly ConcurrentBag<IoTDevice> _bolsa = new();
    // Semáforo: controla cuántas hay simultáneamente
    private readonly SemaphoreSlim _semaforo;
    // Tamaño máximo del pool
    private readonly int _tamanoMax;
    // Contador de objetos creados
    private int _creados = 0;
    // Consumo de los aparatos
    private readonly double _consumoDispositivo;
    
    public ObjectPool(int maxSize, double ConsumoporSegundo)
    {
        if (maxSize <= 0) throw new ArgumentException("", nameof(maxSize));
        _tamanoMax = maxSize;
        _semaforo = new SemaphoreSlim(maxSize, maxSize);
        _consumoDispositivo = ConsumoporSegundo;
    }

    // aqui empieza la magia del Object Pool MÉTODO PRINCIPAL :D
    public async Task<IoTDevice?> AcquireAsync(TimeSpan timeout)
    {
        // Espera hasta tener permiso para continuar
        var acquired = await _semaforo.WaitAsync(timeout);
        if (!acquired) return null;

        // Reutilizar si hay uno disponible en la bolsa.
        if (_bolsa.TryTake(out var device))
        {
            device.MarcarEnUso();
            Console.WriteLine($"[Pool] Dispositivo #{device.Id} salió de la piscina (reutilizado). Estado: {device.Estado}");
            return device;
        }
        // se ve si hay espacios libres 
        var creadosAhora = Interlocked.Increment(ref _creados);
        if (creadosAhora <= _tamanoMax)
        {
            var nuevo = new IoTDevice(_consumoDispositivo);
            nuevo.MarcarEnUso();
            Console.WriteLine($"[Pool] Dispositivo #{nuevo.Id} creado y sale de la piscina. Estado: {nuevo.Estado}");
            return nuevo;
        }

        
        // aqui se decrementa si se llego al limite y se espera hasta que halla espacios nuevos 
        Interlocked.Decrement(ref _creados);
        while (true)
        {
            if (_bolsa.TryTake(out device))
            {
                device.MarcarEnUso();
                Console.WriteLine($"[Pool] Dispositivo #{device.Id} salió de la piscina (espera resuelta). Estado: {device.Estado}");
                return device;
            }
            await Task.Delay(20);
        }
    }

    // Devuelve un dispositivo al pool: resetea, marca y libera
    public void Release(IoTDevice dispositivo)
    {
        dispositivo.Reset();
        dispositivo.MarcarEnPiscina();
        _bolsa.Add(dispositivo);
        Console.WriteLine($"[Pool] Dispositivo #{dispositivo.Id} devuelto a la piscina. Estado: {dispositivo.Estado}");
        _semaforo.Release();
    }
    
    //este metodo solo limpia el semaforo
    public void Dispose()
    {
        _semaforo?.Dispose();
    }
}