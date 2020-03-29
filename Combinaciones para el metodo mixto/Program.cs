using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace Combinaciones_para_el_metodo_mixto
{
    class Program
    {
        private static double x0 = 6, a = 1, c = 101, m = 211,
                              prom, miu = 0.5, varz = 0.8333,
                              n, z0, x2, x2T = 7.81, FE, lambda = 8,
                              Ra = 2, Rb = 10, p = 0.2, k, ER, uel = 8,
                              mul, Bp = 0.2, Plambda = 2, PosX;
        private static double[] rMixto, list, FO, exp, emp, unf, brn, ern, px, Px, psn;
        private static string[] Combinaciones, poscom;
        static void Main(string[] args)
        {
            Console.WriteLine("Combinacion mixta 1");
            int election = int.Parse(Console.ReadLine());
            switch (election)
            {
                case 1:
                    if (CombinacionesMixto().Length > 0)
                    {
                        Console.WriteLine("Se han generado con exito " + rMixto.Count());
                        File.WriteAllLines("Exito.txt", Combinaciones);
                    }
                    else
                        Console.WriteLine("No se genero ninguna combinacion");

                    break;
                default:
                    break;
            }
            Console.ReadLine();
        }

        #region Methods

        private static double[] Mixto()
        {
            rMixto = new double[(int)m];
            x0 = (a * x0) + c;
            rMixto[0] = x0 % m;
            for (int i = 1; i < m; i++)
            {
                x0 = (a * rMixto[i - 1]) + c;
                rMixto[i] = x0 % m;
            }
            x0 = 0;
            return rMixto;
        }

        private static string[] CombinacionesMixto()
        {
            try
            {
                Combinaciones = new string[1000000];
                int n = 0;
                for (int k = 200; k < 1000; k++)
                {
                    if (EsPrimo(k))
                    {
                        m = k;
                        for (int i = 0; i < 200; i++)
                        {
                            if (i % 2 != 0 && i % 3 != 0 && i % 5 != 0)
                            {
                                a = i;
                                for (int j = 0; j < 200; j++)
                                {
                                    if (j % 2 != 0 && EsPrimoRelativo(j, m) && a < j)
                                    {
                                        c = j;
                                        Mixto();
                                        if (!Repetido(rMixto) && Promedio() && Frecuencias() && Series())
                                        {
                                            Combinaciones[n] = "a = " + a + ", c = " + c + ", m = " + m;
                                            n++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Combinaciones = Combinaciones.Where(c => c != null).ToArray();
                return Combinaciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Aun asi aqui estan las combinaciones posibles que alcanzaron a salir");
                return Combinaciones;
            }
        }

        private static bool Promedio()
        {
            Muestra();
            prom = list.Sum() / n;
            z0 = ((prom - miu) * Math.Sqrt(n)) / Math.Sqrt(varz);

            if (-1.96 < z0 && z0 < 1.96)
            {
                return true;
            }
            return false;
        }

        private static bool Frecuencias()
        {
            FO = new double[4];
            Muestra();
            for (int i = 0; i < list.Length; i++)
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

            FE = list.Length / 4;

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

        public static bool Series()
        {
            FO = new double[4];
            Muestra();
            for (int i = 0; i < list.Length - 1; i++)
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

        public static void Exponencial()
        {
            exp = new double[rMixto.Count()];
            for (int i = 0; i < rMixto.Length; i++)
            {
                exp[i] = -(1 / lambda) * Math.Log(rMixto[i] / m);
            }
        }

        public static void Uniforme()
        {
            unf = new double[rMixto.Count()];
            for (int i = 0; i < rMixto.Length; i++)
            {
                unf[i] = Ra + (Rb - Ra) * rMixto[i] / m;
            }
        }

        public static void Empirica()
        {
            emp = new double[rMixto.Count()];
            for (int i = 0; i < rMixto.Length; i++)
            {
                emp[i] = rMixto[i] / m <= 0.5 ? Math.Sqrt(2 * (rMixto[i] / m)) : 2 * (rMixto[i]);
            }
        }

        public static void Bernulli()
        {
            brn = new double[rMixto.Count()];
            for (int i = 0; i < rMixto.Length; i++)
            {
                brn[i] = rMixto[i] / m >= p ? 0 : 1;
            }
        }

        public static void Erlang()
        {
            ern = new double[20];
            k = ern.Length;
            for (int i = 0; i < ern.Length; i++)
            {
                ern[i] = 1 - rMixto[i] / m;
            }
            mul = ern[0];
            for (int i = 1; i < ern.Length; i++)
            {
                mul *= ern[i];
            }
            ER = -(uel / k) * (Math.Log(mul));
        }

        public static void Poisson()
        {
            px = new double[20];
            Px = new double[20];
            psn = new double[rMixto.Length];
            poscom = new string[rMixto.Length];
            for (int i = 0; i < 20; i++)
            {
                px[i] = (Math.Pow(Plambda, i) * Math.Exp(-Plambda)) / Factorial(i);
            }
            Px[0] = px[0];
            for (int i = 1; i < 20; i++)
            {
                Px[i] = Px[i - 1] + px[i];
            }

            for (int j = 0; j < rMixto.Length; j++)
            {
                if (rMixto[j] / m <= Px[0] && rMixto[j] / m > 0)
                {
                    poscom[j] = rMixto[j] / m + " <= " + Px[0] + " && " + rMixto[j] / m + " > " + 0;
                    psn[j] = 0;
                }
                for (int i = 1; i < 20; i++)
                {
                    if (rMixto[j] / m > Px[i - 1] && rMixto[j] / m <= Px[i])
                    {
                        poscom[j] = rMixto[j] / m + " > " + Px[i - 1] + " && " + rMixto[j] / m + " <= " + Px[i];
                        psn[j] = i;
                    }
                }
            }

        }

        #endregion


        #region Helpers

        private static double Factorial(int n)
        {
            double fact = 1;
            for (int i = 1; i <= n; i++)
            {
                fact *= i;
            }
            return fact;
        }

        private static void Muestra()
        {
            int aux = (int)Math.Round(m / 20);
            list = new double[aux];
            for (int i = 0; i < aux; i++)
            {
                list[i] = rMixto[i] / m;
            }
            n = list.Count();
        }

        private static bool EsPrimoRelativo(double numero1, double numero2)
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

        private static bool EsPrimo(double numero)
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

        private static bool Repetido(double[] list)
        {
            foreach (var grouping in list.GroupBy(t => t).Where(t => t.Count() != 1))
            {
                return true;
            }
            return false;
        }

        private static void Imprimir(string[] list)
        {
            Console.WriteLine("Combinaciones posibles " + list.Length);
            for (int i = 0; i < list.Length; i++)
            {
                Console.WriteLine(list[i]);
            }
        }

        #endregion

        #region Pruebas

        //public static bool Repeat(int[] num, int val, int vec)
        //{
        //    for (int i = 0; i < num.Length; i++)
        //    {
        //        if (num[i] == val)
        //            vec -= 1;
        //    }
        //    if (vec == 0)
        //        return true;
        //    else
        //        return false;
        //}

        //public static int mayorDiferencia(int[] nums)
        //{
        //    int max = 0;
        //    int min = nums[0];
        //    for (int i = 0; i < nums.Length; i++)
        //    {
        //        if (max < nums[i])
        //            max = nums[i];
        //    }
        //    for (int i = nums.Length - 1; i >= 0; i--)
        //    {
        //        if (min > nums[i])
        //            min = nums[i];
        //    }
        //    return max - min;
        //}

        //public static bool mismaDiferencia(double[] nums)
        //{
        //    double dif = nums[0] < nums[1] ? nums[1] - nums[0] : nums[0] - nums[1];
        //    dif = Math.Round(dif, 1);

        //    for (int i = 0; i < nums.Length - 1; i++)
        //    {
        //        double aux = nums[i] < nums[i + 1] ? nums[i + 1] - nums[i] : nums[i] - nums[i + 1];
        //        aux = Math.Round(aux, 1);
        //        if (aux != dif)
        //            return false;
        //    }
        //    return true;
        //}
        //public static int[] devolver(int[] arr, int[] arr2)
        //{
        //    int[] arr3 = new int[arr.Length + arr2.Length];
        //    int aux = 0, set = 0;
        //    for (int i = 0; i < arr3.Length; i++)
        //    {
        //        if (i < arr.Length)
        //        {
        //            arr3[i] = arr[aux];
        //            aux++;
        //        }
        //        else
        //        {
        //            arr3[i] = arr2[set];
        //            set++;
        //        }
        //    }
        //    int k;
        //    for (int i = 1; i < arr3.Length; i++)
        //    {
        //        for (int j = arr3.Length - 1; j >= i; j--)
        //        {
        //            if (arr3[j - 1] > arr3[j])
        //            {
        //                k = arr3[j - 1];
        //                arr3[j - 1] = arr3[j];
        //                arr3[j] = k;
        //            }
        //        }
        //    }
        //    return arr3;
        //} 

        #endregion
    }
}
