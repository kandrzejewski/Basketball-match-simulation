using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCR_basket;
using System.Threading;

namespace SCR_basket
{
    public class CPlayerModel:CTeam
    {

        //Parametry dla zawodnika 
        bool bCovering = false; // Czy jest kryty
        bool bAssisting = false; // Czy jest asystowany (ma komu podać)
        bool bLock = false; //Blokada wyświetlania napisów
        public bool bHaveBall = false; //Czy ma piłkę
        public int iEnergy; //Energia
        public int iPlayerId; //ID zawodnika
        public int iPosition =0; //Pozycja zawodnika 
        public bool bTeam; //Drużyna zawodnika 
        public Mutex WaitGenerate = new Mutex(); 
        public List<CPlayerModel> lOLocalPlayersList;

        public CPlayerModel(int _iPlayerId, List<CPlayerModel> _lOPlayersList) //Konstruktor 
        {
            iPlayerId = _iPlayerId;
            lOLocalPlayersList = _lOPlayersList;
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Metody dla zawodnika 
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Decision()
        {
            while (bDrawPause) { } //Dopóki jest włączona pauza, czeka
            while (!bEnd) //Tymczasowo do 1 trafienia
            {
                while (bPointPause) { } //Dopuki jest wąłczona pauza, czeka
                //Jeżeli posiada piłkę
                if (bHaveBall)
                {
                    if (bTeam == true) //I jest z teamu true 
                    {
                        bBallTeam = true; //To teamem posiadającym piłką jest true
                    }
                    else
                    {
                        bBallTeam = false; //W przeciwnym wypadku teamem posiadającym piłke jest false 
                    }
                    //Jeżeli zawodnik posiadający piłke jest kryty przez jakiegoś przeciwnika 
                    if (lOLocalPlayersList.Any(bCov => bCov.bCovering == true))
                    {
                        //I jeżeli ma asyste 
                        if (lOLocalPlayersList.Any(bAss => bAss.bAssisting == true))
                        {
                            mPass(); //Wywołaj podanie 
                        }
                        else
                        {
                            mLost(); //Jeżeli nie ma asysty wywołaj utratę
                        }
                    }
                    foreach (CPlayerModel TempPlayer in lOLocalPlayersList) //Zerowanie parametrów 
                    {
                        TempPlayer.bCovering = false;
                        TempPlayer.bAssisting = false;
                    }
                    mMove(); //Wywołaj poruszaj się 
                }
                //Jeżelni zawodnik nie ma piłki to
                if (!bHaveBall && !bChanges)
                {
                    //Jeżeli zawodnik jest z tej samej drużyny co posiadacz piłki 
                    if (bTeam == bBallTeam)
                    {
                        mAssist(); //Asystuj
                        Thread.Sleep(400);
                    }
                    //Jeżeli zawodnik jest z przeciwnej drużyny co posiadacz piłki 
                    if (bTeam != bBallTeam)
                    {
                        mCover(); //Kryj
                        Thread.Sleep(400);
                    }
                }
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mGeneratePlayerParam() //Generowanie parametrów początkowych 
        {   
            iEnergy = 500; //Ilość energii zawodnika 
            //Generowanie przynależności do drużyny
            if (iPlayerId < 5) 
                bTeam = true;   //Druzyna gospodarzy
            else
                bTeam = false; //Drużyna gości 
            mGeneratePosition(); //Generowanie pozycji zawodników 
           // Console.WriteLine("Zawodnik o id " + iPlayerId + " ma pozycje na boisku: " + iPosition +" jest w drużynie: " + bTeam + " ma piłkę: " + bHaveBall);
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mGeneratePosition()
        {
            //Generowanie pozycji początkowej zawodnika na boisku
            WaitGenerate.WaitOne();
            Random rGeneratePosition = new Random();
            if (bTeam == true) //Jeżeli drużyna gospodarzy 
            {
                iPosition = rGeneratePosition.Next(-100, -1); //1 połowa boiska 
            }
            if (bTeam == false) //Jeżeli drużyna gości 
            {
                iPosition = rGeneratePosition.Next(1, 100); //2 połowa boiska
            }
            WaitGenerate.ReleaseMutex();
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mPass() //Podanie
        {
            //Ma asystę i może podać bez problemu
          //  Console.WriteLine("Wywołano metode mPass. ID:" + iPlayerId);
            int BestPlayerID = 0; //Pomocnicza
            bool first = false; //Pomocnicza
            foreach (CPlayerModel TempPlayer in lOLocalPlayersList)
            {
                if (TempPlayer.bAssisting == true) //Jeżeli asystuje 
                {
                    if (bTeam == TempPlayer.bTeam) //I jest z tego samego teamu co posiadacz piłki 
                    {
                        if (first == false) //Za pierwszym razem podstaw pierwszego znalezionego zawodnika 
                        {
                            BestPlayerID = TempPlayer.iPlayerId; 
                        }
                        if (first && lOLocalPlayersList[BestPlayerID].iEnergy < TempPlayer.iEnergy) //Jeżeli kolejny zawodnik ma więcej energi niż poprzedni znaleziony
                        {
                            BestPlayerID = TempPlayer.iPlayerId; //Zostaje on tymczasowo najlepszym graczem do odebrania podania 
                        }
                        first = true; //Pierszy element podstawiony 
                    }
                }
            }
            iAPBestEnergyLevel = BestPlayerID; //podstawienie znalezionego zawodnika z największą ilością energii
            if (iAPBestEnergyLevel != -1) //Jeżeli znaleziono asystującego to można podać piłką 
            {
                lOLocalPlayersList[iAPBestEnergyLevel].bHaveBall = true; //Nowy zawodnik posiada piłkę
                bHaveBall = false; //Stary zawodnik traci piłkę 
                iHaveBallID = lOLocalPlayersList[iAPBestEnergyLevel].iPlayerId; //Id zawodnika posiadającego piłkę 
             //   Console.WriteLine("Zawodnik o id: " + iPlayerId + " przekazał piłkę zawodnikowi o ID: " + iAPBestEnergyLevel);
                iAPBestEnergyLevel = -1;
                bChanges = true; //Ustawianie flagi zmian
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mLost()
        {
            //Nie ma asysty więc musi walczyć z blokującym o piłkę
          //  Console.WriteLine("Wywołano metode mLost. ID:" + iPlayerId);
            int BestPlayerID = -1; //Pomocnicza
            bool first = false; //Pomocnicza
            foreach (CPlayerModel TempPlayer in lOLocalPlayersList)
            {
                if (TempPlayer.bCovering == true) //Jeżeli kryje 
                {
                    if (bTeam != TempPlayer.bTeam) //I nie jest z tego samego teamu co zawodnik posiadający piłkę 
                    {
                        if (first == false) //Za pierwszym razem podstaw pierwszego znalezionego zawodnika 
                        {
                            BestPlayerID = TempPlayer.iPlayerId;
                        }
                        if (first && lOLocalPlayersList[BestPlayerID].iEnergy < TempPlayer.iEnergy) //Jeżeli kolejny zawodnik ma więcej energii niż poprzedni to on jest najlepszym kandydatem do odebrania piłki przeciwnikowi 
                        {
                            BestPlayerID = TempPlayer.iPlayerId; //Zostaje tymczasowo najlepszym zawodnikiem do odebrania piłki 
                        }
                        first = true; //Pierwszy element podstawiony 
                    }
                }
            }
            iCPBestEnergyLevel = BestPlayerID; //podstawienie znalezionego zawodnika z największą ilością energii
            if (iCPBestEnergyLevel != -1) //Jeżeli znaleziono blokującego zawodnika to może on odebrać mu piłkę 
            {
                lOLocalPlayersList[iCPBestEnergyLevel].bHaveBall = true; //Nowy zawodnik posiada piłkę
                bHaveBall = false; //Stary zawodnik traci piłkę 
                iHaveBallID = lOLocalPlayersList[iCPBestEnergyLevel].iPlayerId; //Id zawodnika posiadającego piłkę 
             //   Console.WriteLine("Zawodnik o id: " + iPlayerId + " stracił piłkę w pojedynku z zawodnikiem o ID: " + iCPBestEnergyLevel);
                iCPBestEnergyLevel = -1;
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mThrow() //Rzut
        {
          //  Console.Write("Wywołano metode mThrow. ");
            Random luck = new Random(); //Współczynnik szczęścia
            double dThrow = (double)(iEnergy)/500 * (double)(Math.Abs(iPosition))/100 * (double)(luck.Next(60, 100))/100; //Ocena rzutu
           // Console.WriteLine("dThrow:" + dThrow);
            //Punkt dla gospodarzy 
            if (bTeam==true)
            {
                if (dThrow >= 0.5) iScoreHost+= 2; //Jeżeli ocena rzutu jest większa niż 0,5 wtedy dostają punkt 
               // Console.WriteLine("Wynik gospodarzy: " + iScoreHost);
            }
            //Punkt dla gości
            else if (bTeam==false)
            {
                if (dThrow >= 0.5) iScoreGuest+= 2; //Jeżeli ocena rzutu jest większa niż 0,5 wtedy dostają punkt 
              //  Console.WriteLine("Wynik gości: " + iScoreGuest);
            }
            bChanges = true; //ustawianie flagi zmian
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mAssist() //Asysta
        {
            //Jeżeli pozycja względem posiadacza piłki mieści się w przedziale to asystuje
            if ((iPosition > lOLocalPlayersList[iHaveBallID].iPosition - 5)
               && (iPosition < lOLocalPlayersList[iHaveBallID].iPosition + 5))
            {
                bAssisting = true;
              //  Console.WriteLine("Wywołano metode mAssist zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " asystuje i ma odpowiednią pozycje:" + iPosition);
            }
            // Jeżeli pozycja względem posiadacza piłki jest w tym przedziale zbliżaj się do niego cofając się
            if ((iPosition >= lOLocalPlayersList[iHaveBallID].iPosition + 5)
                && (iPosition - lOLocalPlayersList[iHaveBallID].iPosition <= 30))
            {
                iPosition -= 6;
                bAssisting = false;
                bLock = false;
            //    Console.WriteLine("Wywołano metode mAssist zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " ruszył wstecz i ma pozycje:" + iPosition);
            }
            // Jeżeli pozycja względem posiadacza piłki jest w tym przedziale zbliżaj się do niego podążająć w przód
            if ((iPosition <= lOLocalPlayersList[iHaveBallID].iPosition - 5)
                && (lOLocalPlayersList[iHaveBallID].iPosition - iPosition <= 30))
            {
                iPosition += 6;
                bAssisting = false;
                bLock = false; 
            //    Console.WriteLine("Wywołano metode mAssist zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " ruszył w przód i ma pozycje:" + iPosition);
            }
            bCovering = false;
            //Jeżelni nie uczestniczy w rozgrywce to odpoczywa i regeneruje energię 
            if ((iPosition - lOLocalPlayersList[iHaveBallID].iPosition > 30)
                || (lOLocalPlayersList[iHaveBallID].iPosition - iPosition > 30))
            {
                iEnergy += 5;
            }

            //Jeżeli pozycja względem posiadacza piłki jest zbyt duża nie może asystować bo go nie dogoni 
                if (((iPosition - lOLocalPlayersList[iHaveBallID].iPosition > 30) 
                || (lOLocalPlayersList[iHaveBallID].iPosition - iPosition > 30)) && !bLock) 
            {
            //    Console.WriteLine("Wywołano metode mAssist zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " jest za daleko by asystować i ma pozycje: " + iPosition);
                bLock = true;
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mCover() //Krycie
        {
            //Jeżeli pozycja względem posiadacza piłki mieści się w przedziale to blokuje
            if ((iPosition > lOLocalPlayersList[iHaveBallID].iPosition - 5)
               && (iPosition < lOLocalPlayersList[iHaveBallID].iPosition + 5))
            {
                bCovering = true;
           //     Console.WriteLine("Wywołano metode mCover zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " kryje i ma odpowiednią pozycje:" + iPosition);
            }
            // Jeżeli pozycja względem posiadacza piłki jest w tym przedziale zbliżaj się do niego cofając się
            if ((iPosition >= lOLocalPlayersList[iHaveBallID].iPosition + 5)
                && (iPosition - lOLocalPlayersList[iHaveBallID].iPosition <= 20))
            {
                iPosition -= 6;
                bCovering = false;
                bLock = false; //Tymczasowo
             //   Console.WriteLine("Wywołano metode mCover zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " ruszył wstecz i ma pozycje:" + iPosition);
            }
            // Jeżeli pozycja względem posiadacza piłki jest w tym przedziale zbliżaj się do niego podążająć w przód
            if ((iPosition <= lOLocalPlayersList[iHaveBallID].iPosition - 5)
                && (lOLocalPlayersList[iHaveBallID].iPosition - iPosition <= 20))
            {
                iPosition += 6;
                bCovering = false;
                bLock = false; //Tymczasowo
            //    Console.WriteLine("Wywołano metode mCover zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " ruszył w przód i ma pozycje:" + iPosition);
            }
            bAssisting = false;
            //Jeżelni nie uczestniczy w rozgrywce to odpoczywa i wzrasta mu energia 
            if ((iPosition - lOLocalPlayersList[iHaveBallID].iPosition > 20)
                || (lOLocalPlayersList[iHaveBallID].iPosition - iPosition > 20))
            {
                iEnergy += 5;
            }
            //Jeżeli pozycja względem posiadacza piłki jest zbyt duża nie może blokować bo go nie dogoni 
            if (((iPosition - lOLocalPlayersList[iHaveBallID].iPosition > 20)
                || (lOLocalPlayersList[iHaveBallID].iPosition - iPosition > 20)) && !bLock)
            {
            //    Console.WriteLine("Wywołano metode mCover zawodnika o id: " + iPlayerId + " z druzyny: " + bTeam + " jest za daleko by blokować i ma pozycje: " + iPosition);
                bLock = true;
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void mMove() //Ruch
        {
            //Poruszanie się w stronę kosza
            if (bHaveBall == true)
            {
            //    Console.WriteLine("Wywołano metode mMove");
                if (bTeam == true) //Jeżeli poruszają się gospodarze wartosci rosną 
                {
                    iPosition += 10;
                }
                if (bTeam == false) //Jeżeli poruszają się goście wortości maleją 
                {
                    iPosition -= 10;
                }
            //    Console.WriteLine("Pozycja zawodnika o id: " + iPlayerId + " posiadającego piłke: " + iPosition);
                //Zezwolenie na wykonanie rzutu do kosza
                if (Math.Abs(iPosition) >= 95) //Jeżeli zawodnik znajduje się 5 jednostek od kosza może wykonać rzut 
                {
                    mThrow(); //Wywołaj rzut 
                }
                iEnergy -= 10; //utrata energii na jeden ruch
                Thread.Sleep(1000);
                bChanges = false; //resetowanie flagi zmian
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
