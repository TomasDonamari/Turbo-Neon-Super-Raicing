using UnityEngine;

public class BotonMoviil : MonoBehaviour
{
    public PlayerController player;
    public bool esAcelerador;
    public bool esReversa;
    public bool esIzquierda;
    public bool esDerecha;
    public bool esFreno;

    public void BotonPresionado()
    {
        if (player == null) return;

        if (esAcelerador) player.aceleracionMovil = 1f;
        if (esReversa) player.aceleracionMovil = -1f;
        if (esIzquierda) player.direccionMovil = -1f;
        if (esDerecha) player.direccionMovil = 1f;
        if (esFreno) player.frenoMovil = true;
    }

    public void BotonSoltado()
    {
        if (player == null) return;

        if (esAcelerador || esReversa) player.aceleracionMovil = 0f;
        if (esIzquierda || esDerecha) player.direccionMovil = 0f;
        if (esFreno) player.frenoMovil = false;
    }
}