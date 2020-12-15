
using UnityEngine;

public class ProximityOffsetOverride : OffsetOverride
{
    [SerializeField] private Transform _obj1;
    [SerializeField] private Transform _obj2;
    [SerializeField] private float _refDistance;

    public override float Offset
    {
        get
        {
            float d = (_obj1.position - _obj2.position).magnitude;
            if(d > _refDistance)
            {
                return 0;
            }
            return 1024 * (1 - d / _refDistance);
        }
    }
    
}