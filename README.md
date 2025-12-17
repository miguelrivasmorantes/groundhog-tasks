# groundhog-tasks

# 🏠 Gestor de Tareas Domésticas

Este proyecto surgió para resolver el problema de la repartición de responsabilidades entre compañeros de nuestro piso. Su objetivo es automatizar la asignación, recordatorio y seguimiento de las tareas de limpieza mediante ciclos personalizados, por ejemplo un mes.

---

### 🚀 Funcionalidades Principales

El sistema opera sobre una **API REST** que gestiona:

* **Gestión de Grupos:** Creación de espacios de convivencia con roles de administrador y miembros.
* **Asignación Flexible:** Tareas asignables a uno o varios usuarios simultáneamente.
* **Ciclos de Limpieza:** Configuración de periodicidad, fecha de inicio y repeticiones (N veces).
* **Notificaciones:**
    * 🔔 Recordatorios automáticos previos a la tarea (Email).
    * 📝 Solicitud de informe de cumplimiento post-tarea.

---

### Tecnologías

| Componente | Tecnología | Descripción |
| :--- | :--- | :--- |
| **Backend** | ![.NET](https://img.shields.io/badge/.NET-C%23-purple) | Lógica de negocio y API. |
| **Frontend** | ![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4) | SPA Cliente. |
| **Database** | ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Db-blue) | Persistencia de datos. |
| **Scheduler** | ![Quartz.NET](https://img.shields.io/badge/Quartz.NET-Jobs-green) | Tareas programadas. |
| **Mail** | ![SMTP](https://img.shields.io/badge/Gmail-SMTP-red) | Envío de correos. |

---