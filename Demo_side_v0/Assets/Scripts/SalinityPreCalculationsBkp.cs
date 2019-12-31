﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CoordinateSharp;
using System.Diagnostics;




public class SalinityPreCalculationsBkp : MonoBehaviour
{

    public static void Save(List<int>[] list, int number)
    {
  
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedArray" + Convert.ToString(number) +  ".gd");
        bf.Serialize(file, list);
        file.Close();
    }

    public static void SaveSalinityPoints(SalinityPoint[] salinityPoints)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/salinityPoints.gd");
        bf.Serialize(file, salinityPoints);
        file.Close();
    }

    public List<int>[] Load(int number)
    {


       if (File.Exists(Application.persistentDataPath + "/savedArray" + Convert.ToString(number) + ".gd"))
        {
            List<int>[] array;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedArray" + Convert.ToString(number) + ".gd", FileMode.Open);
            array = (List<int>[])bf.Deserialize(file);
            file.Close();

            return array;
        }
       else
        {
            return null;
        }
    }

    public SalinityPoint[] LoadSalinityPoints()
    {


        if (File.Exists(Application.persistentDataPath + "/salinityPoints.gd"))
        {
            SalinityPoint[] array;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/salinityPoints.gd", FileMode.Open);
            array = (SalinityPoint[])bf.Deserialize(file);
            file.Close();

            return array;
        }
        else
        {
            return null;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        List<Dictionary<string, object>> dataSalinity = CSVReader.Read("alldata2006-2010-2012");


        SalinityPoint[] salinityPoints = new SalinityPoint[dataSalinity.Count];
        List<int>[] salinityIndexesXYearMixUlimit = new List<int>[5];
        List<int>[] salinityIndexesXYearMixDLimit = new List<int>[5];

        string path = Application.persistentDataPath;

        


        for (int i = 0;i< 5; i++)
        {
            salinityIndexesXYearMixUlimit[i] = new List<int>();
            salinityIndexesXYearMixDLimit[i] = new List<int>();
        }


        for (int i = 0; i < dataSalinity.Count; i++)
        {
            float n;




            if (float.TryParse(dataSalinity[i]["X"].ToString(), out n) && float.TryParse(dataSalinity[i]["var"].ToString(), out n) &&
                float.TryParse(dataSalinity[i]["Y"].ToString(), out n))
            {
                salinityPoints[i].x = float.Parse(dataSalinity[i]["X"].ToString());
                salinityPoints[i].y = float.Parse(dataSalinity[i]["Y"].ToString());

                salinityPoints[i].salinity = float.Parse(dataSalinity[i]["var"].ToString());
                salinityPoints[i].waterLayer = int.Parse(dataSalinity[i]["level"].ToString());
                salinityPoints[i].year = int.Parse(dataSalinity[i]["year"].ToString());

                if (salinityPoints[i].salinity <= 0.5 && salinityPoints[i].year != 0)
                {

                    salinityIndexesXYearMixDLimit[(salinityPoints[i].year - 2005) / 2].Add(i);
                }

                if (salinityPoints[i].salinity <= 10 && salinityPoints[i].year != 0)
                {

                    salinityIndexesXYearMixUlimit[(salinityPoints[i].year - 2005) / 2].Add(i);
                }

            }


        }


        SaveSalinityPoints(salinityPoints);

        int dummyIndex;

        for(int i = 0; i < 5; i++)
        {
            for (int j = 0; j < salinityIndexesXYearMixDLimit[i].Count - 1; j++)
            {
                for (int k = j + 1; k < salinityIndexesXYearMixDLimit[i].Count; k++)
                {
                    if(salinityPoints[salinityIndexesXYearMixDLimit[i][k]].salinity > salinityPoints[salinityIndexesXYearMixDLimit[i][j]].salinity)
                    {
                        dummyIndex = salinityIndexesXYearMixDLimit[i][j];
                        salinityIndexesXYearMixDLimit[i][j] = salinityIndexesXYearMixDLimit[i][k];
                        salinityIndexesXYearMixDLimit[i][k] = dummyIndex;
                    }
                }

            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < salinityIndexesXYearMixUlimit[i].Count - 1; j++)
            {
                for (int k = j + 1; k < salinityIndexesXYearMixUlimit[i].Count; k++)
                {
                    if (salinityPoints[salinityIndexesXYearMixUlimit[i][k]].salinity > salinityPoints[salinityIndexesXYearMixUlimit[i][j]].salinity)
                    {
                        dummyIndex = salinityIndexesXYearMixUlimit[i][j];
                        salinityIndexesXYearMixUlimit[i][j] = salinityIndexesXYearMixUlimit[i][k];
                        salinityIndexesXYearMixUlimit[i][k] = dummyIndex;
                    }
                }

            }
        }

        Save(salinityIndexesXYearMixDLimit, 1);
        Save(salinityIndexesXYearMixUlimit, 2);


        List<int>[] exArray1 = Load(1);
        List<int>[] exArray2 = Load(2);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}