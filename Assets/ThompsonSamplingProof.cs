using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class ThompsonSamplingProof : MonoBehaviour
{
    BetaDistribution betaDistribution;
    System.Random rnd;
    [SerializeField] double[] conversionRates; 
    [SerializeField] int sampleSize;
    int numberOfRates;
    double[,] dataSet;

    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random();
        betaDistribution = new BetaDistribution();
        numberOfRates = conversionRates.Length;
    }

    internal void Sample()
    {
        dataSet = createDataSet();

        var nPosReward = new int[numberOfRates];
        var nNegReward = new int[numberOfRates];
        var nSelected = new int[numberOfRates];
        
        // Cycle through every sample
        for(int i = 0; i < sampleSize; i ++)
        {
            var selected = 0;
            var maxDraw = 0.0;
            // Cycle through every conversion rate for this sample
            for(int j = 0; j < numberOfRates; j++)
            {
                // Take a random draw from the beta distribution from this conversion rate at this sample
                var randomDraw = betaDistribution.Sample(nPosReward[j] +1, nNegReward[j] +1);

                // If that draw is higher than the previous highest draw in this sample select this conversion rate and record the as the new highest draw
                if(randomDraw > maxDraw)
                {
                    selected = j;
                    maxDraw = randomDraw;
                }
            }

            // If the selected conversion rate was succesful this sample +1 to its overall positive rewards if not +1 to its overall negative rewards
            if(dataSet[i,selected] == 1)
            {
                nPosReward[selected] += 1;
            }
            else
            {
                nNegReward[selected] += 1;
            }
        }

        // Sum up the total number of times each conversion rate waas selected
        for(int i = 0; i < numberOfRates; i++)
        {
            nSelected[i] = nNegReward[i] + nPosReward[i];
        }
        
        // Present the results
        for(int i = 0; i < numberOfRates; i++)
        {
            print(String.Format("Conversion rate number {0} ({1}%) was selected: {2} times.", i, conversionRates[i] * 100, nSelected[i]));
        }

        var maxValue = nSelected.Max();
        var indexOfMax = nSelected.ToList().IndexOf(maxValue);
        print(String.Format("Conversion rate number {0} ({1}%) is the best choice.", indexOfMax, conversionRates[indexOfMax] * 100));

    }

    internal double[,] createDataSet()
    {
        double[,] data = new double[sampleSize,numberOfRates];
        for(int i = 0; i < sampleSize; i++)
        {
            for(int j = 0; j < numberOfRates; j++)
            {
                if(rnd.NextDouble() < conversionRates[j])
                {
                    data[i,j] = 1;
                }
                else
                {
                    data[i,j] = 0;
                }
            }
        }

        return data;
    }

    internal void printMultiDimensionalArray(double[,] arr)
    {
        int rowLength = arr.GetLength(0);
        int colLength = arr.GetLength(1);
        string output = "";

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                output += string.Format("{0} ", arr[i, j]);
            }
            output += "\n";
        }

        print(output);
    }
}

[CustomEditor(typeof(ThompsonSamplingProof))]
public class ThompsonSamplingProofEditor : Editor
{  
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        ThompsonSamplingProof myTarget = (ThompsonSamplingProof)target;

        if(GUILayout.Button("Create Data Set"))
        {
            myTarget.printMultiDimensionalArray(myTarget.createDataSet());
        }

        if(GUILayout.Button("Sample"))
        {
            myTarget.Sample();
        }
    }
}