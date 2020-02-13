using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCR_basket
{
    public class CCourt:CTeam
    {
        private int iLocalGuestScore = 0; //Pomocniczy wynik gości 
        private int iLocalHostScore = 0; //Pomocniczy wynik gospodarzy
        public List<CPlayerModel> lOLocalPlayer;

        public CCourt(List<CPlayerModel> _lOPlayer) //Konstruktor
        {
            lOLocalPlayer = _lOPlayer;
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Run()
        {
            mInitiation(); //Generowanie warunków początkowych wszystkich zawodników 
            while (iScoreGuest < 10 && iScoreHost < 10) //Dopóki wynik <10 to monitoruje zmiany wyniku meczu
            {
                mMonitor(); //Monitorowanie zmian wyniku
            }
            bEnd = true; //Flaga zakończenia meczu
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("                        Koniec Meczu                             ");
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("                 | Gospodarze     Gości    |                     ");
            Console.WriteLine("                 |     " + iScoreHost + "            " + iScoreGuest + "      |");
            Console.WriteLine("-----------------------------------------------------------------");
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mInitiation() //Generowanie warunków początkowych wszystkich zawodników 
        {
            mDrawBallPlayer(0, 9); //Losuj który zawodnik i drużyna dostanie piłkę
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("              Zawodnik o ID: " + iHaveBallID + " wylosował piłkę.");
            if (iHaveBallID < 5)
            {
                Console.WriteLine("                 Rozpoczyna drużyna gospodarzy");
            }
            else
            {
                Console.WriteLine("               Rozpoczyna drużyna gości");
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("          Wciśnij dowolny przycisk, aby kontynuować              ");
            Console.WriteLine("-----------------------------------------------------------------");
            Console.ReadKey();
            foreach (CPlayerModel LocalPlayer in lOLocalPlayer)
            {
                LocalPlayer.mGeneratePlayerParam(); //Generowanie parametrów poczatkowych zawodników
            }
            bDrawPause = false; //Wyłączenie pauzy na wygenerowanie paramaterów początkowych 
        }

        public void mMonitor() //Czuwa nad zmianami  wyniku 
        {
            if (iScoreGuest > iLocalGuestScore) // Jeżeli goście zdobyli punkt 
            {
                bPointPause = true; //uruchom pauzę na ponowne ustawienie zawodników i wybór rozgrywającego
                mDrawBallPlayer(0, 4); //Losowanie zawodnika posiadającego piłkę z drużyny tracącej punkt
                mGenerateNewPosition(); //Generowanie nowych pozycji zawodników
                iLocalGuestScore = iScoreGuest; //Zapisanie zdobytego punktu 
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("                     Przerwa po trafieniu                        ");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("                 | Gospodarze     Gości    |                    ");
                Console.WriteLine("                 |     " + iScoreHost + "            " + iScoreGuest + "      |");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("          Wciśnij dowolny przycisk, aby kontynuować              ");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.ReadKey();
                bPointPause = false; //Wyłączenie pauzy 
            }
            if (iScoreHost > iLocalHostScore) // Jeżeli gospodarze zdobyli punkt 
            {
                bPointPause = true; //uruchom pauzę na ponowne ustawienie zawodników i wybór rozgrywającego
                mDrawBallPlayer(5, 9); //Losowanie zawodnika posiadającego piłkę z drużyny tracącej punkt
                mGenerateNewPosition(); //Generowanie nowych pozycji zawodników
                iLocalHostScore = iScoreHost; //Zapisanie zdobytego punktu 
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("                     Przerwa po trafieniu                        ");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("                 | Gospodarze     Gości    |                    ");
                Console.WriteLine("                 |     " + iScoreHost + "            " + iScoreGuest + "      |");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("          Wciśnij dowolny przycisk, aby kontynuować              ");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.ReadKey();
                bPointPause = false; //Wyłączenie pauzy 
            } 
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mGenerateNewPosition() //Generowanie nowych pozycji zawodników 
        {
            foreach (CPlayerModel LocalPlayer in lOLocalPlayer)
            {
                LocalPlayer.mGeneratePosition(); //Wywołanie funkcji generującej pozycje każdego zawodnika
                Console.WriteLine("Zawodnik o id " + LocalPlayer.iPlayerId + " ma nowa pozycje na boisku: " + LocalPlayer.iPosition + " jest w drużynie: " + LocalPlayer.bTeam + " ma piłkę: " + LocalPlayer.bHaveBall);
                Thread.Sleep(20);
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mDrawBallPlayer(int _iCountFrom, int _iCountTo) //Losowanie zawodnika posiadającego piłkę 
        {
            Random DrawID = new Random();
            iHaveBallID = DrawID.Next(_iCountFrom, _iCountTo); //Losowanie ID zawodnika z podanego zakresu 
            foreach(CPlayerModel LocalPlayer in lOLocalPlayer)
            {
                LocalPlayer.bHaveBall = false; //Zerowanie flagi posadania piłki 
            }
            lOLocalPlayer[iHaveBallID].bHaveBall = true; //Ustawienie flagi posiadania piłki wylosowanego zawodnika 
        }
    }
}
