using System;
using System.Threading;
using System.Threading.Tasks;

public enum DeviceState
{
    Creado,
    EnPiscina,
    EnUso
}

// Dispositivo IoT reutilizable
public class IoTDevice
{
    // Contador de IDs 
    private static int _siguienteId = 0;
    public int Id { get; }
    // Consumo por segundo del dispositivo
    public double ConsumoPorSegundo { get; }
    // Estado del dispositivo
    public DeviceState Estado { get; private set; }

    public IoTDevice(double consumoPorSegundo)
    {
        // Generar Id único 
        Id = Interlocked.Increment(ref _siguienteId);
        ConsumoPorSegundo = consumoPorSegundo;
        Estado = DeviceState.Creado;
    }

    // Simula el uso del dispositivo durante 'segundosDuracion' segundos
    // Devuelve true si la central pudo suministrar la energía requerida
    public async Task<bool> UseAsync(double segundosDuracion)
    {
        if (segundosDuracion <= 0) return true;
        var energiaRequerida = ConsumoPorSegundo * segundosDuracion;
    
        await Task.Delay(TimeSpan.FromSeconds(segundosDuracion * 0.1));
        // Devuelve true si la central pudo restar la energía
        return Singleton.Instance.TryConsume(energiaRequerida);
    }

    public void Reset()
    {
        // Reseteo antes de ponerlo en la bolsa
        Estado = DeviceState.Creado;
    }

    public void MarcarEnUso()
    {
        // Marcar como en uso cuando el pool lo entrega a una tarea
        Estado = DeviceState.EnUso;
    }

    public void MarcarEnPiscina()
    {
        // Marcar como disponible en la piscina
        Estado = DeviceState.EnPiscina;
    }
}