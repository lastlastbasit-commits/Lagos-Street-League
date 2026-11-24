
using UnityEngine;

public class FootballPitchBuilder : MonoBehaviour
{
    [Header("Pitch Dimensions")]
    public float pitchLength = 105f;
    public float pitchWidth = 68f;
    public float lineWidth = 0.12f;

    [Header("Goal Dimensions")]
    public float goalWidth = 7.32f;
    public float goalHeight = 2.44f;
    public float goalDepth = 2f;

    private Material grassMaterial;
    private Material lineMaterial;
    private Material goalMaterial;

    void Start()
    {
        CreateMaterials();
        BuildPitch();
    }

    void CreateMaterials()
    {
        grassMaterial = CreateMaterial(
            new Color(0.12f, 0.38f, 0.16f));

        lineMaterial = CreateMaterial(Color.white);

        goalMaterial = CreateMaterial(Color.white);
    }

    Material CreateMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.color = color;
        return mat;
    }

    void BuildPitch()
    {
        CreateCube(
            "Grass Pitch",
            new Vector3(0, -0.1f, 0),
            new Vector3(pitchWidth, 0.2f, pitchLength),
            grassMaterial
        );

        CreateBoundaryLines();
        CreateCentreMarkings();
        CreatePenaltyAreas();
        CreateGoals();
    }

    void CreateCube(string objectName, Vector3 position,
        Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(
            PrimitiveType.Cube);

        obj.name = objectName;
        obj.transform.SetParent(transform);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;

        obj.GetComponent<Renderer>().material = material;
    }

    void CreateLine(string name, float x, float z,
        float width, float length)
    {
        CreateCube(
            name,
            new Vector3(x, 0.015f, z),
            new Vector3(width, 0.03f, length),
            lineMaterial
        );
    }

    void CreateBoundaryLines()
    {
        float halfWidth = pitchWidth / 2;
        float halfLength = pitchLength / 2;

        CreateLine("Top Touchline", 0, halfLength,
            pitchWidth, lineWidth);

        CreateLine("Bottom Touchline", 0, -halfLength,
            pitchWidth, lineWidth);

        CreateLine("Left Goal Line", -halfWidth, 0,
            lineWidth, pitchLength);

        CreateLine("Right Goal Line", halfWidth, 0,
            lineWidth, pitchLength);
    }

    void CreateCentreMarkings()
    {
        CreateLine("Centre Line", 0, 0,
            pitchWidth, lineWidth);

        CreateCircle("Centre Circle", 9.15f, 0, 0);

        CreateCube("Centre Spot",
            new Vector3(0, 0.025f, 0),
            new Vector3(0.35f, 0.04f, 0.35f),
            lineMaterial);
    }

    void CreateCircle(string name, float radius,
        float centreX, float centreZ)
    {
        int segments = 100;

        for (int i = 0; i < segments; i++)
        {
            float a1 = i * Mathf.PI * 2 / segments;
            float a2 = (i + 1) * Mathf.PI * 2 / segments;

            Vector3 p1 = new Vector3(
                centreX + Mathf.Cos(a1) * radius,
                0.025f,
                centreZ + Mathf.Sin(a1) * radius);

            Vector3 p2 = new Vector3(
                centreX + Mathf.Cos(a2) * radius,
                0.025f,
                centreZ + Mathf.Sin(a2) * radius);

            Vector3 middle = (p1 + p2) / 2;
            Vector3 direction = p2 - p1;

            GameObject segment = GameObject.CreatePrimitive(
                PrimitiveType.Cube);

            segment.name = name + " Segment";
            segment.transform.SetParent(transform);
            segment.transform.localPosition = middle;
            segment.transform.localScale =
                new Vector3(lineWidth, 0.03f, direction.magnitude);

            segment.transform.rotation =
                Quaternion.LookRotation(direction);

            segment.GetComponent<Renderer>().material = lineMaterial;
        }
    }

    void CreatePenaltyAreas()
    {
        float halfLength = pitchLength / 2;

        for (int side = -1; side <= 1; side += 2)
        {
            float goalZ = side * halfLength;

            CreateLine("Penalty Area Side",
                -20.16f, goalZ - side * 8.25f,
                lineWidth, 16.5f);

            CreateLine("Penalty Area Side",
                20.16f, goalZ - side * 8.25f,
                lineWidth, 16.5f);

            CreateLine("Penalty Area Front",
                0, goalZ - side * 16.5f,
                40.32f, lineWidth);

            CreateLine("Goal Area Side",
                -9.16f, goalZ - side * 2.75f,
                lineWidth, 5.5f);

            CreateLine("Goal Area Side",
                9.16f, goalZ - side * 2.75f,
                lineWidth, 5.5f);

            CreateLine("Goal Area Front",
                0, goalZ - side * 5.5f,
                18.32f, lineWidth);

            CreateCube("Penalty Spot",
                new Vector3(0, 0.025f, goalZ - side * 11f),
                new Vector3(0.35f, 0.04f, 0.35f),
                lineMaterial);
        }
    }

    void CreateGoals()
    {
        float halfLength = pitchLength / 2;

        for (int side = -1; side <= 1; side += 2)
        {
            float z = side * halfLength;

            CreateCube("Goal Left Post",
                new Vector3(-goalWidth / 2,
                    goalHeight / 2, z),
                new Vector3(0.15f, goalHeight, 0.15f),
                goalMaterial);

            CreateCube("Goal Right Post",
                new Vector3(goalWidth / 2,
                    goalHeight / 2, z),
                new Vector3(0.15f, goalHeight, 0.15f),
                goalMaterial);

            CreateCube("Goal Crossbar",
                new Vector3(0, goalHeight, z),
                new Vector3(goalWidth, 0.15f, 0.15f),
                goalMaterial);
        }
    }
}
