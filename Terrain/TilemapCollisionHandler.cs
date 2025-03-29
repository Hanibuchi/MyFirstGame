using MyGame;
using UnityEngine;
using UnityEngine.Tilemaps;

// public class TilemapCollisionHandler : MonoBehaviour
// {
    // public Tilemap tilemap; // 置き換えたいTilemapを指定
    // public TileBase objectTile; // GroundTileを持つtile
    // Vector3Int cellPosition;

    // private void OnTriggerEnter2D(Collider2D collider)
    // {
    //     if (collider.gameObject.layer == LayerMask.NameToLayer(GameManager.Layers.Projectile))
    //     {
    //         cellPosition = tilemap.WorldToCell(collider.transform.position);
    //         if (tilemap.GetTile(cellPosition) != null)
    //             tilemap.SetTile(cellPosition, objectTile);
    //     }
    // }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.layer == LayerMask.NameToLayer(GameManager.Layers.Projectile))
    //         // 衝突したすべてのコンタクトポイントをループ
    //         foreach (ContactPoint2D contact in collision.contacts)
    //         {
    //                 // 各コンタクトポイントの位置をワールド座標からセル座標に変換
    //                 cellPosition = tilemap.WorldToCell(contact.point);

    //                 // そのセルに既にタイルがあるか確認
    //                 if (tilemap.GetTile(cellPosition) != null && tilemap.GetTile(cellPosition) != objectTile)
    //                 {
    //                     // タイルを新しいオブジェクトタイルに置き換え
    //                     tilemap.SetTile(cellPosition, objectTile);
    //                 }
    //         }
    // }
// }
