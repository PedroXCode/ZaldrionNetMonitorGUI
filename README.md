# ⚡ Zaldrion Net Monitor (GUI)

Herramienta ligera para monitorear conexiones de red **en tiempo real** usando `netstat -ano`, desarrollada en **C# + WPF (.NET 8)**.

---

## 🖥️ Compatibilidad

| Plataforma | Soporte |
|-----------|---------|
| **Windows 10 / 11** | ✔️ |
| **.NET 8 Runtime** | ✔️ Necesario |
| Linux / macOS | ❌ No compatible |

---

## 📦 Instalación

### ✅ Opción 1: Ejecutar EXE (recomendado)
> *Disponible cuando se publique el release.*

1. Descargar `ZaldrionNetMonitorGUI.zip`
2. Extraer la carpeta
3. Ejecutar:
   ```
   ZaldrionNetMonitorGUI.exe
   ```

No requiere instalación. Solo .NET 8 Runtime.

---

### 🛠️ Opción 2: Ejecutar desde el código fuente

**Requisitos:**
- Windows 10/11
- .NET 8 SDK

**Comandos:**
```sh
cd src/ZaldrionNetMonitorGUI
dotnet build
dotnet run
```

---

## 🚀 Cómo usar

1. Abrir la aplicación.  
2. Escribir el **nombre del proceso** a monitorear (ej: `chrome`, `brave`).  
3. Seleccionar protocolo:
   - Todos
   - TCP
   - UDP  
4. Pulsar **Actualizar** para refrescar conexiones.
5. (Opcional) Exportar datos actuales en **JSON**.

---

## 📡 Información que muestra

| Campo | Descripción |
|-------|-------------|
| **Estado** | Established, Listening, etc. |
| **Proceso** | Nombre del ejecutable |
| **PID** | ID del proceso |
| **IP Remota** | Dirección destino |
| **Puerto** | Puerto remoto |
| **Protocolo** | TCP/UDP |
| **Última Vista** | Timestamp |

---

## 🔧 Características principales

- Monitoreo en tiempo real (actualiza cada 3 segundos)
- Filtro avanzado por proceso
- Filtro por protocolo (TCP / UDP / All)
- Exportación a JSON
- Interfaz oscura y moderna
- Bajo consumo de recursos
- No requiere permisos de administrador (pero mejora precisión)

---

## 📁 Estructura del proyecto

```
/src
 └── ZaldrionNetMonitorGUI
      ├── App.xaml
      ├── MainWindow.xaml
      ├── MainWindow.xaml.cs
      ├── ConnectionInfo.cs
      ├── ZaldrionNetMonitorGUI.csproj
/assets
 └── screenshots (opcional)
/README.md
/.gitignore
```

---

## ⚠️ Limitaciones

- No captura ni inspecciona paquetes (no es Wireshark).
- Solo muestra conexiones activas del sistema operativo.
- Algunas conexiones requieren ejecutar como administrador para mayor precisión.

---

## 📜 Licencia

MIT License

Copyright (c) 2025

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
THE SOFTWARE.
