using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Congruencial_mixto_y_multiplicativo
{
    class Program
    {
        private static double x0, a, c, m;
        private static double[] rMultiplicativo, rMixto;

        static void Main(string[] args)
        {
            Console.WriteLine("Que metodo congruencial quiere usar?");
            Console.WriteLine("Multiplicativo 1");
            Console.WriteLine("Mixto 2");
            int election = int.Parse(Console.ReadLine());
            switch (election)
            {
                case 1:
                    Console.WriteLine("Que es X0");
                    x0 = int.Parse(Console.ReadLine());
                    Console.WriteLine("Que es a");
                    a = int.Parse(Console.ReadLine());
                    Console.WriteLine("Que es m");
                    m = int.Parse(Console.ReadLine());
                    if (!Repetido(Multiplicativo()))
                        Imprimir(rMultiplicativo);
                    else
                        Console.WriteLine("Se repiten numeros");
                    break;
                case 2:
                    Console.WriteLine("Que es X0");
                    x0 = int.Parse(Console.ReadLine());
                    Console.WriteLine("Que es a");
                    a = int.Parse(Console.ReadLine());
                    Console.WriteLine("Que es c");
                    c = int.Parse(Console.ReadLine());
                    Console.WriteLine("Que es m");
                    m = int.Parse(Console.ReadLine());
                    if (!Repetido(Mixto()))
                        Imprimir(rMixto);
                    else
                        Console.WriteLine("Se repiten numeros");
                    break;
                default:
                    break;
            }

            Console.ReadLine();
        }

        private static double[] Multiplicativo()
        {
            rMultiplicativo = new double[(int)m];
            rMultiplicativo[0] = (a * x0) % m;
            for (int i = 1; i < m; i++)
            {
                rMultiplicativo[i] = (a * rMultiplicativo[i - 1]) % m;
            }
            return rMultiplicativo;
        }

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

        private static bool Repetido(double[] list)
        {
            foreach (var grouping in list.GroupBy(t => t).Where(t => t.Count() != 1))
            {
                return true;
            }
            return false;
        }

        private static void Imprimir(double[] list)
        {
            Console.WriteLine("Numeros aleatorios " + list.Length);
            for (int i = 0; i < list.Length; i++)
            {
                Console.WriteLine(list[i]);
            }
        }

    }
}
