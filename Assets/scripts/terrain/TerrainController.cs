using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

    public GameObject[] _objectsAvailable;
    public int _objSelection;
    private GameObject _objType;
    public GameObject _terrainParent;
    public GameObject _borderBlock;
    public GameObject _borderParent;
    public Material _mainMaterial;

    public float _terrainMaxHeight = 0.0f;
    public float _terrainMinHeight = 0.0f;


    public int _gridExponential = 10;
    public float _terrainRoughness = 0.08F;
    public float _terrainWave = 0.1F;
    public float _objSize = 1;
    public int _meshLimit = 16;
    public int _normalSmoothAngle = 60;

    private Texture2D _mainTexture;
    private Color[] _mainColors;
    private int _arrayIndex;
    private int _arrayLength;
    private float _waveOffsetX;
    private float _waveOffsetY;
    private Vector3[,] _positionArray;
    private TerrainObject[,] _objArray;
    private List<GameObject> _objRef;
    private float _mainMaterialScale;

    //private System.DateTime _startTime = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
    //private float _count = 0;

    /***************************************************************
    * update the color of a key in the mesh color array      
    * 
    ***************************************************************/
    public void setMeshColorArrayValue(int key, Color color)
    {
        //Debug.Log("called setMeshColorArray - key: " + key + " - color: " + color);d
        _mainColors[key] = color;
    }

    public Color getMeshColorArrayValue(int key)
    {
        return _mainColors[key];
    }

    public List<GameObject> getObjRefArray()
    {
        return _objRef;
    }

    public int getArrayLength()
    {
        return _arrayLength;
    }

    public int getArrayIndex()
    {
        return _arrayIndex;
    }

    public Vector3 getClosestVertex(Vector3 origin)
    {
        Vector3 point = Vector3.zero;

        if (origin.x > _arrayIndex && origin.z > _arrayIndex)
        {
            //origin is out of bounds of the terrain
            return new Vector3(-1, -1, -1);
        }
        
        RaycastHit hit = new RaycastHit();
        int terrainLayer = 1 << LayerMask.NameToLayer("Terrain");

        //fire ray downwards
        if (Physics.Raycast(origin, Vector3.down, out hit, 300.0f, terrainLayer))
        {
            Debug.DrawRay(origin, Vector3.down, Color.red, 10.0f);

            if (hit.collider.gameObject.tag == "Terrain")
            {
                //int meshIndex = _objRef.IndexOf(hit.collider.gameObject);
                //Debug.Log("meshIndex: " + meshIndex);

                Mesh mesh = hit.collider.GetComponent<MeshFilter>().mesh;
                Vector3[] meshVertices = mesh.vertices;

                int triIndex = mesh.triangles[hit.triangleIndex];
                point = mesh.vertices[triIndex];

                //testing mesh deform
                //meshVertices[triIndex] += Vector3.up;
                //mesh.vertices = meshVertices;

                //Debug.Log("point: " + point);
            }
        }

        return point;
    }

    public float getHeightAtVertex(Vector3 origin)
    {
        float height = 0;

        if (origin.x > _arrayIndex && origin.z > _arrayIndex)
        {
            //origin is out of bounds of the terrain
            return -1;
        }

        RaycastHit hit = new RaycastHit();
        int terrainLayer = 1 << LayerMask.NameToLayer("Terrain");

        //fire ray downwards
        if (Physics.Raycast(origin, Vector3.down, out hit, 300.0f, terrainLayer))
        {
            Debug.DrawRay(origin, Vector3.down, Color.blue, 10.0f);

            if (hit.collider.gameObject.tag == "Terrain")
            {
                height = hit.point.y;
                //Debug.Log("height: " + height);
            }
        }

        return height;
    }

    /*************************************************************** 
     * Initialization
     * 
     ***************************************************************/
    void Start() {
        //init
        BuildTerrain();
    }

    public void RefreshTerrain()
    {
        //Debug.Log("called RefreshTerrain");

        if (_objType == null)
        {
            Debug.LogError("TerrainController - _objType not set");
        }
        if (_terrainParent == null)
        {
            Debug.LogError("TerrainController - _terrainParent not set");
        }

        ClearTerrain();
        BuildTerrain();
    }

    public void ClearTerrain()
    {
        //Debug.Log("called ClearTerrain");

        //clear terrain meshes
        for (var i = 0; i < _terrainParent.transform.childCount; i++)
        {
            Destroy(_terrainParent.transform.GetChild(i).gameObject);
        }

        //clear border colliders
        for (var i = 0; i < _borderParent.transform.childCount; i++)
        {
            Destroy(_borderParent.transform.GetChild(i).gameObject);
        }

        //TODO: remove all projectile objects

        //reset values
        _terrainMaxHeight = 0.0f;
        _terrainMinHeight = 0.0f;
}

    public void BuildTerrain()
    {
        //Debug.Log("called BuildTerrain");

        _objType = _objectsAvailable[_objSelection];
        _objRef = new List<GameObject>();

        if (_objType == null)
        {
            Debug.LogError("TerrainController - _objType not set");
        }

        //set up initial params
        _arrayIndex = (int)Mathf.Pow(2, _gridExponential);
        _arrayLength = _arrayIndex + 1;
        _positionArray = new Vector3[_arrayLength, _arrayLength];
        _waveOffsetX = Random.value * _arrayIndex * 2 - _arrayIndex;
        _waveOffsetY = Random.value * _arrayIndex * 2 - _arrayIndex;
        _mainMaterialScale = 1.0f / (_meshLimit * 1.0f);        

        CalcTerrainHeightMap();
        AddBorder();

        SetTerrainHeightParameters();
        _mainColors = new Color[_arrayLength * _arrayLength];
        _mainColors = GenerateColors(_arrayLength, _arrayLength);
        updateTerrainTexture();

        if (_objType.name == "terrainMesh")
        {
            //Debug.Log("found mesh object");
            //set up the _positionArray height map using the Diamond-Square algorithm             
            createMeshTerrain(_positionArray, _meshLimit);
            updateVertexNormalsForAllMeshes();
        }
        else
        {
            //Debug.Log("found terrain object");
            _objArray = new TerrainObject[_arrayLength, _arrayLength];
            createPositionalTerrainUsingObject(_positionArray);
        }

    }

    /***************************************************************
    * creates a border around the terrain
    * 
   ***************************************************************/
    void AddBorder()
    {
        float borderHeight = 64;
        float viewBorderHeight = 12;
        float borderWidth = 2;

        GameObject borderN = Instantiate(_borderBlock, new Vector3(0, 0, _arrayIndex / 2), Quaternion.identity) as GameObject;
        borderN.transform.localScale = new Vector3(borderWidth, borderHeight, _arrayIndex);
        borderN.transform.parent = _borderParent.transform;

        GameObject borderE = Instantiate(_borderBlock, new Vector3(_arrayIndex / 2, 0, _arrayIndex), Quaternion.identity) as GameObject;
        borderE.transform.localScale = new Vector3(_arrayIndex, borderHeight, borderWidth);
        borderE.transform.parent = _borderParent.transform;

        GameObject borderS = Instantiate(_borderBlock, new Vector3(_arrayIndex, 0, _arrayIndex / 2), Quaternion.identity) as GameObject;
        borderS.transform.localScale = new Vector3(borderWidth, borderHeight, _arrayIndex);
        borderS.transform.parent = _borderParent.transform;

        //this side has a lower barrier as the camera view is locked in this direction by default
        GameObject borderW = Instantiate(_borderBlock, new Vector3(_arrayIndex / 2, 0, 0), Quaternion.identity) as GameObject;
        borderW.transform.localScale = new Vector3(_arrayIndex, viewBorderHeight, borderWidth);
        borderW.transform.parent = _borderParent.transform;

    }

    /***************************************************************
    * Calculate the Terrain Height Map using Diamond Square Algorithm
    * 
    ***************************************************************/
    public void CalcTerrainHeightMap()
    {
        if (_positionArray == null || _arrayLength == 0 || _arrayIndex == 0)
        {
            Debug.LogError("TerrainController - CalcTerrainHeightMap - params not set");
        }

        //create random terrain positions
        updatePositionUsingDiamondSquare();
        //normalizeBoundaries();
    }

    /***************************************************************
     * creates a new mesh for the object (does not use terrainobject)
     * 
    ***************************************************************/
    void createMeshTerrain(Vector3[,] posArray, int meshLimit) {

        int meshLength = _arrayIndex % meshLimit;

        if (meshLength > 0)
        {
            Debug.LogError("meshLimit needs to be a factor of the position total.");
        }

        //int meshCount = meshLength * meshLength;

        //generateMesh(meshLimit, _objSize, 0, 32);

        for (int x = 0; x < _arrayIndex; x += meshLimit)
        {
            for (int y = 0; y < _arrayIndex; y += meshLimit)
            {
                //Debug.Log("generateMesh - meshLimit: " + meshLimit + " -_objSize: " + _objSize + " - x: " + x + " - y: " + y);
                generateMesh(meshLimit + 1, _objSize, x, y);
            }
        }
    }

    /***************************************************************
     * generate the mesh based on length required and size
     * 
    ***************************************************************/
    void generateMesh(int length, float size, int startx, int starty)
    {
        Vector3[] meshVertices = new Vector3[length * length];
        Vector2[] meshUV = new Vector2[length * length];
        int[] meshTriangles = new int[(3 * 2 * (length - 1) * (length - 1))];

        //create a new mesh 
        Mesh mesh = new Mesh();

        //set up the mesh vertices array
        int vert_index = 0;
        for (int i = startx; i < length + startx; i++)
        {
            for (int j = starty; j < length + starty; j++)
            {
                meshVertices[vert_index] = _positionArray[i, j];
                vert_index++;
            }
        }

        //set up the mesh uv array
        int uv_index = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                meshUV[uv_index] = new Vector2(i, j);
                uv_index++;
            }
        }

        //set up the mesh triangles array
        int tri_index = 0;
        for (int x = 0; x < length - 1; x++)
        {
            for (int y = 0; y < length - 1; y++)
            {
                //add in first three vertices for mesh
                meshTriangles[tri_index] = (x * length) + y + 1;
                meshTriangles[tri_index + 1] = ((x + 1) * length) + y;
                meshTriangles[tri_index + 2] = (x * length) + y;

                //add in second three vertices for mesh
                meshTriangles[tri_index + 3] = (x * length) + y + 1;
                meshTriangles[tri_index + 4] = ((x + 1) * length) + y + 1;
                meshTriangles[tri_index + 5] = ((x + 1) * length) + y;

                //bump index by 6 for previous puts
                tri_index += 6;
            }
        }

        //Debug.Log("about to calc normals");

        mesh.name = "terrain-" + length + "-" + startx + "-" + starty;
        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.uv = meshUV;
        mesh.triangles = meshTriangles;

        //mesh.RecalculateNormals();
        mesh.RecalculateNormals(_normalSmoothAngle);
        mesh.RecalculateBounds();
        mesh.Optimize();

        //instantiate object with newly added mesh       
        GameObject newMeshObj = Instantiate(_objType, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
        newMeshObj.GetComponent<MeshFilter>().mesh = mesh;
        newMeshObj.AddComponent<MeshCollider>();
        newMeshObj.transform.localScale = new Vector3(size, size, size);
        newMeshObj.transform.parent = _terrainParent.transform;

        float terrainLength = (_arrayIndex * 1.0f) / (_meshLimit * 1.0f);

        Material meshMaterial = new Material(_mainMaterial);
        meshMaterial.SetTextureScale("_MainTex", new Vector2(_mainMaterialScale / terrainLength, _mainMaterialScale / terrainLength));

        //Debug.Log("startx: " + startx + " - starty: " + starty + "- _mainMaterialScale: " + _mainMaterialScale);
        meshMaterial.SetTextureOffset("_MainTex", new Vector2(startx * _mainMaterialScale / terrainLength, starty * _mainMaterialScale / terrainLength));
        newMeshObj.GetComponent<Renderer>().material = meshMaterial;

        //add to object ref array
        _objRef.Add(newMeshObj);

        //GameObject objN = Instantiate(_objRef, new Vector3(0, 0, _arrayIndex * _objSize), Quaternion.Euler(0, 0, 180)) as GameObject;
        //objN.transform.parent = _terrainParent.transform;
    }

    /***************************************************************
     * Generate a flat terrain using TerrainObject
     * 
     ****************************************************************/
    void createFlatTerrainUsingObject() {
        int length = _arrayLength;

        for (int i = 0; i < length; i++) {
            for (int j = 0; j < length; j++) {
                Vector3 pos = new Vector3(i, 0, j);
                TerrainObject obj = new TerrainObject(_objType, pos * _objSize, _objSize, _terrainParent);
                _objArray[i, j] = obj;
            }
        }
    }

    /***************************************************************
     * Generate a terrain based on position array using TerrainObject
     * 
     ****************************************************************/
    void createPositionalTerrainUsingObject(Vector3[,] posArray) {
        int length = _arrayLength;

        for (int i = 0; i < length; i++) {
            for (int j = 0; j < length; j++) {
                Vector3 pos = posArray[i, j];
                TerrainObject obj = new TerrainObject(_objType, pos * _objSize, _objSize, _terrainParent);
                _objArray[i, j] = obj;
            }
        }
    }

    /***************************************************************
    * update existing positional terrain using TerrainObject
    * 
    ****************************************************************/
    void updateDynamicTerrainUsingObject(Vector3[,] posArray) {
        int length = _arrayLength;

        for (int i = 0; i < length; i++) {
            for (int j = 0; j < length; j++) {
                Vector3 pos = posArray[i, j];
                _objArray[i, j].ObjectPosition = pos * _objSize;
            }
        }
    }

    /***************************************************************
    * update the position of a TerrainObject
    * 
    ****************************************************************/
    void updateObjectPosition(int i, int j, Vector3 pos) {
        _objArray[i, j].ObjectPosition = pos * _objSize;
    }

    /***************************************************************
    * move a TerrainObject with after delay
    * 
    ****************************************************************/
    IEnumerator moveObject(int i, int j, Vector3 pos, float sec) {
        yield return new WaitForSeconds(sec);
        updateObjectPosition(i, j, pos);
        _objArray[i, j].setColor(new Color(0, 0.75F + (Random.value / 4), 0));
    }

    /***************************************************************
    * update the edge points to match each other so they can map infinitely
    * 
    ****************************************************************/
    void normalizeBoundaries()
    {
        for (int i = 1; i < _arrayLength - 1; i++)
        {
            //update X coords
            float avgX = (_positionArray[i, 0].y + _positionArray[i, _arrayLength - 1].y) / 2;
            _positionArray[i, 0].y = avgX;
            _positionArray[i, _arrayLength - 1].y = avgX;

            //update Y coords
            float avgY = (_positionArray[0, i].y + _positionArray[_arrayLength - 1, i].y) / 2;
            _positionArray[0, i].y = avgY;
            _positionArray[_arrayLength - 1, i].y = avgY;
        }
    }

    /***************************************************************
    * update vertext normals and draw debug lines
    * 
    ****************************************************************/
    void updateVertexNormals(GameObject meshObj)
    {

        Mesh mesh = meshObj.GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;
        //Color color = new Color(Random.value, Random.value, Random.value);

        //try setting all bounding vertices to vector3.up
        for (int x = 0; x < _meshLimit + 1; x++)
        {
            for (int y = 0; y < _meshLimit + 1; y++)
            {
                //only update normals bordering on other meshes
                if (x == 0 || y == 0 || x == _meshLimit || y == _meshLimit)
                {
                    int index = ((_meshLimit + 1) * x) + y;

                    //draw lines to represent normals
                    //Vector3 fixedVertex = Quaternion.Euler(0, 0, 180) * mesh.vertices[index];
                    //Vector3 fixedNormal = Quaternion.Euler(0, 0, 180) * mesh.normals[index];                    
                    //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, color, 100.0f);

                    //set intersecting normals to default value
                    normals[index] = new Vector3(0.0f, -1.0f, 0.0f);
                }
            }
        }

        //update normal array
        mesh.normals = normals;
    }


    /***************************************************************
    * update the vertice normals to fix lighting issues
    * 
    ****************************************************************/
    void updateVertexNormalsForAllMeshes()
    {
        for (int j = 0; j < _objRef.Count; j++)
        {
            //nw point
            findNeighbouringVertices(_objRef[j], 0, _meshLimit);

            //se point
            findNeighbouringVertices(_objRef[j], _meshLimit, 0);
        }
    }

    /***************************************************************
    * find all neighbouring vertices in other meshes
    * 
    ****************************************************************/
    void findNeighbouringVertices(GameObject mainObj, int x, int y)
    {
        Mesh mainMesh = mainObj.GetComponent<MeshFilter>().mesh;
        Vector3[] mainNormals = mainMesh.normals;

        int objCount = _objRef.Count;
        int mainObjIndex = _objRef.IndexOf(mainObj);
        int terrainLength = _arrayIndex / _meshLimit;

        if (x == 0 && y == _meshLimit)
        {
            int northObjIndex = mainObjIndex + 1;

            if ((northObjIndex % terrainLength) == 0)
            {
                return;
            }

            GameObject northObj = _objRef[northObjIndex];
            Mesh northMesh = northObj.GetComponent<MeshFilter>().mesh;
            Vector3[] northNormals = northMesh.normals;
            float tempColor = 0.0f;

            for (int iter = x; iter < _meshLimit + 1; iter++)
            {
                tempColor += 0.1f;
                int mainIndex = ((_meshLimit + 1) * iter) + y;
                int northIndex = ((_meshLimit + 1) * iter);

                Vector3 newNormal = mainNormals[mainIndex] + northNormals[northIndex];
                newNormal.Normalize();

                mainNormals[mainIndex] = newNormal;
                northNormals[northIndex] = newNormal;

                mainMesh.normals = mainNormals;
                northMesh.normals = northNormals;

                //Vector3 fixedVertex = Quaternion.Euler(0, 0, 180) * mainMesh.vertices[mainIndex];
                //Vector3 fixedNormal = Quaternion.Euler(0, 0, 180) * mainMesh.normals[mainIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(1, tempColor, tempColor), 5.0f);

                //fixedVertex = Quaternion.Euler(0, 0, 180) * northMesh.vertices[northIndex];
                //fixedNormal = Quaternion.Euler(0, 0, 180) * northMesh.normals[northIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(tempColor, tempColor, 1), 8.0f);
            }
        }
        else if (x == _meshLimit && y == 0)
        {
            int westObjIndex = mainObjIndex + terrainLength;
            if (westObjIndex >= objCount)
            {
                return;
            }
            GameObject westObj = _objRef[westObjIndex];
            Mesh westMesh = westObj.GetComponent<MeshFilter>().mesh;
            Vector3[] westNormals = westMesh.normals;
            float tempColor = 0.0f;
            for (int iter = 0; iter < _meshLimit + 1; iter++)
            {
                tempColor += 0.1f;
                int mainIndex = ((_meshLimit + 1) * _meshLimit) + iter;
                int westIndex = iter;
                Vector3 newNormal = mainNormals[mainIndex] + westNormals[westIndex];
                newNormal.Normalize();

                mainNormals[mainIndex] = newNormal;
                westNormals[westIndex] = newNormal;

                mainMesh.normals = mainNormals;
                westMesh.normals = westNormals;

                //Vector3 fixedVertex = Quaternion.Euler(0, 0, 180) * mainMesh.vertices[mainIndex];
                //Vector3 fixedNormal = Quaternion.Euler(0, 0, 180) * mainMesh.normals[mainIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(1, tempColor, tempColor), 5.0f);

                //fixedVertex = Quaternion.Euler(0, 0, 180) * westMesh.vertices[westIndex];
                //fixedNormal = Quaternion.Euler(0, 0, 180) * westMesh.normals[westIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(tempColor, tempColor, 1), 8.0f);
            }
        }
        else
        {
            Debug.LogError("something went wrong");
        }
    }

    /***************************************************************
    * update the positional array using the Diamond Square Algorithim
    * 
    ****************************************************************/
    void updatePositionUsingDiamondSquare() {
        int size = _arrayIndex;
        int length = _arrayLength;
        float roughness = _terrainRoughness;

        //set default corners
        _positionArray[0, 0] = Vector3.zero;
        _positionArray[size, 0] = new Vector3(size, 0.0f, 0.0f);
        _positionArray[0, size] = new Vector3(0.0f, 0.0f, size);
        _positionArray[size, size] = new Vector3(size, 0.0f, size);

        split(size, roughness, length);
    }

    /***************************************************************
    * Diamond Square Algorithim recursive function 
    * 
    ****************************************************************/
    void split(int size, float roughness, int full) {
        //Debug.Log ("split() params size: " + size + " roughness: " + roughness + " full: " + full	);
        int half = size / 2;
        //float scale = roughness * _blockSize;
        float scale = roughness * size;

        if (half < 1) {
            return;
        }

        for (int y = half; y < full; y += size) {
            for (int x = half; x < full; x += size) {
                float rand = Random.value * scale * 2 - scale;
                square(x, y, half, rand, full);
            }
        }

        for (int y = 0; y < full; y += half) {
            for (int x = (y + half) % size; x < full; x += size) {
                float rand = Random.value * scale * 2 - scale;
                diamond(x, y, half, rand, full);
            }
        }

        split(half, roughness, full);
    }

    /***************************************************************
    * Diamond Square Algorithim recursive function 
    * 
    ****************************************************************/
    void diamond(int x, int y, int half, float offset, int full) {
        //Debug.Log ("diamond() params x: " + x + " y: " + y + " half: " + half + " offset: " + offset + " full: " + full);	

        var offsetValue = offset * Mathf.Cos(_terrainWave * (x + _waveOffsetX)) * Mathf.Cos(_terrainWave * (y + _waveOffsetY));
        float roughness = _terrainRoughness;
        var xRand = Random.value * roughness * 2 - roughness;
        var yRand = Random.value * roughness * 2 - roughness;


        float d1 = (y - half) >= 0 ? _positionArray[x, (y - half)].y : 0;
        float d2 = (x + half) < full ? _positionArray[(x + half), y].y : 0;
        float d3 = (y + half) < full ? _positionArray[x, (y + half)].y : 0;
        float d4 = ((x - half) >= 0) ? _positionArray[(x - half), y].y : 0;

        float average = (d1 + d2 + d3 + d4) / 4;
        _positionArray[x, y] = new Vector3(x + xRand, average + offsetValue, y + yRand);
        //StartCoroutine(moveBlock (x, y, _positionArray [x, y], _count +=0.001F));
    }

    /***************************************************************
    * Diamond Square Algorithim recursive function 
    * 
    ****************************************************************/
    void square(int x, int y, int half, float offset, int full) {
        //Debug.Log ("square() params x: " + x + " y: " + y + " half: " + half + " offset: " + offset + " full: " + full);

        var offsetValue = offset * Mathf.Cos(_terrainWave * x) * Mathf.Cos(_terrainWave * y);

        float roughness = _terrainRoughness;
        var xRand = Random.value * roughness * 2 - roughness;
        var yRand = Random.value * roughness * 2 - roughness;

        float s1 = ((x - half) >= 0 || (y - half) >= 0) ? _positionArray[(x - half), (y - half)].y : 0;
        float s2 = ((y - half) >= 0 || (x + half) < full) ? _positionArray[(x + half), (y - half)].y : 0;
        float s3 = ((x + half) < full || (y + half) < full) ? _positionArray[(x + half), (y + half)].y : 0;
        float s4 = ((x - half) >= 0 || (y + half) < full) ? _positionArray[(x - half), (y + half)].y : 0;

        float average = (s1 + s2 + s3 + s4) / 4;
        _positionArray[x, y] = new Vector3(x + xRand, average + offsetValue, y + yRand);
        //StartCoroutine(moveBlock (x, y, _positionArray [x, y], _count +=0.001F));
    }


    /*************************************************************** 
     * Update the positional Array with Default Values
     * 
     ***************************************************************/
    void updatePositionUsingDefault()
    {
        for (int x = 0; x < _arrayLength; x++)
        {
            for (int y = 0; y < _arrayLength; y++)
            {
                _positionArray[x, y] = new Vector3(x, y, 0);
            }
        }
    }

    /***************************************************************
     * sets up the default terrain height params for min and max height
     * based off the dynamic height map
     * 
    ***************************************************************/
    void SetTerrainHeightParameters()
    {
        for (int x = 0; x < _arrayLength; x++)
        {
            for (int y = 0; y < _arrayLength; y++)
            {
                float posHeight = _positionArray[x, y].y;

                if (posHeight > _terrainMaxHeight || _terrainMaxHeight == 0.0f)
                {
                    _terrainMaxHeight = posHeight;
                }
                if (posHeight < _terrainMinHeight || _terrainMinHeight == 0.0f)
                {
                    _terrainMinHeight = posHeight;
                }
            }
        }
    }

    /***************************************************************
     * generate colors based on the dynamic terrain 
     * 
    ***************************************************************/
    private Color[] GenerateColors(int width, int height) {
        //Debug.Log("called GenerateColors");

        Color[] colors = new Color[width * height];

        //if (_terrainMaxHeight == 0.0f && _terrainMinHeight == 0.0f)
        //{
        //    Debug.LogError("Terrain Min and Max need to be set before calling generate texture");
        //}

        float maxHeight = _terrainMaxHeight;
        float minHeight = _terrainMinHeight;
        float heightDiff = maxHeight - minHeight;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float posHeight = _positionArray[x, y].y;
                float posToColor = 0.0f;
                if (posHeight < maxHeight) {
                    posToColor = ((posHeight - minHeight) / heightDiff);
                } else {
                    posToColor = 1.0f;
                }

                if (posHeight < minHeight)
                {
                    posToColor = 0.0f;
                }
                //Debug.Log("posHeight: " + posHeight + " - posToColor: " + posToColor);
                float colorR = posToColor;
                float colorG = 1.0f / posToColor;
                float colorB = posToColor * 0.1f;
                colors[(width * y) + x] = new Color(Mathf.Clamp(colorR, 0.3f, 0.7f), Mathf.Clamp(colorG, 0.2f, 0.5f), colorB);
            }
        }

        return colors;
    }

    /***************************************************************
     * generate a texture based off a color height array
     * 
    ***************************************************************/
    private Texture2D GenerateTexture(Color[] colors, int width, int height)
    {
        //Debug.Log("called GenerateTexture");
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colors, 0);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture;
    }

    /***************************************************************
     * rotate a color array by 90
     * 
    ***************************************************************/
    private Color[] RotateColors(Color[] inputArray, int width, int height)
    {
        Color[] rotatedArray = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                rotatedArray[x * width + y] = inputArray[(height - y - 1) * width + x];
            }
        }

        return rotatedArray;
    }

    /***************************************************************
     * flip the values of a color array
     * 
    ***************************************************************/
    private Color[] FlipColors(Color[] inputArray, int width, int height)
    {
        Color[] rotatedArray = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                rotatedArray[x * width + y] = inputArray[((width - x -1) * height) + y];
            }
        }

        return rotatedArray;
    }

    /***************************************************************
     * rotates terrain texture and updates mesh materials
     * triggered by GUI button
     * 
    ***************************************************************/
    public void RotateTerrainTextureBy90()
    {
        _mainColors = RotateColors(_mainColors, _arrayLength, _arrayLength);
        updateTerrainTexture();
        updateAllMeshMaterials();
    }

    /***************************************************************
     * re-build the terrain texture based off the global colors and
     * update global material 
     * 
    ***************************************************************/
    public void updateTerrainTexture()
    {
        _mainTexture = GenerateTexture(_mainColors, _arrayLength, _arrayLength);
        _mainMaterial.SetTexture("_MainTex", _mainTexture);
    }

    /***************************************************************
     * update all the material values for each mesh
     * 
    ***************************************************************/
    public void updateAllMeshMaterials()
    {
        int terrainIndex = 0;

        for (int startx = 0; startx < _arrayIndex; startx += _meshLimit)
        {
            for (int starty = 0; starty < _arrayIndex; starty += _meshLimit)
            {
                GameObject newMeshObj = _objRef[terrainIndex];
                float terrainLength = (_arrayIndex * 1.0f) / (_meshLimit * 1.0f);

                Material meshMaterial = new Material(_mainMaterial);
                meshMaterial.SetTextureScale("_MainTex", new Vector2(_mainMaterialScale / terrainLength, _mainMaterialScale / terrainLength));

                //Debug.Log("startx: " + startx + " - starty: " + starty + "- _mainMaterialScale: " + _mainMaterialScale);
                meshMaterial.SetTextureOffset("_MainTex", new Vector2(startx * _mainMaterialScale / terrainLength, starty * _mainMaterialScale / terrainLength));
                newMeshObj.GetComponent<Renderer>().material = null;
                newMeshObj.GetComponent<Renderer>().material = meshMaterial;
                terrainIndex++;
            }
        }
    }

    /***************************************************************
     * update material values for a specific mesh
     * 
    ***************************************************************/
    public void updateMeshMaterials(int terrainIndex, int startx, int starty)
    {

        //Debug.Log("called updateMeshMaterials - terrainIndex: " + terrainIndex + " - startx: " + startx + " - starty: " + starty);
        GameObject newMeshObj = _objRef[terrainIndex];
        float terrainLength = (_arrayIndex * 1.0f) / (_meshLimit * 1.0f);

        Material meshMaterial = new Material(_mainMaterial);
        meshMaterial.SetTextureScale("_MainTex", new Vector2(_mainMaterialScale / terrainLength, _mainMaterialScale / terrainLength));

        //Debug.Log("startx: " + startx + " - starty: " + starty + "- _mainMaterialScale: " + _mainMaterialScale);
        meshMaterial.SetTextureOffset("_MainTex", new Vector2(startx * _mainMaterialScale / terrainLength, starty * _mainMaterialScale / terrainLength));
        newMeshObj.GetComponent<Renderer>().material = null;
        newMeshObj.GetComponent<Renderer>().material = meshMaterial;
        terrainIndex++;
    }
}
