using System;

//https://jamesmccaffrey.wordpress.com/2019/06/27/implementing-beta-distribution-sampling-using-c/
public class BetaDistribution
{
    public Random rnd;
    public BetaDistribution() 
    {
        this.rnd = new Random();
    }

    public double Sample(double a, double b)
    {
        //a, b > 0
        double alpha = a + b;
        double beta = 0.0;
        double u1 = 0.0;
        double u2 = 0.0;
        double w = 0.0;
        double v = 0.0;

        if(Math.Min(a,b) <= 1.0)
            beta = Math.Max(1 / a, 1 / b);
        else 
            beta = Math.Sqrt((alpha - 2.0) / (2*a*b-alpha));
        
        double gamma = a + 1 / beta;

        while(true)
        {
            u1 = this.rnd.NextDouble();
            u2 = this.rnd.NextDouble();
            v = beta * Math.Log(u1 / (1 - u1));
            w = a * Math.Exp(v);
            double tmp = Math.Log(alpha / (b + w));
            if (alpha * tmp + (gamma * v) - 1.3862944 >= Math.Log(u1 * u1 * u2))
                break;
    }

    double x = w / (b + w);
    return x;
    }// sample
}//beta distribution
