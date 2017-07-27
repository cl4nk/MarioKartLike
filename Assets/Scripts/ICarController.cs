using UnityEngine;
using System.Collections;

public interface ICarController
{
    void Move(float steer, float acceleration, float brake, float boost);
    void SetTopspeed(float speed);
    void UnsetTopspeed();
}
