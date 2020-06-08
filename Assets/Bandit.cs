using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bandit 
{
   [SerializeField] public double conversionRate;
   System.Random rnd;
   
   public Bandit()
   {
       rnd = new System.Random();
       conversionRate = rnd.NextDouble();
   }

   public Bandit(double _conversionRate)
   {
       rnd = new System.Random();
       this.conversionRate = _conversionRate;
   }

   public int draw()
   {
       if(rnd.NextDouble() < conversionRate){
           return 1;
       }

       return 0;
   }
}
