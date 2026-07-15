# 🦕 PaleoAR — Unity (Módulo de Realidad Aumentada)

![Unity](https://img.shields.io/badge/Engine-Unity-000000?logo=unity)
![C#](https://img.shields.io/badge/Scripting-C%23-239120?logo=csharp)
![AR](https://img.shields.io/badge/Reconocimiento-Image%20Targets-orange)
![React Native Bridge](https://img.shields.io/badge/Integraci%C3%B3n-Deep%20Linking-4630EB)

Lado de Realidad Aumentada de **PaleoAR**, encargado de reconocer los marcadores visuales (image targets) colocados junto a las vitrinas del museo y desplegar los modelos 3D e información interactiva de cada fósil. Este módulo se activa desde la app principal en React Native mediante deep linking; el resto del sistema (autenticación, gamificación, panel de administrador, backend) vive en [PaleoAR](https://github.com/Star0110/PaleoAR).

## 🚀 Tecnologías Clave

| **Componente**          | **Detalle**                                                        |
|---------------------------|---------------------------------------------------------------------|
| Motor                     | Unity                                                               |
| Reconocimiento de marcadores | Motor de image targets (carpeta `QCAR`) para detectar los marcadores físicos del museo |
| Scripting                 | C#                                                                  |
| Integración con la app    | Deep linking desde el módulo `ScanScreen` de la app React Native   |

> **Nota:** el desglose de lenguajes del repositorio (mayoritariamente C++/C) corresponde a las librerías nativas del motor de reconocimiento de imágenes incluidas en el proyecto; la lógica de la experiencia AR está escrita en C#.

## 🔥 Qué hace este módulo

- 📷 Reconoce al menos tres marcadores visuales (image targets) distribuidos en el museo.
- 🦴 Despliega el modelo 3D y contenido interactivo correspondiente a cada fósil escaneado.
- 🔗 Se lanza directamente desde la app principal vía deep linking, activado al tocar una notificación o desde la pantalla de escaneo.
- 🔄 Preparado para que el contenido (imagen de referencia, modelo 3D) pueda actualizarse remotamente por el administrador sin necesidad de reemplazar el marcador físico ni republicar la app.

## 🏗️ Estructura del Proyecto

```
Assets/            # Escenas, scripts C#, prefabs y recursos del módulo AR
Packages/          # Dependencias del proyecto Unity
ProjectSettings/   # Configuración del proyecto
QCAR/              # Motor de reconocimiento de marcadores (image targets)
UnityExport/       # Build exportado para integración con la app
```

## 👥 Participantes del Proyecto

**Equipo**:
- [@Star](https://github.com/Star0110) — Starenka Susana Ortiz Gallegos
- [@Valentina](https://github.com/ValentinaVillarreal) - Valentina Esquivel Villarreal
- [@IsraelJP](https://github.com/IsraelJP) - Israel Jiménez Palomino
- 

## 🔗 Proyecto relacionado

- [PaleoAR (React Native)](https://github.com/Star0110/PaleoAR) — App principal, backend Firebase y panel de administrador.

## ⚖️ Licencia

Proyecto académico desarrollado para la materia de Tecnología Móvil, Instituto Tecnológico de Toluca.
