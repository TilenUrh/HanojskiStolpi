using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HanojskiStolpi
{
    public class HanojskiStolpi
    {
        public List<(byte, byte)> Povezave { get; set; }
        public List<byte> Lokacije { get; set; }

        //funkcija za izračun najkrajše poti za dan problem
        public void NajkrajsaPot(byte konec)
        {
            //deklaracija spremenljivk
            int stolpi = 4;
            List<int> stariPremiki = new List<int>() { };
            List<int> trenutnePozicije = new List<int>(){PozicijeVStevilo(Lokacije)};
            //byte[] zgornjiObrockiDefault = new byte[stolpi];
            //for(int i = 0; i < zgornjiObrockiDefault.Length; i++) { zgornjiObrockiDefault[i] = 100; }
            List<byte> seznam;
            List<int> mozniPremiki = new List<int>();
            int najvecjiObroc = Lokacije.Count - 1;
            List<byte> seznamZacasno = new List<byte>();

            int ponovitev = 0;
            long RAM = 0;

            while (najvecjiObroc >= 0)
            {
                Parallel.ForEach(trenutnePozicije, (pozicija, stanje) =>
                {
                    seznam = new List<byte>();
                    byte[] zgornjiObrocki = new byte[stolpi];
                    for (int i = 0; i < zgornjiObrocki.Length; i++) { zgornjiObrocki[i] = 100; }
                    seznam = SteviloVPozicije(pozicija, Lokacije.Count); //seznam pozicij obrockov
                    

                    //poišči najmanjše obroče v stolpcu
                    for (int i = 0; i < seznam.Count; i++)
                    {
                        if (i < zgornjiObrocki[Convert.ToInt32(seznam[i])])
                        {
                            zgornjiObrocki[Convert.ToInt32(seznam[i])] = Convert.ToByte(i);
                        }
                    }

                    //preveri vse stolpce
                    for (int i = 0; i < zgornjiObrocki.Length; i++)
                    {
                        //preveri vse povezave
                        foreach ( var povezava in Povezave)
                        {
                            

                            //preveri če je obroč na povezavi na prvem mestu
                            if (povezava.Item1 == (Convert.ToByte(i)))
                            {
                                //ne premakni večjega na manjšega
                                if (zgornjiObrocki[Convert.ToInt32(povezava.Item1)] < zgornjiObrocki[Convert.ToInt32(povezava.Item2)] && zgornjiObrocki[Convert.ToInt32(povezava.Item1)] != 100)
                                {
                                    seznamZacasno = seznam;
                                    seznamZacasno[Convert.ToInt32(zgornjiObrocki[i])] = povezava.Item2;

                                    //preveri če je bil premik že narejen prej
                                    if (!stariPremiki.Contains(PozicijeVStevilo(seznamZacasno)))
                                    {
                                        mozniPremiki.Add(PozicijeVStevilo(seznamZacasno));

                                        //preveri če je obroč prispel na končno pozicijo
                                        if (seznamZacasno.ElementAt(najvecjiObroc) == konec)
                                        {
                                            najvecjiObroc--;
                                            ponovitev++;
                                            stariPremiki = new List<int>() { };
                                            trenutnePozicije = new List<int>() { PozicijeVStevilo(seznamZacasno) };
                                            stanje.Break();
                                        }
                                    }
                                }
                            }

                            //preveri če je obroč na povezavi na drugem mestu
                            if ( povezava.Item2 == (Convert.ToByte(i)))
                            {
                                //ne premakni večjega na manjšega
                                if (zgornjiObrocki[Convert.ToInt32(povezava.Item2)] < zgornjiObrocki[Convert.ToInt32(povezava.Item1)] && zgornjiObrocki[Convert.ToInt32(povezava.Item2)] != 100)
                                {
                                    seznamZacasno = seznam;
                                    seznamZacasno[Convert.ToInt32(zgornjiObrocki[i])] = povezava.Item1;

                                    //preveri če je bil premik že narejen prej
                                    if (!stariPremiki.Contains(PozicijeVStevilo(seznamZacasno)))
                                    {
                                        mozniPremiki.Add(PozicijeVStevilo(seznamZacasno));

                                        //preveri če je obroč prispel na končno pozicijo
                                        if (seznamZacasno.ElementAt(najvecjiObroc) == konec)
                                        {
                                            najvecjiObroc--;
                                            ponovitev++;
                                            stariPremiki = new List<int>() { };
                                            trenutnePozicije = new List<int>() { PozicijeVStevilo(seznamZacasno) };
                                            stanje.Break();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                });
                //premik seznamov za en nivo
                stariPremiki = trenutnePozicije;
                trenutnePozicije = mozniPremiki;

                //stetje korakov
                ponovitev++;

                //poraba ram
                long RAMz = GC.GetTotalMemory(false);
                if (RAMz > RAM)
                {
                    RAM = RAMz;
                }
            }

            //Izpis rešitve in max porabe pomnilnika
            Console.WriteLine($"Število ponovitev: {ponovitev}");
            Console.WriteLine($"Max RAM: {RAM / 1000000} MB");
        }



        //definiranje začetnega stanja obročkov in povezav
        public static HanojskiStolpi ZacetnoStanje(string problem, byte obrocki, byte zacetek)
        {
            HanojskiStolpi hanojskiStolp = new HanojskiStolpi();

            //definiranje vseh možnih povezav za izbran problem
            switch (problem)
            {
                //K4
                case "1":
                    hanojskiStolp.Povezave = new List<(byte, byte)>() { (0, 1), (0, 2), (0, 3), (1, 2), (1, 3), (2, 3) };
                    break;

                //K1,3-e
                case "2":
                    hanojskiStolp.Povezave = new List<(byte, byte)>() { (0, 1), (0, 2), (0, 3), (2, 3) };
                    break;

                //K1,3
                case "3":
                    hanojskiStolp.Povezave = new List<(byte, byte)>() { (0, 1), (0, 2), (0, 3) };
                    break;

                //C4
                case "4":
                    hanojskiStolp.Povezave = new List<(byte, byte)>() { (0, 2), (0, 3), (1, 2), (1, 3) };
                    break;

                //K4-e
                case "5":
                    hanojskiStolp.Povezave = new List<(byte, byte)>() { (0, 1), (0, 2), (0, 3), (1, 2), (1, 3) };
                    break;

                //P1+3
                case "6":
                    hanojskiStolp.Povezave = new List<(byte, byte)>() { (1, 2), (2, 3), (3, 0) };
                    break;
            }

            //definiranje pozicij vseh obrockov (pozicija obrocka, velikost obrocka)
            hanojskiStolp.Lokacije = new List<byte>() { };
            for (int i = 0; i < obrocki; i++)
            {
                hanojskiStolp.Lokacije.Add(zacetek);
            }


            return hanojskiStolp;
        }

        //pretvorba stevila v seznam lokacij
        static List<byte> SteviloVPozicije(int stevilo, int obrocki)
        {
            List<byte> pozicijeObratno = new List<byte>();
            List<byte> pozicije = new List<byte>();
            while (obrocki != 0)
            {
                int ostanek = stevilo % 4;
                stevilo = stevilo / 4;
                pozicijeObratno.Add(Convert.ToByte(ostanek));
                obrocki--;
            }

            //zarotira zaporedje stevil v seznamu
            for (int i = pozicijeObratno.Count - 1; i >= 0; i--)
            {
                pozicije.Add(pozicijeObratno[i]);
            }

            return pozicije;
        }

        //pretvorba seznama lokacij v stevilo
        static int PozicijeVStevilo(List<byte> pozicije)
        {
            int stevilo = 0;
            int faktor = 1;

            for (int i = pozicije.Count - 1; i >= 0; i--)
            {
                stevilo = stevilo + ((int)pozicije[i] * faktor);
                faktor = faktor * 4;
            } 

            return stevilo;
        }
    }
    
    //najdi najvišje obroče v stolpu
}
