using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class ExportGridToPNG : MonoBehaviour
{
    public Grid grid;
    public string fileName = "GridExport.png";

    [ContextMenu("Export Grid To PNG")]
    public void Export()
    {
        if (grid == null)
        {
            Debug.LogError("Grid not assigned.");
            return;
        }

        Tilemap[] tilemaps = grid.GetComponentsInChildren<Tilemap>();
        if (tilemaps.Length == 0)
        {
            Debug.LogError("No tilemaps found.");
            return;
        }

        // Tự động lấy Pixels Per Unit từ sprite đầu tiên tìm được
        int ppu = DetectPPU(tilemaps);
        if (ppu <= 0)
        {
            Debug.LogError("Failed to detect sprite PPU.");
            return;
        }

        BoundsInt bounds = CalculateBounds(tilemaps);

        int width = bounds.size.x * ppu;
        int height = bounds.size.y * ppu;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Clear
        Color[] clear = new Color[width * height];
        for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
        tex.SetPixels(clear);

        // Render từng tilemap
        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile == null) continue;

                Sprite sprite = tilemap.GetSprite(pos);
                if (sprite == null) continue;

                Rect rect = sprite.textureRect;
                Texture2D sourceTex = sprite.texture;

                // Vị trí pixel trong ảnh export
                int px = (pos.x - bounds.xMin) * ppu;
                int py = (pos.y - bounds.yMin) * ppu;

                // Copy đúng theo sprite (giữ nguyên kích thước và PPU thật)
                for (int x = 0; x < rect.width; x++)
                {
                    for (int y = 0; y < rect.height; y++)
                    {
                        Color c = sourceTex.GetPixel((int)rect.x + x, (int)rect.y + y);
                        if (c.a == 0) continue;

                        tex.SetPixel(px + x, py + y, c);
                    }
                }
            }
        }

        tex.Apply();

        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log("Exported to " + path);
    }

    private int DetectPPU(Tilemap[] tilemaps)
    {
        foreach (Tilemap tm in tilemaps)
        {
            foreach (Vector3Int pos in tm.cellBounds.allPositionsWithin)
            {
                Sprite sp = tm.GetSprite(pos);
                if (sp != null)
                    return (int)sp.pixelsPerUnit;
            }
        }
        return -1;
    }

    private BoundsInt CalculateBounds(Tilemap[] tilemaps)
    {
        bool first = true;
        BoundsInt total = new BoundsInt();

        foreach (Tilemap tm in tilemaps)
        {
            if (first)
            {
                total = tm.cellBounds;
                first = false;
            }
            else
            {
                total.xMin = Mathf.Min(total.xMin, tm.cellBounds.xMin);
                total.yMin = Mathf.Min(total.yMin, tm.cellBounds.yMin);
                total.xMax = Mathf.Max(total.xMax, tm.cellBounds.xMax);
                total.yMax = Mathf.Max(total.yMax, tm.cellBounds.yMax);
            }
        }
        return total;
    }
}
