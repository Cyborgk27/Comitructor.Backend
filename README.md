# Comitructor.Backend 🚀

Núcleo de servicios API para la prueba **Comitructor**. Esta solución está construida sobre **.NET 8** utilizando una arquitectura limpia y modular, diseñada para gestionar la lógica de negocio y persistencia de datos del sistema.

## 📋 Descripción del Proyecto
El backend es una Web API robusta que expone los servicios necesarios para la gestión de requerimientos, inventarios y procesos de negocio. Actúa como el puente entre la base de datos SQL Server y el frontend en Angular, garantizando la integridad de los datos y la seguridad mediante una estructura basada en interfaces y servicios.

---

## 🏗️ Estructura de la Solución (Explorador de Soluciones)
Basado en la arquitectura del proyecto:
- **Comitructor.WebApi:** Proyecto principal de entrada. Contiene los `Controllers`, `Middleware` y configuraciones de `Swagger`.
- **Comitructor.Infrastructure:** Capa de acceso a datos y servicios externos. Incluye:
    - **Persistence:** Contexto de base de datos (EF Core).
    - **Migrations:** Historial de cambios en el esquema de BD.
    - **Identity:** Gestión de estado de usuario.
    - **Services:** Implementación de la lógica de servicios.
- **Common / Entities / Interfaces:** Capas que definen el modelo de dominio y los contratos del sistema.

---

## 🛠️ Tecnologías Principales
- **Framework:** .NET 8 (LTS)
- **Base de Datos:** SQL Server (Entity Framework Core)
- **Documentación:** Swagger / OpenAPI (v1)
- **Contenedores:** Docker (Multi-stage build)

---

## 🐳 Despliegue con Docker

El proyecto cuenta con un **Dockerfile** optimizado (como se muestra en el código fuente) que utiliza fases de compilación y publicación separadas para generar una imagen de producción ligera.

### 1. Construir la Imagen
Desde la raíz de la solución, ejecuta:
```bash
docker build -t comitructor-backend .
```

### 2. Ejecutar el Contenedor
Inicia el servicio mapeando los puertos de red:

```Bash
docker run -d -p 32769:8081 --name comitructor-api comitructor-backend
```