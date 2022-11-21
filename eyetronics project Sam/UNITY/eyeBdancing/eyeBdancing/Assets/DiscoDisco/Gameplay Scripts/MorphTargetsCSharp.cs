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
        public BlendShapeVertex[] vertices;
    }

    public String[] attributes = new String[0];
    public Mesh sourceMesh;
    public Mesh[] attributeMeshes = new Mesh[0];
    public float[] attributeProgress = new float[0];
    float[] attributeProgressCounter = new float[0]; // these values are auto-adjusted by the attributeprogress. DO NOT  TOUCH THESE VALUES

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

        int vertexCount = sourceMesh.vertexCount;
        for (int i = 0; i < attributeMeshes.Length; i++)
        { 
            if (attributeMeshes[i].vertexCount != vertexCount)
            {
                Debug.Log("Mesh " + i + " doesn't have the same number of vertices as the first mesh");
                return;
            }
        }

        BuildBlendShapes();
    }

    void BuildBlendShapes()
    {

        blendShapes = new BlendShape[attributes.Length];

        for (int i = 0; i < attributes.Length; i++)
        {
            blendShapes[i] = new BlendShape();
			
            int blendShapeCounter = 0;
            for (int j = 0; j < workingMesh.vertexCount; j++)
            {
                
                if (workingMesh.vertices[j] != attributeMeshes[i].vertices[j])
                {
                    blendShapeCounter++;
                }
            }
            
            blendShapes[i].vertices = new BlendShapeVertex[blendShapeCounter];
			
            blendShapeCounter = 0;
            for (int j = 0; j < workingMesh.vertexCount; j++)
            {
                if (workingMesh.vertices[j] != attributeMeshes[i].vertices[j])
                {
                    BlendShapeVertex blendShapeVertex = new BlendShapeVertex();
                    blendShapeVertex.originalIndex = j;
                    blendShapeVertex.position = attributeMeshes[i].vertices[j] - workingMesh.vertices[j];
                    blendShapeVertex.normal = attributeMeshes[i].normals[j] - workingMesh.normals[j];

					blendShapes[i].vertices[blendShapeCounter]=blendShapeVertex;
                    blendShapeCounter++;
                }
            }
        }
    }


    public void SetMorph()
    {
        Vector3[] morphedVertices = sourceMesh.vertices;
        Vector3[] morphedNormals = sourceMesh.normals;
		
        for (int j = 0; j < attributes.Length; j++)
        {
			float temp = attributeProgress[j];
			if (attributeProgressCounter[j] > 1.0f)
				temp = 0;
			if (attributeProgressCounter[j] < 0.0f)
				temp = 0;	
            if (!Mathf.Approximately(attributeProgress[j], 0))
            {
				
				if (attributeProgressCounter[j] > 1.0f && attributeProgress[j] < 0.0f)
					attributeProgress[j] = 0.0f;
				if (attributeProgressCounter[j] < 0.0f && attributeProgress[j] > 0.0f)
					attributeProgress[j] = 0.0f;
				for (int i = 0; i < blendShapes[j].vertices.Length; i++)
				{
						morphedVertices[blendShapes[j].vertices[i].originalIndex] +=  blendShapes[j].vertices[i].position * -temp;
						morphedNormals[blendShapes[j].vertices[i].originalIndex] +=  blendShapes[j].vertices[i].normal * -temp;
				}
				attributeProgressCounter[j] -= attributeProgress[j];
            }
        }
        workingMesh.vertices = morphedVertices;
        workingMesh.normals = morphedNormals;
        workingMesh.RecalculateBounds();
    }



}
