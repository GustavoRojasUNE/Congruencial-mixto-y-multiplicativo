using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Prob1
{
    class Aleatorio
    {
        private double x0 = 0, m, a, c, prom, miu = 0.5, varz = 0.8333,
                              n, z0, x2, x2T = 7.81, FE, lambda = 8,
                              Ra = 2, Rb = 10, p = 0.2, k, ER, uel = 8,
                              mul, Bp = 0.2, Plambda = 2, PosX, aux, exp, unf;
        private bool flag = true;
        private List<double> rMixto, list, emp, brn, ern, px, Px, psn;
        private double[] FO;
        private List<string> Combinaciones, poscom;

        public Aleatorio(string archivo = "./Exito.txt")
        {
            Combinaciones = CombinacionesMixto();
            File.WriteAllLines(archivo, Combinaciones);
        }

        #region Methods
        private List<string> CombinacionesMixto()
        {
            try
            {
                Combinaciones = new List<string>(10000);
                for (int k = 10; k < 1000; k++)
                {
                    m = k;
                    if (EsPrimo(m))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            a = i;
                            if (a % 2 != 0 && a % 3 != 0 && a % 5 != 0)
                            {
                                for (int j = 0; j < 10; j++)
                                {
                                    c = j;
                                    if (c % 2 != 0 && EsPrimoRelativo(c, m) && a < c)
                                    {
                                        Mixto(x0, m, a, c);
                                        if (!Repetido(rMixto) && Promedio(m) && Frecuencias(m) && Series(m))
                                        {
                                            Combinaciones.Add("a = " + a + ", c = " + c + ", m = " + m);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Combinaciones = Combinaciones.Where(c => c != null).ToList();
                return Combinaciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Aun asi aqui estan las combinaciones posibles que alcanzaron a salir");
                return Combinaciones;
            }
        }

        private void Mixto(double x0, double m, double a, double c)
        {
            double xn = x0;
            rMixto = new List<double>();
            for (int i = 0; i < m; i++)
            {
                rMixto.Add(xn);
                xn = ((a * xn) + c) % m;
                if (rMixto.FindAll(m => m.Equals(xn)).Count() != 0)
                    break;
                if (xn == x0)
                    break;
            }
        }

        private bool Promedio(double m)
        {
            Muestra(m);
            prom = list.Sum() / n;
            z0 = ((prom - miu) * Math.Sqrt(n)) / Math.Sqrt(varz);

            if (-1.96 < z0 && z0 < 1.96)
            {
                return true;
            }
            return false;
        }

        private bool Frecuencias(double m)
        {
            FO = new double[4];
            Muestra(m);
            for (int i = 0; i < list.Count; i++)
            {
                if (0 < list[i] && list[i] <= 0.25)
                {
                    FO[0] += 1;
                }
                else if (0.25 < list[i] && list[i] <= 0.5)
                {
                    FO[1] += 1;
                }
                else if (0.5 < list[i] && list[i] <= 0.75)
                {
                    FO[2] += 1;
                }
                else if (0.75 < list[i] && list[i] <= 1)
                {
                    FO[3] += 1;
                }
            }

            FE = list.Count / 4;

            for (int i = 0; i < FO.Length; i++)
            {
                FO[i] = Math.Pow(FE - FO[i], 2);
            }

            x2 = FO.Sum() / list.Count();

            if (x2 < x2T)
            {
                return true;
            }
            return false;
        }

        public bool Series(double m)
        {
            FO = new double[4];
            Muestra(m);
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] < 0.5 && list[i + 1] < 0.5)
                {
                    FO[0] += 1;
                }
                else if (list[i] > 0.5 && list[i + 1] < 0.5)
                {
                    FO[1] += 1;
                }
                else if (list[i] < 0.5 && list[i + 1] >= 0.5)
                {
                    FO[2] += 1;
                }
                else if (list[i] >= 0.5 && list[i + 1] >= 0.5)
                {
                    FO[3] += 1;
                }
            }

            FE = (n - 1) / 4;

            for (int i = 0; i < FO.Length; i++)
            {
                FO[i] = Math.Pow(FO[i] - FE, 2);
            }

            x2 = (4 / n - 1) * FO.Sum();

            if (x2 < x2T)
            {
                return true;
            }
            return false;
        }

        public double Exponencial(double b)
        {
            exp = GetNumber();
            exp = -b * Math.Log(exp);
            return exp;
        }

        public double Uniforme(double mean, double half)
        {
            unf = GetNumber();
            unf = mean + unf * (half - mean);
            return unf;
        }

        public double Erlang(double b, double k)
        {
            ern = new List<double>((int)k);
            for (int i = 0; i < k; i++)
            {
                ern.Add(1 - rMixto[i] / m);
            }
            mul = ern[0];
            for (int i = 1; i < ern.Count; i++)
            {
                mul *= ern[i];
            }
            ER = -b / k * (Math.Log(mul));
            return ER;
        }

        public double GetNumber()
        {
            aux = m / rMixto.Count;
            aux = aux > rMixto.Count ? rMixto.Count - 1 : aux;
            return rMixto[(int)aux] / m;
        }

        //public double Empirica()
        //{
        //    emp = new List<double>(rMixto.Count);
        //    for (int i = 0; i < rMixto.Count; i++)
        //    {
        //        emp[i] = rMixto[i] / m <= 0.5 ? Math.Sqrt(2 * (rMixto[i] / m)) : 2 * (rMixto[i]);
        //    }
        //    return emp[(int)aux];
        //}

        //public double Bernulli()
        //{
        //    brn = new List<double>(rMixto.Count);
        //    for (int i = 0; i < rMixto.Count; i++)
        //    {
        //        brn[i] = rMixto[i] / m >= p ? 0 : 1;
        //    }
        //    return brn[(int)aux];
        //}


        //public string Poisson()
        //{
        //    px = new List<double>(20);
        //    Px = new List<double>(20);
        //    psn = new List<double>(rMixto.Count);
        //    poscom = new List<string>(rMixto.Count);
        //    for (int i = 0; i < 20; i++)
        //    {
        //        px[i] = (Math.Pow(Plambda, i) * Math.Exp(-Plambda)) / Factorial(i);
        //    }
        //    Px[0] = px[0];
        //    for (int i = 1; i < 20; i++)
        //    {
        //        Px[i] = Px[i - 1] + px[i];
        //    }

        //    for (int j = 0; j < rMixto.Count; j++)
        //    {
        //        if (rMixto[j] / m <= Px[0] && rMixto[j] / m > 0)
        //        {
        //            poscom[j] = rMixto[j] / m + " <= " + Px[0] + " && " + rMixto[j] / m + " > " + 0;
        //            psn[j] = 0;
        //        }
        //        for (int i = 1; i < 20; i++)
        //        {
        //            if (rMixto[j] / m > Px[i - 1] && rMixto[j] / m <= Px[i])
        //            {
        //                poscom[j] = rMixto[j] / m + " > " + Px[i - 1] + " && " + rMixto[j] / m + " <= " + Px[i];
        //                psn[j] = i;
        //            }
        //        }
        //    }
        //    return poscom[0];
        //}

        #endregion


        #region Helpers

        private double Factorial(int n)
        {
            double fact = 1;
            for (int i = 1; i <= n; i++)
            {
                fact *= i;
            }
            return fact;
        }

        private void Muestra(double m)
        {
            int aux = (int)Math.Round(m / rMixto.Count);
            aux = aux > rMixto.Count ? rMixto.Count - 1 : aux;
            list = new List<double>();
            for (int i = 0; i < aux; i++)
            {
                list.Add(rMixto[i]);
            }
            n = list.Count();
        }

        private bool EsPrimoRelativo(double numero1, double numero2)
        {
            double resto;
            while (numero2 != 0)
            {
                resto = numero1 % numero2;
                numero1 = numero2;
                numero2 = resto;
            }

            return numero1 == 1 || numero1 == -1;
        }

        private bool EsPrimo(double numero)
        {
            int divisor = 2;
            double resto = 0;
            while (divisor < numero)
            {
                resto = numero % divisor;
                if (resto == 0)
                {
                    return false;
                }
                divisor = divisor + 1;
            }
            return true;
        }

        private bool Repetido(List<double> list)
        {
            foreach (var grouping in list.GroupBy(t => t).Where(t => t.Count() != 1))
            {
                return true;
            }
            return false;
        }

        private void Imprimir(List<double> list)
        {
            Console.WriteLine("Combinaciones posibles " + list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i]);
            }
        }

        #endregion

    }
}
