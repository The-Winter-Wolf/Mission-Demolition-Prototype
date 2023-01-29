using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;

    // Поля, устанавливаемые в инспекторе Unity
    [Header("Set in  Inspector")]
    public GameObject       prefabProjectile;
    public float            velocityMult = 8f;

    // Поля, устанавливаемые динамически
    [Header("Set dynamically")]
    public GameObject       launchPoint;
    public Vector3          launchPos;
    public GameObject       projectile;
    public bool             aimingMode;

    private Rigidbody       projectileRigidbody;

    static public Vector3 LAUNCH_POS {
        get {
            if (S == null) {return Vector3.zero;}
            return S.launchPos;
        }
    }

    void Awake() // Вызывается при создании экземпляра класса, т.е перед Start
    {
        // Найти дочерний объект и получить ссылку на его GameObject
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }

    // Вызывается при вхождении мыши в область коллайдера или триггера
    void OnMouseEnter() {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);   
    }

    // Вызывается при выходе мыши из области коллайдера или триггера
    void OnMouseExit() {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    // Вызывается только в одном кадре после нажатия кнопки мыши
    void OnMouseDown() {
        // Игрок нажал кнопку мыши, когда указатель находился над рогаткой
        aimingMode = true;
        // Создать снаряд
        projectile = Instantiate(prefabProjectile) as GameObject;
        // Поместить в точку LaunchPoint
        projectile.transform.position = launchPos;
        // Сделать его кинематическим
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;
    }

    void Update() {
        // Если рогатка не в режиме прицеливания не выполнять этот код
        if (!aimingMode) return;
        
        // Получить текущие экранные координаты указателя мыши
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Найти разность координат между launchPos и mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        // Ограничить mouseDelta радиусом коллайдера объекта Slingshot
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();  // Устанавливает длину вектора равной 1 с сохранением его направления
            mouseDelta *= maxMagnitude;
        }

        // Передвинуть снаряд в новую позицию
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        // Если кнопка мыши (0) отпущена, возвращает true в одном следующем кадре
        if (Input.GetMouseButtonUp(0)) {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
        }
    }
}
