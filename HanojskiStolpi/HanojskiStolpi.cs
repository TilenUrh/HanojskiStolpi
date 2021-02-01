using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HanojskiStolpi
{
    public abstract class HanojskiStolpi
    {
        abstract public HashSet<(byte, byte)> Povezave { get; }
        public byte Problem { get; set; }
        public byte Obrocki { get; set; }
        public byte Zacetek { get; set; }
        public byte Konec { get; set; }

        public HanojskiStolpi(byte problem, byte obrocki, byte zacetek, byte konec)
        {
            this.Problem = problem;
            this.Obrocki = obrocki;
            this.Zacetek = zacetek;
            this.Konec = konec;
        }

        //funkcija za izračun najkrajše poti za dan problem
        public virtual void NajkrajsaPot(List<byte> lokacije)
        {
            //deklaracija spremenljivk
            int stolpi = 4;
            HashSet<int> stariPremiki = new HashSet<int>() { };
            List<int> trenutnePozicije = new List<int>(){PozicijeVStevilo(lokacije) };
            HashSet<int> mozniPremiki = new HashSet<int>();
            int najvecjiObroc = lokacije.Count - 1;
            
            Boolean zmanjsajObroc = false;
            int zaporedjeVNovKrog = 0;

            int ponovitev = 0;
            long RAM = 0;

            //se izvaja dokler ne pride mimo najmanjšega obročka
            while (najvecjiObroc >= 0)
            {
                //preveri vse trenutne možnosti stolpcev
                ParallelOptions options = new ParallelOptions();
                options.MaxDegreeOfParallelism = 4;
                Parallel.ForEach(trenutnePozicije, options,  (pozicija, stanje) =>
                {
                    List<byte> seznamZacasno = new List<byte>();
                    //seznam = new List<byte>();
                    byte[] zgornjiObrocki = new byte[stolpi];
                    List<byte> seznam = SteviloVPozicije(pozicija, this.Obrocki); //seznam pozicij obrockov

                    zgornjiObrocki = ZgornjiObrocki(seznam, stolpi);


                    //preveri vse stolpce
                    for (int i = 0; i < stolpi; i++)
                    {
                        //preveri vse povezave
                        foreach ( var povezava in Povezave)
                        {                            
                            //preveri če je obroč na povezavi na prvem mestu
                            if (povezava.Item1 == (Convert.ToByte(i)))
                            {
                                //ne premakni večjega na manjšega
                                if (zgornjiObrocki[povezava.Item1] < zgornjiObrocki[povezava.Item2])
                                {
                                    seznamZacasno = new List<byte> (seznam);
                                    seznamZacasno[zgornjiObrocki[i]] = povezava.Item2;

                                    //preveri če je bil premik že narejen prej in če je obroč že na končni poziciji
                                    int vsebuje = PozicijeVStevilo(seznamZacasno);
                                    if (!stariPremiki.Contains(vsebuje)) 
                                    {
                                        lock (mozniPremiki)
                                        {
                                            mozniPremiki.Add(PozicijeVStevilo(seznamZacasno));
                                        }

                                        //preveri če je obroč prispel na končno pozicijo
                                        if (seznamZacasno.ElementAt(najvecjiObroc) == this.Konec)
                                        {
                                            zaporedjeVNovKrog = PozicijeVStevilo(seznamZacasno);
                                            zmanjsajObroc = true;
                                            stanje.Break();
                                            goto EndOfLoop;
                                        }
                                    }
                                }
                            }
                        }
                    }
                EndOfLoop:;
                });
                //premik seznamov za en nivo
                stariPremiki = new HashSet<int> (trenutnePozicije);
                trenutnePozicije = new List<int>(mozniPremiki);
                mozniPremiki.Clear();

                if (zmanjsajObroc == true)
                {
                    najvecjiObroc--;
                    stariPremiki.Clear();
                    trenutnePozicije.Clear();
                    trenutnePozicije.Add(zaporedjeVNovKrog);
                    zmanjsajObroc = false;
                }
                
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
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

        //najdi najvišje obroče v stolpu
        static byte[] ZgornjiObrocki(List<byte> seznam, int stolpi)
        {
            byte[] zgornjiObrocki = new byte[stolpi];
            for (int i = 0; i < stolpi; i++) { zgornjiObrocki[i] = 100; }
            for (int i = 0; i < seznam.Count; i++)
            {
                if (i < zgornjiObrocki[seznam[i]])
                {
                    zgornjiObrocki[seznam[i]] = Convert.ToByte(i);
                }
            }
            return zgornjiObrocki;
        }

        public static List<byte> ZacetneLokacije(byte zacetek, byte obrocki)
        {
            //definiranje pozicij vseh obrockov (pozicija obrocka, velikost obrocka)
            List<byte> lokacije = new List<byte>() { };
            for (int i = 0; i < obrocki; i++)
            {
                lokacije.Add(zacetek);
            }

            return lokacije;
        }
    }
    
    static class HanojskiStolpiFactory
    {
        public static HanojskiStolpi ZacetnoStanje (byte problem, byte obrocki, byte zacetek, byte konec)
        {
            byte stolpci = 4;
            HanojskiStolpi hanojskiStolpi = null;

            switch (problem)
            {
                case 1:
                    hanojskiStolpi = new Han_K4(stolpci, obrocki, zacetek, konec);
                    break;
                case 2:
                    hanojskiStolpi = new Han_K13e(stolpci, obrocki, zacetek, konec);
                    break;
                case 3:
                    hanojskiStolpi = new Han_K13(stolpci, obrocki, zacetek, konec);
                    break;
                case 4:
                    hanojskiStolpi = new Han_C4(stolpci, obrocki, zacetek, konec);
                    break;
                case 5:
                    hanojskiStolpi = new Han_K4e(stolpci, obrocki, zacetek, konec);
                    break;
                case 6:
                    hanojskiStolpi = new Han_P13(stolpci, obrocki, zacetek, konec);
                    break;
            }

            return hanojskiStolpi;
        }
    }

    public class Han_K4 : HanojskiStolpi
    {
        public Han_K4(byte stolpci, byte obrocki, byte zacetek, byte konec) : base(stolpci, obrocki, zacetek, konec)
        {

        }

        public override HashSet<(byte, byte)> Povezave
        {
            get
            {
                return new HashSet<(byte, byte)>() { (0, 1), (1, 0), (0, 2), (2, 0), (0, 3), (3, 0), (1, 2), (2, 1), (1, 3), (3, 1), (2, 3), (3, 2) };
            }
        }
    }

    public class Han_K13e : HanojskiStolpi
    {
        public Han_K13e(byte stolpci, byte obrocki, byte zacetek, byte konec) : base(stolpci, obrocki, zacetek, konec)
        {

        }

        public override HashSet<(byte, byte)> Povezave
        {
            get
            {
                return new HashSet<(byte, byte)>() { (0, 1), (1, 0), (0, 2), (2, 0), (0, 3), (3, 0), (2, 3), (3, 2) };
            }
        }
    }

    public class Han_K13 : HanojskiStolpi
    {
        public Han_K13(byte stolpci, byte obrocki, byte zacetek, byte konec) : base(stolpci, obrocki, zacetek, konec)
        {

        }

        public override HashSet<(byte, byte)> Povezave
        {
            get
            {
                return new HashSet<(byte, byte)>() { (0, 1), (1, 0), (0, 2), (2, 0), (0, 3), (3, 0) };
            }
        }
    }

    public class Han_C4 : HanojskiStolpi
    {
        public Han_C4(byte stolpci, byte obrocki, byte zacetek, byte konec) : base(stolpci, obrocki, zacetek, konec)
        {

        }

        public override HashSet<(byte, byte)> Povezave
        {
            get
            {
                return new HashSet<(byte, byte)>() { (0, 2), (2, 0), (0, 3), (3, 0), (1, 2), (2, 1), (1, 3), (3, 1) };
            }
        }
    }

    public class Han_K4e : HanojskiStolpi
    {
        public Han_K4e(byte stolpci, byte obrocki, byte zacetek, byte konec) : base(stolpci, obrocki, zacetek, konec)
        {

        }

        public override HashSet<(byte, byte)> Povezave
        {
            get
            {
                return new HashSet<(byte, byte)>() { (0, 1), (1, 0), (0, 2), (2, 0), (0, 3), (3, 0), (1, 2), (2, 1), (1, 3), (3, 1) };
            }
        }
    }

    public class Han_P13 : HanojskiStolpi
    {
        public Han_P13(byte stolpci, byte obrocki, byte zacetek, byte konec) : base(stolpci, obrocki, zacetek, konec)
        {

        }

        public override HashSet<(byte, byte)> Povezave
        {
            get
            {
                return new HashSet<(byte, byte)>() { (1, 2), (2, 1), (2, 3), (3, 2), (3, 0), (0, 3) };
            }
        }
    }
}
