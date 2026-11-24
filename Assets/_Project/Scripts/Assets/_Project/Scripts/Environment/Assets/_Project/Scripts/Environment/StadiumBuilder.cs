
using UnityEngine;

public class StadiumBuilder : MonoBehaviour
{
    [Header("Stadium Dimensions")]
    public float pitchLength = 105f;
    public float pitchWidth = 68f;
    public float standHeight = 12f;
    public int seatingRows = 8;

    private Material standMaterial;
    private Material seatMaterial;
    private Material lightMaterial;

    void Start()
    {
        CreateMaterials();
        BuildSideStands();
        BuildEndStands();
        BuildFloodlights();
    }

    void CreateMaterials()
    {
        standMaterial = MakeMaterial(
            new Color(0.12f, 0.14f, 0.18f));

        seatMaterial = MakeMaterial(
            new Color(0.75f, 0.12f, 0.08f));

        lightMaterial = MakeMaterial(Color.white);
    }

    Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find(
            "Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    void CreateBlock(string name, Vector3 position,
        Vector3 scale, Material material)
    {
        GameObject block = GameObject.CreatePrimitive(
            PrimitiveType.Cube);

        block.name = name;
        block.transform.SetParent(transform);
        block.transform.localPosition = position;
        block.transform.localScale = scale;

        block.GetComponent<Renderer>().material = material;
    }

    void BuildSideStands()
    {
        float sideX = pitchWidth / 2 + 12f;

        for (int side = -1; side <= 1; side += 2)
        {
            CreateBlock(
                "Main Stand Structure",
                new Vector3(side * sideX,
                    standHeight / 2, 0),
                new Vector3(24f, standHeight,
                    pitchLength + 12f),
                standMaterial
            );

            for (int row = 0; row < seatingRows; row++)
            {
                float height = 2f + row * 1.15f;
                float offset = row * 1.3f;

                CreateBlock(
                    "Stadium Seating Row",
                    new Vector3(
                        side * (pitchWidth / 2 + 3f + offset),
                        height,
                        0),
                    new Vector3(
                        2.2f,
                        0.65f,
                        pitchLength + 5f),
                    seatMaterial
                );
            }
        }
    }

    void BuildEndStands()
    {
        float endZ = pitchLength / 2 + 12f;

        for (int side = -1; side <= 1; side += 2)
        {
            CreateBlock(
                "Goal End Stand",
                new Vector3(0, standHeight / 2,
                    side * endZ),
                new Vector3(
                    pitchWidth + 20f,
                    standHeight,
                    24f),
                standMaterial
            );

            for (int row = 0; row < seatingRows; row++)
            {
                float height = 2f + row * 1.15f;
                float offset = row * 1.3f;

                CreateBlock(
                    "End Stand Seating Row",
                    new Vector3(
                        0,
                        height,
                        side * (pitchLength / 2 + 3f + offset)),
                    new Vector3(
                        pitchWidth + 5f,
                        0.65f,
                        2.2f),
                    seatMaterial
                );
            }
        }
    }

    void BuildFloodlights()
    {
        float x = pitchWidth / 2 + 25f;
        float z = pitchLength / 2 + 20f;

        for (int sideX = -1; sideX <= 1; sideX += 2)
        {
            for (int sideZ = -1; sideZ <= 1; sideZ += 2)
            {
                Vector3 position = new Vector3(
                    sideX * x,
                    16f,
                    sideZ * z);

                CreateBlock(
                    "Floodlight Pole",
                    position,
                    new Vector3(0.8f, 32f, 0.8f),
                    standMaterial
                );

                CreateBlock(
                    "Floodlight Panel",
                    position + new Vector3(0, 16f, 0),
                    new Vector3(5f, 1.5f, 1f),
                    lightMaterial
                );
            }
        }
    }
}
