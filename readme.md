# AIM Trainer

## Idea

Es un juego de entrenamiento de duelos, estilo MOBA, en una grilla, donde se premia la agilidad, la precisión y la reacción del jugador. El jugador recibe una serie de enemigos con más, menos vida, enemigos que solo atacan físico, que atacan con magia, etc. El usuario debería poder seleccionar la dificultad del enemigo que quiere enfrentar. La velocidad de los ataques de los enemigos también se podrá configurar. Los puntos se definen por el tiempo y cantidad de acciones necesarias para eliminar el enemigo y son solo para uso personal de jugador.

## Futuro

Tener una plantilla de puntos por sesión.

Poder hacer seguimiento del progreso del jugador.

Poder guardar repeticiones de los duelos.

Modalidades de duelo múltiple. Inclusive teniendo aliados, para 2v2, 3v3.

## Mecánicas básicas

Todos los movimientos se basan en una grilla. Nadie puede caminar diagonalmente.

Los enemigos van a avanzar hacia el jugador haciendo esquives y movimientos aleatorios mientras rodea e intenta golpear al personaje fisicamente, tambien siendo un enemigo con magia puede cad atanto frenarse y atacar con magia a la distancia.

Nos movemos con las flechas o WASD, para esquivar y dirigir los ataques.

Con la tecla CTRL hacemos daño físico al enemigo que se encuentre delante.

Con la tecla U nos podemos curar HP.

Con el cursor podemos cargar el hechizo de ataque y descargarlo sobre el enemigo. Si erramos el ataque, perdemos la carga y debemos volver a cargarlo.

En caso de morir podemos revivirnos con la tecla R.

PARA DEPURAR: Con la tecla X podemos crear más enemigos.

## Stats

El personaje principal puede configurar su vida de 50 a 800 HP.

Los enemigos pueden tener de 50 a 800 HP.

## Teclado

- Golpe - Hace 50 de daño. Tiene intervalo de 1 por segundo para el jugador.
- Poción - Cura 50 de vida. Solo puede usarlo el jugador. Tiene intervalo de 1 por segundo.

## Hechizos

- Daño - Hace 50 daño sobre el enemigo al clickearlo. Se puede usar solo sobre un enemigo. No se puede usar a mayor distancia que la de juego. Tiene intervalo de 1 por segundo.
- Cura - Cura 50 daño sobre el jugador al clickearlo. Se puede usar solo sobre el jugador. Tiene intervalo de 1 por segundo.
- Paralizar - Deja detenido al personaje en su movimiento. No se puede usar a mayor distancia que la de juego. Require 1 segundo para cargar, y 1 para volver a usarlo.
- Quitar Parálisis - Remueve el efecto de parálisis del movimiento. Require 1 segundo para cargar, y 1 para volver a usarlo.