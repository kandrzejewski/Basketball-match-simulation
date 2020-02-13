using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SCR_basket;

namespace SCR_basket
{
    public class Program
    {
        public static List<CPlayerModel> lOPlayersList;

        static void Main(string[] args)
        {
            //Tworzenie listy obiektów klasy Zawodnik 
            lOPlayersList = new List<CPlayerModel>();
            //Wypełnianie list obiektami z argumentem całej listy w celu komunikacji mędziy zawodnikami 
            for (int i = 0; i < 10; i++)
            {
                lOPlayersList.Add(new CPlayerModel(i, lOPlayersList)); 
            }

            //Tworzenie obiektu klasy Boisko
            CCourt OCourt = new CCourt(lOPlayersList);
            Class1 View = new Class1(lOPlayersList); 
            //Tworzenie listy wątków 
            List<Thread> lThreadList = new List<Thread>();
            //Generowanie listy wątków (dla każdego obiektu zawodnik osobny wątek)
            foreach(CPlayerModel TempPlayer in lOPlayersList)
            {
                lThreadList.Add(new Thread(TempPlayer.Decision));
            }

            //Dodanie do listy wątków wątku dla obiektu klasy boisko
            lThreadList.Add(new Thread(OCourt.Run));
            lThreadList.Add(new Thread(View.View));
            //Uruchomienie wszystkich wątków 
            foreach(Thread TempThread in lThreadList)
            {
                TempThread.Start();
            }

            //Czekanie na zakńczenie pracy wszystkich wątków
            foreach (Thread TempThread in lThreadList)
            {
                TempThread.Join();
            }
        }
    }
}
