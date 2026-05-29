# Sistema de Gestión de Inventario (SGI-Inventario)

Este proyecto es un **Sistema Avanzado de Gestión Logística e Inventario**, diseñado con Arquitectura de 3 Capas (UI, BLL, DAL) utilizando **C# .NET** y **SQL Server**. Está especialmente optimizado para gestionar ingresos, salidas, auditorías físicas y visualizar un Dashboard de Inteligencia de Negocios en tiempo real para almacenes de materia prima (como cacao y café).

**Desarrollado para la Asociación de Productores Agroindustriales Tingo María.**

---

## 🚀 Características Principales

* **Dashboard BI en Tiempo Real:** Gráficos interactivos de entradas del periodo y stock físico con filtros por fechas (Hoy, Semana, Mes, Año).
* **Gestión de Stock:** Control absoluto de entradas, salidas y cantidad física actual por categorías (Base Cacao, Base Café, etc.).
* **Auditoría Transaccional:** Tabla de registros que muestra la fecha exacta, tipo de operación, cantidad y el usuario que realizó la transacción en el sistema.
* **Sistema de Roles Seguros:** Privilegios de acceso estrictos dependiendo si eres Administrador o Usuario Operativo.

---

## 🔐 Credenciales de Acceso

Para acceder al sistema de pruebas, utilice las siguientes cuentas pre-configuradas:

**Para Perfil Administrador (Acceso Total + Dashboard):**
* **Usuario:** `admin`
* **Contraseña:** `admin`

**Para Perfil Usuario (Solo Gestión de Entradas/Salidas):**
* **Usuario:** `user`
* **Contraseña:** `user`

---

## 🛠️ Instrucciones de Instalación

1. **Restaurar la Base de Datos:**
   * Abra **Microsoft SQL Server Management Studio (SSMS)**.
   * Haga clic derecho en *Databases* y seleccione **Restore Database...**
   * Seleccione *Device*, busque el archivo **`AnyStore_DB_Backup.bak`** incluido en este repositorio y restáurelo (esto cargará la base de datos `AnyStore` junto con los más de 5,000 registros de prueba).
   
2. **Ejecutar el Proyecto:**
   * Abra la solución **`AnyStore.sln`** utilizando **Microsoft Visual Studio**.
   * Verifique que la cadena de conexión (`connstrng`) en el archivo `App.config` apunte al nombre de su servidor SQL Server local.
   * Presione **F5** (o *Iniciar*) para compilar y lanzar la aplicación.
