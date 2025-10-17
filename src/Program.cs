using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Singleton (Central de Energía) + Object Pool (Dispositivos IoT)");

        const int tamanoPool = 5; // máximo dispositivos simultáneos reutilizables
        const double consumoPorSegundoDispositivo = 5.0; // unidades por segundo
        using var pool = new ObjectPool(tamanoPool, consumoPorSegundoDispositivo);

        // Inicializar la central de energía
        Singleton.Instance.AddEnergy(200.0);
        Console.WriteLine($"Energía inicial: {Singleton.Instance.GetAvailableEnergy():F2}");

        // Simular peticiones de dispositivos 
        var rnd = new Random();
        const int cantidadTareas = 5;
        var tareas = new Task[cantidadTareas];
        
        for (int i = 0; i < cantidadTareas; i++)
        {
            tareas[i] = RunDeviceTaskAsync(i, pool, rnd);
        }

        await Task.WhenAll(tareas);

        Console.WriteLine($"Simulación finalizada. Energía restante: {Singleton.Instance.GetAvailableEnergy():F2}");
        Console.WriteLine("Fin.");
    }

    static async Task RunDeviceTaskAsync(int idTarea, ObjectPool pool, Random rnd)
    {
        // pequeño retardo para aumentar concurrencia
        await Task.Delay(rnd.Next(0, 200));
        Console.WriteLine($"Tarea {idTarea}: solicitando dispositivo...");

        var dispositivo = await pool.AcquireAsync(TimeSpan.FromSeconds(5));
        if (dispositivo == null)
        {
            Console.WriteLine($"Tarea {idTarea}: timeout al adquirir dispositivo.");
            return;
        }

        Console.WriteLine($"Tarea {idTarea}: adquirió dispositivo #{dispositivo.Id} (consumo {dispositivo.ConsumoPorSegundo}/s)");
        var segundos = rnd.NextDouble() * 3.0 + 0.5; 
        var exitoso = await dispositivo.UseAsync(segundos);
        if (exitoso)
        {
            Console.WriteLine($"Tarea {idTarea}: dispositivo #{dispositivo.Id} usó {segundos:F2}s con éxito. Energía restante: {Singleton.Instance.GetAvailableEnergy():F2}");
        }
        else
        {
            Console.WriteLine($"Tarea {idTarea}: dispositivo #{dispositivo.Id} NO pudo consumir energía suficiente (requería {dispositivo.ConsumoPorSegundo * segundos:F2}). Energía restante: {Singleton.Instance.GetAvailableEnergy():F2}");
        }

        pool.Release(dispositivo);
        Console.WriteLine($"Tarea {idTarea}: liberó dispositivo #{dispositivo.Id}");
    }
}