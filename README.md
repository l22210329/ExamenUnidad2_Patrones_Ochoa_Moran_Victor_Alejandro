
# Examen Unidad 2 - Patrones de Diseño
**Autor:** Ochoa Morán Víctor Alejandro  
**Materia:** Patrones de Diseño  
**Tema:** Control de Energía y Dispositivos IoT Domésticos

---

## Descripción del Proyecto
El programa implementa un sistema de control para dispositivos IoT domésticos, aplicando patrones de diseño estudiados en la unidad.

### Funcionamiento
- **Clase Principal:** `Program`
- **Ejecución:** Mediante consola con el comando `dotnet run`
- **Salida:** Resultados impresos en consola

---

## Instalación y Ejecución

### Prerrequisitos
| Componente | Enlace |
|------------|--------|
| .NET 8.0 SDK | [Descargar aquí](https://dotnet.microsoft.com/download) |
| Git | [Descargar aquí](https://git-scm.com/downloads) |

### Pasos para Ejecutar

1. **Clonar Repositorio** en Visual Studio Code
   ```bash
   git clone https://github.com/l22210329/ExamenUnidad2_Patrones_Ochoa_Moran_Victor_Alejandro.git
   
### Para que se usa el operador lambda "=>" 
Cuando se accede al "Instance" e evalúa la expresión a la derecha y ese valor se devuelve. Es decir, la propiedad devuelve el resultado de _instance.Value .

Es decir que el quivalente de "public static Singleton Instance => _instance.Value;" seria, esto es por que se esta en modo lectura.
```C#
public static Singleton Instance
{
get { return _instance.Value; }
}
