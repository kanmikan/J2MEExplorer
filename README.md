# J2ME Explorer
J2ME explorer es un pequeño programa diseñado para listar y probar carpetas enteras de aplicaciones j2me, de manera mas ordenada y visual.

![cap3](https://i.ibb.co/q0R98mz/cap3.png)

# Caracteristicas
Al arrastrar una carpeta que contiene archivos midlet validos, el programa trata de leer el manifest y mostrar el icono y el nombre de la app, si existen.

El programa es capaz de cargar una carpeta entera con por ejemplo 10000 archivos midlet sin tildarse ni relentizar el scroll. Se puede configurar el script "emu_launch.bat" para que apunte a un emulador de j2me, como kemulator nnmod x64, Al hacer click sobre un elemento automáticamente se llama al emulador. 
- Posibilidad de ordenar los elementos alfabeticamente usando el nombre especificado en el manifest, el vendor o el nombre de archivo.

- El programa va añadiendo los elementos asincronicamente, sin interrumpir el uso, tambien hace una busqueda recursiva de subcarpetas.

# Cosas que faltan

- Todavía no está implementado el filtro o buscador, por ejemplo "240x320_AquaDig.jar" se podría filtrar usando "240x320", o "Herocraft" trataría de mostrar solo los juegos que especifiquen como vendor a "herocraft"
