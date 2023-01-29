using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    static public bool          goalMet = false;

    void OnTriggerEnter(Collider other) {
        // Когда в область действвия триггера попадает что-то, 
        // проверить является ли это снарядом
        if (other.gameObject.tag == "Projectile") {
            // Если это снаряд, присвоить полю goalMet значение true
            Goal.goalMet = true;
            // Изменить альфа-канал цвета, чтобы увеличить непрозрачность
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 1;
            mat.color = c;
        }
    }
}
