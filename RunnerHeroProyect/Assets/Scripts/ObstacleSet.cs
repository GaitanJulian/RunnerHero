using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSet : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float destroyPoint = -10f;
    private bool moving = true;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnPlayerDead += StopMoving;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (transform.position.x < destroyPoint)
            {
                Destroy(gameObject);
            }
        }
        
    }

    private void StopMoving()
    {
        moving = false;
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerDead -= StopMoving;
    }

}
