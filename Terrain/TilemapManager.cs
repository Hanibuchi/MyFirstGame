using MyGame;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    Tilemap _tilemap;
    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollision(collision);
    }

    void OnCollision(Collision2D collision)
    {
        // Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out Projectile projectile))
        {
            foreach (var contact in collision.contacts)
            {
                Vector2 offset = new(contact.normal.x * _tilemap.cellSize.x * 0.5f, contact.normal.y * _tilemap.cellSize.y * 0.5f);
                Vector3Int tilePos = _tilemap.WorldToCell(contact.point + offset);
                TileBase tile = _tilemap.GetTile(tilePos);
                Debug.Log($"normal: {contact.normal}, point: {contact.point}, tilePos: {tilePos}, tile: {tile}, isMyTile: {tile is MyTile}");

                if (tile is MyTile customTile)
                {
                    projectile.Hit(customTile);
                    if (customTile.IsDead())
                    {
                        _tilemap.SetTile(tilePos, null);
                    }
                    break;
                }
            }
        }
    }
}
