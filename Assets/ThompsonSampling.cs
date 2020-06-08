using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class ThompsonSampling : MonoBehaviour
{
    BetaDistribution betaDistribution;
    System.Random rnd;
    [SerializeField] int numberOfBandits; 
    [SerializeField] int sampleSize;
    Bandit[] bandits;
    double[,] dataSet;

    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random();
        betaDistribution = new BetaDistribution();  

        bandits = new Bandit[numberOfBandits]; 

        for(int i = 0; i < numberOfBandits; i ++)
        {
            bandits[i] = new Bandit(rnd.NextDouble());
        }    
    }

    internal void Sample()
    {
        dataSet = createDataSet();

        var nPosReward = new int[numberOfBandits];
        var nNegReward = new int[numberOfBandits];
        var nSelected = new int[numberOfBandits];
        
        // Cycle through every sample
        for(int i = 0; i < sampleSize; i ++)
        {
            var selected = 0;
            var maxDraw = 0.0;
            // Cycle through every conversion rate for this sample
            for(int j = 0; j < numberOfBandits; j++)
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
        for(int i = 0; i < numberOfBandits; i++)
        {
            nSelected[i] = nNegReward[i] + nPosReward[i];
        }
        
        // Present the results
        for(int i = 0; i < numberOfBandits; i++)
        {
            print(String.Format("Conversion rate number {0} ({1}%) was selected: {2} times.", i, (bandits[i].conversionRate * 100).ToString("0"), nSelected[i]));
        }

        var maxValue = nSelected.Max();
        var indexOfMax = nSelected.ToList().IndexOf(maxValue);
        print(String.Format("Conversion rate number {0} ({1}%) is the best choice.", indexOfMax, (bandits[indexOfMax].conversionRate * 100).ToString("0")));

    }

    internal double[,] createDataSet()
    {
        double[,] data = new double[sampleSize,numberOfBandits];
        for(int i = 0; i < sampleSize; i++)
        {
            for(int j = 0; j < numberOfBandits; j++)
            {
                if(rnd.NextDouble() < bandits[j].draw())
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

[CustomEditor(typeof(ThompsonSampling))]
public class ThompsonSamplingEditor : Editor
{  
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        ThompsonSampling myTarget = (ThompsonSampling)target;

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