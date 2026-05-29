# Sistema de Gestión de Inventario (SGI-Inventario)

Bienvenido al repositorio oficial del **Sistema Avanzado de Gestión Logística e Inventario**. Este proyecto fue desarrollado utilizando la robusta Arquitectura de 3 Capas (UI, BLL, DAL) con **C# .NET** y **SQL Server**. 

Está diseñado específicamente para optimizar operaciones en almacenes de materia prima (como cacao y café), pero puede adaptarse fácilmente a cualquier tipo de inventario físico. Destaca por incluir un Dashboard de Inteligencia de Negocios (BI) de alto rendimiento, gestión de stock en tiempo real y una pista de auditoría inquebrantable.

**Desarrollado para la Asociación de Productores Agroindustriales Tingo María.**

---

## 🚀 Características Principales

* **Dashboard BI en Tiempo Real:** Gráficos interactivos de ingresos y stock físico con filtros automáticos de fecha (Hoy, Semana, Mes, Año).
* **Gestión Inteligente de Stock:** Control detallado de entradas, salidas y sumatorias automáticas por categorías de producto.
* **Auditoría Transaccional:** Historial automático que registra la fecha exacta, tipo de movimiento, cantidad (Kg) y el operario responsable de la transacción.
* **Seguridad y Roles:** Privilegios de acceso estrictos (Administrador con acceso a estadísticas globales y Operario para tareas del día a día).

---

## ⚙️ Cómo descargar y usar este proyecto (Guía Rápida)

Sigue estos sencillos pasos para levantar el sistema y la base de datos en tu propia computadora:

### 1. Clonar el Repositorio
Abre tu terminal (Git Bash o CMD) y ejecuta el siguiente comando para descargar el código fuente a tu PC:
```bash
git clone https://github.com/CharsDarwin/Sistemas-de-informacion.git
```

### 2. Implementar la Base de Datos (SQL Server)
El proyecto incluye una base de datos lista con **más de 5,000 registros de prueba** para que puedas experimentar el rendimiento del Dashboard de inmediato.
1. Abre **Microsoft SQL Server Management Studio (SSMS)**.
2. En el panel izquierdo, haz clic derecho sobre la carpeta **Databases** y selecciona **Restore Database...** (Restaurar base de datos).
3. Selecciona la opción **Device** (Dispositivo), haz clic en el botón `...`, y busca el archivo **`AnyStore_DB_Backup.bak`** que viene incluido en la carpeta principal que acabas de descargar.
4. Haz clic en **OK** para restaurar. Esto instalará automáticamente la base de datos `AnyStore` con todas sus tablas y datos pre-poblados.

### 3. Configurar la Conexión del Sistema
1. Abre el archivo **`AnyStore.sln`** haciendo doble clic (se abrirá en Microsoft Visual Studio).
2. En el panel de la derecha (*Explorador de Soluciones*), busca y abre el archivo **`App.config`**.
3. Localiza la etiqueta de conexión (`connectionString`) y cambia el `Data Source` por el nombre de tu servidor SQL local (por ejemplo, `.\SQLEXPRESS` o `localhost`):
   ```xml
   <add name="connstrng" connectionString="Data Source=TU_SERVIDOR_AQUI;Initial Catalog=AnyStore;Integrated Security=True;" providerName="System.Data.SqlClient" />
   ```

### 4. Compilar y Ejecutar
1. En el menú superior de Visual Studio, ve a **Compilar** (Build) y selecciona **Recompilar Solución**.
2. Presiona **F5** o haz clic en **Iniciar** (Start) para ejecutar la aplicación.

---

## 🔐 Credenciales de Acceso (Pruebas)

Una vez que la pantalla de inicio cargue, utiliza estas cuentas pre-configuradas para explorar el sistema:

**Perfil Administrador (Acceso Total y Dashboard Analítico):**
* **Usuario:** `admin`
* **Contraseña:** `admin`

**Perfil Operario (Solo Gestión de Entradas/Salidas de Almacén):**
* **Usuario:** `user`
* **Contraseña:** `user`

---
*¡Siéntete libre de descargar, explorar el código y aprender de este proyecto! Si te ha sido de utilidad, no olvides dejarle una ⭐ a este repositorio.*
