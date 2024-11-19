using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region Pool handling

    public IEnumerator LoadPool(List<UnityEngine.Object> pool, MonoBehaviour pooledType, Transform poolParent, int poolSize, Action endAction = null)
    {
        for (var i = 0; i < poolSize; ++i)
        {
            var item = Instantiate(pooledType, poolParent);
            item.gameObject.SetActive(false);
            pool.Add(item);
        }
        yield return null;

        if (endAction != null) endAction();
    }

    #endregion
}
