using UnityEngine;

public struct ProjectileProperties  //Un struct es basicamente una clase pero solo tiene
                                    //valores, no permite herencia, y mejora el rendimiento si son pocos datos los que guarda
{
    public Vector3 direction;
    public Vector3 initialPosition;
    public float initialSpeed;
    public float mass;
    public float drag;
}