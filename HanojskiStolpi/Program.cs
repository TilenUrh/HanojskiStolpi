using System;
using System.Collections.Generic;
using System.IO;

namespace HanojskiStolpi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hanojski stolpi!");

            Boolean izvajanje = true;

            while (izvajanje == true)
            {

                String line;
                try
                {
                    //Pass the file path and file name to the StreamReader constructor
                    StreamReader sr = new StreamReader("MoznePovezave.txt");
                    //Read the first line of text
                    line = sr.ReadLine();
                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        //write the lie to console window
                        Console.WriteLine(line);
                        //Read the next line
                        line = sr.ReadLine();
                    }
                    //close the file
                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                //izbira problema
                Console.WriteLine("Izberi problem (1-6; 0=zapri): ");
                string problem = Console.ReadLine();
                byte problemBy = Convert.ToByte(problem);
                if (problemBy == 0) { izvajanje = false; break; }

                //izbira stevila obrockov
                Console.WriteLine("Vpiši velikost stolpa: ");
                string obrocki = Console.ReadLine();
                byte obrockiBy = Convert.ToByte(obrocki);

                //izbira zacetka
                Console.WriteLine("Izberi začetno pozicijo:");
                string zacetek = Console.ReadLine();
                byte zacetekBy = Convert.ToByte(zacetek);

                //izbira konca
                Console.WriteLine("Izberi končno pozicijo: ");
                string konec = Console.ReadLine();
                byte konecBy = Convert.ToByte(konec);

                HanojskiStolpi izbira = HanojskiStolpiFactory.ZacetnoStanje (problemBy, obrockiBy, zacetekBy, konecBy);
                List<byte> zacetneLokacije = HanojskiStolpi.ZacetneLokacije(izbira.Zacetek, izbira.Obrocki);

                Console.WriteLine("Preračunavam...");
                //shrani začetni čas
                DateTime zacetniCas = DateTime.Now;

                izbira.NajkrajsaPot(zacetneLokacije);

                Console.WriteLine("Končano!");
                //shrani končni čas in izpis časa izvajanja
                DateTime koncniCas = DateTime.Now;
                Console.WriteLine($"Čas izvajanja= {(koncniCas - zacetniCas).TotalSeconds}");
            }
        }
    }
}
