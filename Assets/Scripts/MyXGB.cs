
using System;

public class MyXGB
{
    public float base_score;
    public int num_trees;
    public Tree[] trees;

    public void Predict(float[,] inputs, float[] preds)
    {
        for (int i=0;i<preds.GetLength(0);i++){
            preds[i] = base_score;
        }
        for (int t=0;t<num_trees;t++)
        {
            // Console.WriteLine(preds[0]);
            // Debug.Log(preds[0]);
            trees[t].Predict(inputs,preds,true);
        }
    }
}

[Serializable]
public class Tree
{
    public int[] left_children;
    public int[] right_children;
    public int[] split_indices;
    public float[] split_conditions;

    public void Predict(float[,] x, float[] pred, bool add = true){
        int node = 0;
        for (int i=0;i<x.GetLength(0);i++)
        {
            while (left_children[node] != -1 || right_children[node] != -1){
                float feature = x[i,split_indices[node]];
                float thres = split_conditions[node];
                if (feature < thres){
                    node = left_children[node];
                } else {
                    node = right_children[node];
                }
            }
            pred[i] += split_conditions[node];
        }
    }
}