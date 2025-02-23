using UnityEngine;

public class TextureTerrainData : MonoBehaviour
{
    private Terrain terrain;
    private byte[,] splatIndex;
    private Vector3 size;
    private Vector3 tPos;
    private int width;
    private int height;

    void Start ()
    {
        terrain = GetComponent<Terrain>();
        CalcHiInflPrototypeIndexesPerPoint();
    }

    // Подготавливает итоговый массив
    private void CalcHiInflPrototypeIndexesPerPoint()
    {
        TerrainData terrainData = terrain.terrainData;
        size = terrainData.size;
        width = terrainData.alphamapWidth;
        height = terrainData.alphamapHeight;
        int prototypesLength = terrainData.splatPrototypes.Length;
        tPos = terrain.GetPosition();

        // Массив с силами воздействия каждой текстуры
        float[, ,] alphas = terrainData.GetAlphamaps(0, 0, width, height);
        splatIndex = new byte[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                byte ind = 0;
                float t = 0f;
                
                for (byte i = 0; i < prototypesLength; i++)
                    if (alphas[x, y, i] > t)
                    {
                        t = alphas[x, y, i];
                        ind = i;
                    }

                    splatIndex[x, y] = ind;
              }
          }
      }

      // Возвращает индекс текстуры
      public int GetTextureIndex(Vector3 position)
      {
          position = position - tPos;
          position.x /= size.x;
          position.z /= size.z;

          return splatIndex[(int)(position.z * (width - 1)), (int)(position.x * (height - 1))];
      }
}