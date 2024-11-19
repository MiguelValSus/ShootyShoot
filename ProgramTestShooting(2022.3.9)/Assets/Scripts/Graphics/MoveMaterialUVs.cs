using UnityEngine;

public class MoveMaterialUVs : MonoBehaviour
{
    [Header("UV Movement")]
    public float UVSpeed = .1f;
    public bool AnimateUv_XAxis;
    public bool AnimateUv_YAxis;

    private Material SpriteMaterial;
    private Vector2 _initialOffset;
    private Vector2 _movingOffset = Vector2.zero;

    #region Initialization
    private void Awake()
    {
        var meshRenderer = GetComponent<SpriteRenderer>();
        if (meshRenderer != null) {
            SpriteMaterial = meshRenderer.material;
            _initialOffset = SpriteMaterial.mainTextureOffset;
        } else {
            Debug.LogError("MeshRenderer type not found!");
        }
    }
    #endregion

    #region UV movement
    private void Update()
    {
        if (SpriteMaterial == null) return;
        MoveSpriteUVs();
    }

    private void MoveSpriteUVs()
    {
        _movingOffset.x = AnimateUv_XAxis ? Mathf.Repeat(_initialOffset.x + (Time.time * UVSpeed), 1f) : _initialOffset.x;
        _movingOffset.y = AnimateUv_YAxis ? Mathf.Repeat(_initialOffset.y + (Time.time * UVSpeed), 1f) : _initialOffset.y;

        SpriteMaterial.mainTextureOffset = _movingOffset;
    }
    #endregion
}
