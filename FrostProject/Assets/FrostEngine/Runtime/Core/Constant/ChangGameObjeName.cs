using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrostEngine
{
    public class ChangGameObjeName : MonoBehaviour
    {
        public GameObject centerPoint;
        public float radius = 2f;
        public float rotationSpeed = 50f;
        public float startAngle = 0f;

        public List<Transform> objectsToRotate = new List<Transform>();
        public void Update()
        {
            float angleStep = 360f / objectsToRotate.Count;
            float angle = startAngle;

            foreach (Transform obj in objectsToRotate)
            {
                float posX = centerPoint.transform.position.x + Mathf.Cos(angle) * radius;
                float posY = centerPoint.transform.position.y + Mathf.Sin(angle) * radius;

                Vector3 newPos = new Vector3(posX, posY, obj.position.z);
                obj.position = newPos;

                angle += angleStep * rotationSpeed * Time.deltaTime;
            }
        }
    }
}
