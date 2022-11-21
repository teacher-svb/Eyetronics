/**
 * Just a 20min port of trypho's JS script to C# by skahlert
 * In my experience the performance improved significantly (about 57x the speed of the JavaScript).
 * Have fun with it!!
 * 
 * If you find a nicer way of compensating the lack of dynamic arrays (line 84 and following) it would be nice to hear!
 * 
 */


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

class MorphTargetsCSharp : MonoBehaviour
{
    internal class BlendShapeVertex
    {
        public int originalIndex;
        public Vector3 position;
        public Vector3 normal;
    }

    internal class BlendShape
    {
        public BlendShapeVertex[] vertices;// = new Array();
    }

    public String[] attributes = new String[0]; //Names for the attributes to be morphed
    public Mesh sourceMesh; //The original mesh
    public Mesh[] attributeMeshes = new Mesh[0]; //The destination meshes for each attribute.
    public float[] attributeProgress = new float[0];
    public float[] attributeProgressCounter = new float[0]; // don't touch!

    private BlendShape[] blendShapes;
    private Mesh workingMesh;
	
	public void AddBlendMesh(string BlendName, Mesh attributeMesh) 
	{
		String[] tempAttributes = new String[attributes.Length + 1]; 
		Mesh[] tempAttributeMeshes = new Mesh[attributeMeshes.Length + 1];
		float[] tempAttributeProgress = new float[attributeProgress.Length + 1];
		float[] tempAttributeProgressCounter = new float[attributeProgressCounter.Length + 1];
		
		for (int i = 0; i < attributes.Length; ++i)
			tempAttributes[i] = attributes[i];
		for (int i = 0; i < attributeMeshes.Length; ++i)
			tempAttributeMeshes[i] = attributeMeshes[i];
		for (int i = 0; i < attributeProgress.Length; ++i)
			tempAttributeProgress[i] = attributeProgress[i];
		
		tempAttributes[tempAttributes.Length - 1] = BlendName;
		tempAttributeMeshes[tempAttributeMeshes.Length - 1] = attributeMesh;
		tempAttributeProgress[tempAttributeProgress.Length - 1] = 0.0f;
		tempAttributeProgressCounter[tempAttributeProgressCounter.Length - 1] = 0.0f;
		
		attributes = new String[tempAttributes.Length]; 
		attributeMeshes = new Mesh[tempAttributeMeshes.Length];
		attributeProgress = new float[tempAttributeProgress.Length];
		attributeProgressCounter = new float[tempAttributeProgressCounter.Length];
		
		attributes = (String[]) tempAttributes.Clone();
		attributeMeshes = (Mesh[]) tempAttributeMeshes.Clone();
		attributeProgress = (float[]) tempAttributeProgress.Clone();
		attributeProgressCounter = (float[]) tempAttributeProgressCounter.Clone();
	}
	
	public void Start () {
        gameObject.GetComponent<MeshFilter>().sharedMesh = sourceMesh;
        workingMesh = gameObject.GetComponent<MeshFilter>().mesh;
	}

    public void Init()
    {
        for (int i = 0; i < attributeMeshes.Length; i++)
        {
            if (attributeMeshes[i] == null)
            {
                Debug.Log("Attribute " + i + " has not been assigned.");
                return;
            }
        }

        //Populate the working mesh

        //Check attribute meshes to be sure vertex count is the same.
        int vertexCount = sourceMesh.vertexCount;
//        Debug.Log("Vertex Count Source:"+vertexCount);
        for (int i = 0; i < attributeMeshes.Length; i++)
        {
 //           Debug.Log("Vertex Mesh "+i+":"+attributeMeshes[i].vertexCount); 
            if (attributeMeshes[i].vertexCount != vertexCount)
            {
                Debug.Log("Mesh " + i + " doesn't have the same number of vertices as the first mesh");
                return;
            }
        }

        //Build blend shapes
        BuildBlendShapes();
    }

    void BuildBlendShapes()
    {

        blendShapes = new BlendShape[attributes.Length];

        //For each attribute figure out which vertices are affected, then store their info in the blend shape object.
        for (int i = 0; i < attributes.Length; i++)
        {
            //Populate blendShapes array with new blend shapes
            blendShapes[i] = new BlendShape();

            /** TODO: Make this a little more stylish!
             *  UGLY hack to compensate the lack of dynamic arrays in C#. Feel free to improve!
             */
			
            int blendShapeCounter = 0;
            for (int j = 0; j < workingMesh.vertexCount; j++)
            {
                
                if (workingMesh.vertices[j] != attributeMeshes[i].vertices[j])
                {
                    blendShapeCounter++;
                }
            }
            
            blendShapes[i].vertices = new BlendShapeVertex[blendShapeCounter];
			
			//ArrayList test = new ArrayList();
			
            blendShapeCounter = 0;
            for (int j = 0; j < workingMesh.vertexCount; j++)
            {
                //If the vertex is affected, populate a blend shape vertex with that info
                if (workingMesh.vertices[j] != attributeMeshes[i].vertices[j])
                {
                    //Create a blend shape vertex and populate its data.
                    
                    BlendShapeVertex blendShapeVertex = new BlendShapeVertex();
                    blendShapeVertex.originalIndex = j;
                    blendShapeVertex.position = attributeMeshes[i].vertices[j] - workingMesh.vertices[j];
                    blendShapeVertex.normal = attributeMeshes[i].normals[j] - workingMesh.normals[j];

                    //Add new blend shape vertex to blendShape object.
                    //test.Add(blendShapeVertex);
					blendShapes[i].vertices[blendShapeCounter]=blendShapeVertex;
                    blendShapeCounter++;
                }
            }

            //Convert blendShapes.vertices to builtin array
            //blendShapes[i].vertices = blendShapes[i].vertices.ToBuiltin(BlendShapeVertex);
        }
    }


    public void SetMorph()
    {
        //Set up working data to store mesh offset information.
        Vector3[] morphedVertices = sourceMesh.vertices;
        Vector3[] morphedNormals = sourceMesh.normals;
		
		
		
        //For each attribute...
        for (int j = 0; j < attributes.Length; j++)
        {
			float temp = attributeProgress[j];
			if (attributeProgressCounter[j] > 1.0f)
				temp = 0;
			if (attributeProgressCounter[j] < 0.0f)
				temp = 0;
            //If the weight of this attribute isn't 0	
            if (!Mathf.Approximately(attributeProgress[j], 0))
            {
				
				if (attributeProgressCounter[j] > 1.0f && attributeProgress[j] < 0.0f)
					attributeProgress[j] = 0.0f;
				if (attributeProgressCounter[j] < 0.0f && attributeProgress[j] > 0.0f)
					attributeProgress[j] = 0.0f;
				//~ if (attributeProgressCounter[j] >= -1 && attributeProgressCounter[j] <= 0) {
					//For each vertex in this attribute's blend shape...
				for (int i = 0; i < blendShapes[j].vertices.Length; i++)
				{
						//...adjust the mesh according to the offset value and weight
						morphedVertices[blendShapes[j].vertices[i].originalIndex] +=  blendShapes[j].vertices[i].position * -temp; //attributeProgress[j];
						//Adjust normals as well
						morphedNormals[blendShapes[j].vertices[i].originalIndex] +=  blendShapes[j].vertices[i].normal * -temp; // attributeProgress[j];
				}
				attributeProgressCounter[j] -= attributeProgress[j];
				//~ }
				//~ else {
					//~ if (attributeProgressCounter[j] < -1.0f)
						//~ attributeProgressCounter[j] -= attributeProgressCounter[j] + 1.0f;
					//~ if (attributeProgressCounter[j] > 0.0f)
						//~ attributeProgressCounter[j] -= attributeProgressCounter[j];
				//~ }
				//~ if (attributeProgressCounter[j] >= -1 && attributeProgressCounter[j] <= 0)
				//~ else {
					//~ attributeProgressCounter[j] = Mathf.Round(attributeProgressCounter[j]);
				//~ }
            }
        }

        //Update the actual mesh with new vertex and normal information, then recalculate the mesh bounds.		
        workingMesh.vertices = morphedVertices;
        workingMesh.normals = morphedNormals;
        workingMesh.RecalculateBounds();
    }



}
