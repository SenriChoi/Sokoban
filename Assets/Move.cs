using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Move : MonoBehaviour

{    /// <summary>�ړ��܂łɂ����鎞�ԁi�b�j</summary>
    public float duration = 0.2f;
    /// <summary>�ړ����n�߂Ă���o�߂������ԁi�b�j</summary>
    float elapsedTime;
    Vector3 destination;
    Vector3 origin;

    public void MoveTo(Vector3 destination)
    {
        elapsedTime = 0;
        origin = this.destination;
        // �ړ����������ꍇ�̓L�����Z�����ĖړI�Ƀ��[�v����
        transform.position = origin;
        this.destination = destination;
    }


    // Start is called before the first frame update
    void Start()
    {
        destination = transform.position;
        origin = destination;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == destination)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float timeRatio = elapsedTime / duration;
        float easing = timeRatio;

        Vector3 currentPosition
            = Vector3.Lerp(origin, destination, easing);
        transform.position = currentPosition;
    }
}
