using System;
using System.Collections.Generic;
using System.Linq;

namespace Prob1
{
    class Program
    {
        class Evento
        {
            public bool inspeccionado;
            public string prev_evento, tipo_evento;
            public double tiempo_creacion, tiempo_salida, tiempo_evento, tiempo_inspeccion;
            public Evento()
            {
                this.inspeccionado = false;
                this.tipo_evento = "";
                this.prev_evento = "";
                this.tiempo_creacion = 0;
                this.tiempo_salida = 0;
                this.tiempo_evento = 0;
                this.tiempo_inspeccion = 0;
            }
        }
        static void Main(string[] args)
        {
            Aleatorio random = new Aleatorio();

            double tiempo = 0, tiempo_max = 70000;
            Evento evento;
            Evento pieza;
            List<Evento> eventos = new List<Evento>();

            while (tiempo < tiempo_max)
            {
                evento = new Evento();
                tiempo += random.Uniforme(125, 25);
                evento.tiempo_creacion = tiempo;
                evento.tiempo_evento = evento.tiempo_creacion;
                evento.tipo_evento = "llegada_cliente1";
                eventos.Add(evento);
            }

            tiempo = 0;
            while (tiempo < tiempo_max)
            {
                evento = new Evento();
                tiempo += 120;
                evento.tiempo_creacion = tiempo;
                evento.tiempo_evento = evento.tiempo_creacion;
                evento.tipo_evento = "llegada_cliente2";
                eventos.Add(evento);
            }

            tiempo = 0;
            List<Evento> cola_de_espera = new List<Evento>();
            List<Evento> salidas = new List<Evento>();
            bool inspector_ocupado = false;
            double pieza_max = 0;

            while (tiempo < tiempo_max && salidas.Where(s => s.tipo_evento == "salida_inspeccion_cliente2").Count() != 500)
            {
                eventos = eventos.OrderBy(e => e.tiempo_evento).ToList();
                evento = eventos[0];
                eventos.RemoveAt(0);
                tiempo = evento.tiempo_evento;
                pieza_max = pieza_max > cola_de_espera.Count ? pieza_max : cola_de_espera.Count;
                if (evento.tipo_evento == "llegada_cliente1")
                {
                    if (cola_de_espera.Count == 0 && inspector_ocupado == false)
                    {
                        inspector_ocupado = true;
                        evento.tiempo_inspeccion = tiempo;
                        evento.tiempo_evento = tiempo + random.Exponencial(25);
                        evento.prev_evento = evento.tipo_evento;
                        evento.tipo_evento = "salida_inspeccion_cliente1";
                        evento.tiempo_salida = evento.tiempo_evento;
                        evento.inspeccionado = true;
                        eventos.Add(evento);
                    }
                    else
                    {
                        cola_de_espera.Add(evento);
                    }
                }
                else if (evento.tipo_evento == "salida_inspeccion_cliente1")
                {
                    evento.tiempo_salida = tiempo;
                    salidas.Add(evento);
                    inspector_ocupado = false;
                    if (cola_de_espera.Count > 0)
                    {
                        pieza = cola_de_espera[0];
                        cola_de_espera.RemoveAt(0);
                        inspector_ocupado = false;
                        pieza.tiempo_inspeccion = tiempo;
                        pieza.tiempo_evento = tiempo + (pieza.tipo_evento == "llegada_cliente1" ? random.Exponencial(25) : random.Erlang(2, 35));
                        pieza.prev_evento = pieza.tipo_evento;
                        pieza.tipo_evento = pieza.prev_evento == "llegada_cliente1" ? "salida_inspeccion_cliente1" : "salida_inspeccion_cliente2";
                        pieza.tiempo_salida = pieza.tiempo_evento;
                        pieza.inspeccionado = true;
                        eventos.Add(pieza);
                    }
                }
                else if (evento.tipo_evento == "llegada_cliente2")
                {
                    if (cola_de_espera.Count == 0 && inspector_ocupado == false)
                    {
                        inspector_ocupado = true;
                        evento.tiempo_inspeccion = tiempo;
                        evento.tiempo_evento = tiempo + random.Erlang(2, 35);
                        evento.prev_evento = evento.tipo_evento;
                        evento.tipo_evento = "salida_inspeccion_cliente2";
                        evento.tiempo_salida = evento.tiempo_evento;
                        evento.inspeccionado = true;
                        eventos.Add(evento);
                    }
                    else
                    {
                        cola_de_espera.Add(evento);
                    }
                }
                else if (evento.tipo_evento == "salida_inspeccion_cliente2")
                {
                    evento.tiempo_salida = tiempo;
                    salidas.Add(evento);
                    inspector_ocupado = false;
                    if (cola_de_espera.Count > 0)
                    {
                        pieza = cola_de_espera[0];
                        cola_de_espera.RemoveAt(0);
                        inspector_ocupado = true;
                        pieza.tiempo_inspeccion = tiempo;
                        pieza.tiempo_evento = tiempo + (pieza.tipo_evento == "llegada_cliente2" ? random.Erlang(2, 35) : random.Exponencial(25));
                        pieza.prev_evento = pieza.tipo_evento;
                        pieza.tipo_evento = pieza.prev_evento == "llegada_cliente2" ? "salida_inspeccion_cliente2" : "salida_inspeccion_cliente1";
                        pieza.tiempo_salida = pieza.tiempo_evento;
                        pieza.inspeccionado = true;
                        eventos.Add(pieza);
                    }
                }
            }

            double total_clientes1 = 0;
            List<Evento> promedio_cliente1 = salidas.Where(s => s.tipo_evento == "salida_inspeccion_cliente1").ToList();
            List<Evento> promedio_cliente2 = salidas.Where(s => s.tipo_evento == "salida_inspeccion_cliente2").ToList();

            double prom1 = promedio_cliente1.Sum(p1 => p1.tiempo_salida - p1.tiempo_creacion) / promedio_cliente1.Count;
            double prom2 = promedio_cliente2.Sum(p2 => p2.tiempo_salida - p2.tiempo_creacion) / promedio_cliente2.Count;


            total_clientes1 = salidas.Where(s => s.tipo_evento == "salida_inspeccion_cliente1").Count();

            Console.WriteLine(Math.Round(tiempo / 60, 2) + " hrs totales de simulacion");
            Console.WriteLine(total_clientes1 + " clientes del tipo 1");
            Console.WriteLine(Math.Round(prom1 / 60, 2) + "hrs en promedio por el tipo 1 y " + Math.Round(prom2 / 60, 2) + "hrs en promedio por el tipo 2");
            Console.WriteLine(pieza_max + " es el numero maximo de clientes en el sistema");
        }
    }
}
